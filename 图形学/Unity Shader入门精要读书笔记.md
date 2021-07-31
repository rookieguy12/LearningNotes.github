# Unity Shader入门精要读书笔记

## 第二章  渲染流水线
### 2.1 综述

+ 2.1.2 渲染流水线
  1. 渲染流程
    * 应用阶段-----几何阶段-----光栅化阶段
    * 应用阶段:输出渲染图元。 
    * 几何阶段:输出屏幕空间的顶点信息，主要是进行坐标变换以及输出深度值，顶点着色信息。
      +  主要分为模型视图变换、顶点着色、[几何着色器]、投影、裁剪、屏幕映射等阶段
    * 光栅化阶段:决定每个渲染图元中需要绘制显示的像素，对顶点数据进行插值。
        * 主要分为三角形设定、三角形遍历、像素着色、以及合并阶段

![RenderPipeline](D:\学习\就业相关(游戏客户端)\图形学\书籍与笔记\Pictures\RenderPipeline.png)
* 其中绿色可编程，黄色可配置但不可编程，蓝色是固定的
### 2.2 CPU和GPU之间的通信

+ 2.2.1 把数据加载到显存中
	+ 数据加载过程: 硬盘(所有数据)--->内存(所有数据)--->显存(网格和纹理等数据)
+ 2.2.2 设置渲染状态
+ 2.2.3 调用Draw Call
	+ CPU调用Draw Call，使GPU开始渲染
### 2.3 GPU流水线
+ 2.3.2 顶点着色器
	+ 作用: 实现顶点的空间变换、顶点着色
+ 2.3.3 几何着色器
	+ 作用:让GPU进行高效地创建和销毁几何图元，主要对三角形/四边形图元的顶点进行操作。
+ 2.3.4 裁剪
	+ 作用: 对部分在视野内的图元进行裁剪
+ 2.3.5 屏幕映射
	+ 作用：把图元的x、y坐标转换到屏幕坐标系
	+ 窗口坐标系：屏幕坐标系+z坐标（深度）
+ 2.3.6 三角形设置
	+ 作用：计算光栅化网格所需的信息
+ 2.3.7 三角形遍历
	+ 作用：通过扫描变换找到被网格覆盖的像素，生成片元，并根据顶点信息对覆盖区域像素进行插值，生成片元信息。
+ 2.3.8 片元着色器
	+ 作用：完成纹理采样，依据插值得到的片元信息计算输出颜色
+ 2.3.9 逐片元操作
	+ 作用/任务
		1. 进行深度测试和模板测试，透明度测试。决定片元可见性 
		2. 将通过测试的片元(即可见的片元)的颜色值和颜色缓冲区的颜色合并混合
	+ 模板测试：
		* 限制渲染区域。
	+ 深度测试：
		* 根据z值来确定xy值相同的片段哪个要被显示，哪个要进行z消隐。一般的，不透明的物体关闭混合操作，只保留一个片元的信息，透明的则需要进行颜色混合，以确保其看起来是透明的。
		* 深度测试一般排在片元着色器处理之前，提高效率
+ 2.3.10 合并
### 2.4 其它
+ 2.4.1 OpenGL & DirectX（两种图像编程接口）
	+ 显卡驱动负责图像编程接口和显卡GPU的交互
	+ OpenGL原点在左下角，DirectX在左上角
+ 2.4.2 HLSL(DirectX)  GLSL(OpenGL) CG
+ 2.4.3 Draw Call
	+ CPU和GPU如何实现并行工作
		+ 命令缓冲区
		+ 效率问题往往出在CPU提交命令的速度上，采用批处理的方法进行优化
		+ 为减少Draw Call开销，一是要避免使用大量很小的网格，二是要避免使用过多的材质。
+ 2.4.4 固定管线渲染(固定函数的流水线即固定管线)
## 第三章 Unity Shader基础
### 3.1 概述(P42)
+ 3.1.1 材质与shader
+ 3.1.2 Unity中的材质
+ 3.1.3 Unity中的Shader
	
	* 4种Unity Shader模板：
	
	1. Standard Surface Shader：标准表面着色器，是一种基于物理的着色系统（使用PBR).包含了标准光照模型的表面着色器模板
	2. Unilit Shader：产一个不包含光照但包含雾效的基本顶点/片元着色器（Vertex/Fragment Shader）多用于特效、UI上的效果制作。
	3. Image Effect Shader：也是顶点片断着色器，只不过是针对后处理而定制的模版，后处理是什么呢？Bloom（也有人叫Glow/泛光/辉光等说法）、调色、景深、模糊等，这些基于最终整个屏幕画面而进行再处理的Shader就是后处理。
	4. Compute Shader：Compute Shader是运行在图形显卡上的一段程序，独立于常规渲染管线之外的，它可以直接将GPU作为并行处理器加以利用，从而使GPU不仅具有3D渲染能力，还具有其他的运算能力。
	5. Shader Variant Collection：Shader变体收集器，与上面几个不同，它不是制作Shader的模版,而只是对Shader变体进行打包用的容器。

