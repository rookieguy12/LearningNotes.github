# Linux高性能服务器编程

## 第五章 Linux网络编程基础API

### 5.1 socket地址API

#### 5.1.1 主机字节序和网络字节序

+ 大端存储和小端存储
+ 原因：不同机器的字节序可能不同，需要转换；不同进程也可能存在不同。

转换的API：

```c++
#include<netinet/in.h>
unsigned long int htonl(unsigned long int);
unsigned long int ntohl(unsigned long int);
unsigned short int htons(unsigned short int);
unsigned short int ntohl(unsigned short int);
```

#### 5.1.2 协议通用socket地址（不好用）

+ socket结构体

```c++
#include <bits/socket.h>
struct sockaddr_storage
{
    sa_family_t sa_family;//地址族类型，与协议族对应
    unsigned long int __ss_align;
    char __ss_padding[128-sizeof(__ss_align)];//存放socket地址值
}
```

+ 地址族和协议族一一对应

  ![](Images/1.png)

#### 5.1.3 协议专用socket地址（一般用这个）

+ TCP/IP协议族

  + IPv4和IPv6

    ![](Images/2.png)

+ 注意在使用时，这些专用，通用的都得转换成sockaddr这个类型。

#### 5.1.4 IP地址转换函数

+ 转char数组为可供处理的整型

```c++
#include <arpa/inet.h>
int_addr_t inet_addr(const char* strptr);
//将点分十进制变整型
int inet_aton(const char* cp, struct in_addr* inp);
//将点分十进制转整型并放imp中，0失败，1成功
char* inet_ntoa(struct in_addr in);
//反过来。但是是不可重入的


// 新版的
int inet_pton(int af, const char* src, void * dst);
const char* inet_ntop(int af, const void * src, char* dst, socklen_t cnt);
```

### 5.2 创建socket

```c++
#include <sys/types.h>
#include <sys/socket.h>
int socket(int domain, int type, int protocol);
/*
domain:底层协议族
		PF_INET,PF_INET6用于ipv4,6
type:指定服务类型
		SOCK_STREAM流服务，即TCP用的
		SOCK_DGRAM数据报服务，即UDP用的

*/
```

