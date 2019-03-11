/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using LitJson;
using System;
using System.Reflection;

public class Serializable
{
    public static string ToJson<T>(T t)
    {
        return JsonMapper.ToJson(t);
    }
    public static T ToObject<T>(string json)
    {
        return JsonMapper.ToObject<T>(json);
    }
    public class SerializeBase
    {
        public const bool EnableLog = false;
        //fill default value when de-serialize(emplace)
        public virtual void FillMemberDefault()
        {

        }
        //fill serialize object data with real object
        //用uniy 对象 初始化（填充） 序列化对象
        public virtual void Fill<T>(T t)
        {
        }
        // fill real object data with  seriablize object data
        //用序列化对象 填充（初始化） unity 对象
        public virtual void Emplace<T>(T t)
        {
        }
        //t1 return type(serializable.XXX),,,, t2 prototype (unity)
        public static T1 Create<T1, T2>(T2 t2) where T1 : SerializeBase, new()
        {
            T1 ret = new T1();
            ret.Fill<T2>(t2);
            return ret;
        }
        // when de-serialize use this function to check is valid object ,verify data and object via this function
        //override in sub-class
        public virtual bool IsValid()
        {
            return true;
        }
    }

    public static bool SaveToFile(string file, string json)
    {
        try
        {
            if (string.IsNullOrEmpty(file))
            {
                return false;
            }

            file = LocalStorageMap.ins.GetRootDirectory() + "/" + file;

            using (FileStream f = new FileStream(file, FileMode.Create))
            {
                var ss = new StreamWriter(f);
                ss.Write(json);
                ss.Flush();
                f.Flush();
                ss.Close();
                f.Close();
            }
            return true;
        }
        catch (Exception e)
        {
            Debug.Log("save file error  " + e.Message);
            return false;
        }
    }
    //创建地图会有存在临时保存的 就存储在临时保存目录下
    public static bool SaveToLocalTmpFile(string file, string json)
    {
        try
        {
            if (string.IsNullOrEmpty(file))
            {
                return false;
            }
            using (FileStream f = new FileStream(file, FileMode.Create))
            {
                var ss = new StreamWriter(f);
                ss.Write(json);
                ss.Flush();
                f.Flush();
                ss.Close();
                f.Close();
            }
            return true;
        }
        catch (Exception e)
        {
            Debug.Log("save tmp file error  " + e.Message);
            return false;
        }
    }
    public static bool SaveToFileEx(string uuid, string json)
    {
        return SaveToFile(uuid, json);
    }
    public static string GetDownloadWWWUrl(string uuid)
    {
        return DevConfig.MapDownloadUrl + "map_" + uuid + ".json";
    }
    /*  public static string GetDownloadWWWFileUrl(string uuid)
      {
          return "file://" + uuid + ".json";
      }*/
    public static string LoadFromFile(string file)
    {
        try
        {
            if (string.IsNullOrEmpty(file))
            {
                //    file = "test.json";

                Debug.Log("MapEditor Runtime:   Load From file:" + file);
                return string.Empty;
            }
            file = LocalStorageMap.ins.GetRootDirectory() + "/" + file;


            using (FileStream f = new FileStream(file, FileMode.Open))
            {
                var ss = new StreamReader(f);
                var ret = ss.ReadToEnd();
                ss.Close();
                f.Close();
                return ret;
            }
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.Log("load file error " + file + "  " + e.Message);
#endif
            return string.Empty;
        }
    }

    public static string LoadFromFileEx(string uuid)
    {
        return LoadFromFile(uuid);
    }

    [System.Serializable]
    public class Vector3 : SerializeBase
    {
        public float x;
        public float y;
        public float z;
        public UnityEngine.Vector3 ToString()
        {
            return new UnityEngine.Vector3(x, y, z);
        }


        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public Vector3()
        {

        }

        //---------------for type cast

