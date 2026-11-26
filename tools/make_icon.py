"""Generates the RingHawk icon from assets/ringhawk.svg:
src/RingHawk.Desktop/Assets/ringhawk.ico + ringhawk.png, and assets/ringhawk-256.png / -512.png.
Run: python tools/make_icon.py   (needs Pillow and rsvg-convert from librsvg)

The mark: a white phone handset with two ring waves on a blue rounded tile.
Small sizes (16-32 px) use thicker ring waves so they stay readable.
"""
import io, os, re, subprocess
from PIL import Image

here = os.path.dirname(os.path.abspath(__file__))
root = os.path.join(here, "..")
svg = open(os.path.join(root, "assets", "ringhawk.svg"), encoding="utf-8").read()
# Same mark with bolder ring waves for tiny icon sizes.
svg_small = svg.replace('stroke-width="1.9"', 'stroke-width="2.8"')


def render(source, size):
    png = subprocess.run(["rsvg-convert", "-w", str(size), "-h", str(size)],
                         input=source.encode("utf-8"), capture_output=True, check=True).stdout
    return Image.open(io.BytesIO(png)).convert("RGBA")


assets = os.path.join(root, "src", "RingHawk.Desktop", "Assets")
os.makedirs(assets, exist_ok=True)
render(svg, 256).save(os.path.join(assets, "ringhawk.png"))

frames = {s: render(svg_small if s <= 32 else svg, s) for s in (16, 20, 24, 32, 40, 48, 64, 128, 256)}
big = frames[256]
big.save(os.path.join(assets, "ringhawk.ico"), sizes=[(s, s) for s in frames],
         append_images=[frames[s] for s in frames if s != 256])

render(svg, 256).save(os.path.join(root, "assets", "ringhawk-256.png"))
render(svg, 512).save(os.path.join(root, "assets", "ringhawk-512.png"))
print("ok")
