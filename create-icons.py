from PIL import Image, ImageDraw
import os

os.makedirs("wwwroot/images", exist_ok=True)

for size in [192, 512]:
    img = Image.new("RGB", (size, size), "#2563eb")
    draw = ImageDraw.Draw(img)

    cx, cy = size // 2, size // 2
    w, h = int(size * .30), int(size * .12)

    # White graduation cap
    draw.polygon([
        (cx, cy-h),
        (cx+w, cy),
        (cx, cy+h),
        (cx-w, cy)
    ], fill="white")

    # Cap tassel
    draw.line(
        [(cx+w, cy), (cx+w+size//10, cy+size//8)],
        fill="white",
        width=max(2, size//80)
    )

    img.save(f"wwwroot/images/learntrack-{size}.png")

print("LearnTrack icons created successfully!")