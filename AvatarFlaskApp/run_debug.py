import subprocess
import webbrowser
import time
import os
import sys
import socket


def find_repo_root():
    return os.path.abspath(os.path.join(os.path.dirname(__file__), os.pardir))


def wait_for_port(host, port, timeout=30.0, interval=0.5):
    end_time = time.time() + timeout
    while time.time() < end_time:
        try:
            with socket.create_connection((host, port), timeout=interval):
                return True
        except OSError:
            time.sleep(interval)
    return False


def main():
    script_dir = os.path.dirname(os.path.abspath(__file__))
    repo_root = find_repo_root()

    venv_dir = os.path.join(repo_root, "bofs_venv")
    if os.name == "nt":
        venv_scripts = os.path.join(venv_dir, "Scripts")
    else:
        venv_scripts = os.path.join(venv_dir, "bin")

    env = os.environ.copy()
    if os.path.isdir(venv_dir) and os.path.isdir(venv_scripts):
        env["VIRTUAL_ENV"] = venv_dir
        env["PATH"] = venv_scripts + os.pathsep + env.get("PATH", "")
    else:
        print(f"Warning: virtualenv not found at {venv_dir} — continuing with current environment")

    app_path = os.path.abspath(os.path.join(script_dir, "app"))

    cmd = "BOFS run AvatarGame.cfg -d"
    print("Launching BOFS process...")
    process = subprocess.Popen(cmd, cwd=app_path, shell=True, env=env)

    host = "127.0.0.1"
    port = 5000

    if wait_for_port(host, port, timeout=30.0):
        # Open the reset endpoint first so the browser session starts fresh at the consent page
        webbrowser.open(f"http://{host}:{port}/reset_session")
    else:
        print(f"Server not responding at {host}:{port} after timeout — opening browser anyway")
        webbrowser.open(f"http://{host}:{port}/reset_session")

    process.wait()


if __name__ == "__main__":
    main()