# HeadFirst设计模式

## 1 设计模式入门：欢迎来到设计模式世界(涉及模式：策略模式)

### 1.1 OO设计简介

+ 良好的OO设计必须具有可复用，可扩充，可维护三个特性。

### 1.2 鸭子模拟应用

![](HeadFirst_Pictures\1.png)

+ 加入新功能fly时：

  + 方法一，在基类中加virtual fly()，不同的鸭子继承它。可导致的问题是，有些鸭子不能飞，只能把fly置空操作。只有部分子类成员具有的行为不应该被定义为基类的方法

    这样的缺点是：

    ![](HeadFirst_Pictures\2.png)
    A,B,D,F

  + 方法二，改成接口。能飞的鸭子继承并实现接口，其它的就不继承。但这样的问题在于，代码没法复用

### 1.3 解决办法及其步骤

1. 分开变化和不变的部分

    + **<u>设计原则1：找出可能要变化的部分，将它们独立出来，与不变化的部分区分开来，进行封装</u> **

    + 这里把fly抽取出来放到别的类去，因为有的鸭子能飞，有的不能飞，不放基类中，但也不应放能飞的子类中去实现，代码复用率低。

2. 设计鸭子行为

   + **<u>设计原则2：针对接口编程，而非实现</u> **

   + 这里定义一个IFlyBehavior接口，而抽出来的Fly的类就用来实现这个接口。以往的在子类中去实现的这种就叫依赖实现。如果还有其它的只有某些子类具有的行为，我们也另外定义一个接口，一个抽象基类，一群继承类。但不考虑合并。一般地，如果Fly行为和某个行为，如quack行为都只在某些子类同时出现，我们是不会把它们定义到同一个接口中去的。

     ```c#
     interface IFlyBehavior
     {
         void Fly();
     }
     
     
     public class FlyWithWings : IFlyBehavior
     {
         public void Fly()
         {
             Debug.Log("FlyWithWings");
         }
     }
     
     // 统一了能飞的和不能飞的东西，如果针对实现编程，那么不能飞的鸭子的这些继承类都得把FlyNoWay的Fly函数写一遍。
     public class FlyNoWay : IFlyBehavior
     {
         public void Fly()
         {
             Debug.Log("I can't fly!");
     	}
     }
     
     public class Example : MonoBehaviour, IPointerClickHandler
     {
         public virtual void OnPointerClick(PointerEventData eventData)
         {
             //声明是基类类型，实现是在子类中
             IFlyBehavior flyBehavior = new FlyWithWings();
             flyBehavior.Fly();
             Debug.Log(name);
         }
     }
     ```
     ![](HeadFirst_Pictures\3.png)
   
     ```c#
     public abstract class Animal
     {
         public void abstract makeSound();
     }
     public Dog : Animal
     {
         public override void makeSound()
         {
             Debug.Log("Dog barking!");
     	}
     }
     //针对实现编程
     Dog d = new Dog();
     d.bark();
     
     //针对接口编程,Animal可以是抽象类，也可以是继承了接口的基类
     Animal animal = new Dog();
     animal.makeSound();
     
     //子类实例化的动作new Dog()甚至也不用再在代码中写了，可以在运行时指定具体对象
     a = getAnimal();
     a.makeSound();
     ```
   
     
   
   + **针对接口编程的优点：**
   
     + 多态。上面这种做法实际上是针对超类型编程(接口或者抽象类)这样，不管这些继承它的子类是咋实现这个接口的，我们在用的时候完全不用管，因为用的时候统一都是声明它的基类，指向的是实现的子类。
     + 便于复用。Fly的动作已经被剥离出鸭子类了，还可以被其它的对象复用，同一个飞行动作我也不用写多遍代码了。

3. 整合鸭子的行为

   + 首先把之前打算写在基类里的fly函数改成PerformFly()

   + 给基类定义一个IFlyBehavior类型的成员。

   + 如下所示,可以发现，这种鸭子的飞行行为是可以动态改变的，只要给flyBehavior赋值成不同的实现就行了，这里我们在基类中定义一些设定鸭子飞行行为的方法，给flyBehavior配套。这样咱们就可以随时改变鸭子的行为了。

     ```c#
     public class Duck
     {
         IFlyBehavior flyBehavior;
         public void PerformFly()
         {
             flyBehavior.fly();
     	}
         public void SetFlyBehavior(FlyBehavior targetFlyBehavior)
         {
             flyBehavior = targetFlyBehavior;
     	}
     }
     
     public class MallardDuck : Duck
     {
         public MallarDuck()
         {
             flyBehavior = new FlyWithWings();
     	}
     }
     ```

