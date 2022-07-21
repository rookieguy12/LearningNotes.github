## Makefile学习

### 1 基本例子

#### 例子1 —— 基本布局

```makefile
# CXXFLAGS += -std=c++11
cc = g++
main:1.o test.o
#执行的命令起手加个tab
	$(cc) -o main 1.o test.o
1.o:1.cpp
test.o:test.cpp
#利用自动推断，省略g++ -c test.o
#有一些固定的变量
$@ #表示目标文件
$^ #表示依赖文件
$< #表示依赖文件的第一个

#PHONY表示伪目标，这样，当真的存在clean文件的时候，依然能执行make clean的命令
.PHONY:clean
clean:
	rm -f *.o *.exe
```
#### 例子2 —— 生成和使用动态库、静态库
```makefile
#生成和使用动态库(Linux下后缀.so，windows下后缀.dll)
#g++ -shared -fPIC libSources.cpp -o libExample.so
#g++ main.cpp -L$(动态库文件所在目录) -lExample -o main
#也可以先生成.o文件，再链接为动态库，涉及多个文件一起生成动态库时建议使用下面的办法
#g++ -c -fPIC libSources.cpp -o source.o
#g++ -shared source.o -o libExample.so
	
#生成和使用静态库
.PHONY: build test
build: libmylib.a
libmylib.a: my_math.o my_print.o
	ar crv $@ my_math.o my_print.o
my_math.o: my_math.c
	gcc -c my_math.c
my_print.o: my_print.c
	gcc -c my_print.c
test: a.out
a.out: test.c
	gcc test.c -L. -lmylib
```


#### 例子3 —— 规范化
```makefile
#源文件，自动找所有.c和.cpp文件，并将目标定义为同名.o文件
SOURCE  := $(wildcard *.c) $(wildcard *.cpp)
OBJS    := $(patsubst %.c,%.o,$(patsubst %.cpp,%.o,$(SOURCE))) 
#target you can change test to what you want
#目标文件名，输入任意你想要的执行文件名
TARGET  := test
#compile and lib parameter
#编译参数
CC      := g++      #编译器
LIBS    :=          #链接哪些库
LDFLAGS :=          #lib库路径
DEFINES :=
INCLUDE := -I.
CFLAGS  := -g -Wall -O3 $(DEFINES) $(INCLUDE)  #-g -Wall -O3 -Iinclude
CXXFLAGS:= $(CFLAGS) -DHAVE_CONFIG_H         #CFLAGS 表示用于 C 编译器的选项，CXXFLAGS 表示用于 C++ 编译器的选项
#i think you should do anything here
#下面的基本上不需要做任何改动了
.PHONY : everything objs clean veryclean rebuild
everything : $(TARGET)
all : $(TARGET)
objs : $(OBJS)
rebuild: veryclean everything
clean :
    rm -fr *.so
    rm -fr *.o
veryclean : clean
    rm -fr $(TARGET)
$(TARGET) : $(OBJS)
    $(CC) $(CXXFLAGS) -o $@ $(OBJS) $(LDFLAGS) $(LIBS)
#2、生成静态链接库的makefile
######################################

#

#

######################################
#target you can change test to what you want
#共享库文件名，lib*.a
TARGET  := libtest.a
#compile and lib parameter
#编译参数
CC      := gcc
AR      = ar
RANLIB  = ranlib
LIBS    :=
LDFLAGS :=
DEFINES :=
INCLUDE := -I.
CFLAGS  := -g -Wall -O3 $(DEFINES) $(INCLUDE)
CXXFLAGS:= $(CFLAGS) -DHAVE_CONFIG_H
#i think you should do anything here
#下面的基本上不需要做任何改动了
#source file
#源文件，自动找所有.c和.cpp文件，并将目标定义为同名.o文件
SOURCE  := $(wildcard *.c) $(wildcard *.cpp)
OBJS    := $(patsubst %.c,%.o,$(patsubst %.cpp,%.o,$(SOURCE)))
.PHONY : everything objs clean veryclean rebuild
everything : $(TARGET)
all : $(TARGET)
objs : $(OBJS)
rebuild: veryclean everything       
clean :
    rm -fr *.o
veryclean : clean
    rm -fr $(TARGET)
$(TARGET) : $(OBJS)
    $(AR) cru $(TARGET) $(OBJS)
    $(RANLIB) $(TARGET)
#3、生成动态链接库的makefile
#target you can change test to what you want
#共享库文件名，lib*.so
TARGET  := libtest.so
#compile and lib parameter
#编译参数
CC      := gcc
LIBS    :=
LDFLAGS :=
DEFINES :=
INCLUDE := -I.
CFLAGS  := -g -Wall -O3 $(DEFINES) $(INCLUDE)
CXXFLAGS:= $(CFLAGS) -DHAVE_CONFIG_H
SHARE   := -fPIC -shared -o
#i think you should do anything here
#source file
#源文件，自动找所有.c和.cpp文件，并将目标定义为同名.o文件
SOURCE  := $(wildcard *.c) $(wildcard *.cpp)
OBJS    := $(patsubst %.c,%.o,$(patsubst %.cpp,%.o,$(SOURCE)))
.PHONY : everything objs clean veryclean rebuild
everything : $(TARGET)
all : $(TARGET)
objs : $(OBJS)
rebuild: veryclean everything              
clean :
    rm -fr *.o
veryclean : clean
    rm -fr $(TARGET)
$(TARGET) : $(OBJS)
    $(CC) $(CXXFLAGS) $(SHARE) $@ $(OBJS) $(LDFLAGS) $(LIBS)
```

