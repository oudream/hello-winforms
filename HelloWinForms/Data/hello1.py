import sys
import time

# 获取传入的文件路径参数
file_path = sys.argv[1]

# 循环读取文件 10 次，每次间隔 2 秒
for i in range(10):
    try:
        with open(file_path, 'r') as file:
            content = file.read()
        print(f"Reading {i+1}: File Content of {file_path}:")
        print(content)
    except Exception as e:
        print(f"Error reading file: {e}")

    # 每次读取后等待 2 秒
    time.sleep(2)