### 3.2 Unity Shader基础：ShaderLab  (P45)
1. Unity Shader基础结构
	> ```ShderLab
	> //ShaderName将显示在material选择材质的列表
	> Shader "ShaderName"{
	> 	Properties{
	> 		// 属性
	> 	}
	> 	SubShader{
	> 		// 显卡A使用的子着色器
	> 	}
	> 	SubShader{
	> 		// 显卡B使用的子着色器
	> 	}
	> 	Fallback "VertexLit"	//一旦没用这里的任何一个SubShader，就转到名为"VertexLit"的Shader去
	> }
	> ```

### 3.3 Unity Shader的结构(P46)

+ 3.3.1  Shader的名字
+ 3.3.2 Properties
	
	> ```c#
	> Properties{
	> 	/*Name ("display name", PropertyType) = DefaultValue*/
	>     _Cube("Cube", Cube) = "white"{}
	> }
	> ```

![Properties](D:\学习\就业相关(游戏客户端)\图形学\书籍与笔记\Pictures\Shader Properties.png)

+ 3.3.3 ***SubShader***
1. 每个Unity Shader可以包含多个SubShader，但至少要有一个。要加载时，扫描并选择第一个能在目标平台上运行的SubShader，都不支持才使用Fallback定义的Unity Shader

> ```C#
> 	SubShader{
>	// 可选标签：指定了渲染顺序或者时机及其他一些设置
>	[Tags]
>	// 可选状态
>	[RenderSetup]	
>	Pass{
>		//每个Pass定义了一次完整的渲染流程
>		//Pass不要定义太多个，会影响性能
>	}
>}
> ```

2. SubShader内定义的RenderSetup和Tags也可以定义在Pass中，作用域不同罢了，但在Pass中的标签设置方式与之不同

    + 状态设置RenderSetup

    + ![状态设置RenderSetup](D:\学习\就业相关(游戏客户端)\图形学\书籍与笔记\Pictures\ShaderLab[RenderSetup].png)

    + SubShader内定义的标签Tags设置
    	+ Tags是一个键值对，两个都是字符串类型
    	> ```
    	> Tags{"TagName1" = "Value1" "TagName2" = "Value2"} 
    	> ```
    	
    + ![Tags](D:\学习\就业相关(游戏客户端)\图形学\书籍与笔记\Pictures\ShaderLab[Tags].png)

      + RenderType标签

        "Opaque"：绝大部分不透明的物体都使用这个；
        "Transparent"：绝大部分透明的物体、包括粒子特效都使用这个；
        "Background"：天空盒都使用这个；
        "Overlay"：GUI、镜头光晕都使用这个；
      + Queue标签。定义渲染顺序。预制的值为
    "Background"。值为1000。比如用于天空盒。
    "Geometry"。值为2000。大部分物体在这个队列。不透明的物体也在这里。这个队列内部的物体的渲染顺序会有进一步的优化（应该是从近到远，early-z test可以剔除不需经过FS处理的片元）。其他队列的物体都是按空间位置的从远到近进行渲染。
    "AlphaTest"。值为2450。已进行AlphaTest的物体在这个队列。
    "Transparent"。值为3000。透明物体。
    "Overlay"。值为4000。比如镜头光晕。
    用户可以定义任意值，比如"Queue"="Geometry+10" 

    + Pass语义块：内部是CG/HLSL语言代码块

      + Cg/HLSL的数据类型

        1. float/half/fixed
            * float为32位高精度，常用于世界坐标下的位置，纹理的UV，或者复杂函数的标量计算
            * half为16位中精度，数值为[-60000,60000]，常用于本地坐标下的位置、方向向量、HDR颜色。不过一般会在PC上被合并到float
            * fixed为11位低精度，数值范围为[-2,2]，常用于常规的颜色与贴图，低精度间的一些运算变量等
        2. interger
            * 整形类型，常用于循环以及数组的索引
        3. sampler2D、sampler3D、samplerCUBE(纹理类型)
            * 这些都分为float和half两种精度类型，sampler2D_half和sampler2D_float
      > ```c#
      > Pass{
      	[Name]
      	[Tags]
      	[RenderSetup]
      	// other code
      }
      > ```

      1. Name部分
      >  ``` c#
      >  Name "MyPassName"				//自定义名称
      >  UsePass "Shader/MYPASSNAME"	//使用其它shader的Pass，但名称得大写
      >  ```

      2. RenderSetup部分
      和SubShader一致
      3. Tags部分

    + ![Pass_Tags](D:\学习\就业相关(游戏客户端)\图形学\书籍与笔记\Pictures\Pass_Tags.png)
    	4. 其它Pass定义
    		* UsePass
    		* GrabPass
