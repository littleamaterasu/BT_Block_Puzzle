import plistlib
import re

def parse_atlas(atlas_path):
    with open(atlas_path, "r", encoding="utf-8") as f:
        lines = f.readlines()
    
    frames = {}
    current_key = None
    xy = ""
    
    for line in lines:
        
        line = line.strip()
        
        if not line or line.endswith(".png") or line.startswith("size:"):
            continue
        
        match = re.match(r"(.+)", line)
        if match and '.' not in line and ':' not in line:
            
            current_key = match.group(1)
            frames[current_key] = {}
            continue
        
        if current_key and ":" in line:
            print(line)
            key, value = line.split(":", 1)
            print(key, value)
            key, value = key.strip(), value.strip()
            print(key, value)
            if key == "xy":
                xy = f"{{{value}}}"
            elif key == "orig":
                frames[current_key]["spriteSize"] = f"{{{value}}}"
                frames[current_key]["spriteSourceSize"] = f"{{{value}}}"
                frames[current_key]["textureRect"] = f"{{{xy}, {{{value}}}}}"
            elif key == "rotate":
                frames[current_key]["textureRotated"] = value.lower() == "true"
    
    return frames

def convert_to_plist(atlas_path, plist_path, texture_file):
    frames = parse_atlas(atlas_path)
    
    plist_data = {
        "frames": {},
        "metadata": {
            "format": 3,
            "pixelFormat": "RGBA8888",
            "premultiplyAlpha": False,
            "realTextureFileName": texture_file,
            "textureFileName": texture_file,
        },
    }
    
    for key, values in frames.items():
        plist_data["frames"][key] = {
            "aliases": [],
            "spriteOffset": "{0,0}",
            "spriteSize": values.get("spriteSize", "{0,0}"),
            "spriteSourceSize": values.get("spriteSourceSize", "{0,0}"),
            "textureRect": values.get("textureRect", "{{0,0},{0,0}}"),
            "textureRotated": values.get("textureRotated", False),
        }
    
    with open(plist_path, "wb") as f:
        plistlib.dump(plist_data, f)
    
    print(f"Converted {atlas_path} -> {plist_path}")

# Usage example
convert_to_plist("Block8.atlas.txt", "Block8.plist", "Block8.png")
