# Unity的async&await #

## 背景 ##

### 协程 ###

Unity协同程序使用C＃对迭代器块的内置支持来实现。 您提供给StartCoroutine方法的IEnumerator迭代器对象由Unity保存，每个框架都会向前转发此迭代器对象，以获取由协调程序返回的新值。 然后，您可以通过Unity读取不同的值，以触发特殊情况行为，例如执行嵌套协同程序（返回另一个IEnumerator）时，延迟几秒钟（返回类型为WaitForSeconds的实例） 等到下一帧（返回null时）。

#### 协程何时终止？ ####

禁用 MonoBehaviour 时，不会停止协程，仅在明确销毁 MonoBehaviour 时才会停止协程。
可以使用 MonoBehaviour.StopCoroutine 和 MonoBehaviour.StopAllCoroutines 来停止协程。
销毁 MonoBehaviour 时，也会停止协程。
MonoBehaviour所绑定的GameObject，SetActive(false)时，也会停止协程

### 协程的缺陷 ###

 	1. 无法返回值
 	2. 难以处理它内部的错误和异常。因为无法将yield放在try-catch中。在异常发生时，堆栈跟踪只会说在哪抛出，还得猜可能调用了哪些协程。

## async&await替代协程 ##

```c#
using System.Threading.Tasks;

public class AsyncExample : MonoBehaviour
{
    IEnumerator coroutineExample()
    {
        Debug.Log("wait for 1 second!");
        yield return new WaitForSeconds(1f);
        Debug.Log("Over");
    }

    async void asyncExample()
    {
        Debug.Log("Wait for 1 second!");
        await Task.Delay(TimeSpan.FromSeconds(1));
        Debug.Log("Done!");
    }
}
```

Unity异步方法默认情况下将在主unity线程上运行。 在非Unity的C＃应用程序中，异步方法通常会在单独的线程中自动运行，这在Unity中将是一个大问题，因为在这些情况下，我们并不总是能够与Unity API进行交互。 没有Unity引擎的支持，我们对Unity方法/对象的调用有时会失败，因为它们将在单独的线程上执行。 在引擎框架下，它的工作原理是因为Unity提供了一个名为UnitySynchronizationContext的默认SynchronizationContext，它会自动收集每个帧排队的任何异步代码，并在主要的Unity线程上继续运行它们。