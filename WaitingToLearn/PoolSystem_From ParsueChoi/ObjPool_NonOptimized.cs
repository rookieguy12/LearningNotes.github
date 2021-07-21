using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjPool_NonOptimized : MonoBehaviour
{

    static ObjPool_NonOptimized instance;
    public static ObjPool_NonOptimized Instance
    {
        get
        {
            return instance;
        }
    }

    //Pool的設定
    [Header("Setting")]
    public ObjPoolSetting_NonOptimized[] objPool;

    //每個Pool中Object的數量
    //Key：string=>Pool name ; Value：ObjPoolCounter=>指定ObjectPool裡的Object Pool Counter
    static Dictionary<string, ObjPoolCounter_NonOptimized> objPoolCounter = new Dictionary<string, ObjPoolCounter_NonOptimized>();
    
    //每個Pool中每件Object是否已經借出
    //Key：string=>Pool name ; Value：Dictionary<GameObject, bool>
    //Son Key : GameObject => object Prefab ; Value : bool => borrow status
    static Dictionary<string, Dictionary<GameObject, bool>> poolObjStatus = new Dictionary<string, Dictionary<GameObject, bool>>();

    //每件object在哪個pool
    //Key : GameObject=> object in hierarchy ; Value : string=> Pool Name
    static Dictionary<GameObject, string> poolObjList = new Dictionary<GameObject, string>();

    //每個Pool的所在Transform(objects 的 parent)
    //Key : string=> pool name ; Value : Transform=> parent Transform Component
    static Dictionary<string, Transform> poolParentList = new Dictionary<string, Transform>();


    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        CreatePoolObject();
    }

    void CreatePoolObject()
    {
        GameObject newObj = null;
        if(objPool.Length == 0) { return; } //沒有池子就return

        //遍歷所有池子
        foreach (ObjPoolSetting_NonOptimized ops in objPool)
        {
            GameObject pool = new GameObject(ops.name); //根據池子名字新建一個池子的父物體
            pool.transform.SetParent(transform);

            //為該池子添加一個物件計算器並記錄到字典裡
            //默認totalObj為池子大小、已借出obj為0、未借出obj為池子大小
            objPoolCounter.Add(ops.name, new ObjPoolCounter_NonOptimized(ops.Quantity, 0, ops.Quantity));

            //將該池子的所有物體借出情況記錄起來
            poolObjStatus.Add(ops.name, new Dictionary<GameObject, bool>());

            //記錄池子的物體，作為池子物件在後面產生時的父物體使用
            poolParentList.Add(ops.name, pool.transform);

            //根據池子的容量生成物件
            for (int i = 1; i <= ops.Quantity; i++)
            {
                newObj = Instantiate(ops.prefab, pool.transform);
                newObj.SetActive(ops.enableInPool); //根據池子的狀態決定物件是否顯示
                newObj.transform.position = transform.position;
                newObj.name = string.Concat(ops.name, " (", i, ")");
                poolObjStatus[ops.name].Add(newObj, false); //將物件作為key，借出狀態作為value加到poolObjStatus的指定池子索引中的字典裡
                poolObjList.Add(newObj, ops.name); //將物件和它所在的pool名記錄起來
            }
        }
    }

    //從池子裡提出物體
    public static Transform TakeFromPool(string pool)
    {
        //如果已經沒有剩餘的未借出物體直接Return null
        if(objPoolCounter[pool].inObj == 0)
        {
            return null;
        }

        Transform t = null;
        //根據池子名拿到池子裡所有物件的借出狀態字典
        Dictionary<GameObject, bool> objList = poolObjStatus[pool];
        //根據Key（GameObject）來遍歷所有物件的Value（bool）
        foreach (GameObject g in objList.Keys)
        {
            //如果有物件是未借出的，Value = false
            if (!objList[g])
            {
                //將其狀態改為借出 => Value = true
                objList[g] = true;
                //顯示物件
                g.SetActive(true);
                t = g.transform;
                break;
            }
        }

        //從池子裡取出一個物件
        objPoolCounter[pool].Take();

        return t;
    }

    //將物件還給池子
    public static void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        //根據物體找到他所屬於的池子名
        string p = poolObjList[obj];
        //將物體所屬池子中的該物體的狀態設為false=未借出
        poolObjStatus[p][obj] = false;
        //還原位置與父物體
        obj.transform.SetParent(instance.transform);
        obj.transform.SetPositionAndRotation(instance.transform.position, instance.transform.rotation);
        //將物體放回池子
        objPoolCounter[p].Return();
    }

}


#region Base Script
[Serializable]
//每個池子的相關設定
public struct ObjPoolSetting_NonOptimized
{
    public string name;
    public GameObject prefab;
    public ushort Quantity;
    public bool enableInPool;
}

//池子裡的物件計算器
public struct ObjPoolCounter_NonOptimized
{
    public ushort totalObj;
    public ushort outObj;
    public ushort inObj;

    public ObjPoolCounter_NonOptimized(ushort totalObj, ushort outObj, ushort inObj)
    {
        this.totalObj = totalObj;
        this.outObj = outObj;
        this.inObj = inObj;
    }

    public void Take()
    {
        outObj++;
        inObj--;
    }

    public void Return()
    {
        outObj--;
        inObj++;
    }
}


#endregion
