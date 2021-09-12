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

![RenderPipeline](Pictures\RenderPipeline.png)
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

![Properties](Pictures\Shader Properties.png)

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

    + ![状态设置RenderSetup](Pictures\ShaderLab[RenderSetup].png)

    + SubShader内定义的标签Tags设置
    	+ Tags是一个键值对，两个都是字符串类型
    	> ```
    	> Tags{"TagName1" = "Value1" "TagName2" = "Value2"} 
    	> ```
    	
    + ![Tags](Pictures\ShaderLab[Tags].png)

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

    + ![Pass_Tags](Pictures\Pass_Tags.png)
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
* ![Shader](Pictures\Shader.png)
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
	* ![矩阵分解](Pictures\分解基础变换矩阵.jpg)
	* 左上M为线性变换矩阵，右上t表示平移矩阵
* 4.5.4 平移矩阵
	* ![点](Pictures\点平移矩阵.jpg)
	* ![矢量](Pictures\平移矩阵.jpg)
	* 上图所示为对点的平移以及对矢量的平移。
	* 平移矩阵取逆正好就是反向平移得到的矩阵
* 4.5.5 缩放矩阵
	* ![缩放矩阵](Pictures\缩放矩阵.jpg)
	* 缩放矩阵即把M设为一个对角矩阵。
	* 参照初等矩阵的逆，缩放矩阵的逆矩阵就是把缩放系数变成倒数
* 4.5.6 旋转矩阵
	* 分别绕三个轴的旋转(参照数电卡诺图的相邻最小项写基本的旋转因子)
	* ![xy](Pictures\旋转xy矩阵.jpg)
	* ![z](Pictures\旋转z矩阵.jpg)
	* 基本的因子
		* ![因子](Pictures\基本组成.jpg)
* 4.5.7 复合变换
	* P<sub>new</sub> = M<sub>translation</sub>M<sub>rotation</sub>M<sub>scalθ</sub>P<sub>old</sub>

	* 一般约定是先缩放、再旋转再平移。对于旋转，在unity中是zxy的顺序。
### 4.6 坐标空间(P85)
* 4.6.2 坐标空间变换
* 例子：已知子坐标空间C的3个坐标轴矢量在父坐标空间P下的表示x<sub>c</sub>,y<sub>c</sub>,z<sub>c</sub>,以及原点位置O<sub>c</sub>，且已知一个点A<sub>c</sub>，则A<sub>p</sub> = O<sub>c</sub> + ax<sub>c</sub> + by<sub>c</sub> + cz<sub>c</sub>
* A<sub>p</sub> = M<sub>c->p</sub>A<sub>c</sub>
	
	* ![M](Pictures\M.png)
	* 上述矩阵相当于把C的三个基矢量放入前三列，最后一列放入原点坐标
	* 对于矢量，由于平移不改变矢量，故M可以变成三阶，如下图所示。也是为何在shader中常常截取交换矩阵M的前三行/三列
	* ![M](Pictures\矢量M.jpg)

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
			![相机](Pictures\Camera.jpg)
			![裁剪高度](Pictures\Camera_cut_height.jpg)
			* 根据纵横比Aspect = Width/Height(如16:9)得到横向的宽度关于上述高度和Aspect表达式
			* 最终的投影矩阵如下:
			![裁剪矩阵](Pictures\frustum.jpg)
			可以发现，相当于对x，y，z做了缩放，对z再做了个平移，将其变换到了裁剪空间中，之后需要对顶点判断，必须保证x、y、z都在[-w,w]之间
			![点透视变换](Pictures\透视投影.jpg)
			* 上图为对点的变换结果，实际上由于unity里面的其实都是左手系，而不是书上说的右手系。所以最后一行要写0,0,1,0而不应该是0,0,-1,0，故变换后的w是z而不是-z
		* 正交投影：
			* 类似的透视投影，也可以得到正交头用的裁剪矩阵和对点的裁剪结果
			![正交投影裁剪矩阵](Pictures\正交投影裁剪矩阵.jpg)
			![裁剪高度](Pictures\Transforms.jpg)
			* 需要注意的是：正交投影对顶点变换后，w依然是1
			* 用于判断变换后顶点是否位于视椎体内的不等式和透视投影一样。都是要三个分量全部在[-w,w]之间。
* 4.6.8 屏幕空间
	* 经过透视投影变换到裁剪空间后，还需要将视椎体投影到屏幕空间 	

## 第五章 unity shader入门

### 5.2  简单的顶点/片元着色器

![](Pictures\1.png)

+ 例子1：

  ![](Pictures\2.png)

  要点：

  + 语句块基本是固定的
  + pragma声明了函数的作用，传入参数的语义限定说明了传入参数的性质。
