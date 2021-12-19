# Unity事件系统(EventSystem)总结 #

## 简介 ##

EventSystem基于Input，可以对键盘，鼠标，触摸，以及自定义输入进行处理。
EventSystem本身是一个管理控制器，核心功能依赖InputModule和Raycaster模块。

## 原理 ##

EventSystem对象负责管理所有事件相关对象，挂载了EventSystem组件和StandaloneInputModule组件，前者为管理脚本，后者为输入模块。
Canvas对象下挂载了GraphicRaycaster负责处理射线相关运算，用户的操作都会通过射线检测来映射到UGUI组件上，InputModule将用户的操作转化为射线检测，Raycaster则找到目标对象并通知EventSystem，最后EventSystem发送事件让目标对象进行响应。

## 事件响应 ##

### 方法1：将实现IXXXHandler接口的脚本挂物体上 ###

```c#
public class EventTest : MonoBehaviour, IPointerClickHandler, IDragHandler, 
IPointerDownHandler, IPointerUpHandler {
    public void OnDrag(PointerEventData eventData) {
 			//鼠标拖动
    }
    public void OnPointerClick(PointerEventData eventData) {
 			//鼠标点击
    }
    public void OnPointerDown(PointerEventData eventData) {
 			//鼠标按下
    }
    public void OnPointerUp(PointerEventData eventData) {
 			//鼠标抬起
    }
}
```
### 方法2：使用EventTrigger组件 ###



## 使用 ##

+ UGUI使用
  1. 新建UGUI任意组件时，会自动添加EventSystem对象（集成了EventSystem组件和StandaloneInputModule组件）；
  2. Canvas默认挂载了GraphicRaycaster组件，所以在Canvas对象之下的所有GUI对象都可以通过挂载脚本并且实现一些和事件相关的接口来处理事件；
  3. 参考“事件响应”章节，实现事件监听与处理；

+  **场景物体使用**
  1. **新建EventSystem对象**；
  2. **Camera添加Physics2D Raycaster或者Physics Raycaster组件**;
  3. **物体上挂上碰撞体，然后根据上面的用法进行使用**

## 相关组件和类 ##

EventSystem组件

主要负责：

	1. InputModule切换
	2. InputModule的激活与反激活

