# Java核心技术

## 第0章 笔记策略

+ 基于C++的对比学习方式。没特别说的直接按C++的类比。
+ 只对关键内容作重要记录。语法类采取举例方式。底层部分详细记录。其余杂项略作说明。
+ 有些设计模式的内容不多赘述，评价是不如直接看设计模式。

## 第1章 Java程序设计概述

+ Java的可移植性：
  + 与 C 和 C++ 不同，Java 规范中没有“ 依赖具体实现” 的地方。基本教据类型的大小以及有关运算都做了明确的说明 。
  + 二进制数据以固定的格式进行存储和传输， 消除了字节顺序的困扰。字 符串是用标准的 Unicode 格式存储的。
  + 跨平台能力。

## 第2章 Java程序设计环境

+ JDK是编写要用的；JRE是运行要用到的；SE是桌面、简单服务器要用的Java平台。

```bash
# 编译命令，生成对应的XXX.class。这里XXX和类名一致。大小写敏感
javac XXX.java
# 运行命令，不带class后缀
java XXX
```

## 第3章 Java的基本程序设计结构

### 3.1 一个简单Java程序

```Java
/**
 *
 * File: Example.java文件名和公共类名一致，大小写敏感
 */
public class Example 
{
    // main必须是public的
	public static void main(String[] args)
    {
        System.out.println("Hello world.");
    }
}
```

### 3.3 数据类型

+ 整型：byte（1字节）、short（2字节）、int（4字节）、long（8字节）。

  + 整型后缀可以加L、l。
  + 前缀加0b可以写二进制的；还可以加下划线。八进制的前缀是0；16进制的前缀是0x

+ 浮点：float（4字节）、double（8字节）

  + float浮点后面可以有F、f；
  + double浮点后面可以加D、d；没有的默认是double
  + 指数用p，而不是e。比如0x1.0p-3就是0.125
  + Double.POSITIVE_INFINITY、Double.NEGATIVE_INFINITY、Double.NaN表示正负无穷和NaN。
    + 一般用Double.isNaN(x)函数来判断

+ 字符类型：char（2字节）

  + 比如\uXXX，就可能表示一个字符的Unicode转义序列。
  + 现在最好用String之类的字符串，不用char

+ 布尔类型：boolean

+ 枚举类型

  ```java
  enum Size {SMALL, MEDIUM, BIG};
  Size s = Size.MEDIUM;
  ```

### 3.6 字符串

```Java
String e = "abcdefghijklmn";
// 子串
String sub = e.substring(0, 3);	// abc
// 拼接
String add = e + sub;
// 判等号，不用==
"abceaf".equals(e);	// false
// 取某个字符
e.charAt(3); // d
// 其他常用API
int compareTo(String other);	//按字典序返回比较的正负，相同则0，在other之后则负。
boolean startsWith(String prefix);
boolean endsWith(String suffix);
int indexOf(String str, int fromtIndex);
int indexOf(int codePoint, int fromIndex);
int lastindexOf(int codePoint, int fromIndex);
```

+ String是不可变的，不像C++直接改字符。实际是一个共享内存之类的东西。而不是char[]数组的实现

### 3.9 大数值

+ BigInteger实现任意精度的整数运算
+ BigDecimal实现任意精度的浮点运算

### 3.10 数组

```java
int [] a = new int[100];	// 类比 int *a = new int[100];
int [] b = {1,3,45,5};
new int[] {12,3,31,24};

// 多维数组
int [][]c = new int[100][200];
int [][]d = new int[100][];

// copy
int [] temp = Arrays.copyOf(b, 2 * b.length);
// quick sort
Array.sort(a);
```

### 3.11 命令行参数

```bash
# args为-g,cruel,world
java Example -g cruel world
```

### 3.x 其他杂项

