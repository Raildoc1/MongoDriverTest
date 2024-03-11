import subprocess
import os
import psutil
import sys
import time

def run_process(path):
    process = subprocess.Popen(path)
    return process

def print_process_mem(process, file):
    globalTime = str(time.time())
    localTime = str(time.process_time())
    memory = str(process.memory_info().rss)
    file.write(globalTime + "," + localTime + "," + memory + "\n")

def prepare_log_file(path):
    try:
        os.remove(path)
    except OSError:
        pass
    file = (open(path, "x"))
    file.write("GlobalTime,LocalTime,Memory\n")
    file.close()

if __name__ == '__main__':
    sysProcess = run_process(sys.argv[1])
    process = psutil.Process(sysProcess.pid)
    prepare_log_file(sys.argv[2])

    try:
        while True:
            file = open(sys.argv[2], "a")
            time.sleep(3)
            print_process_mem(process, file)
            file.close()

    finally:
        sysProcess.terminate()
        sysProcess.wait()
        print("terminated")

