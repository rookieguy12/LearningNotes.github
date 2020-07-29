// C# notes
using system;
dynamic d = 20;	//动态类型，可储存任何类型的值

Console.ReadLine();
Console.WriteLine();
Convert.ToInt32(Console.ReadLine());

class ForEachTest
{
	public void swap(ref int x, ref int y)
	{
		int temp;
		temp = x;
		x = y;
		y = x;
	}	//按引用传递参数,用ref关键字
	public void getvalues(out int x, out int y)
	{
		Console.WriteLine("请输入第一个值： ");
        x = Convert.ToInt32(Console.ReadLine());
        Console.WriteLine("请输入第二个值： ");
        y = Convert.ToInt32(Console.ReadLine());
    }	//输出参数，传入的参数可不初始化
    static void Main(string[] args)
    {
        int[] fibarray = new int[] { 0, 1, 1, 2, 3, 5, 8, 13 };	//C#的array定义方法
        double[] balance = new double[10];
		double[] balance = new double[] {4,2,4,5};
		int [,] a = new int [3,4]	//二维数组
		a[2,3] = 5; //访问元素的办法
		int[][] scores = new int[2][] {new int[] {92,93,94}, new int[] {1,2,3,4,5}};	//交错数组
		
		ForEachTest class1 = new ForEachTest();		//new的用法
		foreach (int element in fibarray)
        {
            System.Console.WriteLine(element);
        }
		int? num1 = null;	//可空类型
		int? num2 = 3;	
		int num3 = num1 ?? num2;//Null合并运算符,第一个为null，就返回第二个，否则返回第一个
	}
}	//foreach 循环

//C#类与接口
//1.类继承类和接口
public Tabletop(double l, double w) : base(l, w){ };//Tabletop是一个派生的类，C#用base表示基类，C#不支持多重继承

//2.密封类sealed
sealed class classname{
}//sealed关键字声明一个密封类，表示无法继承

//3.接口interface
//一个类只能bai继承一个类但是可以继承多个接口
//接口interface
public interface PaintCost
   {
      int getCost(int area);
   }
   
//4.抽象类abstract
 abstract class Shape
   {
       abstract public int area();//抽象函数只能在抽象类里面声明；
   }//abstract方法在继承的类里面用override重写
   
//5.虚函数，也是在继承类里面用override重写,但是不像C++在使用指针或者引用调用函数方法时才显示多态性，
namespace PolymorphismApplication
{
   class Shape
   {
      protected int width, height;
      public Shape( int a=0, int b=0)
      {
         width = a;
         height = b;
      }
      public virtual int area()
      {
         Console.WriteLine("父类的面积：");
         return 0;
      }
   }
   class Rectangle: Shape
   {
      public Rectangle( int a=0, int b=0): base(a, b)
      {

      }
      public override int area ()
      {
         Console.WriteLine("Rectangle 类的面积：");
         return (width * height);
      }
   }
   class Caller
   {
      public void CallArea(Shape sh)
      {
         int a;
         a = sh.area();
         Console.WriteLine("面积： {0}", a);
      }
   }  
   class Tester
   {
      static void Main(string[] args)
      {
         Caller c = new Caller();
         Rectangle r = new Rectangle(10, 7);
         c.CallArea(r);
         Console.ReadKey();
      }
   }
//6.运算符重载
public static Box operator+ (Box b, Box c)
{
   Box box = new Box();
   box.length = b.length + c.length;
   box.breadth = b.breadth + c.breadth;
   box.height = b.height + c.height;
   return box;
}

//命名空间
//1.C#命名空间访问是用.,而不是::
//2.using system;不用using namespace system;

//预处理
//1.基本预处理命令
#define
#undef
#if
#elif
#endif
//2.#warning和#error
/*当编译器遇到它们时，会分别产生警告或错误。如果编译器遇到#warning 指令，
会给用户显示 #warning 指令后面的文本，之后编译继续进行。如果编译器遇到#error
指令，就会给用户显示后面的文本，作为一条编译错误消息，然后会立即退出编译。
使用这两条指令可以检查 #define 语句是不是做错了什么事，使用 #warning 语句
可以提醒自己执行某个操作。*/


//委托
//1.例子
using System;
delegate double Function(double x);//关键字为delegate，像函数指针或者函数引用之类的一个东西，此处相当于把Function定义为一个专门的类型，
class Multiplier
{
    double factor;
    public Multiplier(double factor)
    {
        this.factor = factor;
    }
    public double Multiply(double x)
    {
        return x * factor;
    }
}
class DelegateExample
{
    static double Square(double x)
    {
        return x * x;
    }
    static double[] Apply(double[] a, Function f)//Function就像一个类型，不过传递的是函数
    {
        double[] result = new double[a.Length];
        for (int i = 0; i < a.Length; i++) result[i] = f(a[i]);
        return result;
    }
    static void Main()
    {
        double[] a = {0.0, 0.5, 1.0};
        double[] squares = Apply(a, Square);
        double[] sines = Apply(a, Math.Sin);
        Multiplier m = new Multiplier(2.0);
        double[] doubles =  Apply(a, m.Multiply);
    }
}
//2.多播
class TestDelegate
   {
      static int num = 10;
      public static int AddNum(int p)
      {
         num += p;
         return num;
      }

