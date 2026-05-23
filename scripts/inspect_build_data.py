import gzip, os, sys
# Compute path relative to this script
script_dir = os.path.dirname(os.path.abspath(__file__))
repo_root = os.path.abspath(os.path.join(script_dir, '..'))
p = os.path.join(repo_root, 'AvatarFlaskApp', 'app', 'AvatarGame', 'Build', 'build.data')
if not os.path.exists(p):
    print('MISSING',p); sys.exit(1)
with gzip.open(p,'rb') as f:
    data=f.read()
print('uncompressed size',len(data))
print('Il2CppData' if b'Il2CppData' in data else 'NO Il2CppData')
print('global-metadata.dat' if b'global-metadata.dat' in data else 'NO global-metadata.dat')
# print a small slice around occurrence if found
idx = data.find(b'global-metadata.dat')
if idx!=-1:
    start=max(0,idx-40)
    print(data[start:idx+40])
