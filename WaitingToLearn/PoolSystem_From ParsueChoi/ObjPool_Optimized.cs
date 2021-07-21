using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjPool_Optimized : MonoBehaviour
{
    static ObjPool_Optimized instance;
    public static ObjPool_Optimized Instance { get { return instance; } }

    [Header("Setting")]
    public ObjPoolSetting_Optimized[] objPool;

    static Dictionary<string, ObjPoolInfo_Optimized> poolInfo = new Dictionary<string, ObjPoolInfo_Optimized>();
    static Dictionary<GameObject, string> poolObjList = new Dictionary<GameObject, string>();


    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        CreatePoolObject();

        GameManager.poolReady = true;
    }

    void CreatePoolObject()
    {
        foreach (ObjPoolSetting_Optimized ops in objPool)
        {
            GameObject pool = new GameObject(ops.name);
            pool.transform.SetParent(transform);

            poolInfo.Add(ops.name, new ObjPoolInfo_Optimized(pool.transform, ops.prefab, ops.enableInPool));

            for (int i = 0; i < ops.Quantity; i++)
            {
                GameObject newObj = poolInfo[ops.name].AddNewObj();
                poolObjList.Add(newObj, ops.name);
            }
        }
    }

    void OnApplicationQuit()
    {
        foreach (ObjPoolSetting_Optimized ops in objPool)
        {
            string pool = ops.name;
            int maxUse = poolInfo[pool].maxOut; //歷史最大用量
            string recAmt = 
                poolInfo[pool].addMoreCounter > 0 //曾額外增加過物件，擴大池子 
                || ops.Quantity - maxUse > 15 //餘額用量大於15個
                ? (maxUse + 15).ToString() : "-";
            Debug.Log(string.Concat("Pool [ ", pool, " ] max out value: ", maxUse, " (", recAmt, ")\n"));
        }
    }

    public static Transform TakeFromPool(string pool)
    {
        Transform t = poolInfo[pool].Take();

        //提取後檢查池子裡的剩餘物件量
        if(poolInfo[pool].inObj < 10)
        {
            //如果少於10個就增加
            instance.AddMore(pool);
        }

        return t;
    }


    //用於一幀增加一個物件，而不是一次過增加
    void AddMore(string pool)
    {
        if(poolInfo[pool].corou == null)
        {
            poolInfo[pool].corou = StartCoroutine(AddMoreProcess(pool));
        }
    }
    IEnumerator AddMoreProcess(string pool)
    {
        poolInfo[pool].addMoreCounter++;

        //自動增加量為當前池子物件總量的20%
        int addAmt = (int)(poolInfo[pool].totalObj * 0.2f);
        if(addAmt < 10)
        {
            //如果20%少於10個就直接增加10個
            addAmt = 10;
        }

        for (int i = 0; i < addAmt; i++)
        {
            GameObject newObj = poolInfo[pool].AddNewObj();
            poolObjList.Add(newObj, pool);
            yield return null;
        }

        poolInfo[pool].corou = null;
    }

    static void ReturnToPool(GameObject obj)
    {
        poolInfo[poolObjList[obj]].Return(obj);
    }

}



[System.Serializable]
public struct ObjPoolSetting_Optimized
{
    public string name;
    public GameObject prefab;
    [Range(20, 1000)]
    public ushort Quantity;
    public bool enableInPool;
}

public class ObjPoolInfo_Optimized
{
    Transform pool;
    GameObject prefab;
    readonly bool enableInPool;

    public ushort totalObj;
    public ushort outObj;
    public ushort inObj;
    public ushort maxOut; //最大借出量
    public ushort addMoreCounter; //記錄增加的量，用於在下次打開遊戲時進行增加初次生成量

    Dictionary<GameObject, bool> objList;

    //用於一幀增加一個物件，而不是一次過增加
    public Coroutine corou;

    public ObjPoolInfo_Optimized(Transform pool, GameObject prefab, bool enableInPool)
    {
        this.pool = pool;
        this.prefab = prefab;
        this.enableInPool = enableInPool;

        totalObj = outObj = inObj = maxOut = addMoreCounter = 0;
        objList = new Dictionary<GameObject, bool>();
    }

    public GameObject AddNewObj()
    {
        GameObject newObj = GameObject.Instantiate(prefab, pool.transform);
        newObj.SetActive(enableInPool);
        newObj.transform.position = pool.transform.position;
        newObj.name = string.Concat(prefab.name, " (", totalObj + 1, ")");
        objList.Add(newObj, false);
        totalObj++;
        inObj++;
        return newObj;
    }

    public Transform Take()
    {
        if(objList == null || objList.Count == 0 || inObj == 0)
        {
            return null;
        }

        Transform t = null;

        foreach (GameObject obj in objList.Keys)
        {
            if (!objList[obj])
            {
                outObj++;
                inObj--;
                objList[obj] = true;
                obj.SetActive(true);
                t = obj.transform;
                break;
            }
        }

        //每次借出物體時，檢查借出量是否大於歷史最大借出量，是則更新最大借出量
        if(outObj > maxOut)
        {
            maxOut = outObj;
        }

        return t;
    }

    public void Return(GameObject obj)
    {
        if (!objList.ContainsKey(obj))
        {
            return;
        }

        outObj--;
        inObj++;
        obj.SetActive(enableInPool);
        obj.transform.SetParent(pool);
        obj.transform.SetPositionAndRotation(pool.position, pool.rotation);
        objList[obj] = false;
    }
}