      public static int MultNum(int q)
      {
         num *= q;
         return num;
      }
      public static int getNum()
      {
         return num;
      }

      static void Main(string[] args)
      {
         // 创建委托实例
         NumberChanger nc;
         NumberChanger nc1 = new NumberChanger(AddNum);
         NumberChanger nc2 = new NumberChanger(MultNum);
         nc = nc1;
         nc += nc2;
         // 调用多播
         nc(5);
         Console.WriteLine("Value of Num: {0}", getNum());
         Console.ReadKey();
      }
   }//例子
   
   
   
   
   
   
   
   
   
//lambda表达式












//C#属性
//1.访问器Accessors
//可用于实现对私有域的访问，读取修改
using System;
namespace runoob
{
   class Student
   {
      /*private string code = "N.A";
      public string Code
      {
         get
         {
            return code;
         }
         set
         {
            code = value;
         }
      }*/现在直接public string Code{get;set}就行了
   }
}
//C#索引器
//例子
using System;
namespace IndexerApplication
{
   class IndexedNames
   {
      private string[] namelist = new string[size];
      static public int size = 10;
      public IndexedNames()
      {
         for (int i = 0; i < size; i++)
         namelist[i] = "N. A.";
      }
      public string this[int index]//索引器直接用this代替到时候实例化的对象，并利用访问器。实现和数组相近的效果。它当然还能重载，看应用
      {
         get
         {
            string tmp;

            if( index >= 0 && index <= size-1 )
            {
               tmp = namelist[index];
            }
            else
            {
               tmp = "";
            }

            return ( tmp );
         }
         set
         {
            if( index >= 0 && index <= size-1 )
            {
               namelist[index] = value;
            }
         }
      }

      static void Main(string[] args)
      {
         IndexedNames names = new IndexedNames();
         names[0] = "Zara";
         names[1] = "Riz";
         names[2] = "Nuha";
         names[3] = "Asif";
         names[4] = "Davinder";
         names[5] = "Sunil";
         names[6] = "Rubic";
         for ( int i = 0; i < IndexedNames.size; i++ )
         {
            Console.WriteLine(names[i]);
         }
         Console.ReadKey();
      }
   }
}











//特性Attribute
//一，菜鸟教程学习笔记
//1.预定义特性
//1.1  AttributeUsage
//AttributeUsage 描述了如何使用一个自定义特性类
[AttributeUsage(
   validon,
   AllowMultiple=allowmultiple,
   Inherited=inherited
)]
参数 validon 规定特性可被放置的语言元素。它是枚举器 AttributeTargets 的值的组合。默认值是 AttributeTargets.All。
参数 allowmultiple（可选的）为该特性的 AllowMultiple 属性（property）提供一个布尔值。如果为 true，则该特性是多用的。默认值是 false（单用的）。
参数 inherited（可选的）为该特性的 Inherited 属性（property）提供一个布尔值。如果为 true，则该特性可被派生类继承。默认值是 false（不被继承）。
//例子
[AttributeUsage(AttributeTargets.Class |
AttributeTargets.Constructor |
AttributeTargets.Field |
AttributeTargets.Method |
AttributeTargets.Property, 
AllowMultiple = true)]

//1.2    Conditional
//该预定义特性标记了一个条件方法，其执行依赖于指定的预处理标识符。
//例子
#define DEBUG
using System;
using System.Diagnostics;
public class Myclass
{
    [Conditional("DEBUG")]//若是DEBUG没有define，那就无法执行
    public static void Message(string msg)
    {
        Console.WriteLine(msg);
    }
}
class Test
{
    static void function1()
    {
        Myclass.Message("In Function 1.");
        function2();
    }
    static void function2()
    {
        Myclass.Message("In Function 2.");
    }
    public static void Main()
    {
        Myclass.Message("In Main function.");
        function1();
        Console.ReadKey();
    }
}

//1.3   Obsolete
//这个预定义特性标记了不应被使用的程序实体。它可以让您通知编译器丢弃某个特定的目标元素。
[Obsolete(message)]
[Obsolete(message,iserror)]
//参数 message，是一个字符串，描述项目为什么过时以及该替代使用什么
//参数 iserror，是一个布尔值。如果该值为 true，编译器应把该项目的使用当作一个错误。默认值是 false（编译器生成一个警告）

//2.自定义特性
//自定义特性必须派生于Attribute类
//例子
[AttributeUsage(AttributeTargets.Class |
AttributeTargets.Constructor |
AttributeTargets.Field |
AttributeTargets.Method |
AttributeTargets.Property,
AllowMultiple = true)]

public class DeBugInfo : System.Attribute
{
  private int bugNo;
  private string developer;
  private string lastReview;
  public string message;

