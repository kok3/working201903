using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 目前开放给自己用 给玩家用的话都要重调 因此可以优先考虑快速实现


//左加宽 右加宽
//上加高  下加高  地图范围调整

// 摄像机控制  开启酷跑模式
public class UIPanelMapInfoParkour : MonoBehaviour
{
    public static UIPanelMapInfoParkour ins = null;
    [SerializeField]
    GameObject _panel;

    Camera main_camera;
    GameObject obj_MapEdge;
    BackGround background = null;

    public InputField input;
    void Awake()
    {
        ins = this;
    }
    void Start()
    {

#if !UNITY_EDITOR
        //in editor
        if (MapEditor.MapEditorConfig.CurrentMapGameMode == MapGameMode.Normal)
        {
            GameObject.DestroyImmediate(_panel);
        }
        else if (MapEditor.MapEditorConfig.CurrentMapGameMode == MapGameMode.Parkour)
        {
            main_camera = Camera.main;
            obj_MapEdge = GameObject.Find("MapEdge").gameObject;
            background = GameObject.FindObjectOfType<BackGround>();
        }
#else
        //in editor
        if (MapEditor.MapEditorConfig.CurrentMapGameMode == MapGameMode.Normal)
        {
            GameObject.DestroyImmediate(_panel);
        }
        else if (MapEditor.MapEditorConfig.CurrentMapGameMode == MapGameMode.Parkour)
        {
            main_camera = Camera.main;
            obj_MapEdge = GameObject.Find("MapEdge").gameObject;
            background = GameObject.FindObjectOfType<BackGround>();
        }
#endif

    }
    void OnDestroy()
    {
        if (ins == this)
        {
            ins = null;
        }
    }

#if UNITY_EDITOR
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F7))
        {
            this._panel.SetActive(!this._panel.activeSelf);
        }
        //摄像机控制
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            this.OnClickCameraLeft();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            this.OnClickCameraRight();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            this.OnClickCameraUp();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            this.OnClickCameraDown();
        }
    }
#endif
    //从data 里面恢复 edgebox位置
    public void Reset()
    {

        {
            var pos = obj_MapEdge.transform.Find("left").transform.position;
            pos.z = MapEditor.MapEditorStroageData.edgebox_left;
            obj_MapEdge.transform.Find("left").transform.position = pos;
        }
        {
            var pos = obj_MapEdge.transform.Find("right").transform.position;
            pos.z = MapEditor.MapEditorStroageData.edgebox_right;
            obj_MapEdge.transform.Find("right").transform.position = pos;
        }
        {
            var pos = obj_MapEdge.transform.Find("down").transform.position;
            pos.y = MapEditor.MapEditorStroageData.edgebox_down;
            obj_MapEdge.transform.Find("down").transform.position = pos;
        }
        {
            var pos = obj_MapEdge.transform.Find("up").transform.position;
            pos.y = MapEditor.MapEditorStroageData.edgebox_up;
            obj_MapEdge.transform.Find("up").transform.position = pos;
        }

    }
#if UNITY_EDITOR
    // 地图大小 控制
    public void OnClickWidthLeft()
    {
        var pos = obj_MapEdge.transform.Find("left").transform.position;
        if (Input.GetKey(KeyCode.Minus))
        {
            pos.z -= (2 * inputNumber);
        }
        else
        {
            pos.z += (2 * inputNumber);
        }
        MapEditor.MapEditorStroageData.edgebox_left = pos.z;
        obj_MapEdge.transform.Find("left").transform.position = pos;
    }
    public void OnClickWidthRight()
    {
        var pos = obj_MapEdge.transform.Find("right").transform.position;
        if (Input.GetKey(KeyCode.Minus))
        {
            pos.z += (2 * inputNumber);
        }
        else
            pos.z -= (2 * inputNumber);
        MapEditor.MapEditorStroageData.edgebox_right = pos.z;
        obj_MapEdge.transform.Find("right").transform.position = pos;
    }
    public void OnClickHeightUp()
    {
        var pos = obj_MapEdge.transform.Find("up").transform.position;
        if (Input.GetKey(KeyCode.Minus))
        {
            pos.y -= (2 * inputNumber);
        }
        else
        {
            pos.y += (2 * inputNumber);
        }
        MapEditor.MapEditorStroageData.edgebox_up = pos.y;
        obj_MapEdge.transform.Find("up").transform.position = pos;
    }
    public void OnClickHeightDown()
    {
        var pos = obj_MapEdge.transform.Find("down").transform.position;
        if (Input.GetKey(KeyCode.Minus))
        {
            pos.y += (2 * inputNumber);
        }
        else
            pos.y -= (2 * inputNumber);
        MapEditor.MapEditorStroageData.edgebox_down = pos.y;
        obj_MapEdge.transform.Find("down").transform.position = pos;
    }
    //摄像机位置控制
    public void OnClickCameraLeft()
    {
        var pos = this.main_camera.transform.position;
        pos.z += (5 * inputNumber);
        this.main_camera.transform.position = pos;
        var pos_background = background.transform.position;
        pos_background.z = pos.z;
        pos_background.y = pos.y;
        background.enabled = false;
        background.transform.position = pos_background;
    }
    public void OnClickCameraRight()
    {
        var pos = this.main_camera.transform.position;
        pos.z -= (5 * inputNumber);
        this.main_camera.transform.position = pos;
        var pos_background = background.transform.position;
        pos_background.z = pos.z;
        pos_background.y = pos.y;
        background.enabled = false;
        background.transform.position = pos_background;
    }
    public void OnClickCameraUp()
    {
        var pos = this.main_camera.transform.position;
        pos.y += (5 * inputNumber);
        this.main_camera.transform.position = pos;
        var pos_background = background.transform.position;
        pos_background.z = pos.z;
        pos_background.y = pos.y;
        background.enabled = false;
        background.transform.position = pos_background;
    }
    public void OnClickCameraDown()
    {
        var pos = this.main_camera.transform.position;
        pos.y -= (5 * inputNumber);
        this.main_camera.transform.position = pos;
        var pos_background = background.transform.position;
        pos_background.z = pos.z;
        pos_background.y = pos.y;
        background.enabled = false;
        background.transform.position = pos_background;
    }
    //复位
    public void OnClickCameraReset()
    {
        var pos = new Vector3(-200, 0, 0);
        this.main_camera.transform.position = pos;
        var pos_background = background.transform.position;
        pos_background.z = pos.z;
        pos_background.y = pos.y;
        background.enabled = false;
        background.transform.position = pos_background;
    }


    //  内部使用 写的变量名比较随意了
    public void OnEndEditNumber()
    {
        float.TryParse(input.text, out inputNumber);
    }

    float inputNumber = 1;
#else
    // 地图大小 控制
    public void OnClickWidthLeft()
    {
    }
    public void OnClickWidthRight()
    {
    }
    public void OnClickHeightUp()
    {
    }
    public void OnClickHeightDown()
    {
    }
    //摄像机位置控制
    public void OnClickCameraLeft()
    {
    }
    public void OnClickCameraRight()
    {
    }
    public void OnClickCameraUp()
    {
    }
    public void OnClickCameraDown()
    {
    }
    //复位
    public void OnClickCameraReset()
    {
    }

#endif


}
