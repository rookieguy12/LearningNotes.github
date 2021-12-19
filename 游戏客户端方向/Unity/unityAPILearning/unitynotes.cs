//API

//GameObject
activeInHierarchy;//物体是否被激活(要求父物体和自身是激活)
activeSelf;//自身是否被激活
//1.静态方法
Find(string name);
FindGameObjectsWithTag(string tag);
FindWithTag(string tag);
//2.公有方法
public void BroadcastMessage (string methodName, object parameter= null, SendMessageOptions options= SendMessageOptions.RequireReceiver);
//调用此游戏对象或其任何子项中的每个 MonoBehaviour 上名为 methodName 的方法,并且设置参数列表为parameter
public void SendMessage (string methodName, object value= null, SendMessageOptions options= SendMessageOptions.RequireReceiver);
//调用此游戏对象中的每个 MonoBehaviour 上名为 methodName 的方法。
public void SendMessageUpwards (string methodName, object value= null, SendMessageOptions options= SendMessageOptions.RequireReceiver);
//调用此游戏对象中的每个 MonoBehaviour 上或此行为的每个父级上名为 methodName 的方法。


/*场景加载*/
SceneManager.LoadScene("Scene/sample1", LoadSceneMode.Addictive);//同步加载场景(附加)
SceneManager.LoadSceneAsync("scenepath", LoadSceneMode.Addictive);//异步加载场景, 返回一个类AsyncOperation
AsyncOperation.isDone;//判断场景加载跳转是否完成，所以当前允许跳转还无法为true，而是跳转场景允许的后一帧才会显示true
AsyncOperation.allowSceneActivation;//加载完成时是否允许跳转
AsyncOperation.progress;//加载进度(0到0.9)


//切换场景时的物体保留
GameObject Cube = GameObject.Find("Cube");
DontDestroyOnLoad(Cube);//加载新场景时保留Cube物体
SceneManager.LoadScene("Scene/sample1");	


/*预制件的保存，读取，更新，删除*/
//查找并加载一个Resources内的预制件
GameObject hero = Resources.Load<GameObject>("Sphere");
GameObject.Instantiate(hero);

//加载预制件,也可先创建空白public GameObject,做相同引入操作,再到unity里头指定该GameObject对象,不使用resources文件夹可以这么做
public GameObject hero;
GameObject.Instantiate(hero);

/*脚本中对场景物体的交互*/
//GameObject.Find();无法查找隐藏对象或任意父节点被隐藏的对象
//通过路径查找
GameObject Cubetemp = GameObject.Find("Cube1");
//通过标签查找
GameObject Cube2 = GameObject.FindGameObjectWithTag("Cube");
//Transform.Find();用于从本结点查找直接子对象，只要自己没被隐藏，就能找子对象
inputfield = transform.Find("InputField").GetComponent<InputField>();


//修改物体
Cubetemp.name = "Cube2";	//修改物体名字
Cubetemp.transform.Position = new Vector3(x, y, z);//修改物体在全域的位置
Cubetemp.transform.localPosition = new Vector3.zero;//修改物体到相较于所属物体组件原点的位置
sphere.transform.position = new Vector3(23, 51, 34);//改位置
sphere.transform.localScale = new Vector3(5, 5, 5);	//缩放
sphere.transform.rotation = Quaternion.Euler(0,0,90);//旋转
sphere.transform.eulerAngles = new Vector3(100, 40.5f, 120);//旋转

/*旋转*/
Transform.Rotate();
transform.LookAt(target : Transform, worldUp : Vector3 = Vector3.up);//旋转物体使z轴(forward)指向目标物体。

Vector3.RotateTowards()
Quaternion LookRotation(Vector3 targetforward, Vector3 upwards);//建立一个旋转，使z轴朝向targetforward，y轴朝向默认为世界坐标up。返回为向量角度差值，将其赋给rotation就能旋转到该方向
//所得的结果是 以forward为目标朝向时的旋转角，用于控制角色旋转很有用


//删除物体
Destroy(sphere);//删的物体不能是assert内的资源，注意名称问题