+ 例子2:
  ![](Pictures\3.png)
  要点:
  + 传入多个参数时用结构体，语义限定就在结构体各个参数后面写。一般结构体的名称也是有含义的：
    + a2v（application To vertex应用到顶点）
    + v2f（vertex To fragment顶点到片元着色器）
  + 这些语义一般有：POSITION（顶点坐标），TANGENT，NORMAL（法线），TEXCOORD0~3，COLOR等。
  + 这些参数是从Mesh Render组件来的，每帧调用Draw Call时就把三角面的信息发送过来。

#### 5.2.3 顶点着色器与片元着色器的通信

+ 例子1：

  ![](Pictures\4.png)

+ 要点：

  + SV_POSITION是一定要赋值得到的，所以这里声明了返回必须有表示SV_POSITION的
  + COLOR0一般存放颜色，还有COLOR1等。如逐顶点的高光反射颜色，逐顶点的漫反射颜色等。

#### 5.2.4 属性的使用

+ 材质提供了在Shader中调节参数的方式，这些参数就写在Properties中。

+ 例子1：颜色拾取器

  ![](Pictures\5.png)

  要点：

  + ShaderLab的属性相当于变量声明，传入一些参数，如贴图，颜色之类。后面pass要用的时候就得用CG语言再定义变量，且类型和名称得与Properties中的相匹配。
  + uniform fixed4 _Color。uniform可省略。

![](Pictures\6.png)

### 5.3 Unity的内置文件与变量

#### 5.3.1 内置的包含文件

+ 类似头文件，也是#include "name"。但后缀为cginc

![](Pictures\7.png)

+ UnityCG.cginc

  + 常用结构体

    ![](Pictures\8.png)

  + 常用帮助函数

    ![](Pictures\9.png)

    联系记忆：模型空间是物体自身的空间，比如子物体的transform就显示的是在父物体模型空间下的坐标旋转数据。而没有父物体的就是世界坐标。

### 5.4 Unity提供的CG/HLSL语义

#### 5.4.1 什么是语义

+ 如SV_POSITION,TANGENT,NORMAL等，都是语义。它们就是用来修饰参数的含义，让unity知道要从这里读取或者输出数据。
+ 有的语义有特别规定。如TEXCOORD0，在顶点着色器的输入结构体中，unity将模型的第一组纹理的坐标存入其中。而在其输出中，就能由自己决定了。
+ 带SV的表示system-value，这些叫系统数值语义。SV_POSITION表示其次裁剪空间的坐标。

#### 5.4.2 Unity支持的语义

+ a2v的常用语义

  ![](Pictures\10.png)

  注：

  ![](Pictures\11.png)

+ v2f的常用语义

  ![](Pictures\12.png)
  
  一般地，如果要把自定义的一些数据传给片元着色器，则用TEXCOORD0

+ 5.4.3 定义复杂变量
  + 一般没法直接定义矩阵，float4x4得拆成4个float4

### 5.5 Debug

### 5.6 渲染平台的差异

#### 5.6.1 渲染纹理的坐标差异

![](Pictures\13.png)

一般情况下，在DirectX平台上，Unity会自动翻转，但当开启抗锯齿（Edit->Project Settings->Quality->Anti Aliasing）时不会，因为此时会使主纹理的问纹素大小在竖直方向上变为负值，以便对主纹理采样，这时得把其它纹理进行竖直方向上的翻转。这时要在顶点着色器中反转某些渲染纹理的纵坐标，如深度纹理，亮部纹理。噪声纹理一般不处理。

![](Pictures\14.png)

### 5.7 Shader代码规范化

#### 5.7.1 float、half、fixed选择

![](Pictures\15.png)

+ 桌面float，移动测试决定。fixed基本不用，被half代替。
+ 一般用fixed存颜色及单位矢量

#### 5.7.3 避免不必要的计算

+ 后果：寄存器数目不足

![](Pictures\16.png)

#### 5.7.4 慎用分支和循环

+ 由于if-else，for，while在GPU中实现方式与CPU不同，性能耗费大，应少用。
+ 解决办法是：计算向渲染管线上端移动，如把片元着色器的计算放到顶点着色器，或直接在CPU中与计算，把结果传shader。

#### 5.7.4 别除以0

# 第六章 Unity中的基础光照

+ 像素颜色 = 可见性 + 光照

## 6.1 如何观察世界

### 6.1.1 光源

### 6.1.2  吸收和散射

+ 散射只改变光线方向，不改变光线密度和颜色，吸收改变光的密度和颜色，不改变光线方向
+ 光线在物体表面散射：
  + 一部分散射到外部，即反射。一般用specular来代表
  + 一部分继续到内部，部分经过多次散射可能出表面，即漫反射。一般用diffuse代表

### 6.1.3 着色

+ 着色：根据材质属性（如diffuse属性）、光源信息（如lightDirection， lightColor）用等式计算沿某个方向的出射度的过程。
+ 该等式一般称光照模型