+ 3.3.4 Fallback
> ```c#
> Fallback "name"	//使用名为name的shader
> Fallbakc off	//关闭这个功能
> ```

+ 作用：一般要用的时候就放在所有SubShader后面，如果上面的SubShader都没法运行，就运行名为name的shader


### 3.4 Unity Shader的形式(P51)
* ![Shader](D:\学习\就业相关(游戏客户端)\图形学\书籍与笔记\Pictures\Shader.png)
* 表面着色器(Surface Shader)代码写SubShader语块内，顶点/片元/固定着色器(Vetex/Fragment/Fixed Function Shader)写Pass语块内
* 3.4.1 表面着色器
* 3.4.2 顶点/片元着色器
* 3.4.3 固定着色器(已舍弃)
* 3.4.4 Unity Shader选择
	* 光源多的用表面着色器，否则用顶点/片元着色器。
	* 自定义的渲染效果很多，则使用顶点/片元着色器

### 3.6 答疑解惑(P53)
* 3.6.2 Unity Shader和CG/HLSL的关系
	* Unity Shader用ShaderLab语言编写，但对于表面着色器、顶点着色器、片元着色器，可以在内部嵌套CG/HLSL语言来编写，嵌套在CGPROGRAM和ENDCG之间。

## 第四章 数学基础(P56)
### 4.2 笛卡尔坐标系(P57)
* 4.2.1 二维笛卡尔坐标系
	* OpenGL原点在左下角，DirectX左上角，二维的坐标系可相互转换
* 4.2.2 三维笛卡尔坐标系
	* 基本概念：基矢量、正交基、标准正交基
	* 两种坐标系：左手系与右手系、拇指x，食指y，中指z。旋转正方向分别基于左手法则，右手法则。在模型空间和世界空间中，unity用的左手系。
### 4.3 点和矢量(P63)(略)
### 4.4 矩阵(略)
### 4.5 矩阵的几何意义：变换(P79)
* 4.5.1 变换的类型
	* 线性变换：可以保留矢量相加和标量相乘的变换。对于一个3维矢量，用一个3×3的矩阵就可以表示所有的线性变换。
		1. 缩放
		2. 旋转
		3. 错切、镜像、正交投影
	* 平移变换：如f(X) = x + (1,2,3)
	* 仿射变换：合并了线性变换和平移变换。仿射变换可以用一个4×4的矩阵表示。
* 4.5.2 齐次坐标：将三维的转换成思维
	* 对于一个三维的坐标，将w值设为1；而对矢量，将w值设为0，因为平移不改变矢量
* 4.5.3 分解基础变换矩阵
	* ![矩阵分解](D:\学习\就业相关(游戏客户端)\图形学\书籍与笔记\Pictures\分解基础变换矩阵.jpg)
	* 左上M为线性变换矩阵，右上t表示平移矩阵
* 4.5.4 平移矩阵
	* ![点](D:\学习\就业相关(游戏客户端)\图形学\书籍与笔记\Pictures\点平移矩阵.jpg)
	* ![矢量](D:\学习\就业相关(游戏客户端)\图形学\书籍与笔记\Pictures\平移矩阵.jpg)
	* 上图所示为对点的平移以及对矢量的平移。
	* 平移矩阵取逆正好就是反向平移得到的矩阵
* 4.5.5 缩放矩阵
	* ![缩放矩阵](D:\学习\就业相关(游戏客户端)\图形学\书籍与笔记\Pictures\缩放矩阵.jpg)
	* 缩放矩阵即把M设为一个对角矩阵。
	* 参照初等矩阵的逆，缩放矩阵的逆矩阵就是把缩放系数变成倒数
