import os
base = r'C:\Users\kacpe\Downloads\AvatarGameBEP-main\AvatarGameBEP-main\AvatarFlaskApp\app\AvatarGame\Build'
for name in ('build.wasm','build.data','AvatarGame.wasm.unityweb'):
    path = os.path.join(base, name)
    if not os.path.exists(path):
        print(name, 'MISSING')
        continue
    with open(path,'rb') as f:
        b = f.read(4)
    print(name, 'size=', os.path.getsize(path), 'bytes:', ' '.join(f'{x:02x}' for x in b))