### 6.1.4 BRDF模型

+ BRDF可以在给出入射光线的方向和辐照度后给出某个出射方向的光照能量分布

## 6.2 标准光照模型（Blinn-Phong）

+ 特点：

  + 只关心直接光照

+ 基本组成

  + 环境光：c<sub>ambient</sub> = globalAmbient

  + 自发光：c<sub>emissive</sub> = m<sub>emissive</sub> 使用材质的自发光颜色

  + 漫反射：

    + 遵循兰伯特定律

    + **兰伯特定律**

      **c<sub>diffuse</sub> = (c<sub>light</sub>  * m<sub>diffuse</sub>) max(0, normalDirection * lightDirection)**

    + **半兰伯特定律**

      **c<sub>diffuse</sub> = (c<sub>light</sub>  * m<sub>diffuse</sub>) (a * normalDirection * lightDirection + b)**

      当a = b = 0.5时就为半兰伯特模型，实现阴影面的加光

  + 高光反射：
    + 反射方向r：
      + r = 2 [ n · l ] · n - l
    + 对于Phong模型，没使用半程向量
      + **c<sub>specular</sub> = (c<sub>light</sub>  * m<sub>specular</sub>) * pow(max(0, viewDirection* reflectDirection), m<sub>gloss</sub>**)
    + 对于Blinn-Phong模型，则使用半程向量
      + halfway = normalize(lightDirection + viewDireciton)
      + **c<sub>specular</sub> = (c<sub>light</sub>  * m<sub>specular</sub>) * pow(max(0, normal · halfway), m<sub>gloss</sub>**)

### 6.2.5 逐像素&逐顶点

+ vertex为顶点着色，frag为片元着色

+ **Phong着色：在frag中，会以每个像素为基础得到其法线，可以从顶点法线差值得到，也可以从法线纹理采样得到。**

  **这种在面间插值顶点法线的技术为Phong着色**

+ Gouraud着色：在每个顶点上计算光照，在渲染图元内部进行线性插值。
  + 优点：
    + 一般地，顶点数量<像素数量，计算量会小些
  + 缺点：
    + 插值是线性的，对于非线性的光照，如Specular，产生问题。所以一般放到frag里面处理和计算
    + 插值时是对渲染图元内部的顶点的颜色插值，导致其颜色总是暗于顶点的最高颜色值

### 6.2.6 局限性

+ 菲涅尔反射问题
+ 是各向同性的，对于各向异性很难搞

## 6.6 Unity内置函数

![](pictures/17.png)

+ 用前记得归一化

# 第七章  基础纹理

+ 纹理映射：又称纹理贴图，是将纹理空间中的纹理像素映射到屏幕空间中的像素的过程
+ 纹理映射坐标：也称uv坐标。一般在0到1.但是采样时的纹理坐标可能不在0到1，要看_MainTex_ST怎么设置的uv
+ **OpenGL里面纹理空间的原点在左下角，DirectX在左上角**

## 7.1 单张纹理

```glsl
Shader "Unity Shaders Book/Chapter7/Single Texture"
{
    Properties
    {
        _Color("Color Tint", Color) = (1,1,1,1)
        _MainTex("Main Tex", 2D) = "white" {}//定义纹理
        _Specular("Specular", Color) = (1,1,1,1)
        _Gloss("Gloss", Range(8, 256)) = 20
    }

    SubShader
    {
        pass
        {
            Tags
            {
                "LightMode" = "ForwardBase"//设置光照模式为前向渲染
            }
            CGPROGRAM
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            #pragma vertex vert
            #pragma fragment frag

            fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;//纹理定义一个sampler2D和一个偏移缩放
            fixed4 _Specular;
            float _Gloss;

            struct a2v
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float2 uv : TEXCOORD2;
            };

            v2f vert(a2v v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                //等价于o.uv = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST_zw;
                return o;
            }

            fixed4 frag(v2f i) : SV_TARGET
            {
                fixed3 worldNormal = normalize(i.worldNormal);
                fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
                fixed3 viewDirection = normalize(UnityWorldSpaceViewDir(i.worldPos));
                fixed3 halfway = normalize(viewDirection + worldLightDir);

                fixed3 albedo = tex2D(_MainTex, i.uv).rgb * _Color.rgb;
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;

                fixed3 diffuse = _LightColor0.rgb * albedo * max(0, dot(worldLightDir, worldNormal));
                
                fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(max(0, dot(halfway, worldNormal)), _Gloss);

                return fixed4(ambient + diffuse + specular, 1.0);
            }
            ENDCG

        }
    }
    Fallback "Specular"
}
```

## 7.2 凹凸映射

