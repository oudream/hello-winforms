#include <windows.h>
#include <iostream>
#include <sstream>
#include <cstring>
#include <map>
#include <thread>
#include <chrono>

#define SHARED_MEM_SIZE (2 * 1024 * 1024 + 2 * 40 * 1024 * 1024) // Total size: (2MB req + resp + 80MB req + resp)

struct SharedMemory {
    char reqInfo[1024 * 1024];  // 1MB for request
    char respInfo[1024 * 1024]; // 1MB for response
    char reqData[40 * 1024 * 1024];  // 40MB for request data (image)
    char respData[40 * 1024 * 1024]; // 40MB for response data (image)
};

// 解析请求信息
std::map<std::string, std::string> ParseRequest(const std::string& request) {
    std::map<std::string, std::string> requestMap;
    std::istringstream stream(request);
    std::string line;

    while (std::getline(stream, line)) {
        size_t pos = line.find('=');
        if (pos != std::string::npos) {
            std::string key = line.substr(0, pos);
            std::string value = line.substr(pos + 1);
            requestMap[key] = value;
        }
    }

    return requestMap;
}

// 生成反馈信息
std::string GenerateResponse(const std::string& status, const std::string& command, const std::string& imageName, int imageSize, long long timestamp, float score, int height, int width, int matTypeInt) {
    std::stringstream ss;
    ss << "Status=" << status << "\n";
    ss << "Command=" << command << "\n";
    ss << "ImageName=" << imageName << "\n";
    ss << "ImageSize=" << imageSize << "\n";
    ss << "Timestamp=" << timestamp << "\n";
    ss << "Score=" << score << "\n";
    ss << "Height=" << height << "\n";
    ss << "Width=" << width << "\n";
    ss << "MatTypeInt=" << matTypeInt << "\n";
    return ss.str();
}

// 获取当前时间的字符串
std::string GetCurrentTimeString() {
    auto now = std::chrono::system_clock::now();
    std::time_t currentTime = std::chrono::system_clock::to_time_t(now);
    std::tm* localTime = std::localtime(&currentTime);

    char buffer[100];
    std::strftime(buffer, sizeof(buffer), "%Y-%m-%d %H:%M:%S", localTime);

    return std::string(buffer);
}

int main() {
    const char* sharedMemoryName = "Global\\SharedMemoryAQ";
    HANDLE hMapFile;
    SharedMemory* pSharedMemory;

    // 创建共享内存映射
    hMapFile = CreateFileMapping(
        INVALID_HANDLE_VALUE,  // Use paging file
        NULL,                  // Default security
        PAGE_READWRITE,        // Read/write access
        0,                     // Maximum object size (high-order DWORD)
        SHARED_MEM_SIZE,       // Maximum object size (low-order DWORD)
        sharedMemoryName);     // Name of mapping object

    if (hMapFile == NULL) {
        std::cerr << "无法创建文件映射对象 (" << GetLastError() << ").\n";
        return 1;
    }

    // 映射文件视图到进程地址空间
    pSharedMemory = (SharedMemory*)MapViewOfFile(
        hMapFile,              // Handle to map object
        FILE_MAP_ALL_ACCESS,   // Read/write permission
        0,
        0,
        SHARED_MEM_SIZE);

    if (pSharedMemory == NULL) {
        std::cerr << "无法映射文件视图 (" << GetLastError() << ").\n";
        CloseHandle(hMapFile);
        return 1;
    }

    std::cout << "共享内存已创建，开始监听请求..." << std::endl;

    // 主循环：定时检查请求，并生成响应
    while (true) {
        // 读取请求信息
        std::string requestStr(pSharedMemory->reqInfo);
        if (!requestStr.empty()) {
            // 解析请求
            std::map<std::string, std::string> requestMap = ParseRequest(requestStr);
            std::cout << "收到请求: " << std::endl;
            for (const auto& entry : requestMap) {
                std::cout << entry.first << " = " << entry.second << std::endl;
            }

            std::string currentTime = GetCurrentTimeString();

            // 模拟处理图像并生成响应
            std::string status = "OK";  // 假设图像处理成功
            std::string response = GenerateResponse(status, requestMap["Command"], requestMap["ImageName"], currentTime.size()/*std::stoi(requestMap["ImageSize"])*/,
                                                    std::stoll(requestMap["Timestamp"]), 0.95f, std::stoi(requestMap["Height"]), std::stoi(requestMap["Width"]), std::stoi(requestMap["MatTypeInt"]));

            // 写入响应字符串到共享内存的 respInfo
            std::strcpy(pSharedMemory->respInfo, response.c_str());
            std::cout << "响应已发送: " << response << std::endl;

            // 写入当前时间到共享内存的 respData
            std::strcpy(pSharedMemory->respData, currentTime.c_str());
            std::cout << "当前时间已写入共享内存: " << currentTime << std::endl;

            // 清空请求信息，避免重复处理
            std::memset(pSharedMemory->reqInfo, 0, 1024 * 1024);

            // 模拟图像处理完毕后的延迟
            std::this_thread::sleep_for(std::chrono::seconds(1));
        }

        // 每秒检查一次
        std::this_thread::sleep_for(std::chrono::milliseconds(50));
    }

    // 退出时清理
    UnmapViewOfFile(pSharedMemory);
    CloseHandle(hMapFile);

    return 0;
}