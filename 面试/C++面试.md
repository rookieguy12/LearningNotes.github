# C++面试

#### define，const，enum，inline的区别

1. 编译器的处理方式不同

   define在预处理阶段对定义的常量进行替换展开

   const在编译运行阶段使用

   enum在程序运行时起作用

2. 分配内存不同

   define不分配内存，只进行替换展开。

   const常量会分配内存，在堆或者栈

   enum则把内存分配在静态存储区

3. 类型检查方面

   define无类型检查和安全检查，会导致边际效应，出现不可预知的错误。

   const在编译阶段进行类型检查和安全检查

   enum在编译阶和运行阶段段类型检查，但只能进行整形的定义

   inline进行参数类型的安全检查

一般都不使用宏，对于变量，一般是使用enum和const来替换它。对于形似函数的宏定义，则是使用inline替换。

#### 指针和引用的联系和区别

1. 指针可以为空，引用也行。
2. 指针可以不初始化，但引用必须初始化
3. 指针可以随时改变指向的目标，但引用不行

#### 系统如何知道指针越界？

1. VC下有一个结构体_CrtMemBlockHeader，里面有一个Gap属性，这个Gap数组放在你的指针数据的后面，默认为0xFD，当检测到你的数据后不是0xFD的时候就说明的你的数据越界了

#### C++编译器常见的优化

1. 常量替换：

   int a = 2； int b = a； return b；优化成return 2

2. 无用代码消除。若返回值与参数与之无关，则直接优化掉。

3. RVO和NRVO：

   RVO：return value optimization，返回值优化

#### C++模板的几个类型

```c++
template <typename T1, typename T2>
void Print(T1 x,T2 y)
{
    cout << x;
    cout << y;
}

template <typename T>
T add(T x, T y)
{
    return x + y;
}

//模板的显示具体化
template<> add<int>(int x, int y)
{
    return x + y;
}

//模板的部分具体化
template <typename T2>
void Print<int>(int x,T2 y)
{
    cout << x;
    cout << y;
}

//非类型模板参数必须是整形类数据（bool、char、int、long、long long，enum）或者指向外部链接对象的指针
template <typename T,int num=10>
int func(T x,T y)
{
    return x*y*num;
}

//默认模板参数
template<typename T=int> void testTemplateFunc(T param)
{
	cout<<"TemplateFunc's param="<<param<<endl;
}
//类的成员模板
class X
{
   template <class T> void mf(T* t) {}
};
```