/*物体移动*/
float xmove = Input.GetAxis("Horizontal");//AD控制
float zmove = Input.GetAxis("Vertical");//WS控制,此处设置为前后，也可设置为上下，看后面的那条语句
sphere.transform.Translate(new Vector3(xmove, 0, zmove) * step * Time.deltaTime);
/*视角变化*/
public class shoot : MonoBehaviour
{
    //设置鼠标灵敏度
    public float sensitivity = 3f;
    public float rotver;
    public float rothor;
    public float upver = -60;
    public float downver = 45;
    void Start()
    {
        rotver = transform.eulerAngles.x;
		rothor = transform.eulerAngles.y;
    }
    void Update()
    {
        float mouseVer = Input.GetAxis("Mouse Y");
        float mouseHor = Input.GetAxis("Mouse X");
        //上下视角变换,注意,鼠标向下划为负，但是视角向下转值变大，故用减法
        rotver -= mouseVer * sensitivity;
        rotver = Mathf.Clamp(rotver, upver, downver);//竖直方向的视角需要有控制范围的函数
        //水平视角变换
        rothor += mouseHor * sensitivity;
        transform.localEulerAngles = new Vector3(rotver, rothor, 0);
	}
/*transform.forward与Vector3.forward*/
forward==>(0,0,1);//蓝轴z
up==>(0,1,0);//绿轴y
right==>(0,1,0);//红轴x
Vector3.forward;//表示世界坐标系下(0,0,1)
Vector3 dir = transform.TransformDirection(Vector3.forward);//将相对于本地坐标系的(0,0,1)转换成为世界坐标系dir,dir也等于transform.forward
/*Translate方法*/
public void Translate (Vector3 translation);
public void Translate (Vector3 translation, Space relativeTo= Space.Self);
public void Translate (float x, float y, float z);
public void Translate (float x, float y, float z, Space relativeTo= Space.Self);

/*鼠标的隐藏与显示*/
public enum CursorLockMode
    {
        None = 0,
                //游标行为不作修改
        Locked = 1,
                //锁定游标到游戏窗口的中心
        Confined = 2
                //将游标限制在游戏窗口中
    }
public static CursorLockMode lockState { get; set;}//确定硬件指针是锁定到视图的中心，是否受窗口限制，或者根本不受约束。
public static bool visible { get; set; }//确定硬件指针是否可见
Cursor.lockState = CursorLockMode.Locked;//锁定指针到视图中心
Cursor.visible = false;//隐藏指针


//组件管理  通过代码添加和查找组件
//添加组件
GameObject textcolomn = GameObject.Find("Text");
textcolomn.AddComponent<AudioSource>();	//给Text物体对象添加一个音频组件

//查找自身组件并修改组件内容
//单个组件查找，直接返回该组件
textcolomn.GetComponent<Text>().text = "查找到该组件";
//查找自身及子物体的组件并修改组件内容，优先查自己的，查到就就用这个了
textcolomn.GetComponentInChildren<Text>().text = "查到该组件了";
//查找自身及父物体的组件并修改组件内容，优先查自己的，查到就就用这个了
textcolomn.GetComponentInParent<Text>().text = "查到该组件了";

//多个相同组件的查找，返回该组件的数组
Text [] textcolomns = textcolomn.GetComponents<Text>();
//查找自身及子物体的组件内容
Text [] textcolomns = textcolomn.GetComponentsInChildren<Text>();
//查找自身及父物体的组件内容
Text [] textcolomns = textcolomn.GetComponentsInParent<Text>();



//通过代码激活，删除，关闭组件
textcolomn.GetComponent<Text>().enabled = false;	
textcolomn.GetComponent<Text>().enabled = true;//组件都有一个bool成员enabld，false为关闭状态，true为打开状态
this.enabled = false;	//关闭脚本自身
//删除组件
AudioSource audio = textcolomn.GetComponent<AudioSource>();
Destroy(audio,5);//用Destroy删除组件

//音乐添加
public class Player : MonoBehaviour
{
	.....
    //音源AudioSource相当于播放器，而音效AudioClip相当于磁带
    public AudioSource music;
    public AudioClip jump;//这里我要给主角添加跳跃的音效
	
