## Makefile学习

### 1 基本例子

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

#### 2.4 环境变量

### 3 书写规则

#### 3.1 规则举例

#### 3.3 使用通配符

+ 三个通配符

  + \* 匹配0个或者是任意个字符，和%一样
  + ？匹配任意一个字符
  + []指定匹配的字符放在 "[]" 中

+ ```makefile
  #用*命名变量时，需要用wildcard修饰
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
```

#### 4.2 赋值

+ 简单赋值:=
+ 递归赋值=
  + 所有目标变量相关的其他变量都会受影响
+ 条件赋值?=
  + 如果变量没定义，则赋值；否则不赋
+ 追加赋值+=
  + 原变量用空格隔开的方式追加一个新值





















