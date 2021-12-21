# Linux高性能服务器编程

## 第7章 Linux服务器程序规范

### 7.0 导论

+ Linux服务器一般以<font color = "red">后台进程形式运行，即守护进程。</font>其父进程一般是init进程(pid = 1)
+ <font color = "red">有一套日志系统</font>负责输出log到文件(/var/log下)/专门的UDP server
+ 以非root身份运行。
+ 通常是可配置的。一般都有配置文件，放/etc下
+ 启动时生成PID文件并存入/var/run目录，来记录后台进程的PID
+ 设计时得考虑系统资源和限制，预测自身能承受多大的负荷，比如进程可以用文件描述符总数和内存总量。

### 7.1 日志

+ server的调试和维护离不开log系统

#### 7.1.1 Linux系统日志

+ Linux由rsyslogd守护进程来处理日志。

+ rsyslogd守护进程既能接收用户进程的输出日志，也能接收内核日志。

  使用syslog函数

+ rsyslogd则监听文件，获取用户进程的输出。

+ 对于系统日志，则使用printk打印到内核的环状缓存ring buffer，其内容会映射到/proc/kmsg文件中。rsyslogd则通过读取文件得到内核日志。

+ 文件目录

  + debug信息->/var/log/debug
  + normal信息->/var/log/messages
  + kernal信息->/var/log/kern.log
  + 【可以在/etc/rsyslog.conf中设置】

![](Images/4.png)

#### 7.1.2 syslog函数

```c++
#include<syslog.h>
void syslog(int priority, const char * message, ...);
```

+ priority等级

![](Images/5.png)

+ priority是LOG_USER和上面这些相或

```c++
#include<syslog.h>
void openlog(const char* ident, int logopt, int facility);
// 用于改变syslog的默认输出方式
// ident：其指定的字符串被添加到日志消息的date + time后，一般设置成程序名
// logopt：对syslog调用的行为进行配置
// facility:修改syslog函数的默认设施值
```

![](Images/6.png)

```c++
#include<syslog.h>
int setlogmask(int maskpri);
// 设置日志掩码，改变日志级别，从开发到发布时就可能修改。返回之前的日志掩码值
```

```c++
void closelog();//关闭日志功能
```

### 7.2 用户信息

#### 7.2.1 UID,EUID,GID,EGID

+ 大部分服务器以root启动，不以root运行。

  + UID:真实user id

  + EUID:有效用户id
  + GID:真实组
  + EGID:有效组

```c++
#include<sys/types.h>
#include<unistd.h>
uid_t getuid();
uid_t geteuid();
gid_t getgid();
gid_t getegid();
int setuid(uid_t uid);
int seteuid(uid_t uid);
int setgid(gid_t gid);
int setegid(git_t gid);
```

+ 每个进程都有UID和EUID。EUID方便资源访问，使得运行程序的用户uid拥有该程序的有效用户euid的权限。有效用户为root的进程为特权进程。
+ 例子：

```c++
#include<unistd.h>
#include<stdio.h>
#include<iostream>
using namespace std;
int main()
{
    uid_t uid = getuid();
    uid_t euid = geteuid();
    cout << uid << endl << euid << endl;
}

> g++ test.cpp -o test && ./test
> 1000
> 1000
//一般直接执行就是user的uid，euid

>sudo chown root:root test #设置文件所有者为root
>sudo chmod +s test #设置文件的set-user-id
>./test
>1000 #是实际执行这个程序的进程的id
>0 #就是root的id
```

#### 7.2.2切换用户

 ### 7.3 进程间关系

#### 7.3.1 进程组

+ Linux中每个进程隶属一个进程组，所以进程除了pid外，还有pgid（进程组id）
+ 每个进程组都有一个首领进程。它的PGID和PID相同。进程组一直存在，直到其中所有进程退出/被加到其它进程组。

```c++
#include<unistd.h>
pid_t getpgid(pid_t pid);
// 获取进程为pid的进程组的pgid
// return：成功则返回进程pid所属进程组pgid，失败返回-1，设置errno

int setpgid(pid_t pid, pid_t pgid);
// 设置进程为pid的进程组PGID为pgid
// return：成功返回0，反之-1，并设置errno
// 1. 若pid == pgid，则设置pid进程为进程组的首领
// 2. 若pid == 0，则是设置当前进程的PGID，不是设置root进程
// 3. 若pgid为0，则使用pid作为PGID
```

+ 一个进程只能设置自己及其子进程的PGID，且子进程调用exec系列函数后，也没法再在父进程中设置其PGID。

#### 7.3.2 会话session

+ 有关联的进程组会形成会话。

```c++
#include<unistd.h>
pid_t setsid(void);
// 由非进程组首领的进程调用。
// return：成功则返回PGID，失败返回-1，设置errno

pid_t getsid(pid_t pid);
```