### 1.4 封装行为的大局观

![](HeadFirst_Pictures\4.png)

+ 我们把抽出去的”一组行为“定义成”一组算法“。
+ 类与类的关系大致分为继承和组合。组合指的是类中包含另一个类的实例的情况。组合往往更加flexible。既可以将算法封装成类，还可以在运行时弹性地变化改变行为。
+ **<u>设计原则3：多用组合，少用继承</u> **

### 1.5 策略模式

+ 定义：策略模式定义算法组，分别封装起来，实现同一个接口。让同一算法组的可以相互替换，使得算法(Fly)的变化独立于使用算法的客户(Duck or others)。

#### 1.5.1 适用场景和优劣分析

+ 适用场景：
  + 在一个系统里面有许多类，它们之间的区别仅在于它们的行为，那么使用策略模式可以动态地让一个对象在许多行为中选择一种行为。
  +  一个系统需要动态地在几种算法中选择一种。
  +  一个对象有很多的行为，如果不用恰当的模式，这些行为就只好使用多重的条件选择语句来实现
+ 优点:
  + 算法可以自由切换
  + 避免使用多重条件判断
  + 扩展性良好，符合开闭原则
### 1.6 章节总结

+ 三个原则：
  + 封装变化：提取并独立出变化的部分
  + 面向接口编程
  + 多用组合，少用继承。
+ 一个模式：策略模式

### 1.7 其它要点

![](HeadFirst_Pictures\15.png)

## 2 观察者模式

### 2.2 气象监测应用

####  2.1 基本分析

+ 主要工作：建立一个应用，利用WeatherDate取得数据并实时更新目前状况，气象统计，天气预报![](HeadFirst_Pictures\5.png)

![](HeadFirst_Pictures\6.png)

+ 需求分析：
  + WeatherData类有三个获取数据的方法。
  + 当有新数据时，measurementsChanged()就得被调用。
  + 有三个布告板得实现。
  + 鉴于OO设计的可扩充，可复用，可维护。我们应当允许布告板的随意添加和删除。

#### 2.2 错误实例

![](HeadFirst_Pictures\7.png)

+ 存在的问题：A,B,C,E

![](HeadFirst_Pictures\8.png)

### 2.3 观察者模式

#### 2.3.1 观察者模式定义与实现

+ 定义：定义了对象之间的一对多的依赖关系，当一个对象改变状态时，这个对象的所有依赖者都会收到通知并自动更新。这个对象被称为主题subject，其它依赖它的就是观察者
+ 实现：实现方式多，以下图这个包含Subject与Observer接口的类设计最常见。

![](HeadFirst_Pictures\9.png)

+ subject和Observer都作为一个接口，这里是面向接口的编程。

+ subject包含注册观察者registerObserver，注销观察者removeObserver，更新观察者notifyObserver等函数。是一个具有状态的对象，也是数据的拥有者。常常是把数据”推“向观察者

+ Observer主要有个update方法，用于更新数据。

#### 2.3.2 适用场景和优劣分析

+ 适用场景：
  + 一个抽象模型有两个方面，其中一个方面依赖于另一个方面。将这些方面封装在独立的对象中使它们可以各自独立地改变和复用。
  + 一个对象的改变将导致其他一个或多个对象也发生改变，而不知道具体有多少对象将发生改变，可以降低对象之间的耦合度。
  + 一个对象必须通知其他对象，而并不知道这些对象是谁。
  + 需要在系统中创建一个触发链，A对象的行为将影响B对象，B对象的行为将影响C对象……，可以使用观察者模式创建一种链式触发机制。
+ 优点:
  + 让subject和Observer之间松耦合。我们可以知道，改变其中一方时，并不会影响另一方。
  + 能建立一套触发机制。