* 4.5.6 旋转矩阵
	* 分别绕三个轴的旋转(参照数电卡诺图的相邻最小项写基本的旋转因子)
	* ![xy](D:\学习\就业相关(游戏客户端)\图形学\书籍与笔记\Pictures\旋转xy矩阵.jpg)
	* ![z](D:\学习\就业相关(游戏客户端)\图形学\书籍与笔记\Pictures\旋转z矩阵.jpg)
	* 基本的因子
		* ![因子](D:\学习\就业相关(游戏客户端)\图形学\书籍与笔记\Pictures\基本组成.jpg)
* 4.5.7 复合变换
	* P<sub>new</sub> = M<sub>translation</sub>M<sub>rotation</sub>M<sub>scalθ</sub>P<sub>old</sub>

	* 一般约定是先缩放、再旋转再平移。对于旋转，在unity中是zxy的顺序。
### 4.6 坐标空间(P85)
* 4.6.2 坐标空间变换
* 例子：已知子坐标空间C的3个坐标轴矢量在父坐标空间P下的表示x<sub>c</sub>,y<sub>c</sub>,z<sub>c</sub>,以及原点位置O<sub>c</sub>，且已知一个点A<sub>c</sub>，则A<sub>p</sub> = O<sub>c</sub> + ax<sub>c</sub> + by<sub>c</sub> + cz<sub>c</sub>
* A<sub>p</sub> = M<sub>c->p</sub>A<sub>c</sub>
	
	* ![M](D:\学习\就业相关(游戏客户端)\图形学\书籍与笔记\Pictures\M.png)
	* 上述矩阵相当于把C的三个基矢量放入前三列，最后一列放入原点坐标
	* 对于矢量，由于平移不改变矢量，故M可以变成三阶，如下图所示。也是为何在shader中常常截取交换矩阵M的前三行/三列
	* ![M](D:\学习\就业相关(游戏客户端)\图形学\书籍与笔记\Pictures\矢量M.jpg)

	* 特别地，当M为正交矩阵时，由于M的转置矩阵M<sub>T</sub>等于M的逆矩阵，所以可以容易地得到父子坐标系的基在另一个坐标系里的矢量表示
* 4.6.3 顶点的坐标空间变换过程
	* 第一步：将顶点坐标从模型空间变换到世界空间（模型变换）
	* 第二步：将世界空间的坐标变换到摄像机空间。
	* 第三步：用投影矩阵为投影做准备，然后利用齐次除法将摄像机空间的顶点转换/投影到裁剪空间
* 4.6.4 模型空间(对象空间/局部空间)
	* 指的是unity的space.self 
* 4.6.5 世界空间(space.world)
* 4.6.6 观察空间(摄像机空间)
  * 与书上说的不同，unity摄像机空间还是使用的左手系，即z表示的是镜头面对的方向
* 4.6.7 裁剪空间
	* 使用的矩阵：裁剪矩阵
	* 视椎体
		* 由6个裁剪平面组成，可分为正交投影和透视投影。
		* 正交投影看到的线都是平行的，网格大小都是一样的，保留了物体的距离和角度；而透视投影模拟了人眼看世界的方式。
		* 远、近裁剪平面：决定了摄像机可看到的深度
	* 投影的含义：
		* 降维
		* 对坐标分量的缩放
	* 投影矩阵：
		* 透视投影：
			* 根据张开的角度FOV和远近裁剪平面的距离可以得到两裁剪平面的纵向的高度：
			![相机](D:\学习\就业相关(游戏客户端)\图形学\书籍与笔记\Pictures\Camera.jpg)
			![裁剪高度](D:\学习\就业相关(游戏客户端)\图形学\书籍与笔记\Pictures\Camera_cut_height.jpg)
			* 根据纵横比Aspect = Width/Height(如16:9)得到横向的宽度关于上述高度和Aspect表达式
			* 最终的投影矩阵如下:
			![裁剪矩阵](D:\学习\就业相关(游戏客户端)\图形学\书籍与笔记\Pictures\frustum.jpg)
			可以发现，相当于对x，y，z做了缩放，对z再做了个平移，将其变换到了裁剪空间中，之后需要对顶点判断，必须保证x、y、z都在[-w,w]之间
			![点透视变换](D:\学习\就业相关(游戏客户端)\图形学\书籍与笔记\Pictures\透视投影.jpg)
			* 上图为对点的变换结果，实际上由于unity里面的其实都是左手系，而不是书上说的右手系。所以最后一行要写0,0,1,0而不应该是0,0,-1,0，故变换后的w是z而不是-z
		* 正交投影：
			* 类似的透视投影，也可以得到正交头用的裁剪矩阵和对点的裁剪结果
			![正交投影裁剪矩阵](D:\学习\就业相关(游戏客户端)\图形学\书籍与笔记\Pictures\正交投影裁剪矩阵.jpg)
			![裁剪高度](D:\学习\就业相关(游戏客户端)\图形学\书籍与笔记\Pictures\Transforms.jpg)
			* 需要注意的是：正交投影对顶点变换后，w依然是1
			* 用于判断变换后顶点是否位于视椎体内的不等式和透视投影一样。都是要三个分量全部在[-w,w]之间。