	    private void Awake()
	{
		...
        //给对象添加一个AudioSource组件
        music = gameObject.AddComponent<AudioSource>();
        //设置不一开始就播放音效
        music.playOnAwake = false;
        //加载音效文件，我把跳跃的音频文件命名为jump
        jump = Resources.Load<AudioClip>("music/jump");
    }
	void Update()
	{
		...
			if (Input.GetKeyDown(KeyCode.UpArrow))//如果输入↑
			{
				....
				//把音源music的音效设置为jump
                music.clip = jump;
                //播放音效
                music.Play();
			}
			....
	}
}
	
//实现颜色渐变//也可以用协程实现颜色反复(update反复启动协程)
void Update()
    {
        timecount += Time.deltaTime;
        if (timecount < 5f)
        {
            ColorAlpha -= Time.deltaTime * 0.1f;
        }
        else if (timecount < 10f)
        {
            ColorAlpha += Time.deltaTime * 0.1f;
        }
        else timecount = 0;//反复变换
        image.GetComponent<Image>().color = new Color(255, 255, 255, ColorAlpha);
    }


//Time类
deltaTime;
fixedDeltaTime;
fixedTime;//游戏开始到目前所用的时间
Time；//游戏开始到目前所用的时间
smoothDeltaTime;//更平滑的时间
timeScale;//游戏播放速度
frameCount;//到目前运行的帧数
realtimeSinceStartup;
timeSinceLevelLoad;//从当前场景加载开始到目前的时间

//MonoBehaviour类
//1.一些未分类的方法
InvokeRepeating("方法名", 2, 0.3f);//两秒后开始调用function，之后每0.3s重复调用，不接受有参数的函数
CancleInvoke();//取消上述调用,有参数就取消参数的函数，不然就全停止
IsInvoking("方法名");
StartCoroutine();
StopCoroutine();
print();//同Debug.Log();
isActiveAndEnabled;//判断是否enabled
//2.与鼠标行为相关的,物品必须是GUIelement或者collider//在属于 Ignore Raycast 层的对象上，不调用该函数。
void OnMouseDown();
void OnMouseUp();//鼠标按下或抬起，按下后如果跑到其它的物体再抬起也会执行原来这个物体的OnMouseUp()
void OnMouseDrag();//鼠标拖拽，一直拽着就会反复调用
void OnMouseUpAsButton();//在同一个物品上按下并抬起，就触发

void OnMouseEnter();//
void OnMouseExit();//
void OnMouseOver();//会反复调用



//Mathf结构的方法
//一些静态变量
Deg2Rad;//1度对应的弧度
Rad2Deg;//与上面相反
Epsilon;//一个无限小的正数
Infinity;//正无穷
NegativeInfinity;//负无穷
PI;
//一些常用函数
Abs();//取绝对值
Sin(float);Cos(float);Tan(float);//参数为弧度
Asin(float);Acos(float);Atan(float);//返回的是弧度
Atan2(float x, float y);//返回tan值为y/x的弧度
Ceil();//向上取整，返回的是float类型
Floor();//向下取整，返回float
Clamp(float value,float min,float max);//value在min，max之间就返回value，小于就min，大于就max
Clamp01(float value);//限制在01之间
DeltaAngle(float angle1, float angle2);//返回angle1与angle2之差在0到360°的值(去掉了周期)
Max();Min();//返回多个数的最大/最小值
Lerp(float a, float b, float t);//t为[0,1]的数，返回比例的数，如t为0.5就返回中点的值,可以用来实现物体从初始位置到目标位置的运动
MoveTowards(float a, float b, float t);//实现a到b的一个匀速变化，走个t距离,t负的就为远离
LerpAngle(float minAngle, float maxAngle, float t);//如果maxAngle-minAngle的值大于180度的话,那maxAngle将相当于360-maxAngle,所以会从minAngle逆向转到360-maxAngle
PingPong(float t, float max);//随着t的不断增大，返回值会从0到max匀速地来回，可以用来设置一个匀速反复运动


//Input类
bool GetKey(KeyCode key);//按下key触发，不松就会一直触发，每帧都会触发
bool GetKeyDown(KeyCode key);//按下key的那帧触发
bool GetKeyUp(KeyCode key);//松开key的那帧触发
//三者的参数也可以是按键的字符串名字
//key的字符串名字
//1.字母	"a"
//2.数字	"1""2"
//3.方向键	"up""down""left"
//4.小键盘	"[1]""[+]""[equals]"
//5.shift，alt，cmd		"left shift""right shift" 
//6.鼠标 	"mouse 0"鼠标左键"mouse 1"右键"mouse 2"中键
//7.其它 	"backspace" "tab" "space" "escape" "enter" "delete"

bool GetMouseButton(int buttonindex);
bool GetMouseButtonDown(int buttonindex);
bool GetMouseButtonUp(int buttonindex);//同上，不过是鼠标版，0,1,2和上面的第六条对应

float GetAxis(string Axisname);//获得平滑的输入轴的值，这些输入轴名称在Edit-settings里面有，如Horizontal,Vertical,Jump,Fire1，Mouse X, Mouse Y,Mouse ScrollWheel平滑程度与灵敏度设置有关
float GetAxisRaw(string Axisname);//获得不平滑的输入轴的值，只有-1,0,1三个值

bool GetButton(string buttonname);
bool GetButtonUp(string buttonname);
bool GetButtonDown(string buttonname);//对于虚拟按键的触发，虚拟按键就是上面的Axisname，如Horizontal，Vertical啥的

bool anyKey;//是否有键处于按下的状态，包括鼠标键盘
bool anyKeyDown;//是否有任何键被按下

Vector3 mousePosition;//鼠标当前的像素位置，以左下角为(0,0,0)


GetTouch();//与触摸有关，详情看Input类


//Transform类
















//Vector2结构体
//变量
up;down;left;right;one;zero;//一些基本的常用向量
magnitude;//取得向量长度
normalized;//单位化
sqrMagnitude;//求magnitude的前一步，即x^2+y^2的值
x;y;//也可用Vector2[0];Vector2[1]
//方法
bool Equals(Vector2 other);//两个向量









//Vector3
//静态方法
RotateTowards(Vector3 current, Vector3 target, float maxRadiansDelta, float maxMagnitudeDelta);
//current表示背离的矢量，target表示朝向的矢量。然后maxRadiansDelta是角度的瞬时变化，maxMagnitudeDelta是大小的变化。





















//天空盒
//地形工具Terrain








//UGUI图形系统
//1.画布与事件系统
//2.文本输入框
InputField inputfield;
void Start()
    {
        inputfield = transform.Find("InputField").GetComponent<InputField>();
    }
//3.button
Button button;
 void Start()
    {
        inputfield = transform.Find("InputField").GetComponent<InputField>();
        button = transform.Find("Loginbtn").GetComponent<Button>();
        button.onClick.AddListener(Loginbtnonclick);//参数为一个无返回值且无参的函数
    }
    void Loginbtnonclick()
    {
		Application.OpenURL("www.baidu.com");//打开URL的一个函数
    }
//4.toggle切换
Toggle toggle;
toggle.isOn;//一个bool值，打勾了就为true
//toggle监听代码
Toggle.onValueChanged.AddListener((bool value) => resultToggleChange(Toggle, value));
void resultToggleChange(Toggle toggle, bool value){}
//里面还有开关组，可指定一个object的开关组组件，使用实现单选

//5.滑动条slider
Slider slider;
slider.onChange.AddListener(onvaluechange)//参数为一个无返回值，参数为float类型的函数，参数自动被赋值为slider.value

//6.滚动视图
//7.通过图片image实现进度条的变化：使用两个image，父物体作背景，子物体作前景，子物体指定一个sprite，采用填充方式,改变fillamount的值(0到1)实现填充变换
Image image;
image.fillAmount；







//ScriptableObjects

//一些方法
Awake();
Ondisable();
On




//














//unity中常用特性https://blog.csdn.net/sh6518140/article/details/79800245		实际上实现了个性化的面板
//1.
Serializable//序列化一个类，实际用处是把数据存储到硬盘上，表面用处可以把私有的在检视面上显示出来
//2.
Noserializable//反序列化一个变量，实际用处是把数据从硬盘上行读取出来，表面用处是在检视面板上隐藏。定义了private 字                         段，可以在检视面板上显示出来
//3.
[AddComponentMenu(string menuName)];//创建组件里的菜单
[AddComponentMenu(string menuName, int order)];//创建的如果是一个文件路径似的，就有索引
//4.[CreateAssetMenu(string menuName, string filename, int order)]
//作用:对 ScriptableObject 派生类型进行标记，使其自动列在 Assets/Create 子菜单中，以便能够轻松创建该类型的实例并将其作为“.asset”文件存储在项目中。
//定义
using System;
namespace UnityEngine
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class CreateAssetMenuAttribute : Attribute
    {
        public CreateAssetMenuAttribute();
        
        public string menuName { get; set; }//此类型的新建实例使用的默认文件名。
        
        public string fileName { get; set; }//Assets/Create 菜单中显示的此类型的显示名称，可以以路径的形式比如:Inventory/sword;Inventory/armour.菜单会将两者放一个Inventory里面
        
        public int order { get; set; }//Assets/Create 菜单中菜单项的位置。
    }
}
//5.
[ContextMenu(string menuName)]//用于函数，其将显示在script的小栏里面，点击就调用对应的函数
//6.
[ContextMenuItem(string menuName, string functionName)]//用于字段，常常用来对public的字段调用functionName进行快速设置
//7.
[Header(string colomnName)];//用于创建一个头标题
//8.
[HideInInspector];//用于公有字段，让其不在检查器里面显示(好像没用)
//9.
[Multiline(int lineNumber)];//用于string,在检查器里面给它留个三行位置
//10.
[Range(int min, int max)];//用于数字型，在检查器里面给它一个拖动滚轮限制范围
//11.
[SerializeField];//强制序列化一个私有类型，这些类型包括
// - 继承 UnityEngine.Object 的所有类，例如 GameObject、Component、MonoBehaviour、Texture2D、AnimationClip。
// - All basic data types, such as int, string, float, bool.
// - Some built-in types, such as Vector2, Vector3, Vector4, Quaternion, Matrix4x4, Color, Rect, LayerMask.
// - 可序列化类型数组
// - Lists of a serializable type
// - 枚举
// - 结构
//12.
[TextArea];//给一个序列化的string一个空间，默认三行，少了就有滚轮，比Multiline好用点
[TextArea(int min, int max)];//上面的重载版，至少min行，至多max行
//13.
[Tooltip(string info)];//给一个序列化的字段一个信息说明，鼠标移动到上面就显示
