  public DeBugInfo(int bg, string dev, string d)
  {
      this.bugNo = bg;
      this.developer = dev;
      this.lastReview = d;
  }

  public int BugNo
  {
      get
      {
          return bugNo;
      }
  }
  public string Developer
  {
      get
      {
          return developer;
      }
  }
  public string LastReview
  {
      get
      {
          return lastReview;
      }
  }
  public string Message
  {
      get
      {
          return message;
      }
      set
      {
          message = value;
      }
  }
}

//二，https://www.cnblogs.com/rohelm/archive/2012/04/19/2456088.html学习笔记
//1.例子
[System.Serializable]//特性放在[]中，将其置于元素前，一个元素可以有多个特性
public class SampleClass
{
    // Objects of this type can be serialized.
}


//2.根据约定，所有特性名称都以单词“Attribute”结束，
//以便将它们与“.NET Framework”中的其他项区分。但是，
//在代码中使用特性时，不需要指定Attribute 后缀。
[Conditional("DEBUG"), Conditional("TEST1")]//ConditionalAttribute才是本来的名称，by the way,ConditionalAttribute是一个可多次用的特性，一般一个特性只能用一次
void TraceMethod()
{
    // ...
}


//3.特性参数
//许多特性都有参数，而这些参数可以是定位参数、未命名参数或命名参数。就像函数参数一样
//任何定位参数都必须按特定顺序指定并且不能省略，而命名参数是可选的且可以按任意顺序指定。
//首先指定定位参数。以下三者等价
[DllImport("user32.dll")]
[DllImport("user32.dll", SetLastError=false, ExactSpelling=false)]
[DllImport("user32.dll", ExactSpelling=false, SetLastError=false)]
//第一个参数（DLL的名称）是定位参数并且总是第一个出现，其他参数为命名参数。


//4.特性目标
//默认情况下，特性应用于它后面的元素。但是也可以用下面的语法显式标识要将特性应用于方法还是它的参数或返回值。
[target : attribute-list]
//target可能的值，见C#特性1.png
// default: applies to method
[SomeAttr]
int Method1() { return 0; }

// applies to method
[method: SomeAttr]
int Method2() { return 0; }

// applies to return value
[return: SomeAttr]
int Method3() { return 0; }




//反射Reflection









//C#集合
//1.ArrayList动态数组
//一些访问器
Capacity	获取或设置 ArrayList 可以包含的元素个数。
Count	获取 ArrayList 中实际包含的元素个数。
IsFixedSize	获取一个值，表示 ArrayList 是否具有固定大小。
IsReadOnly	获取一个值，表示 ArrayList 是否只读。
IsSynchronized	获取一个值，表示访问 ArrayList 是否同步（线程安全）。
Item[Int32]	获取或设置指定索引处的元素。
SyncRoot	获取一个对象用于同步访问 ArrayList


//C#使用List创建泛型集合(泛型类似C++模板)
//1.创建
List<person> persons = new list<person>();
//2.排序。
//2-1.默认比较规则在CompareTo方法中定义，该方法属于IComparable<T>泛型接口。
class Person ：IComparable<Person>
{
    //按年龄比较
    public int CompareTo(Person p)
    {
        return this.Age - p.Age;
    }
}//定义类时继承接口，来定义Sort()使用的规则。Sorted()默认升序排序
//2-2.实际使用中，经常需要对集合按照多种不同规则进行排序，这就需要定义其他比较规则，可以在Compare方法中定义，该方法属于IComparer<T>泛型接口，
