* 4.6.8 屏幕空间
	* 经过透视投影变换到裁剪空间后，还需要将视椎体投影到屏幕空间 	

## 第五章 unity shader入门

### 5.2  简单的顶点/片元着色器

![](D:\gitwork\LearningNotes.github\图形学\Pictures\1.png)

+ 例子1：

  ![](D:\gitwork\LearningNotes.github\图形学\Pictures\2.png)

  要点：

  + 语句块基本是固定的
  + pragma声明了函数的作用，传入参数的语义限定说明了传入参数的性质。
+ 例子2:
  ![](D:\gitwork\LearningNotes.github\图形学\Pictures\3.png)
  要点:
  + 传入多个参数时用结构体，语义限定就在结构体各个参数后面写。一般结构体的名称也是有含义的：
    + a2v（application To vertex应用到顶点）
    + v2f（vertex To fragment顶点到片元着色器）
  + 这些语义一般有：POSITION（顶点坐标），TANGENT，NORMAL（法线），TEXCOORD0~3，COLOR等。
  + 这些参数是从Mesh Render组件来的，每帧调用Draw Call时就把三角面的信息发送过来。

#### 5.2.3 顶点着色器与片元着色器的通信

+ 例子1：

  ![](D:\gitwork\LearningNotes.github\图形学\Pictures\4.png)

+ 要点：

  + SV_POSITION是一定要赋值得到的，所以这里声明了返回必须有表示SV_POSITION的
  + COLOR0一般存放颜色，还有COLOR1等。如逐顶点的高光反射颜色，逐顶点的漫反射颜色等。

#### 5.2.4 属性的使用

+ 材质提供了在Shader中调节参数的方式，这些参数就写在Properties中。

+ 例子1：颜色拾取器

  ![](D:\gitwork\LearningNotes.github\图形学\Pictures\5.png)

  要点：

  + ShaderLab的属性相当于变量声明，传入一些参数，如贴图，颜色之类。后面pass要用的时候就得用CG语言再定义变量，且类型和名称得与Properties中的相匹配。
  + uniform fixed4 _Color。uniform可省略。

![](D:\gitwork\LearningNotes.github\图形学\Pictures\6.png)

### 5.3 Unity的内置文件与变量

#### 5.3.1 内置的包含文件

+ 类似头文件，也是#include "name"。但后缀为cginc

![](D:\gitwork\LearningNotes.github\图形学\Pictures\7.png)

+ UnityCG.cginc

  + 常用结构体

    ![](D:\gitwork\LearningNotes.github\图形学\Pictures\8.png)

  + 常用帮助函数

    ![](D:\gitwork\LearningNotes.github\图形学\Pictures\9.png)

    联系记忆：模型空间是物体自身的空间，比如子物体的transform就显示的是在父物体模型空间下的坐标旋转数据。而没有父物体的就是世界坐标。

### 5.4 Unity提供的CG/HLSL语义

#### 5.4.1 什么是语义

+ 如SV_POSITION,TANGENT,NORMAL等，都是语义。它们就是用来修饰参数的含义，让unity知道要从这里读取或者输出数据。
+ 有的语义有特别规定。如TEXCOORD0，在顶点着色器的输入结构体中，unity将模型的第一组纹理的坐标存入其中。而在其输出中，就能由自己决定了。
+ 带SV的表示system-value，这些叫系统数值语义。SV_POSITION表示其次裁剪空间的坐标。

#### 5.2.4 Unity支持的语义

+ a2v的常用语义

  ![](D:\gitwork\LearningNotes.github\图形学\Pictures\10.png)

  注：

  ![](D:\gitwork\LearningNotes.github\图形学\Pictures\11.png)

+ v2f的常用语义

  ![](D:\gitwork\LearningNotes.github\图形学\Pictures\12.png)

