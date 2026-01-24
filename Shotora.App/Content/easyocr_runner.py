import os
import sys

try:
    sys.stdout.reconfigure(encoding='utf-8')
    sys.stderr.reconfigure(encoding='utf-8')
except Exception:
    pass

os.environ['KMP_DUPLICATE_LIB_OK'] = 'TRUE'
langs = [p.strip() for p in sys.argv[1].split(',') if p.strip()]
if not langs:
    langs = ['en']
image_path = sys.argv[2]
try:
    import easyocr
except Exception as exc:
    sys.stderr.write('EASYOCR_IMPORT_ERROR:' + str(exc))
    sys.exit(2)


class _DevNull:
    def write(self, s):
        pass

    def flush(self):
        pass


_old_stdout = sys.stdout
try:
    sys.stdout = _DevNull()
    reader = easyocr.Reader(langs, gpu=False)
    result = reader.readtext(image_path, detail=0, paragraph=True)
finally:
    sys.stdout = _old_stdout

try:
    if result is None:
        result = []
    texts = []
    for r in result:
        if isinstance(r, str):
            stripped = r.strip()
            if stripped:
                texts.append(stripped)
        elif isinstance(r, (list, tuple)) and len(r) > 0:
            text_part = str(r[-1]).strip() if r else ''
            if text_part:
                texts.append(text_part)
    if texts:
        print('\n'.join(texts))
except Exception as exc:
    sys.stderr.write('EASYOCR_RUN_ERROR:' + str(exc))
    sys.exit(3)