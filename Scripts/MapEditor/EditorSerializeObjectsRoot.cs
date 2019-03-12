/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MapEditor
{
    public class EditorSerializeObjectsRoot : EditorSerializeBase
    {
        public void Awake()
        {
            LayerMgr.ins.transform = transform;
        }
        //scene-->map
        public override bool SerializeObject(Serializable.Map map)
        {
            //
                      
            int count = transform.childCount;
            if (count <= 0) return false;

            //是否支持多层
            bool supportMultiLayer = false;
            string name = transform.GetChild(0).gameObject.name;
            if (name.Contains("layer"))
            {
                supportMultiLayer = true;
            }

            if (!supportMultiLayer)
            {
                for (int i = 0; i < count; i++)
                {
                    Serializable.MapObject obj = new Serializable.MapObject();
                    if (obj == null) return false;
                    var child = transform.GetChild(i);
                    if (child.GetComponent<DontSerializeThisToJson>() != null) continue;

                    //如果是隐藏的就不序列化
                    if (!child.gameObject.activeSelf) continue;

                    obj.Fill<GameObject>(child.gameObject);

                    map.objects.Add(obj);
                }
            }
            else//多层地图
            {
                for (int i = 0; i < count; i++) //遍历层
                {
                    Serializable.MapLayer layer = new Serializable.MapLayer();
                    if (layer == null) return false;
                    
                    
                    var child = transform.GetChild(i);
                    layer.Fill<GameObject>(child.gameObject);

                    //如果是隐藏的就不序列化
                    if (!child.gameObject.activeSelf) continue;

                    int childchildcount = child.childCount;
                    int layerIndex = 0;
                    int.TryParse(child.gameObject.name.Substring(5), out layerIndex);

                    map.layers.Add(layer);

                    for (int j = 0; j < childchildcount; j++) //遍历层里的物件
                    {
                        
                        Serializable.MapObject obj = new Serializable.MapObject();
                        if (obj == null) return false;
                        var childchild = child.GetChild(j);
                        if (childchild.GetComponent<DontSerializeThisToJson>() != null) continue;

                        //如果是隐藏的就不序列化
                        if (!childchild.gameObject.activeSelf) continue;

                        obj.Fill<GameObject>(childchild.gameObject);

                        map.objects.Add(obj);
                    }

                }
            }

            if (count != map.objects.Count) return false;
            return base.SerializeObject(map);//default ok
        }
        //map-->scene
        public override bool DeSerializeObject(Serializable.Map map)
        {
            Debug.LogError("2222222222222222222222222222222222 DeSerializeObject set transform");
            LayerMgr.ins.transform = transform;
            LayerMgr.ins.Clear();
            //Transform[] layers = new Transform[LayerMgr.MAX_LAYER_COUNT];
            foreach (var l in map.layers)
            {
                LayerMgr.ins.CreateLayer(l.layerIndex, l.moveFactor);
            }

            foreach (var p in map.objects)
            {
                GameObject obj =//PrefabsMgr.LoadMapObjectWithoutTheme(map.theme.ToString() +"/"+p.prefab);

                MapLoader.ins.LoadMapObjectV1(map.theme, p.prefab);
                if (obj == null)
                {
                    Debug.LogError("Can not find map object=" + map.theme + "  " + p.prefab);
                }
                else
                {
                    if ((p.layerIndex > 0) && (p.layerIndex <= LayerMgr.MAX_LAYER_COUNT))
                    {
                        obj = GameObject.Instantiate<GameObject>(obj, LayerMgr.ins.GetLayerByIndex(p.layerIndex));
                        p.Emplace<GameObject>(obj);
                    }
                    else
                    {
                        obj = GameObject.Instantiate<GameObject>(obj, transform);
                        p.Emplace<GameObject>(obj);
                    }

                }
            }
            return base.DeSerializeObject(map);//default ok
        }
    }

}