using MapEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerMgr //: MonoBehaviour
{
    static LayerMgr _instance = null;

    public static LayerMgr ins
    {
        get
        {
            if (_instance == null)
            {
                _instance = new LayerMgr();
            }

            return _instance;
        }
    }

    public Transform transform { get; set; }
    public int curEditLayer = 1;
    //当前有的总层数
    public int curLayerCount = 0;
    public const int MAX_LAYER_COUNT = 60;
    public Transform[] layers = new Transform[MAX_LAYER_COUNT];

    public MapEditorStroageData.MapLayerData[] layerdatas = new MapEditorStroageData.MapLayerData[MAX_LAYER_COUNT];
    //public static LayerMgr ins = null;

    public void Clear()
    {
        for (int i=0; i< MAX_LAYER_COUNT; ++i)
        {
            layers[i] = null;
            layerdatas[i] = null;
        }
    }

    public Transform GetCurLayerTransform()
    {
        if ((curEditLayer > 0) && (curEditLayer <= MAX_LAYER_COUNT))
        {
            if (layers[curEditLayer - 1] != null)
                return layers[curEditLayer - 1];
        }

        return transform;
    }

    public GameObject GetLayerObjectByIndex(int layerIndex)
    {
        if (layerIndex > 0)
        {
            return layers[layerIndex - 1].gameObject;
        }

        return null;
    }

    public Transform GetLayerByIndex(int layerIndex)
    {
        if (layerIndex > 0)
        {
            return layers[layerIndex - 1];
        }

        return null;
    }

    /// <summary>
    /// 获取未用的层索引
    /// </summary>
    /// <returns></returns>
    public int GetFreeLayerIndex()
    {
        int count = 0;
        int layerIndex = curLayerCount + 1;
        
        while ((layers[layerIndex-1] != null) && (count <= 60))
        {
            layerIndex = layerIndex % MAX_LAYER_COUNT + 1;
        }

        if (layers[layerIndex - 1] == null)
            return layerIndex;
        else
            return 0;
    }
    /// <summary>
    /// 创建层
    /// </summary>
    /// <param name="layerIndex">1-->MAX_LAYER_COUNT</param>
    public void CreateLayer()
    {
        int layerIndex = GetFreeLayerIndex();
        float moveFactor = 1.0f;
        if (layerIndex == 2)
        {
            moveFactor = 0.8f;
        }
        else if (layerIndex == 3)
        {
            moveFactor = 0.5f;
        }
        else  if (layerIndex == 4)
        {
            moveFactor = 0.2f;
        }

        Debug.Log("@@@CreateLayer layerIndex: " + layerIndex + " moveFactor: " + moveFactor);

        MapEditorStroageData.MapLayerData data = new MapEditorStroageData.MapLayerData { layerIndex = layerIndex, moveFactor = moveFactor };

        CreateLayer(data);
    }

    public void CreateLayer(int layerIndex, float moveFactor)
    {
        MapEditorStroageData.MapLayerData data = new MapEditorStroageData.MapLayerData { layerIndex = layerIndex, moveFactor = moveFactor };

        CreateLayer(data);
    }

    public void CreateLayer(MapEditorStroageData.MapLayerData data)
    {
        Debug.Log("@@@@@@@@@@@@@Create Layer:  " + data.layerIndex);

        if (data.layerIndex > 0)
        {
            if (layers[data.layerIndex - 1] == null)
            {
                GameObject layerObj = new GameObject("layer" + data.layerIndex.ToString());
                layerObj.transform.parent = transform;
                layers[data.layerIndex - 1] = layerObj.transform;
                layerdatas[data.layerIndex - 1] = data;
                curLayerCount += 1;

                //新加的层属于当前正在编辑的层
                curEditLayer = data.layerIndex;
            }
            else
            {
                Debug.LogError("@@@CreateLayer Error data.layerIndex: " + data.layerIndex  + " data.moveFactor: " + data.moveFactor);
            }
        }
    }



    /// <summary>
    /// 删除层
    /// </summary>
    /// <param name="layerIndex">1-->MAX_LAYER_COUNT</param>
    public void DeleteLayer(int layerIndex)
    {
        if (layerIndex > 0)
        {
            GameObject.DestroyImmediate(layers[layerIndex - 1].gameObject);
            layers[layerIndex - 1] = null;

            layerdatas[layerIndex - 1] = null;
            
        }
    }

    /// <summary>
    /// 交换层，调整层的前后顺序
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    public void SwitchLayer(int leftIndex , int rightIndex)
    {
        Transform leftTs = layers[leftIndex - 1];
        Transform rightTs = layers[rightIndex - 1];

        //交换深度坐标
        float z = leftTs.position.z;
        Vector3 lpos = leftTs.position;
        lpos.z = rightTs.position.z;
        leftTs.position = lpos;

        Vector3 rpos = rightTs.position;
        rpos.z = z;
        rightTs.position = rpos;

        //交换层索引
        layers[leftIndex - 1] = rightTs;
        layers[rightIndex - 1] = leftTs;

        //修改名字
        layers[leftIndex - 1].gameObject.name = "layer" + leftIndex.ToString();
        layers[rightIndex - 1].gameObject.name = "layer" + rightIndex.ToString();

        /////////////////////////////////////////////////////////
        //交换数据（层索引不用交换，设计规则是数组下标决定层）
        float tmpMoveFactor = layerdatas[leftIndex - 1].moveFactor;
        layerdatas[leftIndex - 1].moveFactor = layerdatas[rightIndex - 1].moveFactor;
        layerdatas[rightIndex - 1].moveFactor = tmpMoveFactor;
        
    }

    /// <summary>
    /// 选择层
    /// </summary>
    /// <param name="layerIndex">1-->MAX_LAYER_COUNT</param>
    public void SelectLayer(int layerIndex)
    {
        curEditLayer = layerIndex;
    }


    /// <summary>
    /// 相机移动的时候相应的层也对应移动
    /// </summary>
    /// <param name="offsetZ">因为相机设置的关系这是水平方向</param>
    /// <param name="offsetY">垂直方向</param>
    public void OnCameraMove(Vector3 offsetPos)
    {
        for (int i=0; i < MAX_LAYER_COUNT; i++)
        {
            //moveFactor接近1不处理，减少运算
            if (layers[i] != null && layerdatas[i].moveFactor < 0.999f)
            {
                Vector3 pos = layers[i].position;
                //pos.z += offsetPos.z * (1 - layerdatas[i].moveFactor);
                //pos.y += offsetPos.y * (1 - layerdatas[i].moveFactor);
                if (offsetPos.x > 0.05)
                {
                    Debug.Log("offsetPos.x: " + offsetPos.x);
                }

                if (offsetPos.x > 0.05)
                {
                    Debug.Log("offsetPos.y: " + offsetPos.y);
                }

                if (offsetPos.z > 0.05)
                {
                    Debug.Log("offsetPos.z: " + offsetPos.z);
                }
                pos.x += offsetPos.x * (1.0f - layerdatas[i].moveFactor)* 0.3f;
                pos.y += offsetPos.y * (1.0f - layerdatas[i].moveFactor)*0.3f;
                pos.z += offsetPos.z * (1.0f - layerdatas[i].moveFactor)*0.3f;
                layers[i].position = pos;
            }
        }
    }

    //void Awake()
    //{
    //    Debug.LogError("2222222222222222222222 LayerMgr.Awake");

    //    ins = this;
    //}
    //void OnDestroy()
    //{
    //    ins = null;
    //}

}
