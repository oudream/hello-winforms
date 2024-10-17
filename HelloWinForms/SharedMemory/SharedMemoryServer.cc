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

// ����������Ϣ
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

// ���ɷ�����Ϣ
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

// ��ȡ��ǰʱ����ַ���
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

    // ���������ڴ�ӳ��
    hMapFile = CreateFileMapping(
        INVALID_HANDLE_VALUE,  // Use paging file
        NULL,                  // Default security
        PAGE_READWRITE,        // Read/write access
        0,                     // Maximum object size (high-order DWORD)
        SHARED_MEM_SIZE,       // Maximum object size (low-order DWORD)
        sharedMemoryName);     // Name of mapping object

    if (hMapFile == NULL) {
        std::cerr << "�޷������ļ�ӳ����� (" << GetLastError() << ").\n";
        return 1;
    }

    // ӳ���ļ���ͼ�����̵�ַ�ռ�
    pSharedMemory = (SharedMemory*)MapViewOfFile(
        hMapFile,              // Handle to map object
        FILE_MAP_ALL_ACCESS,   // Read/write permission
        0,
        0,
        SHARED_MEM_SIZE);

    if (pSharedMemory == NULL) {
        std::cerr << "�޷�ӳ���ļ���ͼ (" << GetLastError() << ").\n";
        CloseHandle(hMapFile);
        return 1;
    }

    std::cout << "�����ڴ��Ѵ�������ʼ��������..." << std::endl;

    // ��ѭ������ʱ������󣬲�������Ӧ
    while (true) {
        // ��ȡ������Ϣ
        std::string requestStr(pSharedMemory->reqInfo);
        if (!requestStr.empty()) {
            // ��������
            std::map<std::string, std::string> requestMap = ParseRequest(requestStr);
            std::cout << "�յ�����: " << std::endl;
            for (const auto& entry : requestMap) {
                std::cout << entry.first << " = " << entry.second << std::endl;
            }

            std::string currentTime = GetCurrentTimeString();

            // ģ�⴦��ͼ��������Ӧ
            std::string status = "OK";  // ����ͼ����ɹ�
            std::string response = GenerateResponse(status, requestMap["Command"], requestMap["ImageName"], currentTime.size()/*std::stoi(requestMap["ImageSize"])*/,
                                                    std::stoll(requestMap["Timestamp"]), 0.95f, std::stoi(requestMap["Height"]), std::stoi(requestMap["Width"]), std::stoi(requestMap["MatTypeInt"]));

            // д����Ӧ�ַ����������ڴ�� respInfo
            std::strcpy(pSharedMemory->respInfo, response.c_str());
            std::cout << "��Ӧ�ѷ���: " << response << std::endl;

            // д�뵱ǰʱ�䵽�����ڴ�� respData
            std::strcpy(pSharedMemory->respData, currentTime.c_str());
            std::cout << "��ǰʱ����д�빲���ڴ�: " << currentTime << std::endl;

            // ���������Ϣ�������ظ�����
            std::memset(pSharedMemory->reqInfo, 0, 1024 * 1024);

            // ģ��ͼ������Ϻ���ӳ�
            std::this_thread::sleep_for(std::chrono::seconds(1));
        }

        // ÿ����һ��
        std::this_thread::sleep_for(std::chrono::milliseconds(50));
    }

    // �˳�ʱ����
    UnmapViewOfFile(pSharedMemory);
    CloseHandle(hMapFile);

    return 0;
}