+ 凹凸映射分成两种。一种是高度纹理，其模拟表面位移，通过算梯度得到normal的x和y，再叉乘得到normal的z。一种是法线纹理，其存储了顶点切线空间下的normal的x和y，由归一化的特性得到z值，进而得到切线空间下的normal。一般凹凸映射指后者

### 7.2.2 法线纹理

> 常见的映射方式就是pixel = (normal + 1)  / 2

### 7.2.3 法线贴图实践代码

#### 1. 切线空间

```glsl
Shader "Untiy Shaders Book/Chapter 7/Normal Map In Tangent Space"
{
    Properties
    {
        _Color ("Color Tint", Color) = (1,1,1,1)
        _MainTex("Main Tex", 2D) = "white" {}//主纹理
        _BumpMap("Normal Map", 2D) = "bump" {}//法线贴图
        _BumpScale("Bump Scale", float) = 1.0//缩放比例
        _Specular("Specular", Color) = (1,1,1,1)
        _Gloss("Gloss", Range(8,256)) = 20.0
    }
    SubShader
    {
        pass
        {
            Tags
            {
                "LightMode" = "ForwardBase"
            }
            CGPROGRAM
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            #pragma vertex vert
            #pragma fragment frag

            fixed4 _Color;

            sampler2D _MainTex;
            float4 _MainTex_ST;//偏移值

            sampler2D _BumpMap;
            float4 _BumpMap_ST;//一般偏移是相同的，不同的时候v2f的uv得用float4了

            float _BumpScale;
            fixed4 _Specular;
            float _Gloss;

            struct a2v
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 texcoord : TEXCOORD0;//定义模型顶点的纹理映射坐标
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 uv : TEXCOORD0;
                float3 lightDir : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
            };

            v2f vert(a2v v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                
                //得到模型顶点的纹理映射坐标，第一个对应于第一个纹理，第二个则是法线贴图的，不过一般而言都是建模出来的，两个是一样的，这里不一样
                o.uv.xy = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                o.uv.zw = v.texcoord.xy * _BumpMap_ST.xy + _BumpMap_ST.zw;
                TANGENT_SPACE_ROTATION;
                //等价于float3 binormal = cross(v.normal, v.tangent.xyz) * v.tangent.w;
                //float3x3 rotation = float3x3(v.tangent.xyz, binormal, v.normal); 

                o.lightDir = mul(rotation, ObjSpaceLightDir(v.vertex));
                o.viewDir = mul(rotation, ObjSpaceViewDir(v.vertex));
                
                return o;
            }

            fixed4 frag(v2f i) : SV_TARGET
            {
                fixed3 tangentLightDirection = normalize(i.lightDir);
                fixed3 tangentViewDir = normalize(i.viewDir);

                //采样得到切线空间的法线
                fixed4 packedNormal = tex2D(_BumpMap, i.uv.zw);
                fixed3 tangentNormal;
                //如果用的法线贴图的纹理没标记为法线贴图，则
                //tangentNormal.xy = (2 * packedNormal.uv - 1) * _BumpScale;
                //tangentNormal.z = sqrt(1 - saturate(dot(tangentNormal.xy, tangentNormal.xy)));

                //一般都标记为NormalMap，这样它会给你压缩，只用UnpackNormal就能解压并完成 2* packedNormal.uv操作
                tangentNormal = UnpackNormal(packedNormal);
                tangentNormal.xy *= _BumpScale;
                tangentNormal.z = sqrt(1 - saturate(dot(tangentNormal.xy, tangentNormal.xy)));

                fixed3 albedo = tex2D(_MainTex, i.uv).rgb * _Color.rgb;
                
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;

                fixed3 diffuse = _LightColor0.rgb * albedo * saturate(dot(tangentLightDirection,  tangentNormal));

                fixed3 halfWay = normalize(tangentLightDirection + tangentViewDir);
                fixed3 specular = _Specular * _LightColor0.rgb * pow(saturate(dot(halfWay, tangentNormal)), _Gloss);

                return fixed4(ambient + diffuse + specular, 1);
            }   
            ENDCG
        }
    }
    Fallback "Specular"
}
```

#### 2.世界空间