```java
import java.util.*;
public class Example 
{
    // public static final类常量
    public static final double CM_PRE_INCH_TOTAL = 2.56;
    public static void main(String [] args) 
    {
        // 表示常量，不用const
        final double CM_PRE_INCH = 2.5;
        
        // 强制类型转换
        double x = 9.9;
        int nx = (int)Math.round(x);
        
        // 标准输入流
        Scanner in = new Scannner(System.in);
        String name = in.nextLine();
        String first = in.next();
        int age = in.nextInt();
        double x = in.nextDouble();
        
        // 文件输入输出流
        // 第一个参数也可以是String，意味着就是把这个String作为来源
        Scanner inFile = new Scanner(Path.get("myfile.txt"), "UTF-8");
        
        PrintWriter out = new PrintWriter("myfileOutput.txt", "UTF-8");
    }
}
```

+ Java不用逗号运算符

## 第4章 对象与类

+ 注意局部变量不会自动初始化为null，需要手动设置。

+ 类似C#，Java里有所谓访问器、更改器的说法，当然访问器返回的东西要不可变才合适，不破坏封装。

  ```java
  public Date GetHireDay()
  {
      return (Date)hireDay.clone();
  }
  ```

+ 和C#类似，对象也都是引用类型，这意味着总是要用new来产生一个新的对象。

+ LocalDate类可以用于时间处理。

+ <font color = red>在一个源文件里，可以有多个非public类，只能有一个public类，该public类的名字和文件名一致。编译后也会生成对应的非pubilc类.class。一般地，可以一个类放一个源文件里，在使用javac编译的时候，会查找使用的类的class文件，找不到就找类.java再编译出来。</font>

+ final关键字常用语定义常量、不可变域、基本类型域，这种一般是要在构造函数初始化的，类似C++的const指针，注意不是指向const的指针。

+ java里也有static类成员，和C++一致，不赘述。static方法也是一样的，只能访问static成员。java中，常用于定义工厂方法。

+ 每一个class都可以有一个main方法，也是一个static类方法。常用于单元测试。

+ java的this和C++类似，不过用的时候按照引用的方式来用，理解的时候按指针。

+ java还有初始化块这种东西。用一个{}括起来单独出现在class里的，每次构造都会被执行。构造一个class对象的时候，先赋系统默认值，再按声明顺序执行初始化语句，再执行初始化块，最后执行构造函数。

+ Java自动GC，不用管，没析构，但有fanalize函数。

+ Java没有运算符重载。

+ 访问修饰

  + private：仅本类可见
  + public：所有类可见
  + protected：对子类、本包可见
  + 无修饰符：对本包可见


### 4.5 方法参数

+ <font color = red>java的函数参数总是值传递。</font>对象引用也是按照值传递的，不过传递的东西应该类比的是C++指针，而不是C++的引用传递，形参和实参指的同一个内容。

### 4.7 包（package）

+ 类似python的引入方式，在C++中类比namespace。

+ 引入方式：

  ```java
  // 引入一个java包
  import java.lang.*;
      
  // 静态引入
  import static java.lang.System.*;
  ```

+ 把类放入包中

  ```java
  // 把Employee放入包中，没写的话是放默认包中。Employee.java文件也放在./com/hosrtmann/corejava里面。
  package com.hosrtmann.corejava;
  
  public class Employee
  {
      ...
  }
  ```

  + 编译器不对文件的放置做检查，它不依赖其他类就可能能编译成功。总之还是确保文件放置路径正确

+ 一个类不加public时，就默认是同一个包里的类可以用

## 第5章 继承

+ 使用extend关键字继承。<font color = red>在Java中，所有继承都是公有继承。</font>

+ 在调用已经在子类重写过的父类接口时，使用<font color = red>super.XXX</font>来调用。

+ 子类重写时，在方法前面加@override做标记。

  ```java
  @Override public boolean equals(Object otherObject)
  ```

+ 子类构造时，需要把基类的构造放在函数体里面，也是<font color = red>super(...)。</font>不写默认用super()。

+ Java里面都是默认虚函数的。除非声明为<font color = red>final</font>，可以把类定义为final，也可以把函数定义为final 。维护的时候使用的是方法表，类似虚函数表。