### 2 Makefile总述

#### 2.1 文件内容

+ 显式规则
+ 隐式规则
+ 变量定义
+ 文件指示
+ 注释

#### 2.2 Makefile的文件名

+ 默认是makefile，用了别的alias就得用make -f alias

#### 2.3 引用其他Makefile

```makefile
include foo.make *.mk $(bar)
#bar表示了几个mk文件

#指定路径
include -I path
```

### 3 书写规则

#### 3.1 规则举例

#### 3.3 使用通配符

+ 三个通配符

  + \* 匹配0个或者是任意个字符，和%一样
  + ？匹配任意一个字符
  + []指定匹配的字符放在 "[]" 中

+ ```makefile
  #用*命名变量时，需要用wildcard修饰，防止它被认为是*.c文件
  OBJ=*.c
  test:$(OBJ)
  	gcc -o $@ $^
  	
  #正确写法
  OBJ=$(wildcard *.c)
  test:$(OBJ)
  	gcc -o $@ $^
  ```

+ ```makefile
  #静态模规则演示
  %.o : %.c
  	gcc -o $@ $^
  ```

### 4 变量

#### 4.1 定义&使用

```makefile
value = one two three
test : $(value)
	gcc -o test $(value)
	@echo $(value)
```

#### 4.2 赋值

+ 简单赋值:=
+ 递归赋值=
  + 所有目标变量相关的其他变量都会受影响
+ 条件赋值?=
  + 如果变量没定义，则赋值；否则不赋
+ 追加赋值+=
  + 原变量用空格隔开的方式追加一个新值

#### 4.3 自动化变量

+ 就是makefile自动产生的变量

+ 他们还能加上D,F来表示文件的目录部分还是文件本身的名字(带后缀)

  + $(@D)对于dir/foo.o就是dir
  + $(@F)则是foo.o
  
  | 自动化变量 |                             说明                             |
  | :--------: | :----------------------------------------------------------: |
  |     $@     |          表示目标文件名，如果是静态库，那就是文件名          |
  |     $%     |    当目标文件是一个静态库文件时，代表静态库的一个成员名。    |
  |     $<     |                   规则的第一个依赖的文件名                   |
  |     $?     | 所有比目标文件更新的依赖文件列表，空格分隔。如果目标文件时静态库文件，代表的是库文件（.o 文件）。 |
  |     $^     | 所有依赖文件列表，使用空格分隔。静态库文件，那就是文件名。不重复记录 |
  |     $+     |                       同$^，但重复记录                       |
  |     $*     | 在模式规则和静态模式规则中，代表“茎”。“茎”是目标模式中“%”所代表的部分（当文件名中存在目录时，“茎”也包含目录部分）。 |
  
    ```makefile
  test:test.o test1.o test2.o
             gcc -o $@ $^
  test.o:test.c test.h
             gcc -o $@ $<
  test1.o:test1.c test1.h
             gcc -o $@ $<
  test2.o:test2.c test2.h
             gcc -o $@ $<
    ```

### 5 文件搜索(VPATH和vpath)