```glsl
Shader "Untiy Shaders Book/Chapter 7/Normal Map In World Space"
{
    Properties
    {
        _Color("Color Tint", Color) = (1,1,1,1)
        _MainTex("Main Tex", 2D) = "white" {}
        _BumpMap("Bump Map", 2D) = "bump" {}
        _BumpScale("Bump Scale", float) = 1
        _Specular("Specular", Color) = (1,1,1,1)
        _Gloss("Gloss", Range(-80, 200)) = 20
    }
    SubShader
    {
        pass
        {
            Tags
            {
                "LightMode" = "ForwardBase"
            }
            CGPROGRAM
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            #pragma vertex vert
            #pragma fragment frag

            fixed4 _Color;

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _BumpMap;
            float4 _BumpMap_ST;

            float _BumpScale;
            fixed4 _Specular;
            float _Gloss;

            struct a2v
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 uv : TEXCOORD0;
                float4 TtoW0 : TEXCOORD1;
                float4 TtoW1 : TEXCOORD2;
                float4 TtoW2 : TEXCOORD3;
            };

            v2f vert(a2v v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                
                //得到模型顶点的纹理映射坐标，第一个对应于第一个纹理，第二个则是法线贴图的，不过一般而言都是建模出来的，两个是一样的，这里不一样
                o.uv.xy = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                o.uv.zw = v.texcoord.xy * _BumpMap_ST.xy + _BumpMap_ST.zw;
                TANGENT_SPACE_ROTATION;
                //等价于float3 binormal = cross(v.normal, v.tangent.xyz) * v.tangent.w;
                //float3x3 rotation = float3x3(v.tangent.xyz, binormal, v.normal); 

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
                fixed3 worldTangent = UnityObjectToWorldDir(v.tangent);
                fixed3 worldBiNormal = cross(worldNormal, worldTangent) * v.tangent.xyz;

                o.TtoW0 = float4(worldTangent.x, worldBiNormal.x, worldNormal.x, worldPos.x);
                o.TtoW1 = float4(worldTangent.y, worldBiNormal.y, worldNormal.y, worldPos.y);
                o.TtoW2 = float4(worldTangent.z, worldBiNormal.z, worldNormal.z, worldPos.z);
                
                return o;
            }

            fixed4 frag(v2f i) : SV_TARGET
            {
                float3 worldPos = float3(i.TtoW0.w, i.TtoW1.w, i.TtoW2.w);
                fixed3 worldLightDirection = normalize(UnityWorldSpaceLightDir(worldPos));
                fixed3 worldViewDirection = normalize(UnityWorldSpaceViewDir(worldPos));

                fixed3 tangentNormal;
                tangentNormal.xy = UnpackNormal(tex2D(_BumpMap, i.uv.zw)).xy * _BumpScale;
                tangentNormal.z = sqrt(1 - saturate(dot(tangentNormal.xy, tangentNormal.xy)));
                float3x3 rotation = float3x3(i.TtoW0.xyz, i.TtoW1.xyz, i.TtoW2.xyz);
                fixed3 worldNormal = normalize(mul(rotation, tangentNormal));

                fixed3 albedo = tex2D(_MainTex, i.uv).rgb * _Color.rgb;
                
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;

                fixed3 diffuse = _LightColor0.rgb * albedo * saturate(dot(worldLightDirection,  worldNormal));

                fixed3 halfWay = normalize(worldLightDirection + worldViewDirection);
                fixed3 specular = _Specular * _LightColor0.rgb * pow(saturate(dot(halfWay, worldNormal)), _Gloss);

                return fixed4(ambient + diffuse + specular, 1);
            }
            ENDCG
        }
    }
}
```

## 7.3 渐变纹理

+ 用途：得到插画风格的渲染效果

```glsl
Shader "Unity Shaders Book/Chapter 7/Ramp Texture" {
	Properties {
		_Color ("Color Tint", Color) = (1, 1, 1, 1)
		_RampTex ("Ramp Tex", 2D) = "white" {}//定义渐变纹理贴图
		_Specular ("Specular", Color) = (1, 1, 1, 1)
		_Gloss ("Gloss", Range(8.0, 256)) = 20
	}
	SubShader {
		Pass { 
			Tags { "LightMode"="ForwardBase" }
		
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag

			#include "Lighting.cginc"
			
			fixed4 _Color;
			sampler2D _RampTex;
			float4 _RampTex_ST;//定义纹理及其Scale&Translation
			fixed4 _Specular;
			float _Gloss;
			
			struct a2v {
				float4 vertex : POSITION;
				float3 normal : NORMAL;//不使用模型的纹理坐标进行采样
			};
			
			struct v2f {
				float4 pos : SV_POSITION;
				float3 worldNormal : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
			};
			
			v2f vert(a2v v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				
				return o;
			}
			
			fixed4 frag(v2f i) : SV_Target {
				fixed3 worldNormal = normalize(i.worldNormal);
				fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
				fixed3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));

                //主要不同在于：
                //1，ambient没有采样纹理结果，二是直接用了全局光
                //2. diffuse不是使用纹理映射坐标uv.xy采样，而是直接用half-Lambert采样，因为half-Lambert也在0到1
                //3. specular不变，还是一样用半程向量
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
				
				// Use the texture to sample the diffuse color
				fixed halfLambert  = 0.5 * dot(worldNormal, worldLightDir) + 0.5;
                
				fixed3 diffuseColor = tex2D(_RampTex, fixed2(halfLambert, halfLambert)).rgb * _Color.rgb;
				fixed3 diffuse = _LightColor0.rgb * diffuseColor;
				
				
				fixed3 halfDir = normalize(worldLightDir + viewDir);
				fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(max(0, dot(worldNormal, halfDir)), _Gloss);
				
				return fixed4(ambient + diffuse + specular, 1.0);
			}
			ENDCG
		}
	}
	FallBack "Specular"
}
```