+ 众所周知，Java不支持多继承。

+ 方法调用的过程和C++类似，重载也涉及名称重整，重写也有方法表。

+ 如果要进行向下转型，记得用<font color = red>instanceof关键字</font>来判断是不是某个类的实例，这是一种反射。

+ <font color =red>抽象类</font>：java里有抽象类，类似C#，方法也必须声明为抽象函数。C++中则是用纯虚函数。

  ```java
  public abstract class Example 
  {
      public abstract String getDescription();
  };
  ```

### 5.2 Object类

+ 是所有类的基类。

+ 可引用任何对象类型。

+ 常用方法：

  ```java
  // 判断是否是相同的引用。有时候只想用特定值作为判据时是要重写的。
  // 可能要用到instanceof，还有向下转型，记得判空。可能还得用getClass()
  public boolean equals(Object otherObject);
  
  // 每个对象都有一个默认的散列码，表示对象的存储地址。也可以自己定义散列码的值，相同的对象应当具有相同的散列码，这可以作为equals的依据。
  public int hashCode();
  public static int hashCode(...);
  
  // 表示对象字符串描述。常用于调试。Java对于用+号连起来的对象，会自动调用它的toString方法。
  public String toString();
  // 数组也是对象类型，它有自己的toString，如果想让成员分别toString，就用下面这个
  Arrays.toString(...);
  ```

### 5.4 对象包装器和自动装箱

+ 类似C#，装箱拆箱就是把基本类型变成对象类型及其相反操作。
+ 各类型的对应wrapper
  + Integer --- int
  + Long --- long
  + Float --- float
  + Double --- double
  + Short --- short
  + Byte --- byte
  + Character --- char
  + Void --- void
  + Boolean --- boolean
+ <font color = red>自动装箱要求boolean、byte、char在127以下，short、int在-128~127。</font>这是编译器的设计。

### 5.5 可变参数

```java
public class PrintStream
{
    public PrintStream printf(String fmt, Object... args)
    {
        return format(fmt, args);
    }
}
```

### 5.6 枚举类

```java
public enum Size
{
    SMALL("S"),MEDIUM("M"),LARGE("L"),EXTRA_LARGE("XL");
    
    // 甚至可以定义函数、以及其他成员
}

Size.SMALL.toString();	//SMALL
Size s = Enum.valueOf(Size.class, "SMALL");	// s为Size.SMALL
Size[] values = Size.values();	// 包含了所有枚举值的数组
Size.MEDIUM.ordinal;	// 为1，描述了枚举常量的位置
s.compareTo(Size.SMALL);	// s出现位置在SMALL前面就是负数、等于就0、后面就是大于0
s == Size.SMALL;	//True
```

### 5.7 反射

#### 5.7.1 Class类

+ 系统维护了一个运行时的类型标识，可以跟踪每个对象所属的类。由Class类管理

```java
Employee e;
Class c = e.getClass(); // Class c = Employee.class;可直接用于判等==
c.getName();	// Employee，前面有包的话会包括包名
Class cl = Class.forName("Employee");// 甚至可根据名字获得一个对应的Class
cl.newInstance(...);	// 创建了一个Employee对象
```

+ TODO：优先级较低，待完善补充。

## 第6章 接口、lambda表达式与内部类

### 6.1 接口

+ 和C#类似。只是实现接口的类要用implements，不是extends。接口里面还可以定义常量。

```java
public interface Comparable
{
    int compareTo(Object other);	// 接口方法都是自动public
}

// 这种可能更符合设计模式里单例的生成思路。
public interface Comparable<T>
{
    int compareTo(T other);
}
```



## 第7章 异常、断言和日志

## 第8章 泛型程序设计

## 第9章 集合

## 第10章 图形程序设计

## 第11章 事件处理

## 第12章 Swing用户界面组件

## 第13章 部署Java应用程序

## 第14章 并发