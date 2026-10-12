#!/usr/bin/env bash
# Records docs/images/demo.gif on Linux without a desktop: Shotora runs on a virtual X server over a
# drawn desktop (desktop.py), tray.py stands in for the panel's tray (StatusNotifier + dbusmenu) to
# start a region capture, and xdotool plays the mouse and keyboard.
#   docs/demo/record.sh <folder with a built Shotora.App>
#   (dotnet build Shotora.App/Shotora.App.csproj -c Release -o out)
# Needs: xvfb-run, dbus-run-session, xdotool, ffmpeg, ImageMagick, python3 with Pillow and dbus-next,
# tesseract-ocr with English data. Set DOTNET_ROOT when .NET is not installed system-wide.
set -euo pipefail
HERE=$(cd "$(dirname "$(readlink -f "$0")")" && pwd)
APP=$(readlink -f "$1")/Shotora.App
OUT=$HERE/../images
[ -x "$APP" ] || { echo "Build first: $APP not found"; exit 1; }
if [ "${SHOTORA_DEMO_INNER:-}" != 1 ]; then
    export SHOTORA_DEMO_INNER=1
    exec xvfb-run -a -s "-screen 0 1600x900x24" dbus-run-session -- "$0" "$@"
fi

WORK=$(mktemp -d)
export HOME=$WORK/home XDG_RUNTIME_DIR=$WORK/run; mkdir -p "$HOME" "$XDG_RUNTIME_DIR"
mkdir -p "$HOME/.config/Shotora"   # a fresh profile, without the update check (a local build looks old)
echo '{"AutoCheckForUpdates": false}' > "$HOME/.config/Shotora/config.json"
for d in /usr/share/tesseract-ocr/*/tessdata /usr/share/tessdata; do
    [ -f "$d/eng.traineddata" ] && export TESSDATA_PREFIX=$d && break
done
python3 "$HERE/desktop.py" "$WORK/desktop.png"
display -window root "$WORK/desktop.png" || true   # a root background: nothing to click by accident
python3 "$HERE/tray.py" serve & TRAY=$!
sleep 1
"$APP" > "$WORK/shotora.log" 2>&1 & SHOTORA=$!
trap 'kill $SHOTORA $TRAY 2>/dev/null || true; rm -rf "$WORK"' EXIT
sleep 12
xdotool mousemove 944 547 click 1; sleep 1          # close the welcome window
xdotool mousemove 900 760; sleep 0.5

ffmpeg -loglevel error -y -f x11grab -framerate 30 -video_size 1600x900 -i "$DISPLAY" \
    -c:v libx264 -preset veryfast -crf 18 -pix_fmt yuv420p "$WORK/raw.mp4" & REC=$!
sleep 1.5

glide() {  # glide x1 y1 x2 y2 steps: smooth pointer move
    for i in $(seq 1 "$5"); do
        xdotool mousemove $(( $1 + ($3 - $1) * i / $5 )) $(( $2 + ($4 - $2) * i / $5 )); sleep 0.025
    done
}
drag() {  # drag x1 y1 x2 y2
    xdotool mousemove "$1" "$2"; sleep 0.15; xdotool mousedown 1
    glide "$1" "$2" "$3" "$4" 22; sleep 0.1; xdotool mouseup 1; sleep 0.4
}
tool() {  # tool x: click a toolbar button (the toolbar sits at y 852)
    eval "$(xdotool getmouselocation --shell)"
    glide "$X" "$Y" "$1" 852 14; sleep 0.2; xdotool click 1; sleep 0.4
}

python3 "$HERE/tray.py" click "Capture Region"; sleep 1.5          # tray menu: Capture Region
glide 900 760 70 95 18
drag 70 95 1300 640                                                  # select: OCR runs on release
sleep 3.2
glide 1300 640 390 880 16; sleep 0.2; xdotool click 1; sleep 0.6     # close the OCR result
tool 749; drag 965 350 1178 296                                      # arrow to the record month
tool 793; drag 110 236 555 268                                       # box around a sentence
tool 969; drag 110 540 520 574                                       # blur the e-mail address
tool 881; glide 1114 852 790 338 14; xdotool click 1; sleep 0.8       # text label
xdotool type --delay 70 "Record month"; sleep 0.4
glide 790 338 1000 608 12; sleep 0.2; xdotool click 1; sleep 2.2      # Place text
tool 1114; sleep 2                                                   # copy and close
kill -INT $REC; wait $REC || true

ffmpeg -loglevel error -y -ss 0.8 -i "$WORK/raw.mp4" -filter_complex \
    "fps=12,scale=800:-1:flags=lanczos,split[a][b];[a]palettegen=max_colors=128:stats_mode=diff[p];[b][p]paletteuse=dither=bayer:bayer_scale=4:diff_mode=rectangle" \
    "$OUT/demo.gif"
echo "Demo written to $OUT/demo.gif"
