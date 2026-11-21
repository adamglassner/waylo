"""Generates the Waylo icon from assets/waylo.svg:
src/Waylo.Desktop/Assets/waylo.ico + waylo.png, and assets/waylo-256.png / -512.png.
Run: python tools/make_icon.py   (needs Pillow and rsvg-convert from librsvg)

The mark: a white speech bubble on a Waylo-blue rounded tile, with a "W" drawn as a wave
(wave + hello = Waylo). Small sizes (16-32 px) use a thicker W so it stays readable.
"""
import io, os, re, subprocess
from PIL import Image

here = os.path.dirname(os.path.abspath(__file__))
root = os.path.join(here, "..")
svg = open(os.path.join(root, "assets", "waylo.svg"), encoding="utf-8").read()
# Same mark with a bolder W for tiny icon sizes.
svg_small = re.sub(r'stroke-width="[0-9.]+"', 'stroke-width="2.9"', svg)


def render(source, size):
    png = subprocess.run(["rsvg-convert", "-w", str(size), "-h", str(size)],
                         input=source.encode("utf-8"), capture_output=True, check=True).stdout
    return Image.open(io.BytesIO(png)).convert("RGBA")


assets = os.path.join(root, "src", "Waylo.Desktop", "Assets")
os.makedirs(assets, exist_ok=True)
render(svg, 256).save(os.path.join(assets, "waylo.png"))

frames = {s: render(svg_small if s <= 32 else svg, s) for s in (16, 20, 24, 32, 40, 48, 64, 128, 256)}
big = frames[256]
big.save(os.path.join(assets, "waylo.ico"), sizes=[(s, s) for s in frames],
         append_images=[frames[s] for s in frames if s != 256])

render(svg, 256).save(os.path.join(root, "assets", "waylo-256.png"))
render(svg, 512).save(os.path.join(root, "assets", "waylo-512.png"))
print("ok")