+ setsid调用后
  + 调用进程变会话首领，该进程是会话的唯一成员。
  + 新建进程组，PGID就是该进程的PID，会成为该组的首领。但是sid本身不存在这个东西，直接用PGID代替
  + 进程将甩开终端

#### 7.3.3 用ps命令查看进程关系

![](Images/7.png)

### 7.4 系统资源限制

```c++
#include<sys/resource.h>
struct rlimit
{
    rlim_t rlim_cur;
    rlim_t rlim_max;
    //rlim_t是个整数类型，描述资源的级别。
    //cur描述软限制，最好不要超过它，否则可能会发送信号终止运行。
    //max是硬限制，是软限制的上限，普通程序可以减小硬限制，只有以root运行的才能增加。
};

int getrlimit(int resource, struct rlimit * rlim);
int setrlilmit(int resource, const struct rlimit * rlim);
```

![](Images/8.png)

### 7.5 改变工作目录和根目录

+ webserver的逻辑根目录并非文件系统的根目录，而一般是站点的根目录(如：/var/www/)

```c++
#include<unistd.h>
char * getcwd(char * buf, size_t size);
// 获取当前工作目录，buf用于存放目录绝对路径，大小为size，返回NULL或者buf或者指向存放绝对路径的缓存区
// 1. 若路径长度超了，返回NULL，设置errno为ERANGE
// 2. 若buf为NULL，而size!=0，则可能在内部malloc。这之后还得我们自己free它。
int chdir(const char* path);
// 切换工作目录到path，成功0，失败-1，设置errno
int chroot(const char* path);
// path参数指定要切换到的目标根目录。成功时返回0，失败时返回-1并设置errno。这个必须要root权限
// chroot不改变进程的工作目录，基本都是在此之后接着chdir("/")
```

### 7.6 服务器程序后台化

+ 让服务器进程以守护进程的方式运行
+ 详见P139

```c++
#include<unistd.h>
int daemon(int nochdir, int noclose);
// nochdir:是否改变工作目录，0则设置为"/"根目录，否则使用当前目录
// noclose：0表示标准的输入输出和标准的错误输出都被重定向到/dev/null文件，否则使用原来的设备，成功返回0，失败-1，设置errno
```

## <font color = "red">第8章 高性能服务器程序框架</font>

### 8.0 导论

三个模块

+ <font color = "red">I/O处理单元</font>：主要包含4个I/O模型和2个高效事件处理模式
+ <font color = "red">逻辑单元</font>：2个高效并发模式以及高效的逻辑处理方式—有限状态机
+ 存储单元

### 8.1 Server模型

#### 8.1.1 C/S模型

+ 计网里提过，实际C，S在网络传输过程是对等的，区分只在于其主要承担的工作。
+ C/S特点
  + 优势：
    + 适合资源集中型
  + 缺点：
    + 对Server的吞吐量要求挺高。可以采用distributed systems进行效率提高。
+ C/S模型的逻辑
  + Server启动，创建若干监听socket，调用bind绑定到特定server端口
  + 调用listen等待连接
  + Server稳定后，client就可以调用connect发起连接
  + 由于clients的请求往往是随机异步的，Server使用某个I/O模型监听这个event。如I/O复用中的select调用。
  + 接收到了请求，使用accpet接收它，分配逻辑单元为新的连接服务。如弄一个子进程、子线程、或者线程池等。与此同时，主线程/主进程持续监听请求。

+ 例子

  ![](Images/9.png)

#### 8.1.2 P2P模型

+ 特点
  + 优点
    + 资源能充分共享。例子：高性能云计算集群
  + 缺点
    + 传输请求多的时候，网络负载加重。
    + 主机之间难以互相发现。所以实际还有一个专门的发现服务器，提供查找甚至内容提供服务。这就形成了混合模型

![](Images/10.png)

### 8.2 服务器编程框架

![](Images/11.png)
+ Server基本模块的功能描述

| 模块  |对单个服务器程序| 对服务器机群 |
| ---- | ---- | ---- |
|  I/O处理单元    | 处理客户连接，读写网络数据 | 作为接入Server，实现负载均衡 |
| 逻辑单元 | 业务进程或线程 | 逻辑服务器 |
|网络存储单元|本地database，file或者cache|database server|
|请求队列|各单元之间的通信|各服务器之间的永久TCP连接|

#### 8.2.1 I/O处理单元

+ 工作
  + 对单个服务器程序而言：等待、接收client连接，收发客户的data（收发可能会在逻辑单元执行，这取决于采用的事件处理模式）
  + 对服务器机群：作为专门的接入服务器，实现负载均衡，从所有逻辑服务器中选取负荷最小的一台来为新客户服务，这相当于<font color = "red">中介者</font>

#### 8.2.2 逻辑单元