        public static implicit operator UnityEngine.Vector3(Serializable.Vector3 t)
        {
            return new UnityEngine.Vector3(t.x, t.y, t.z);
        }
        public static implicit operator Serializable.Vector3(UnityEngine.Vector3 t)
        {
            return new Serializable.Vector3(t.x, t.y, t.z);
        }

    }
    //--- player spawn points
    public class SpawnPoints : SerializeBase
    {
        const int SPAWN_POINT_COUNT = 4;
        public List<Serializable.Vector3> points = new List<Vector3>();
        public override bool IsValid()
        {
            return points.Count == SPAWN_POINT_COUNT;
        }
        public override void Fill<T>(T t)
        {
            if (t is List<UnityEngine.Vector3>)
            {
                points.Clear();
                var list = t as List<UnityEngine.Vector3>;
                foreach (var p in list)
                {
                    this.points.Add(p);
                    if (EnableLog)
                    {
                        Debug.Log("fill spawn point=" + p);
                    }
                }
            }
            else
            {

                Debug.LogWarning("type error cast " + t.GetType().ToString() + "  to " + this.GetType().ToString());
            }
        }
        public override void Emplace<T>(T t)
        {
            if (t is List<UnityEngine.Transform>)
            {
                var list = t as List<UnityEngine.Transform>;
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].position = points[i];
                    if (EnableLog)
                    {
                        Debug.Log("emplace spawn name=" + list[i].gameObject.name + points[i].ToString());
                    }
                }
            }
            else
            {

                Debug.LogWarning("type error cast " + t.GetType().ToString() + "  to " + this.GetType().ToString());
            }
        }
    }



    //酷跑模式中的  复活点
    public class RevivePoints : SerializeBase
    {
        public List<Serializable.Vector3> points = new List<Vector3>();
        public override bool IsValid()
        {
            //至少有一个复活点
            return points.Count > 0;
        }
        public override void Fill<T>(T t)
        {
            if (t is List<UnityEngine.Vector3>)
            {
                points.Clear();
                var list = t as List<UnityEngine.Vector3>;
                foreach (var p in list)
                {
                    this.points.Add(p);
                    if (EnableLog)
                    {
                        Debug.Log("fill revive point=" + p);
                    }
                }
            }
            else
            {
                Debug.LogWarning("type error cast " + t.GetType().ToString() + "  to " + this.GetType().ToString());
            }
        }
        public override void Emplace<T>(T t)
        {
            if (t is List<UnityEngine.Transform>)
            {
                var list = t as List<UnityEngine.Transform>;
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].position = points[i];
                    if (EnableLog)
                    {
                        Debug.Log("emplace spawn name=" + list[i].gameObject.name + points[i].ToString());
                    }
                }
            }
            else
            {
                Debug.LogWarning("type error cast " + t.GetType().ToString() + "  to " + this.GetType().ToString());
            }
        }
    }



    //weapon spawn points
    public class WeaponSpawnPoints : SerializeBase
    {
        public class Array
        {
            public Array()
            {
                ids = new List<int>();
            }
            public Array(List<int> ids)
            {
                this.ids = new List<int>(ids);
            }
            public List<int> ids;
        }
    //    const int SPAWN_POINT_COUNT = 4;
        public List<Serializable.Vector3> points = new List<Serializable.Vector3>();
        public List<Array> ids = new List<Array>();
        public WeaponSpawnPoints()
        {
            points = new List<Serializable.Vector3>();
            ids = new List<Array>();
           // for (int i = 0; i < SPAWN_POINT_COUNT; i++)
            {
             //   this.ids.Add(new Array());
            }
        }
        public override bool IsValid()
        {
            return points.Count > 0;
        }
        public override void Fill<T>(T t)
        {
            if (t is List<EditorWeaponSpawnPointTag>)
            {
                points.Clear();
                ids.Clear();
                var list = t as List<EditorWeaponSpawnPointTag>;
                foreach (var p in list)
                {
                    this.points.Add(p.transform.position);
                    this.ids.Add(new Array(p.weapon_ids));
                    if (EnableLog)
                    {
                        Debug.Log("fill weapon spawn point=" + p);
                    }
                }
            }
            else
            {
                Debug.LogWarning("type error cast " + t.GetType().ToString() + "  to " + this.GetType().ToString());
            }
        }
        public override void Emplace<T>(T t)
        {
            if (t is List<EditorWeaponSpawnPointTag>)
            {
                var list = t as List<EditorWeaponSpawnPointTag>;
                for (int i = 0; i < list.Count; i++)
                {
                    var l = list[i];
                    if (points.Count > i)
                    {
                        l.transform.position = points[i];
                    }
                    if (ids.Count > i)
                    {
                        l.weapon_ids = (ids[i].ids);
                    }
                }

                /* for (int i = 0; i < list.; i++)
                 {
                     list[i].transform.position = points[i];
                     list[i].weapon_ids = new List<int>(ids[i].ids);
                     if (EnableLog)
                     {
                         Debug.Log("emplace spawn name=" + list[i].gameObject.name);
                     }
                 }*/
            }
            else
            {
                Debug.LogWarning("type error cast " + t.GetType().ToString() + "  to " + this.GetType().ToString());
            }
        }
    }
    // 序列化地图 顶级序列化对象
    //这是一个全集
    public class Map : SerializeBase
    {
        public enum Theme
        {

        }
        public bool TAG_FOR_MAP_JSON = true;
        //  public string runtime_version = "1.0";
        // public string editor_version = "1.0";
        //public string json_version = "1.0";
        public string version = "1.0";

        public ulong uuid = 0;// 地图唯一id
        public ulong creator = 0;//地图创建者唯一id
        public int theme = 1;// 地图主题  主题id 对应 预设资源目录id，背景图是属于 全局资源  全局唯一
        public int background = 1;// 背景图id 全局唯一  主题可动态绑定， 一个主题可多个背景供选择

        //层用来控制层级关系和整体移动 ID分配： 1-20：背景层 21-40：物件层 41-60：前景层
        public List<MapLayer> layers = new List<MapLayer>();
        public List<MapBlock> blocks = new List<MapBlock>();// 地图块  里面包含的是 地图基本信息， 每个地图快 不可重叠
        public List<MapObject> objects = new List<MapObject>();//地图 物体 不可重叠

        public SpawnPoints spawn = new SpawnPoints();// 玩家出生点 必须是4个
        public WeaponSpawnPoints weapon_spawn_points = new WeaponSpawnPoints();//武器刷新点 1-4个

        public string name = "";
        public string brief = "";


        //-------------extension of game-mode

        //游戏模式默认为 经典模式
        public int MapInfoMode = (int)MapGameMode.Normal;//unknow
        //跑酷模式扩展数据
        public MapParkour map_parkour;


        //-----------------end of extension of game-mode


        //---------------- helpers
        public string ToJson()
        {
            var str = JsonMapper.ToJson(this);
            return str;
        }
        public static Map FromJson(string json)
        {
            var ret = JsonMapper.ToObject<Map>(json);
            return ret;
        }
        public static bool IsMapJson(string json)
        {
            if (string.IsNullOrEmpty(json)) return false;
            return json.IndexOf("TAG_FOR_MAP_JSON") > 0;
        }
    }
    public class MapParkour : SerializeBase
    {
        //复活点是物件
   //     public List<RevivePoints> parkour_revive_points = new List<RevivePoints>(); // 跑酷模式中的复活点信息 
      /*  public int MapWidth = 1136;//地图宽度  2D&3D
        public int MapHeight = 640;//地图高度  2D&3D
        public int MapDepth = 100; // 地图深度  3D
        */
        public float edgebox_left;
        public float edgebox_right;
        public float edgebox_down;
        public float edgebox_up;

    }

    public class Transform : SerializeBase
    {
        public Serializable.Vector3 position;
        public Serializable.Vector3 rotation;
        public Serializable.Vector3 scale;
        public override void FillMemberDefault()
        {
            if (position == null)
            {
                position = new Vector3();
            }
            if (scale == null)
            {
                scale = new Vector3();
            }
            if (rotation == null)
            {
                rotation = new Vector3();
            }
        }

        public override void Fill<T>(T t)
        {
            UnityEngine.Transform obj = t as UnityEngine.Transform;
            if (obj != null)
            {
                this.position = obj.position;
                this.rotation = obj.rotation.eulerAngles;
                this.scale = obj.localScale;
            }
            else
            {
                Debug.LogWarning("type error cast " + t.GetType().ToString() + "  to " + this.GetType().ToString());
            }
        }
        public override void Emplace<T>(T t)
        {
            UnityEngine.Transform obj = t as UnityEngine.Transform;
            if (obj != null)
            {
                obj.position = this.position;
                obj.rotation = Quaternion.Euler(this.rotation);
                obj.localScale = this.scale;
            }
            else
            {
                Debug.LogWarning("type error cast " + t.GetType().ToString() + "  to " + this.GetType().ToString());
            }
        }
        /* public static Transform Create(UnityEngine.Transform prototype)
         {
             Transform ret = new Transform();
             ret.Fill<UnityEngine.Transform>(prototype);
             return ret;
         }*/
    }
    public class MapObject : SerializeBase
    {
        public string prefab = "";
        public Transform transform;
        public int layerIndex = 0;

        public int extPropClassIndex = 0;
        public string extPropClassName = "";
        public string extPropJson = "";
        
        public override void FillMemberDefault()
        {
            if (transform == null)
            {
                transform = new Transform();
            }
        }
        public override void Fill<T>(T t)
        {
            if (t is UnityEngine.GameObject)
            {
                var obj = t as UnityEngine.GameObject;
                this.transform = SerializeBase.Create<Serializable.Transform, UnityEngine.Transform>(obj.transform);
                prefab = obj.name;

                //从父节点名字获取所属层
                int.TryParse(obj.transform.parent.gameObject.name.Substring(5), out layerIndex);

                if (EnableLog)
                {
                    Debug.Log("fill map ojbect name=" + obj.name);
                }

                CustomerPropertyBase com = obj.GetComponent<CustomerPropertyBase>();
                //自定义属性
                if (com != null)
                {
                    Type type = com.GetType();

                    FieldInfo field = type.GetField("serialization");
                    if (field != null)
                    {
                        extPropJson = JsonMapper.ToJson(field.GetValue(com));
                        extPropClassName = field.GetValue(com).GetType().Name;
                        extPropClassIndex = MapEditor.MapEditorUtils.GetClassIndex(extPropClassName);
                    }                  
                }

                return;
            }
            else
            {
                Debug.LogWarning("type error cast " + t.GetType().ToString() + "  to " + this.GetType().ToString());
            }
        }

        public override void Emplace<T>(T t)
        {
            UnityEngine.GameObject obj = t as UnityEngine.GameObject;
            if (obj != null)
            {
                transform.Emplace<UnityEngine.Transform>(obj.transform);
                obj.name = prefab;
                if (EnableLog)
                {
                    Debug.Log("emplace object name=" + obj.name);
                }

                CustomerPropertyBase com = obj.GetComponent<CustomerPropertyBase>();
                //自定义属性
                if ((com != null) && (extPropJson != ""))
                {
                    com.OnDeseriazlie(extPropJson);
                    //Type type = com.GetType();

                    //FieldInfo field = type.GetField("serialization");
                    //if (field != null)
                    //{
                    //    extPropJson = JsonMapper.ToJson(field.GetValue(com));
                    //    extPropClassName = field.GetValue(com).GetType().Name;
                    //    extPropClassIndex = MapEditor.MapEditorUtils.GetClassIndex(extPropClassName);
                    //}
                }

            }
            else
            {
                Debug.LogWarning("type error cast " + t.GetType().ToString() + "  to " + this.GetType().ToString());
            }
        }


    }

    //层（实体），用来控制渲染层级和整体移动
    public class MapLayer : SerializeBase
    {
        //层索引
        public int layerIndex = 1;
        //移动系数
        public float moveFactor = 1.0f;
        public override void FillMemberDefault()
        {

        }
        public override void Fill<T>(T t)
        {
            if (t is UnityEngine.GameObject)
            {
                var obj = t as UnityEngine.GameObject;
                int.TryParse(obj.name.Substring(5), out layerIndex);
                moveFactor = LayerMgr.ins.layerdatas[layerIndex - 1].moveFactor;
                if (EnableLog)
                {
                    Debug.Log("fill map layer=" + obj.name);
                }
                return;
            }
            else
            {
                Debug.LogWarning("type error cast " + t.GetType().ToString() + "  to " + this.GetType().ToString());
            }
        }

        public override void Emplace<T>(T t)
        {
            UnityEngine.GameObject obj = t as UnityEngine.GameObject;
            if (obj != null)
            {
                obj.name = layerIndex.ToString();
                if (EnableLog)
                {
                    Debug.Log("emplace object name=" + obj.name);
                }
            }
            else
            {
                Debug.LogWarning("type error cast " + t.GetType().ToString() + "  to " + this.GetType().ToString());
            }
        }


    }
}
public static class SerializableExtension
{
    public static Serializable.Vector3 ToSerialize(this Vector3 v)
    {
        return new Serializable.Vector3(v.x, v.y, v.z);
    }
}

public class MapBlock
{
    public Serializable.Vector3 Position = new Serializable.Vector3();
    //  public Serializable.Vector3 rotation = new Serializable.Vector3();
    public int TypeId = 0;//
    public int PrefabId = 0;//

}


public static class MapEditHelper
{

}


public class Person
{
    public string name = "111";
    public int Age = 5;
    public float AAA = 5.5f;
    public double AAAAA = 6.5;

}


