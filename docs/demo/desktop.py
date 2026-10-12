# A plausible desktop for the Shotora demo: wallpaper, a notes window with readable text, a chart window.
import sys
from PIL import Image, ImageDraw, ImageFont, ImageFilter

W, H = 1600, 900
DJ = "/usr/share/fonts/truetype/dejavu/"
out = sys.argv[1]
im = Image.new("RGB", (W, H))
d = ImageDraw.Draw(im)
for y in range(H):  # wallpaper: deep blue to violet
    t = y / H
    d.line([(0, y), (W, y)], fill=(int(28 + 50 * t), int(36 + 10 * t), int(84 + 60 * t)))
glow = Image.new("RGBA", (W, H), (0, 0, 0, 0))
ImageDraw.Draw(glow).ellipse([900, -300, 1900, 600], fill=(170, 90, 255, 90))
glow = glow.filter(ImageFilter.GaussianBlur(160))
im.paste(glow, (0, 0), glow)
d = ImageDraw.Draw(im)


def window(x, y, w, h, title):
    sh = Image.new("RGBA", (w + 60, h + 60), (0, 0, 0, 0))
    ImageDraw.Draw(sh).rounded_rectangle([30, 30, w + 30, h + 30], 12, fill=(0, 0, 0, 150))
    sh = sh.filter(ImageFilter.GaussianBlur(16))
    im.paste(sh, (x - 30, y - 16), sh)
    d.rounded_rectangle([x, y, x + w, y + h], 10, fill=(250, 250, 252))
    d.rounded_rectangle([x, y, x + w, y + 40], 10, fill=(236, 237, 242))
    d.rectangle([x, y + 30, x + w, y + 40], fill=(236, 237, 242))
    for i, c in enumerate([(255, 95, 87), (254, 188, 46), (40, 200, 64)]):
        d.ellipse([x + 16 + i * 22, y + 14, x + 28 + i * 22, y + 26], fill=c)
    d.text((x + 90, y + 11), title, font=ImageFont.truetype(DJ + "DejaVuSans.ttf", 16), fill=(90, 92, 100))


# notes window
window(80, 110, 620, 540, "Release notes.md")
h1 = ImageFont.truetype(DJ + "DejaVuSans-Bold.ttf", 30)
body = ImageFont.truetype(DJ + "DejaVuSans.ttf", 21)
d.text((120, 180), "Quarterly release summary", font=h1, fill=(25, 27, 35))
lines = ["Downloads grew by 48 percent after the", "new installer shipped in March.",
         "", "Crash reports dropped to a record low,", "and the average startup time is now",
         "under one second on every platform.", "", "Next: faster OCR and a portable build.",
         "", "Contact: maria.lopez@example.com"]
y = 240
for ln in lines:
    d.text((120, y), ln, font=body, fill=(55, 58, 68))
    y += 34

# chart window
window(740, 200, 540, 420, "Downloads per month")
vals = [32, 41, 38, 55, 61, 90]
months = ["Oct", "Nov", "Dec", "Jan", "Feb", "Mar"]
base, left = 580, 790
small = ImageFont.truetype(DJ + "DejaVuSans.ttf", 16)
for i, v in enumerate(vals):
    x = left + i * 80
    col = (236, 72, 120) if i == len(vals) - 1 else (99, 102, 241)
    d.rounded_rectangle([x, base - v * 3.4, x + 50, base], 6, fill=col)
    d.text((x + 25, base + 10), months[i], font=small, fill=(90, 92, 100), anchor="ma")
    d.text((x + 25, base - v * 3.4 - 24), f"{v}k", font=small, fill=(60, 62, 70), anchor="ma")
im.save(out)
