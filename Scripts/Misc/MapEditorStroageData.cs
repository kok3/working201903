/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MapEditor
{
    public class MapEditorStroageData
    {
        //存储 数据结构，  用于map editor 重建  达到在预览的时候 能Unload 掉MapEditor 
        //和本地存储 下次打开 恢复之前的状态
        //TODO 考虑是否存储UI状态 ，目前只有数据状态
        public string map_name = "";//地图名字   
        public string map_brief = "";//地图简介  


        public class MapLayerData
        {
            public int layerIndex;
            public float moveFactor;
        }

        //地图 物件 包含 装饰品
        public class MapObjectData
        {
            public string prefab;
            public Vector3 position;
            public int layerIndex;

            public int extPropClassIndex;
            public string extPropClassName;
            public string extPropJson;
        }

        public List<MapLayerData> _layer_objs;
        public List<MapObjectData> _map_objs;//= new List<MapObjectData>();

        //玩家出生点信息
        public class SpawnPointData
        {
            public Vector3 position;
        }
        public List<SpawnPointData> _spawn_points;// = new List<SpawnPointData>();

        //武器出生点信息
        public class WeaponSpawnPointData
        {
            public Vector3 position;
            public List<int> ids;
        }
        public List<WeaponSpawnPointData> _weapon_spawn_points;// = new List<WeaponSpawnPointData>();

        // 初始化 共享数据，这些数据在编辑器模式和预览下共享 达到单场景目的
        public static void Clear()
        {
            UIRoot.HasPreview = false;
            MapObjectRoot.data = null;
            MapObjectRoot.json = "";
            //清空 地图预览时的共享数据
            //以下2个的修改规则 改为 游戏周期内 只有
            //    current_map_brief = "";
            //  current_map_name = "";
        }
        public void Save(MapObjectRoot root)
        {
            SaveJson(root);
            return;
            {//save map object
                _map_objs = new List<MapObjectData>();
                _map_objs.Clear();
                foreach (var p in root.GetComponentsInChildren<MapObject>(true))
                {
                    _map_objs.Add(new MapObjectData
                    {
                        prefab = p.gameObject.name,
                        position = p.transform.position
                    });
                }
            }
            {//save map object decroate
                foreach (var p in root.GetComponentsInChildren<MapObjectDecorate>(true))
                {
                    _map_objs.Add(new MapObjectData
                    {
                        prefab = p.gameObject.name,
                        position = p.transform.position
                    });
                }
            }
            {//save  spawn point   
                _spawn_points = new List<SpawnPointData>();
                _spawn_points.Clear();
                foreach (var p in root.GetComponentsInChildren<MapObjectSpawnPoint>(true))
                {
                    _spawn_points.Add(new SpawnPointData
                    {
                        position = p.transform.position
                    });
                }
            }

            {//save  weapon spawn point   
                _weapon_spawn_points = new List<WeaponSpawnPointData>();
                _weapon_spawn_points.Clear();
                foreach (var p in root.GetComponentsInChildren<MapObjectWeaponSpawnPoint>(true))
                {
                    _weapon_spawn_points.Add(new WeaponSpawnPointData
                    {
                        position = p.transform.position,
                        ids = new List<int>(p._weapon_ids)
                    });
                }
            }
        }
        public void Reload(MapObjectRoot root)
        {
            //  root.Reload(this);
            // this.ReloadJson( jjson  ,root);
            //   root.Reload(this);
        }
        public void SaveJson(MapObjectRoot root)
        {
            string json = root.SerializeToJson();
            //save to local
        }
        public void ReloadJson()
        {
            this.ReloadJson(MapObjectRoot.json);
        }
        public void ReloadJson(string json)
        {
            if (string.IsNullOrEmpty(json)) return;
            if (!Serializable.Map.IsMapJson(json)) return;

            Serializable.Map map = Serializable.ToObject<Serializable.Map>(json);
            //通过map 重建 data 主题的话 会在 进入时 加载 这里只管物件
            int theme = map.theme;
            this.map_name = map.name;
            this.map_brief = map.brief;
            current_map_brief = this.map_brief;
            current_map_name = this.map_name;
            if (map.MapInfoMode == (int)MapGameMode.Parkour)
            {
                //跑酷模式
                edgebox_up = map.map_parkour.edgebox_up;
                edgebox_down = map.map_parkour.edgebox_down;
                edgebox_left = map.map_parkour.edgebox_left;
                edgebox_right = map.map_parkour.edgebox_right;
            }
            else
            {
                //经典模式
                ResetEdgeBox();
            }
            {//save map layer
                _layer_objs = new List<MapLayerData>();
                _layer_objs.Clear();
                foreach (var p in map.layers)
                {
                    _layer_objs.Add(new MapLayerData
                    {
                        layerIndex = p.layerIndex,
                        moveFactor = p.moveFactor
                    });
                }
            }
            {//save map object
                _map_objs = new List<MapObjectData>();
                _map_objs.Clear();
                foreach (var p in map.objects)
                {
                    _map_objs.Add(new MapObjectData
                    {
                        prefab = p.prefab,
                        position = p.transform.position,
                        layerIndex = p.layerIndex,
                        extPropClassIndex = p.extPropClassIndex,
                        extPropClassName = p.extPropClassName,
                        extPropJson = p.extPropJson
                });
                }
            }
            {//save  spawn point   
                _spawn_points = new List<SpawnPointData>();
                _spawn_points.Clear();
                foreach (var p in map.spawn.points)
                {
                    _spawn_points.Add(new SpawnPointData
                    {
                        position = new Vector3(p.x, p.y, p.z)
                    });
                }
            }

            {//save  weapon spawn point   
                _weapon_spawn_points = new List<WeaponSpawnPointData>();
                _weapon_spawn_points.Clear();
                int i = 0;
                foreach (var p in map.weapon_spawn_points.points)
                {
                    _weapon_spawn_points.Add(new WeaponSpawnPointData
                    {
                        position = new Vector3(p.x, p.y, p.z),
                        ids = new List<int>(map.weapon_spawn_points.ids[i].ids)
                    });
                    i++;
                }
            }
        }

        //只写不清理  只有在新建地图时 才清理
        public static string current_map_name = "";
        public static string current_map_brief = "";

        //酷跑模式的地图设置  case by case
        public static float edgebox_left = 44.87f;
        public static float edgebox_right = -44.3f;
        public static float edgebox_down = -40f;
        public static float edgebox_up = 39.8f;

        public static void ResetEdgeBox()
        {
            edgebox_left = -44.3f;
            edgebox_right = 44.87f;
            edgebox_down = -40f;
            edgebox_up = 39.8f;
        }
    }
}