## 7.4 遮罩纹理

+ 流程：
  1. 采样得到这招纹理的texel的值
  2. 利用其中某些个通道值和某种表面属性相乘。一般地，多个通道可分别控制不同光照
+ 作用：
  + 实现像素级别控制模型表面的各种属性

### 7.4.1 遮罩纹理实践

```glsl
Shader "Unity Shaders Book/Chapter 7/Mask Texture"
{
    Properties
    {
        _Color("Color Tint", Color) = (1,1,1,1)
        _MainTex("Main Tex", 2D) = "white" {}
        _BumpMap("Normai Map", 2D) = "bump"{}
        _BumpScale("Bump Scale", float) = 1
        _SpecularMask("Specular Mask", 2D) = "white" {}
        _SpecularScale("Specular Scale", float) = (1,1,1,1)//定义高光遮罩纹理
        _Specular("Specular", Color) = (1,1,1,1)
        _Gloss("Gloss", Range(8.0, 256)) = 20
    }
    SubShader
    {
        pass
        {
            Tags
            {
                "LightMode" = "ForwardBase"
            }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;//都使用的一个偏移缩放值，所以就只定义一个即可
            sampler2D _BumpMap;
            float _BumpScale;
            sampler2D _SpecularMask;
            float _SpecularScale;
            fixed4 _Specular;;
            float _Gloss;

            struct a2v
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 lightDir : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
            };

            v2f vert(a2v v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);//都使用一个纹理属性_MainTex_ST
                TANGENT_SPACE_ROTATION;
                o.lightDir = mul(rotation, ObjSpaceLightDir(v.vertex));
                o.viewDir = mul(rotation, ObjSpaceViewDir(v.vertex));//转到切线空间计算
                return o;
            }

            fixed4 frag(v2f i) : SV_TARGET
            {
                fixed3 tangentLightDirection = normalize(i.lightDir);
                fixed3 tangentViewDirection = normalize(i.viewDir);

                fixed3 tangentNormal = UnpackNormal(tex2D(_BumpMap, i.uv)).xyz;
                tangentNormal.xy *= _BumpScale;
                tangentNormal.z = sqrt(1 - saturate(dot(tangentNormal.xy, tangentNormal.xy)));
                //计算normal

                fixed3 albedo = tex2D(_MainTex, i.uv.xy).rgb * _Color.rgb;

                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;

                fixed3 diffuse = _LightColor0.rgb * albedo * max(0, dot(tangentNormal, tangentLightDirection));

                fixed3 halfDir = normalize(tangentLightDirection + tangentViewDirection);
                fixed specularMask = tex2D(_SpecularMask, i.uv.xy).r * _SpecularScale;

                fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(max(0, dot(halfDir, tangentNormal)),_Gloss) * specularMask;//使用遮罩纹理的r通道控制specular

                return fixed4(ambient + diffuse + specular, 1.0);
            }
            ENDCG
        }
    }
    Fallback "Specular"
}
```

# 第八章 透明效果

+ 渲染顺序与深度缓冲：
  + 场景渲染时，对于不同的物体有着不同的渲染先后顺序，这是使用**z-buffer**完成的。深度缓冲是用于解决可见性问题的。基本思想：在物体开启深度测试的情况下，渲染一个片元时，需要把它的深度值与已经存在于深度缓冲中的值进行比较，如果距离相机更远，则舍去；更近或者深度缓冲没值，就覆盖或者存入深度值。

+ 实现透明效果的方法：
  + 透明度测试：
    + 基本思想：如果alpha小于某个值，就舍去，否则按正常渲染不透明物体方式进行渲染。
    + 特点：
      + 不关闭深度写入
      + 效果极端，要么看的见，要么看不见
  + 透明度混合
    + 基本思想：使用当前片元透明度作混合因子，与已经存储在颜色缓冲中的颜色值进行混合，得到新颜色。
    + 特点：
      + 关闭深度写入，但深度测试还是有。深度缓冲是只读不写的。
      + 可以实现真正的半透明效果

## 8.1 渲染顺序的重要性

+ 问题：渲染一个半透明物体和不透明物体，且透明物体先渲染，不透明物体后渲染，在半透明物体开启深度写入的情况下，不透明物体被cull掉，而不开启深度写入时，由于半透明物体要混合颜色，则不透明的得先渲染。即使是两个不透明的物体，渲染顺序不同，混合的等式也不同，也要注意渲染顺序
+ 常用方法：
  + 先渲染所有不透明物体，对不透明物体开启深度测试和深度写入
  + 对半透明物体按照从后往前顺序渲染，要关闭深度写入
