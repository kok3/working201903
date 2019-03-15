/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MapEditor
{
    public class MapObjectRoot : MonoBehaviour
    {
        public static MapObjectRoot ins = null;

        //------------地图额外配置信息



        //---------------------------
        public void Reload(MapEditorStroageData data)
        {
            data.ReloadJson();

            {//reload  spawn point   
                var list = this.GetComponentsInChildren<MapObjectSpawnPoint>(true);
                if (list.Length != data._spawn_points.Count)
                {
                    Debug.LogError("0check length is right  when reload MapEditor.Reload  " + list.Length + "      " + data._spawn_points.Count);
                }
                for (int i = 0; i < list.Length; i++)
                {
                    list[i].transform.position = data._spawn_points[i].position;
                }
            }
            {//reload weapon spawn point   
                var list = this.GetComponentsInChildren<MapObjectWeaponSpawnPoint>(true);
                if (list.Length != data._weapon_spawn_points.Count)
                {
                    Debug.LogError("1check length is right  when reload MapEditor.Reload  " + list.Length + "      " + data._weapon_spawn_points.Count);
                }
                for (int i = 0; i < list.Length; i++)
                {
                    list[i].transform.position = data._weapon_spawn_points[i].position;
                    list[i]._weapon_ids = new List<int>(data._weapon_spawn_points[i].ids);
                }
            }

            {//reload layer and map obect and map object decroate
                LayerMgr.ins.Clear();
                foreach (var l in data._layer_objs)
                {
                    LayerMgr.ins.CreateLayer(l);
                    //GameObject layer = new GameObject("layer" + l.layerIndex.ToString());
                    //layer.transform.parent = LayerMgr.ins.transform;
                    //layers[l.layerIndex-1] = layer.transform;
                }

                GameObject obj = null;
                foreach (var p in data._map_objs)
                {
                    if (p.layerIndex > 0)
                    {
                        obj = CreateObject(p.prefab, LayerMgr.ins.GetLayerByIndex(p.layerIndex));
                        if (obj != null)
                        {
                            obj.transform.position = p.position;

                            CustomerPropertyBase com = obj.GetComponent<CustomerPropertyBase>();
                            //自定义属性
                            if ((com != null) && (p.extPropJson != ""))
                            {
                                com.OnDeseriazlie(p.extPropJson);
                            }
                        }
                    }
                    else
                    {
                        obj = CreateObject(p.prefab, LayerMgr.ins.transform);
                        if (obj != null)
                        {
                            obj.transform.position = p.position;

                            CustomerPropertyBase com = obj.GetComponent<CustomerPropertyBase>();
                            //自定义属性
                            if ((com != null) && (p.extPropJson != ""))
                            {
                                com.OnDeseriazlie(p.extPropJson);
                            }
                        }
                    }

                }
            }
            //  this.map_brief = data.map_brief;
            // this.map_name = data.map_name;
            //加载了目标地图 需要恢复状态
            MapEditorStroageData.current_map_brief = data.map_brief;
            MapEditorStroageData.current_map_name = data.map_name;
            //在编辑地图入口时会设置 MapEditor.MapObjectRoot.record_json ;
#if UNITY_EDITOR
            //修改 边缘 大小
            if (UIPanelMapInfoParkour.ins != null && MapEditor.MapEditorConfig.CurrentMapGameMode == MapGameMode.Parkour)
            {
                UIPanelMapInfoParkour.ins.Reset();
            }
#endif

        }

        void Awake()
        {
            ins = this;
        }
        public static MapEditorStroageData data = null;

        //  //UI修改时  写入
        // public string map_name = "";//地图名字  跟随 json 和data
        //  public string map_brief = "";//地图简介 跟随json 和data

        void OnDestroy()
        {
            ins = null;
        }

        void Start()
        {
            //init spawn points
            for (int i = 0; i < 4; i++)
            {

                var obj = MapLoader.ins.LoadMapObjectV1("Common/OneSpawnPoint");

                obj = GameObject.Instantiate<GameObject>(obj);
                //PrefabsMgr.Load("Map/Prefabs/MapObject/Common/OneSpawnPoint");
                var pos = Vector3.zero;
                pos.z = -10f + 6f * i;
                pos.x = 0f;//just for show in the front of everything
                pos.y = 6f;
                obj.transform.position = pos;
                obj.transform.parent = transform;
                //    obj.GetComponentFully<OneMapObjectBase>().Init(i);
                this.CheckAddObject(obj);
                var scale = obj.transform.localScale;
                scale.x = 50f;
                obj.transform.localScale = scale;
                obj.GetComponentInChildren<TextMesh>().text = (i + 1).ToString() + "P";
            }
            //init weapon spawn point
            int WEAPON_SPAWN_POINT = 4;
            if (MapEditor.MapEditorConfig.CurrentMapGameMode == MapGameMode.Parkour)
            {
                WEAPON_SPAWN_POINT = 20;  
            }
            else
            {
#if UNITY_EDITOR
                WEAPON_SPAWN_POINT = 10;
#endif
            }
            EditorWeaponSpawnPointsRoot weapon_spawn_root = this.transform.root.GetComponentInChildren<EditorWeaponSpawnPointsRoot>();
            for (int i = 0; i < WEAPON_SPAWN_POINT; i++)
            {
                var obj =
                     MapLoader.ins.LoadMapObjectV1("Common/OneWeaponSpawnPoint");
                obj = GameObject.Instantiate<GameObject>(obj);
                // PrefabsMgr.Load("Map/Prefabs/MapObject/Common/OneWeaponSpawnPoint");
                var pos = Vector3.zero;
#if UNITY_EDITOR
                pos.z = -15f + 3f * i;
#else
                pos.z = -10f + 6f * i;
#endif
                pos.x = 0f;//just for show in the front of everything
                pos.y = 3f;

                obj.transform.position = pos;
                obj.transform.parent = transform;
                //    obj.GetComponentFully<OneMapObjectBase>().Init(i);

                if (i >= weapon_spawn_root.transform.childCount)
                {
                    //实际武器点大于阈值 需要扩容  runtime 也要处理这个
                    GameObject x = GameObject.Instantiate<GameObject>(weapon_spawn_root.transform.GetChild(0).gameObject, weapon_spawn_root.transform, false);
                    x.transform.position = pos;
                    x.name = "p" + (i + 1);
                }

                this.CheckAddObject(obj);
                var scale = obj.transform.localScale;
                scale.x = 50f;
                obj.transform.localScale = scale;
                obj.GetComponentInChildren<TextMesh>().text = "武器" + (i + 1).ToString();

            }

            //表示是自动恢复的 或者 编辑的地图
            if (data != null)
            {
                this.Reload(data);
                data = null;
            }
            else
            {
                //新建的地图  需要初始化一下数据  方便 预览图预览什么的
                this.SerializeToJson();
            }
            this.SetSpawnPointVisible(false);
            this.SetWeaponSpawnPointVisible(false);
        }
        public void DestroyObject(GameObject obj)
        {
            this.CheckDestroyObject(obj);
            GameObject.Destroy(obj);
        }
        public void DestroyObjectImmediate(GameObject obj)
        {
            this.CheckDestroyObject(obj);
            GameObject.DestroyImmediate(obj);
        }

        void CheckDestroyObject(GameObject obj)
        {
            //TYPE CHECK just for fast get Component
            var cmp = obj.GetComponent<MapObjectBase>();
            if (cmp == null)
            {
                Debug.LogError("  create object error  CreateObject");
            }
            if (cmp is MapObject)
            {
                _list_map_object.Remove(cmp as MapObject);
            }
            else if (cmp is MapObjectDecorate)
            {
                _list_map_object_decroate.Remove(cmp as MapObjectDecorate);
            }
            else if (cmp is MapObjectSpawnPoint)
            {
                _list_spawn_points.Remove(cmp as MapObjectSpawnPoint);
            }
            else if (cmp is MapObjectWeaponSpawnPoint)
            {
                _list_weapon_spawn_points.Remove(cmp as MapObjectWeaponSpawnPoint);
            }
            else
            {
                Debug.LogError("unknow type of " + cmp.GetType().ToString());
            }

        }
        public GameObject CreateObject(string name, Transform parent = null)
        {
            Transform p = transform;
            if (parent != null)
                p = parent;
            GameObject obj1 = //PrefabsMgr.LoadMapObjectEditor(name);

                MapLoader.ins.LoadEditorV1(MapEditor.MapEditorConfig.CurrentSelectTheme, name);

            var obj = GameObject.Instantiate<GameObject>(obj1, p, true);
            Debug.Log(" create new mapobject name=" + name);
            obj.name = name;
            //this.CheckAllConflict();
            this.CheckAddObject(obj);
            return obj;
        }
        void CheckAddObject(GameObject obj)
        {
            //TYPE CHECK just for fast get Component
            var cmp = obj.GetComponent<MapObjectBase>();
            if (cmp == null)
            {
                Debug.LogError("  create object error  CreateObject");
            }
            if (cmp is MapObject)
            {
                _list_map_object.Add(cmp as MapObject);
            }
            else if (cmp is MapObjectDecorate)
            {
                _list_map_object_decroate.Add(cmp as MapObjectDecorate);
            }
            else if (cmp is MapObjectSpawnPoint)
            {
                _list_spawn_points.Add(cmp as MapObjectSpawnPoint);
            }
            else if (cmp is MapObjectWeaponSpawnPoint)
            {
                _list_weapon_spawn_points.Add(cmp as MapObjectWeaponSpawnPoint);
            }
            else
            {
                Debug.LogError("unknow type of " + cmp.GetType().ToString());
            }
        }
        //-----------just for fast check
        //-----------just for fast check
        //-----------just for fast check
        //-----------just for fast check
        public List<MapObject> _list_map_object = new List<MapObject>();
        public List<MapObjectDecorate> _list_map_object_decroate = new List<MapObjectDecorate>();
        public List<MapObjectSpawnPoint> _list_spawn_points = new List<MapObjectSpawnPoint>();
        public List<MapObjectWeaponSpawnPoint> _list_weapon_spawn_points = new List<MapObjectWeaponSpawnPoint>();


        /*   void Update()
           {
               //  this.CheckAllConflict();
               if (Input.GetKeyDown(KeyCode.Space))
               {
                   this.SerializeToJson();

               }
           }*/
        public bool CheckConflict(GameObject who)
        {
            //改为 不处理任何碰撞
            return false;
            bool ok = false;
            for (int i = 0; i < _list_map_object_decroate.Count; i++)
            {
                _list_map_object_decroate[i].DisableColliders();
            }
            ok = who.GetComponentFully<MapObjectBase>().CheckConflict();
            for (int i = 0; i < _list_map_object_decroate.Count; i++)
            {
                _list_map_object_decroate[i].EnabledCollider();
            }
            return ok;
        }

        public bool CheckAllConflict()
        {
            //改为 不处理任何碰撞
            if (DevConfig.MapEditorDisableAllConflictCheck)
            {
                return false;
            }
            /*   //#1000170  改为不限制碰撞和增加物件上限为50
               return false;
   #if UNITY_EDITOR
               //编辑器下  不检查碰撞
               return false;
   #endif*/
            bool ok = false;

            for (int i = 0; i < _list_map_object_decroate.Count; i++)
            {
                _list_map_object_decroate[i].DisableColliders();
            }
            for (int i = 0; i < _list_map_object.Count; i++)
            {
                ok = _list_map_object[i].CheckConflict();
                if (ok)
                {
                    break;
                }
            }
            for (int i = 0; i < _list_map_object_decroate.Count; i++)
            {
                _list_map_object_decroate[i].EnabledCollider();
            }
            return ok;
        }
        public void SetSpawnPointVisible(bool visible)
        {
            int count = _list_spawn_points.Count;
            for (int i = 0; i < count; i++)
            {
                _list_spawn_points[i].gameObject.SetActive(visible);
            }
        }
        public void SetWeaponSpawnPointVisible(bool visible)
        {
            int count = _list_weapon_spawn_points.Count;
            for (int i = 0; i < count; i++)
            {
                _list_weapon_spawn_points[i].gameObject.SetActive(visible);
            }
        }

        public bool CanGoStartPreview(out string error)
        {
            //error = "";
            //#1000170  改为不限制碰撞和增加物件上限为50
            // return true;
            error = "您当前物体有重叠,请检查关卡!";
            //check map object number
#if UNITY_EDITOR
            //编辑器下  不检查碰撞
            return true;
#endif
            if (_list_map_object.Count <= 0)
            {
                error = "场景物件不能为空";
                return false;
            }

            //check threr is any conflict
            bool ok = false;
            for (int i = 0; i < _list_map_object_decroate.Count; i++)
            {
                _list_map_object_decroate[i].DisableColliders();
            }
            if (!ok)
            {//check 
                for (int i = 0; i < _list_map_object.Count; i++)
                {
                    ok = _list_map_object[i].CheckConflict();
                    if (ok)
                    {
                        _list_map_object[i].SetBright(true);
                        for (int ii = 0; ii < _list_map_object_decroate.Count; ii++)
                        {
                            _list_map_object_decroate[ii].EnabledCollider();
                        }
                        return false;
                    }
                }
            }
            if (!ok)
            {//check
                for (int i = 0; i < _list_weapon_spawn_points.Count; i++)
                {
                    ok = _list_weapon_spawn_points[i].CheckConflict();
                    if (ok)
                    {
                        _list_weapon_spawn_points[i].SetBright(true);
                        for (int ii = 0; ii < _list_map_object_decroate.Count; ii++)
                        {
                            _list_map_object_decroate[ii].EnabledCollider();
                        }
                        return false;
                    }
                }
            }
            if (!ok)
            {//check
                for (int i = 0; i < _list_weapon_spawn_points.Count; i++)
                {
                    ok = _list_weapon_spawn_points[i].CheckConflict();
                    if (ok)
                    {
                        _list_weapon_spawn_points[i].SetBright(true);
                        for (int ii = 0; ii < _list_map_object_decroate.Count; ii++)
                        {
                            _list_map_object_decroate[ii].EnabledCollider();
                        }
                        return false;
                    }
                }
            }
            if (ok) return false;

            //check map object done
            /*//check spawn point is valid
            foreach (var p in _spawn_points)
            {
                var pos = p.transform.position;
                //  RaycastHit hit;
                var ray = new Ray(pos, Vector3.down);
                RaycastHit[] hits = null;
                var num = Physics.RaycastNonAlloc(ray, hits, 10000f);
                int hit_obj_num = 0;
                for (int i = 0; i < num; i++)
                {
                    if (hits[i].collider.gameObject.GetComponentFully<MapObjectBase>() != null)
                    {
                        hit_obj_num++;
                        break;
                    }
                }
                if (hit_obj_num <= 0)
                {
                    return false;
                }
                Debug.Log("check hit obj num=" + hit_obj_num);
            }
            */

            /* //check weapon spawn point  property
              int num_of_valid_weapon_spawn_point = 0;
              foreach (var p in _weapon_spawn_points)
              {
                  var c = p.GetComponent<MapObjectWeaponSpawnPoint>();
                  if (c != null)
                  {
                      if (c.IsValidForStartPreview())
                      {
                          ++num_of_valid_weapon_spawn_point;
                      }
                  }
              }
              if (num_of_valid_weapon_spawn_point <= 0)
              {
                  //make sure has at least one valid point 
                  return false;
              }*/


            return true;
        }
        public void SetDecorateColliderEnable(bool enable)
        {
            if (enable)
            {
                for (int ii = 0; ii < _list_map_object_decroate.Count; ii++)
                {
                    _list_map_object_decroate[ii].EnabledCollider();
                }
            }
            else
            {
                for (int ii = 0; ii < _list_map_object_decroate.Count; ii++)
                {
                    _list_map_object_decroate[ii].DisableColliders();
                }
            }
        }
        public void SetAllGameObjectActive(bool active)
        {
            // foreach (var p in this.GetComponentsInChildren<MapObjectBase>(true))
            {
                //   p.gameObject.SetActive(active);
            }

            for (int ii = 0; ii < _list_map_object_decroate.Count; ii++)
            {
                _list_map_object_decroate[ii].gameObject.SetActive(active);
            }
            for (int ii = 0; ii < _list_map_object.Count; ii++)
            {
                _list_map_object[ii].gameObject.SetActive(active);
            }
            for (int ii = 0; ii < _list_spawn_points.Count; ii++)
            {
                _list_spawn_points[ii].gameObject.SetActive(active);
            }
            for (int ii = 0; ii < _list_weapon_spawn_points.Count; ii++)
            {
                _list_weapon_spawn_points[ii].gameObject.SetActive(active);
            }
        }

        public void SetBrightAll(bool bright)
        {
            for (int ii = 0; ii < _list_map_object_decroate.Count; ii++)
            {
                _list_map_object_decroate[ii].SetBright(bright);
            }
            for (int ii = 0; ii < _list_map_object.Count; ii++)
            {
                _list_map_object[ii].SetBright(bright);
            }
            for (int ii = 0; ii < _list_spawn_points.Count; ii++)
            {
                _list_spawn_points[ii].SetBright(bright);
            }
            for (int ii = 0; ii < _list_weapon_spawn_points.Count; ii++)
            {
                _list_weapon_spawn_points[ii].SetBright(bright);
            }
        }
        public int TotalMapObjectCount
        {
            get
            {
                return _list_map_object.Count + _list_map_object_decroate.Count + _list_spawn_points.Count + _list_weapon_spawn_points.Count;
            }
        }
        public void SetPreviewColliderEnable(bool enable)
        {
            {//decorate
                MapObjectRoot.ins.SetDecorateColliderEnable(enable);
            }

            {//spawn
                if (enable)
                {
                    for (int ii = 0; ii < _list_spawn_points.Count; ii++)
                    {
                        _list_spawn_points[ii].EnabledCollider();
                    }
                }
                else
                {
                    for (int ii = 0; ii < _list_spawn_points.Count; ii++)
                    {
                        _list_spawn_points[ii].DisableColliders();
                    }
                }
            }
            {//weapon
                if (enable)
                {
                    for (int ii = 0; ii < _list_weapon_spawn_points.Count; ii++)
                    {
                        _list_weapon_spawn_points[ii].EnabledCollider();
                    }
                }
                else
                {
                    for (int ii = 0; ii < _list_weapon_spawn_points.Count; ii++)
                    {
                        _list_weapon_spawn_points[ii].DisableColliders();
                    }
                }
            }
        }
        //execute serialize to json string  share json data
        public static string json = "";
        public static string record_json = "";
        public string SerializeToJson()
        {
            var serizlizer = this.GetComponentInParent<EditorSerialize>();
            {//emplace weapon spawn info to 
                var tags = serizlizer.GetComponentsInChildren<EditorWeaponSpawnPointTag>();
                if (tags.Length != _list_weapon_spawn_points.Count || tags.Length <= 0)
                {
                    Debug.LogError("EditorWeaponSpawnPointTag length != _weapon_spawn_points     are you sure " + tags.Length + "  " + _list_weapon_spawn_points.Count);
                }
                for (int i = 0; i < tags.Length; i++)
                {
                    tags[i].weapon_ids = _list_weapon_spawn_points[i].GetComponentFully<MapObjectWeaponSpawnPoint>()._weapon_ids;
                    tags[i].transform.position = _list_weapon_spawn_points[i].transform.position;
                }
            }
            // emplace spawn position to 
            {
                var spawn = serizlizer.GetComponentInChildren<EditorSpawnPointsRoot>();
                for (int i = 0; i < _list_spawn_points.Count; i++)
                {
                    spawn.transform.GetChild(i).position = _list_spawn_points[i].transform.position;
                }
            }



            var json = serizlizer.ToJson(this);
            MapObjectRoot.json = json;
            MapObjectRoot.record_json = json;
#if UNITY_EDITOR
            Debug.Log(json);
#endif
            return json;

            return string.Empty;
        }
    }
}