//yield return的常见用法
yield return null; // 下一帧再执行后续代码
yield return 0; //下一帧再执行后续代码
yield return 6;//(任意数字) 下一帧再执行后续代码
yield break; //直接结束该协程的后续操作
yield return asyncOperation;//等异步操作结束后再执行后续代码
yield return StartCoroutine(/*某个协程*/);//等待某个协程执行完毕后再执行后续代码
yield return WWW();//等待WWW操作完成后再执行后续代码
yield return new WaitForEndOfFrame();//等待帧结束,等待直到所有的摄像机和GUI被渲染完成后，在该帧显示在屏幕之前执行
yield return new WaitForSeconds(0.3f);//等待0.3秒，一段指定的时间延迟之后继续执行，在所有的Update函数完成调用的那一帧之后（这里的时间会受到Time.timeScale的影响）;
yield return new WaitForSecondsRealtime(0.3f);//等待0.3秒，一段指定的时间延迟之后继续执行，在所有的Update函数完成调用的那一帧之后（这里的时间不受到Time.timeScale的影响）;
yield return WaitForFixedUpdate();//等待下一次FixedUpdate开始时再执行后续代码
yield return new WaitUntil()//将协同执行直到 当输入的参数（或者委托）为true的时候....如:yield return new WaitUntil(() => frame >= 10);
yield return new WaitWhile()//将协同执行直到 当输入的参数（或者委托）为false的时候.... 如:yield return new WaitWhile(() => frame < 10);