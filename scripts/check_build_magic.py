import os
# Compute repository root relative to this script so paths are portable
script_dir = os.path.dirname(os.path.abspath(__file__))
repo_root = os.path.abspath(os.path.join(script_dir, '..'))
base = os.path.join(repo_root, 'AvatarFlaskApp', 'app', 'AvatarGame', 'Build')
for name in ('build.wasm','build.data','AvatarGame.wasm.unityweb'):
    path = os.path.join(base, name)
    if not os.path.exists(path):
        print(name, 'MISSING')
        continue
    with open(path,'rb') as f:
        b = f.read(4)
    print(name, 'size=', os.path.getsize(path), 'bytes:', ' '.join(f'{x:02x}' for x in b))