+ 工作
  + 通常作为进程/线程。分析处理client的data，然后根据使用的事件处理模式，将message或者result传给I/O处理单元，或是直接发给client，对server cluster而言，一个逻辑单元就是一台逻辑服务器。服务器通常有多个逻辑单元，实现对多个用户任务的并行处理。

#### 8.2.3 网络存储单元

+ 可以是database，cache或者file，甚至是独立的server。

#### 8.2.4 请求队列

+ 各个单元之间通信方式的抽象。I/O处理单元接收到client请求时，需要以某种方式通知一个逻辑单元处理这个请求。同样的，多个逻辑单元同时访问一个存储单元时，也需要通过它来处理竞态条件。
+ 这一般是一个<font color = "red">池</font>。
+ 对server cluster而言，请求队列是各台server之间预先建立的，静态的，永久的tcp连接。能提高server之间交换数据的效率。

### 8.3 <font color = "red">I/O模型</font>

+ 阻塞和非阻塞适用于包括socket、管道在内的所有文件描述符。将I/O分成了阻塞I/O和非阻塞I/O
+ 在socket的基础API中，可能被阻塞的系统调用包括accept、send、recv、connect。
+ 对非阻塞，调用后立即返回，不管是否发生对应事件，没发生，则一般返回-1，设置errno。这时要根据errno来区分是出错还是确实没发生事件。
  + 对accpet，send，recv
    + 未发生为EAGAIN，EWOUOLDBLOCK
  + 对connect
    + 未发生为EINPROGRESS
+ 

## 第5章 Linux网络编程基础API

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
		现在也能传上面两个与SOCK_NONBLOCK和		SOCK_CLOEXEC相与的结果，前者表示非阻		塞，后者则小时fork出的子进程会关闭			socket
protocol:在前两个参数构成的协议集合下再选择一个		具体的协议，它常常唯一，指定为0即可。
return value：成功则返回一个socket文件描述符，否则返回-1
*/
```

### 5.3 命名socket

+ 创建socket，只是指定了地址族，还得将其与socket地址绑定，这就是所谓命名socket。

+ 对client，它通常不用命名，直接匿名让os自动分配。

  对server则需要。

```c++
#include<sys/types.h>
#include<sys/socket.h>
int bind(int sockfd, const struct sockaddr* my_addr, socklen_t addrlen);
/*
功能：将my_addr所指socket地址分配给未命名的sockfd文件描述符，addrlen指出socket地址的长度
成功返回0，否则-1。
*/
```

### 5.4 监听socket

+ 命名后，还得创建一个监听队列来存放待处理的客户连接

```c++
#include<sys/socket.h>
int listen(int sockfd, int backlog);
/*
sockfd：指定被监听的socket
backlog:提示监听队列的最大长度
returnvalue：成0败-1
*/
```

### 5.5 接收连接

```c++
#include<sys/types.h>
#include<sys/socket.h>
int accept(int sockfd, struct sockaddr *addr, socklen_t *addrlen);
/*
sockfd:执行过listen的socket
addr：用于获取被接受连接的远端socket地址，		server会通过读写该socket来与client通信
returnvalue:成0败-1
*/
```

+ accept并不对连接状态敏感，只单纯从监听队列中取出连接

### 5.6 发起连接

```c++
#include <sys/types.h>
#include <sys/socket.h>
int connect(int sockfd, const struct sockaddr *serv_addr, socklen_t addrlen)
/*
功能：成功连接则返回一个socket，否则返回-1，并置erro。erro要么是ECONNEREFUSED要么ETIMEDOUT
*/
```

### 5.7 关闭连接

```c++
#include<unistd.h>
int close(int fd);
/*
功能：实际是把fd的引用计数减1，只有为0时才关闭连接，由于fork会使得父进程中打开的socket的引用计数加1，所以在父子进程中都要close，才会关闭连接
*/

#include<sys/socket.h>
int shutdown(int sockfd, int howto);
/*
sockfd：待关闭的socket
howto：指明了shutdown的行为。
		SHUT_RD表示关闭读，socket接收缓冲区的		数据全部被丢弃
		SHUT_WR表示关闭写，发送缓冲区的数据全		 部发送之后才关。
		SHUT_RDWR同时关
returnvalue：成0败-1
*/
```

### 5.8 数据读写

#### 5.8.1 TCP数据读写

```c++
#include <sys/types.h>
#include <sys/socket.h>
ssize_t recv(int sockfd, void *buf, size_t len, int flags);
ssize_t send(int sockfd, const void *buf, size_t len, int flags);

/*
表示从sockfd读取到buf或发送buf的内容，len指定了buf的大小，返回成功读写的字符个数。失败返回-1。flags一般设置为0
*/
```

+ flags的一些取值

  ![](Images/3.png)

## 第6章 高级i/o函数