import shutil
import os
base = r'C:\Users\kacpe\Downloads\AvatarGameBEP-main\AvatarGameBEP-main\AvatarFlaskApp\app\AvatarGame\Build'
files = [
    ('AvatarGame.wasm.unityweb', 'build.wasm'),
    ('AvatarGame.framework.js.unityweb', 'build.framework.js'),
    ('AvatarGame.data.unityweb', 'build.data'),
    ('AvatarGame.loader.js', 'build.js'),
]
for src, dst in files:
    s = os.path.join(base, src)
    d = os.path.join(base, dst)
    try:
        shutil.copyfile(s, d)
        print(f'Copied {src} -> {dst}')
    except Exception as e:
        print(f'Failed to copy {src} -> {dst}:', e)
print('\nFiles in Build:')
for f in sorted(os.listdir(base)):
    path = os.path.join(base, f)
    if os.path.isfile(path):
        print(f, os.path.getsize(path))
