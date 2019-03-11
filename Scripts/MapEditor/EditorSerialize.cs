/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MapEditor
{//编辑器模式下  顶级 控制脚本
    public class EditorSerialize : MonoBehaviour
    {
        public string ToJson(MapObjectRoot root)
        {
            Serializable.Map map = new Serializable.Map();
            map.theme = MapEditorConfig.CurrentSelectTheme;
            map.creator = (ulong)StaticData.luuid;
            map.uuid = 0; //(ulong)id;
            var serializes = this.transform.GetComponentsInChildren<EditorSerializeBase>();
            foreach (var p in serializes)
            {
                p.SerializeObject(map);
            }
            //填充额外信息
            //因为只能编辑一个地图 因此 除了新建地图 name brief 只读写 不清空

            //填充额外信息

            map.name = MapEditorStroageData.current_map_name;    //root.map_name;
            map.brief = MapEditorStroageData.current_map_brief;  // root.map_brief;

            //地图的模式
            map.MapInfoMode = (int)MapEditorConfig.CurrentMapGameMode;

            if (MapEditorConfig.CurrentMapGameMode == MapGameMode.Parkour)
            {//酷跑模式 
                //     map.map_parkour = new Serializable.MapParkour();
                //关于地图编辑器的模式 先case by case 待需求明确后 重构一下

                map.map_parkour = new Serializable.MapParkour();

                map.map_parkour.edgebox_left = MapEditorStroageData.edgebox_left;
                map.map_parkour.edgebox_right = MapEditorStroageData.edgebox_right;
                map.map_parkour.edgebox_down = MapEditorStroageData.edgebox_down;
                map.map_parkour.edgebox_up = MapEditorStroageData.edgebox_up;
            }
            var str = map.ToJson();

            return str;
        }
        void Start()
        {
            /*  Serializable.Map map = new Serializable.Map();
              var serializes = this.transform.GetComponentsInChildren<EditorSerializeBase>();
              foreach (var p in serializes)
              {
                  p.SerializeObject(map);
              }
              var str = map.ToJson();

              Debug.LogError(str);
              Serializable.SaveToFile("", str);


              WWWForm form = new WWWForm();
              form.AddField("json_data", str);
              form.AddField("file_name", "1");
              StartCoroutine(upload(form));
              */
            /*
                        MapHttpTask.UploadMap(str, (bool ok) =>
                        {
                            if (ok)
                            {  

                            }

                        }, 1, gameObject);
                        */
        }
        string url = "127.0.0.1:7013/upload.php";

        IEnumerator upload(WWWForm form)
        {
            WWW www = new WWW(url, form);

            yield return www;
            //   if(www.isDone)
            {
                if (www.text == "ok")
                {

                }
                else if (www.text == "error")
                {

                }
                else
                {
                    // upload php error
                    Debug.LogError(www.text);
                }
                Debug.LogError(www.text);
            }

        }
    }

}