+ 依然存在的问题：
  + 三个半透明物体循环重叠，无法判断哪个在前，任意一个顺序都错
    + 一般要进行网格分割，或者尽可能让模型是凸面体，将模型进行拆分

## 8.2 Unity Shader的渲染顺序

![](pictures/18.png)

+ 代码示例：

  ![](pictures/19.png)

## 8.3 透明度测试

```glsl
Shader "Unity Shaders Book/Chapter8/Alpha Test"
{
    Properties
    {
        _Color("Main Tint", Color) = (1,1,1,1)
        _MainTex("Main Tex", 2D) = "white"{}
        _Cutoff("Alpha Cutoff", Range(0,1)) = 0.5//定义剔除的阀值
    }
    SubShader
    {
        Tags
        {
            "Queue" = "AlphaTest"	//设置渲染队列
            "IgnoreProjector" = "True"//忽略投影器影响
            "RenderType" = "TransparentCutout"//指明渲染类型，让unity将shader归到TranspareCutout组
        }
        pass
        {
            Tags{
                "LightMode" = "ForwardBase"
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag 
            #include "Lighting.cginc"
            fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed _Cutoff;

            struct a2v
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float2 uv : TEXCOORD2;
            };

            v2f vert(a2v v)
            {
                v2f o;
                o.pos =  UnityObjectToClipPos(v.vertex.xyz);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos =  mul(unity_ObjectToWorld, v.vertex.xyz);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_TARGET
            {
                fixed3 worldNormal = normalize(i.worldNormal);
                fixed3 worldLightDirection = normalize(UnityWorldSpaceLightDir(i.worldPos));

                fixed4 texColor = tex2D(_MainTex, i.uv);
                clip(texColor.a - _Cutoff);
                //等价于if ((texColor.a - _Cutoff) < 0) discard;这里就是透明度测试

                fixed3 albedo = texColor.rgb * _Color;
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
                fixed3 diffuse = _LightColor0.rgb * albedo * max(0, dot(worldLightDirection, worldNormal));
                return fixed4(diffuse + ambient, 1);
            }
            ENDCG
        }
    }
    Fallback "Transparent/Cutout/VertexLit"
}
```

## 8.4 透明度混合

+ Blend命令

  ![](pictures/20.png)

+ Blend命令里的混合因子

![](pictures/21.png)

+ 混合操作

  见P174到P175

### 8.4.1 透明度混合实例

+ 当不开启深度写入时，对于物体本身交错的情况会因为渲染片元顺序产生问题，会导致后面的部分显示在前面。解决办法是使用两个pass，第一个pass开启深度写入，但不输出颜色。第二个才关闭深度写入，由于预先已经存在了深度缓冲，对于前后两个片元，后面的那个会被剔除掉，前面的片元能进行正常渲染。

```glsl
Shader "Unity Shaders Book/Chapter 8/Alpha Blend" {
    Properties {
        _Color ("Color Tint", Color) = (1, 1, 1, 1)
        _MainTex ("Main Tex", 2D) = "white" {}
        _AlphaScale ("Alpha Scale", Range(0, 1)) = 1
        _Specular ("Specular", Color) = (1,1,1,1)
        _Gloss ("Gloss", Range(8, 255)) = 20
    }
    SubShader {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        pass
        {
            ZWrite On  //默认也是开启的
            ColorMask 0//设置不输出颜色
        }
        Pass {
			Tags { "LightMode"="ForwardBase" }
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha//关闭深度写入，开启并设置渲染方式

            CGPROGRAM
            #pragma vertex vert 
            #pragma fragment frag 

            #include "Lighting.cginc"
            
            
			fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed _AlphaScale;
            fixed4 _Specular;
            float _Gloss;

            struct appdata
            {
                float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
				float3 worldNormal : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
				float2 uv : TEXCOORD2;
            };

            v2f vert(appdata v)
            {
                v2f o;

                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

                return o;
            }

            fixed4 frag(v2f i) : SV_TARGET
            {
                fixed3 worldNormal = normalize(i.worldNormal);
                fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
                fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));

                fixed4 texColor = tex2D(_MainTex, i.uv);

                fixed3 albedo = texColor.rgb * _Color.rgb;

                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
                fixed3 diffuse = _LightColor0.rgb * albedo * max(0, dot(worldNormal, worldLightDir));

                fixed3 halfway = normalize(worldViewDir + worldLightDir);
                fixed3 specular = _LightColor0.rgb * _Specular * pow(max(0, dot(halfway, worldNormal)), _Gloss);
                return fixed4(ambient + diffuse + specular, texColor.a * _AlphaScale);
            }
            ENDCG
        }
    }
    Fallback "Transparent/VertexLit"
}
```

