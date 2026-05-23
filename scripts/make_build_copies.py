import shutil
import os
# Compute repository root relative to this script so paths are portable
script_dir = os.path.dirname(os.path.abspath(__file__))
repo_root = os.path.abspath(os.path.join(script_dir, '..'))
base = os.path.join(repo_root, 'AvatarFlaskApp', 'app', 'AvatarGame', 'Build')
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