+ VPATH是环境变量，指定后make会在VPATH目录下查找；

  ```makefile
  VPATH = src car
  test:test.cpp
  	gcc $@ @^

+ vpath是关键字，用于对指定目录下的文件搜索添加限制信息，狗哦率并找出指定文件

  ```makefile
  #表示在指定目录下搜索符合PATTERN的文件
  vpath PATTERN Directory1 Directory2
  #清除符合PATTERN的搜索目录
  vpath PATTERN
  #清楚所有设置的搜索目录
  vpath
  ```

+ 使用实例

  ```makefile
  vpath %.c src
  vpath %.h include
  main:main.o list1.o list2.o
      gcc -o $@ $<
  main.o:main.c
      gcc -o $@ $^
  list1.o:list1.c list1.h
      gcc -o $@ $<
  list2.o:list2.c list2.h
      gcc -o $@ $<
  ```

### 6 Makefile的隐含规则

+ 省略中间目标文件.o的生成规则

  ```makefile
  test:test.o
  	g++ -o test test.o
  ```

+ 内嵌隐含规则里有很多预定义的变量
  - AR：函数库打包程序，可创建静态库 .a 文档。
  - AS：应用于汇编程序。
  - CC：C 编译程序。
  - CXX：C++编译程序。
  - CO：从 RCS 中提取文件的程序。
  - CPP：C程序的预处理器。
  - FC：编译器和与处理函数 Fortran 源文件的编译器。
  - GET：从CSSC 中提取文件程序。
  - LEX：将Lex语言转变为 C 或 Ratfo 的程序。
  - PC：Pascal 语言编译器。
  - YACC：Yacc 文法分析器（针对于C语言）
  - YACCR：Yacc 文法分析器

### 7 条件判断 & 递归循环

+ ifeq、ifneq、ifdef、ifndef、else、endif

  ```makefile
  libs_for_gcc= -lgnu
  normal_libs=
  foo:$(objects)
  ifeq($(CC),gcc)
      $(CC) -o foo $(objects) $(libs_for_gcc)
  else
      $(CC) -o foo $(objects) $(noemal_libs)
  endif
  ```

+ ifdef主要是判断变量的值是不是空，不过最好还是用ifeq来判断

+ 循环for

  ```makefile
  SUBDIRS=foo bar baz
  subdirs:
      for dir in $(SUBDIRS);do $(MAKE) -C $$dir;done
      
  #利用伪目标实现并行化和出错可定位化
  SUBDIRS=foo bar baz
  .PHONY:subdirs $(SUBDIRS)
  subdirs:$(SUBDIRS)
  $(SUBDIRS):
      $(MAKE) -C $@
  foo:baz
  ```

  

### 8 Makefile伪目标

+ 主要是以不创建目标文件的方式去执行目标下面的命令

  ```makefile
  .PHONY:clean
  clean:
  	rm -rf *.o
  ```

+ 实现多文件编辑

  ```makefile
  .PHONY:all
  all:test1 test2 test3
  test1:test1.o
      gcc -o $@ $^
  test2:test2.o
      gcc -o $@ $^
  test3:test3.o
      gcc -o $@ $^
  ```

### 9 常用字符串处理函数

+ 函数调用方式

  ```makefile
  $(<function> <arguments>)
  #也可以是花括号
  #比如
  $(wildcard *.c)
  ```

+ 常用函数

  + patsubst模式字符串替换

    ```makefile
    #把text中的单词中符合pattern的单词用replacement替换
    $(patsubst <pattern>,<replacement>,<text>)
    
    OBJ=$(patsubst %.c,%.o,1.c 2.c 3.c)
    all:
        @echo $(OBJ)
    #输出1.o 2.o 3.o
    ```

  + subst字符串替换，就是上面的单纯替换版了，接口还是一样的

  + strip去空格

    ```makefile
    $(strip <string>)
    #去除string的前面、后面的空格、合并中间部分的空格
    ```

  + findstring查找字符串函数

    ```makefile
    $(findstring <find>,<in>)
    ```

  + filter过滤

    ```makefile
    $(filter <pattern>,<text>)
    ```

  + filter-out反过滤(筛选)

    ```makefile
    $(filter-out <pattern>,<text>)
    ```

  + sort去重的升序排序

    ```makefile
    $(sort <list>)
    ```

  + word取出单词

    ```makefile
    $(word <n>,<text>)
    ```

+ 常用文件名操作函数

  + 取目录函数

    ```makefile
    $(dir <names>)
    #对于names里写了目录的，就返回最后一个/及其之前的部分，其余就是./
    ```

  + 取文件函数

    ```makefile
    $(notdir <names>)
    #对于names里写了目录的，就是返回最后一个/之后的部分
    ```

  + 取后缀函数

    ```makefile
    $(suffix <names>)
    ```

  + 取前缀函数

    ```makefile
    $(basename <names>)
    #注意是带了路径的
    ```

  + 加后缀函数

    ```makefile
    $(addsuffix <suffix>, <names>)
    ```

  + 加前缀函数

    ```makefile
    $(addprefix <prefix>, <names>)
    ```

  + 连接函数

    ```makefile
    $(join <list1>, <list2>)
    ```

    