## 8.7 双面渲染的透明效果

> 剔除指令设置：
>
> Cull Back | Front | Off
>
> Back表示背对相机的不渲染，Front表示正对相机的不渲染，Off表示都渲染

### 8.7.1 透明度测试

在Pass里面加入Cull off指令，即可

### 8.7.2 透明度混合

+ 对于本身交错缠绕的，建议还是只用上面的两个Pass处理，一个负责深度写入，一个关了zwrite并进行渲染

+ 把原来的渲染部分分成两个，第一个Cull Front渲染背面，第二个Cull Back渲染正面。其余代码一样。

****

# 第九章 更复杂的光照

## 9.1 Unity的渲染路径

+ 渲染路径：决定了光照如何应用到Shader上。
+ 一般地，需要为每个Pass指定它使用的渲染路径，如"LightMode" =  "ForwardBase"以让Unity把光源和处理后的光照信息放在数据里

+ 目前渲染路径有两种：前向渲染路径和延迟渲染路径

![](pictures/22.png)

### 9.1.1 前向渲染路径

1.  原理

   进行一次完整的前向渲染，需要渲染该物体的渲染图元，并计算深度缓冲和颜色缓冲的信息。如果片元通过了深度缓冲，则计算光照信息，更新到颜色缓冲区，同时更新帧缓冲。否则就去除。

   如果场景有N个物体，每个物体受到M个光源的影响，则整个场景要有N*M个Pass

2. Unity中的前向渲染

   + 当渲染一个物体时，Unity会计算哪些光源照亮了物体，以及这些光源照亮物体的方式。

   + Unity中，前向渲染有3种处理光照的方式：**逐顶点处理、逐像素处理、球谐函数**处理等。这取决于光源使用的**类型和渲染模式**

     + 光源的类型：平行光/点光源/其它类型
     + 渲染模式：Important / Auto / Not Important

   + Unity会根据场景中各个光源的设置及其对物体的影响程度进行重要度排序，并决定其处理的方式。一定数目的按逐像素处理，最多4个逐顶点处理，剩下的按SH处理。

     **判断规则：**

     1. **平行光    逐像素**
     2. **Not Important    逐顶点/SH**
     3. **Important    逐像素**
     4. **如果上面的完了，逐像素数量小于Quality Setting中的逐像素光数量，则有更多的以逐像素进行**
     
   + 球谐函数的特点

     球谐函数光源的渲染速度_很_快。这些光源的 CPU 成本很低，并且使用 GPU 的_成本基本为零_（也就是说，ForwardBase始终会计算 SH 光照；但由于 SH 光源工作方式的原因，无论 SH 光源有多少，成本都完全相同）。

     SH 光源的缺点：

     - 按对象的顶点而不是按像素计算。这意味着它们不支持光照剪影和法线贴图。
     - SH 光照的频率很低。SH 光源无法实现快速的光照过渡。它们也只影响漫射光照（频率对镜面高光而言太低）。
     - SH 光照不是局部光照；SH 点光源或聚光灯在靠近某种表面时“看起是错误的”。

     总的来说，SH 光源通常足以达到小型动态对象的光照要求。

   ![](pictures/24.png)

   两种pass：

![](pictures/23.png)

3. 内置的光照变量和函数(P184)

### 9.1.2 顶点照明渲染路径

+ 不支持逐像素得到的效果，是前向渲染的一个子集，实际就是逐顶点着色罢了。
+ 通常在一个pass完成对物体的渲染。使用所有光源对物体的照明，光源最多8个，不足8个的为黑色

### 9.1.3 延迟渲染路径

+ 对于前向渲染，如果有大量的实时光源，性能会急速下降。因为对于一个物体，如果有许多光源照在上面，它可能有很多重复的计算，多次计算颜色缓冲区的颜色。
+ 延迟渲染不仅使用颜色缓冲和深度缓冲，还使用G-buffer，其存储了我们关心的表面的其它信息，如normal，position，material属性等

1. 原理：

   主要包含2个pass.

   + 第一个pass不进行光照计算，仅仅计算哪些片元是可见的，这主要使用z-buffer，当一个片元可见，就把它的相关信息存储到G-buffer中。每个物体只执行一次
   + 第二个pass使用G-buffer的片元信息进行真正的光照计算，得到目标片元的光照信息，进行颜色计算，同时更新帧缓冲。

   特点：

   + 使用2个pass，且和光源数目无关，其不依赖于场景复杂度，而与屏幕空间的大小有关

2. Unity中的延迟渲染
   + 适合场景：光源数目多，每个光源都可以逐像素处理
   + 缺点：
     + 不能支持真正的抗锯齿功能
     + 不能处理半透明物体。因为要用深度缓冲和深度写入直接决定哪些片元可见，这就意味着一个位置只有一个片元的数据
     + 硬件要求
   + 