+ 缺点:
  + 如果一个**被观察者对象有很多的直接和间接的观察者**的话，将所有的观察者都通知到会**花费很多时间**
  + 如果在**观察者和观察目标之间有循环依赖**的话，观察目标会触发它们之间进行循环调用，可能导致**系统崩溃**。 
  + 观察者模式没有相应的机制让观察者知道所观察的目标对象是怎么发生变化的，而仅仅只是知道观察目标发生了变化。
+ **<u>设计原则4：OO设计尽量设计成让交互对象松耦合</u> **

### 2.4 实现气象站

```c#
// 接口部分
public interface Subject
{
    public void registerObserver(Observer observer);
    public void removeObserver(Observer observer);
    public void notifyObserver();
    public void setChanged();//标记subject产生了改变，这使得通知观察者们的时机更灵活
}

public interface Observer
{
    public void UpdateInfo(float temp, float humidity, float pressure);
}

public interface DisplayElement
{
    public void disaplay();
}
```

+ 实现部分:

  + WeatherData类

  ![](HeadFirst_Pictures\10.png)

  + CurrentConditionDisplay类（其中的一个布告板）

![](HeadFirst_Pictures\11.png)

### 2.5 Unity中的一个观察者模式的样式

```c#
public interface ISubject
{
    void RegisterObserver(Observer observer);
    void RemoveObserver(Observer observer);
    void NotifyObservers();
    void SetSubjectState();
    bool GetSubjectState();
}

public abstract class Observer
{
    protected ISubject subject;
    protected Observer(ISubject subject)
    {
        this.subject = subject;
    }
}

public class CubeObserver : Observer
{
    public CubeObserver(ISubject subject):base(subject)
    {
        if (subject is CubeSubject)
        {
            CubeSubject cubeSubject = (CubeSubject)subject;
            cubeSubject.RegisterObserver(this);
            cubeSubject.update += this.UpdateInfo;
        }
    }
    public void UpdateInfo(Vector3 position)
    {
        Debug.Log($"Cube is on the position of {position}");
    }
}

public delegate void EventHandler(Vector3 position);
public class CubeSubject : MonoBehaviour, ISubject, IPointerClickHandler
{
    public bool subjectState;
    public event EventHandler update;
    List<Observer> observers = new List<Observer>();
    public void NotifyObservers()
    {
        if (GetSubjectState() == true)
        {
            update(transform.position);
            subjectState = false;
        }
    }

    public void RegisterObserver(Observer observer)
    {
        observers.Add(observer);
    }

    public void RemoveObserver(Observer observer)
    {
        if (observers.Contains(observer))
        {
            observers.Remove(observer);
        }
    }
    public bool GetSubjectState()
    {
        return subjectState;
    }
    public void SetSubjectState()
    {
        subjectState = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SetSubjectState();
        NotifyObservers();
    }
}
```

### 2.6 章节总结

+ 一个原则：交互对象尽量松耦合
+ 一个模式：观察者模式

### 2.7 其它要点

![](HeadFirst_Pictures\16.png)

## 3. 装饰器模式

开闭原则：

+ 原则4：**<u>类应该对扩展开放，对修改封闭</u>**
+ 例子：观察者模式中，我们只要加入新观察者就实现了对主题subject的拓展，且没有修改subject。
+ 没必要在每个地方都用开闭原则，因为开闭原则需要加入新的抽象参差，增加代码复杂度。

### 3.1 咖啡管理器引入

![](HeadFirst_Pictures\12.png)

+ 这些咖啡当加入调料时，又会根据加入的调料收取不同的费用。当然我们不会直接从Beverage派生出多种多样的加了某种调料的咖啡子类，因为一旦调料多了，这个组合数就会导致类的数量爆炸。

+ 一种做法是：基类的cost计算所加调料的价格，子类则在基类基础上加上咖啡价格。如下图所示。这种做法的不足点是，出了一款新饮料时，如果有些调料不适合，则仍将继承它。而且当有新调料时，又要对基类进行修改，调料价格的改变也会对基类进行修改，这样一来，造成基类修改的原因就不止一个了，这不符合单一职责原则。

  ![](HeadFirst_Pictures\13.png)

### 3.2 装饰器模式

+ 装饰器和被装饰器装饰的对象具有相同的基类。这也是继承的目的，不是说违背多用组合的原则。

+ 可以用多个装饰器装饰一个对象。有鉴于此，任何被装饰过的对象，也可以当一个装饰器，去装饰别的对象。

+ **装饰器可以在自己委托的被装饰的对象的行为前后加上自己的行为。**

+ 对象能在任何时候，包括运行时动态不限量地进行装饰。

#### 3.2.1 装饰器模式定义

  + 装饰器模式动态地将责任附加到对象上。如要扩展功能，会比继承更flexible。

#### 3.2.2 装饰器模式的适用场景和优劣分析

+ 适用场景：
  + 需要扩展一个类的功能或给一个类增加附加责任。
  
  + 需要动态地给一个对象增加功能，这些功能可以再动态地撤销。
  
  + 需要增加由一些基本功能的排列组合而产生的非常大量的功能。比如给武器改造附魔，给玩家穿装备，DIY角色机甲等，实际就是给对象附加职责。
  
+ 优点:
  + 用于拓展对象行为，和继承有着一样的目的，却比继承灵活。
  + 通过使用不同的具体装饰类以及这些类的排列组合，设计师可以创造出很多不同行为的组合。装饰器模式有很好的可扩展性，同时对修改封闭，如果要修改某个decorator的元素，我们只要去改就行，如果要对decorator功能拓展，我们只要新增decorator子类就行，符合开闭原则。
+ 缺点:
  + 装饰者模式会导致设计中出现许多小的类，如果过度使用，会让程序变的更复杂。并且更多的对象会使得查错变得困难，特别是这些对象看上去都很像。
  + 装饰器模式是针对抽象而不是实现进行编程的，我们在装饰的过程中使用的是基类对象，如果打算对某个继承的子类，这个子类依赖了其它的对象，那么给这个特定组件加上装饰时，那就有问题

### 3.3 用装饰器模式写咖啡管理器（咖啡的生成：装饰者模式+工厂模式/生成器）

```c#
using UnityEngine;

// 饮料抽象类
public abstract class Beverage
{
    string description = "UnKnown Drink!";
    public abstract double Cost();
    public abstract string GetDescription();
}
// 咖啡饮料
public class Coffee : Beverage
{
    public override double Cost()
    {
        return 1;
    }

    public override string GetDescription()
    {
        return "I ordered a cup of coffee";
    }
}

public abstract class Decorator : Beverage{}
// 牛奶装饰器
public class Milk : Decorator
{
    public Beverage beverage;
    public Milk(Beverage beverage)
    {
        this.beverage = beverage;
    }
    public override double Cost()
    {
        return 0.2 + beverage.Cost();
    }
    public override string GetDescription()
    {
        return beverage.GetDescription() + ", Milk";
    }
}
// 摩卡装饰器
public class Mocha : Decorator
{
    public Beverage beverage;
    public Mocha(Beverage beverage)
    {
        this.beverage = beverage;
    }
    public override double Cost()
    {
        return 0.4 + beverage.Cost();
    }
    public override string GetDescription()
    {
        return beverage.GetDescription() + ", Mocha";
    }
}
public class DecorationExample : MonoBehaviour
{
    private void Start() {
        Beverage newCoffee = new Coffee();
        newCoffee = new Milk(newCoffee);
        newCoffee = new Mocha(newCoffee);
        Debug.Log(newCoffee.GetDescription() + $"! It costs {newCoffee.Cost()} dollars!");
    }
}
```

### 3.4 章节总结

+ 一个原则：开闭原则：对扩展开放，对修改封闭。
+ 一个模式：装饰器模式：

### 3.5 其它要点

![](HeadFirst_Pictures\17.png)

## *4. 工厂模式：实例化的低耦合*

### 4.1 引入——传统new实例化的不合理性

+ 实例化过程中，适用new，而new的时候必然需要知晓具体的类，这不是针对抽象而是针对实现编程，而当加入新的具体类时，又要修改if-else语块，这不对修改关闭，不符合开闭原则。
+ 实例化的具体的类只有在运行时才知道，有时候会写出很多if-else语句，导致难以进行维护更新。

### 4.2 披萨生产模型

#### 4.2.1 传统处理方式

![](HeadFirst_Pictures\18.png)

#### 4.2.2 封装创建对象的代码

+ 抽离if-else实例化语块，迁移到一个新对象factory中

