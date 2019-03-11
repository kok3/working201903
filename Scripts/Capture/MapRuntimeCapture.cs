/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
//地图 预览图片生成器
//可生成JPG上传到服务器 也可以随时动态生成JPG 而无需WWW下载
//注意由于是加载的场景真正的object 可能会导致意外的BUG
//为了不让地图污染场景 因此一个一个加载 而不是一个场景 全部处理完一个队列
//注意 会清理所有的 map产生的物件 请勿在战斗场景调用该函数！！！！ 不过正常情况下 都会任何其他单例场景加载行为都会取消正在运行的任务 因此可以手动调用ClearAll
//
namespace MapEditor
{
    public class MapRuntimeCapture : MonoBehaviour
    {
        [SerializeField]
        Camera camera;
        void Awake()
        {
            is_running = true;
        }
        IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            //            StartCoroutine(AsyncCapture("87306", null));
            if (_tasks.Count <= 0)
            {
                Application.UnloadLevel("MapRuntimeCapture");
                yield break;
            }
            Task task = _tasks.Dequeue();
            if (task == null)
            {
                Application.UnloadLevel("MapRuntimeCapture");
                yield break;
            }

            is_running = true;
            if (string.IsNullOrEmpty(task.json) == false)
            {
                StartCoroutine(AsyncCaptureCurrentEditMap(task.json, task.cb_sprite, task.width, task.height, task.showWeapon));
                yield break;
            }
            if (!task.LoadLocal)
            {
                StartCoroutine(AsyncCapture(task.uuid, task.cb_sprite, task.width, task.height, task.showWeapon));
            }
            else
            {
                StartCoroutine(AsyncCaptureLocalMapTmp(task.uuid, task.cb_sprite, task.width, task.height, task.showWeapon));
            }
        }
        void OnDestroy()
        {
            try
            {
                //清理地图产生的垃圾
                foreach (var p in GameObject.FindObjectsOfType<RemoveOnLevelChange>())
                {
                    if (p == null) continue;
                    GameObject.DestroyImmediate(p.gameObject);
                }
                foreach (var p in GameObject.FindObjectsOfType<RemoveAfterSeconds>())
                {
                    if (p == null) continue;
                    GameObject.DestroyImmediate(p.gameObject);
                }
                /* foreach (var p in GameObject.FindObjectsOfType<DestroyAfterTime>())
                 {
                     if (p == null) continue;
                     GameObject.DestroyImmediate(p.gameObject);
                 }*/
            }
            catch (Exception e)
            {
                //report to bugly
                Debug.LogError("MapEditor.MapRuntimeCapture clear map-grabge errro   " + e.Message);
            }
            GC.Collect();
            is_running = false;
            CheckTask();
        }
        static bool is_running = false;
        class Task
        {
            public string uuid;
            public VoidFuncObject cb_sprite;
            public int width = 1136;
            public int height = 640;
            public bool LoadLocal = false;
            public string json = null;
            public bool showWeapon = false;
        }

        static void CheckTask()
        {
            if (is_running)
            {
            }
            else
            {
                if (_tasks.Count <= 0) return;
                Application.LoadLevelAdditiveAsync("MapRuntimeCapture");
            }
        }
        public static void ClearAll()
        {
            _tasks.Clear();
            //unload scene? if running 
            //clear cache
            MapCaptureLoadFromCache.ClearAll();
        }

        public static void ClearTasks()
        {
            _tasks.Clear();
        }

        static Queue<Task> _tasks = new Queue<Task>();

        public static void AsyncGetSpriteMapCapture(string uuid, VoidFuncObject cb_sprite, int width = 1136, int height = 640, bool showWeapon = false)
        {
            if (string.IsNullOrEmpty(uuid) || width <= 0 || height <= 0)
            {
                if (cb_sprite != null)
                {
                    try
                    {
                        cb_sprite(null);
                    }
                    catch (Exception e)
                    {

                    }
                }
                return;
            }
            ///try load from cache
            if (MapEditor.MapCaptureLoadFromCache.LoadFromCacheAsyncCapture(uuid, cb_sprite, width, height, showWeapon))
            {
                //get ok
                return;
            }
            _tasks.Enqueue(new Task
            {
                uuid = uuid,
                cb_sprite = cb_sprite,
                width = width,
                height = height,
                showWeapon = showWeapon,
            });
            if (_tasks.Count <= 1)
            {
                CheckTask();
            }
        }
        //加载本地地图
        public static void AsyncGetSpriteMapCaptureLocalMapTmp(string uuid, VoidFuncObject cb_sprite, int width = 1136, int height = 640)
        {
            if (string.IsNullOrEmpty(uuid) || width <= 0 || height <= 0)
            {
                if (cb_sprite != null)
                {
                    try
                    {
                        cb_sprite(null);
                    }
                    catch (Exception e)
                    {

                    }
                }
                return;
            }
            ///try load from cache
            if (MapEditor.MapCaptureLoadFromCache.LoadFromCacheAsyncCaptureLocalMapTmp(uuid, cb_sprite, width, height))
            {
                //get ok
                return;
            }
            _tasks.Enqueue(new Task
            {
                uuid = uuid,
                cb_sprite = cb_sprite,
                width = width,
                height = height,
                LoadLocal = true
            });
            if (_tasks.Count <= 1)
            {
                CheckTask();
            }
        }

        //当前正在编辑的截图
        public static void AsyncGetSpriteMapCaptureCurrentEditMap(VoidFuncObject cb_sprite, int width = 1136, int height = 640)
        {
            if (width <= 0 || height <= 0)
            {
                if (cb_sprite != null)
                {
                    try
                    {
                        cb_sprite(null);
                    }
                    catch (Exception e)
                    {

                    }
                }
                return;
            }
            if (MapEditor.MapObjectRoot.ins == null || MapEditor.MapObjectRoot.ins == null)
            {
                return;
            }
            _tasks.Enqueue(new Task
            {
                cb_sprite = cb_sprite,
                width = width,
                height = height,
                json = MapObjectRoot.ins.SerializeToJson()
            });
            if (_tasks.Count <= 1)
            {
                CheckTask();
            }
        }
        //MapEditor.MapObjectRoot.json

        //当前正在编辑的截图 尝试用缓存中拿取
        public static void AsyncTryGetSpriteMapCaptureCurrentEditMap(VoidFuncObject cb_sprite, int width = 1136, int height = 640)
        {
            if (width <= 0 || height <= 0)
            {
                if (cb_sprite != null)
                {
                    try
                    {
                        cb_sprite(null);
                    }
                    catch (Exception e)
                    {

                    }
                }
                return;
            }
            if (string.IsNullOrEmpty(MapEditor.MapObjectRoot.record_json))
            {
                return;
            }
            _tasks.Enqueue(new Task
            {
                cb_sprite = cb_sprite,
                width = width,
                height = height,
                json = MapEditor.MapObjectRoot.record_json
            });
            if (_tasks.Count <= 1)
            {
                CheckTask();
            }
        }

        float since_level_loaded = 0f;
        void Update()
        {
            since_level_loaded += Time.deltaTime;
            if (since_level_loaded > 10F)
            {
                //超时 发生了未知的错误 或者地图加载下载过程过长 强制取消该任务
                Application.UnloadLevel("MapRuntimeCapture");
                since_level_loaded = 0f;
                //      Debug.LogError("MapEditor.MapRuntimeCapture MapCapture timeout force cancle task and cb will not be call");
            }
        }
        bool download_ok = false;

        IEnumerator AsyncCapture(string uuid, VoidFuncObject cb, int WIDTH, int HEIGHT, bool showWeapon = false)
        {
            //先检查缓存图片存在否 如果存在 优先加载缓存
            string weaponExtension = showWeapon ? "_weapon" : "";
            string file_name = LocalStorageMapCaptureImage.ins.GetRootDirectory() + "/" + uuid.ToString() + "_" + WIDTH + "_" + HEIGHT + weaponExtension + ".jpg";
            bool load_local_ok = false;
            if (File.Exists(file_name))
            {
                //  throw new NullReferenceException();
                Texture2D tex = null;
                //try load from disk
                var www_local = LocalStorageMapCaptureImage.ins.LoadFromDisk(uuid.ToString() + "_" + WIDTH + "_" + HEIGHT + ".jpg");
                if (www_local != null)
                {
                    using (www_local)
                    {
                        yield return www_local;
                        if (www_local.isDone && string.IsNullOrEmpty(www_local.error))
                        {
                            tex = www_local.texture;
                        }
                    }
                }
                if (tex != null)
                {
                    if (cb != null)
                    {
                        cb(tex);
                        load_local_ok = true;
                    }
                    else
                    {
                        GameObject.DestroyImmediate(tex);
                    }
                }
                tex = null;
            }
            if (load_local_ok == false)
            {
                download_ok = false;
                yield return MapHttpTask.Download(uuid, (string json) =>
                    {
                        this.download_ok = true;
                    }, () =>
                    {
                        this.download_ok = false;
                    }, true);
                if (download_ok)
                {
                    if (showWeapon)
                    {
                        ShowWeaponSpawnPoints();
                    }
                    var serializer = this.GetComponent<MapEditor.RuntimeSerialize>();
                    long luuid = 0;
                    long.TryParse(uuid, out luuid);

                    //先把地图加载出来
                    serializer.Load(luuid);
                    transform.position = new Vector3(0f, 1000f, 1000f);

                    //销毁所有MapObject脚本 防止以外情况
                    //  yield return new WaitForEndOfFrame();
                    yield return new WaitForEndOfFrame();

                    if (showWeapon)
                    {
                        CheckWeaponSpawnPoints();
                    }

                    foreach (var p in this.GetComponentsInChildren<MonoBehaviour>())
                    {
                        if (p != null)
                        {
                            if (p.gameObject == this.gameObject) continue;
                            GameObject.Destroy(p);
                        }
                    }

                    //开始捕捉图像
                    RenderTexture tex = new RenderTexture(WIDTH, HEIGHT, 20);
                    camera.targetTexture = tex;
                    camera.Render();
                    RenderTexture.active = tex;

                    Texture2D tex2d = new Texture2D(WIDTH, HEIGHT);

                    tex2d.ReadPixels(new Rect(0, 0, WIDTH, HEIGHT), 0, 0);
                    tex2d.Apply();
                    byte[] save_data = tex2d.EncodeToJPG();

                    File.WriteAllBytes(file_name, save_data);
                    RenderTexture.active = null;
                    camera.targetTexture = null;
                    if (cb != null)
                    {
                        cb(tex2d);
                    }
                    else
                    {
                        GameObject.DestroyImmediate(tex2d);
                    }
                    GameObject.DestroyImmediate(tex);
                    tex2d = null;
                    tex = null;
                }
                else
                {
                    cb(null);
                }
            }
            Application.UnloadLevel("MapRuntimeCapture");
        }

        //从本地临时地图开始截屏
        IEnumerator AsyncCaptureLocalMapTmp(string uuid, VoidFuncObject cb, int WIDTH, int HEIGHT, bool showWeapon = false)
        {
            //本地的都带了tmp字样
            // uuid = "tmp" + uuid;
            //先检查缓存图片存在否 如果存在 优先加载缓存
            string weaponExtension = showWeapon ? "_weapon" : "";
            string file_name = LocalStorageMapCaptureImage.ins.GetRootDirectory() + "/" + "tmp" + uuid.ToString() + "_" + WIDTH + "_" + HEIGHT + weaponExtension + ".jpg";
            bool load_local_ok = false;
            if (File.Exists(file_name))
            {
                //  throw new NullReferenceException();
                Texture2D tex = null;
                //try load from disk
                var www_local = LocalStorageMapCaptureImage.ins.LoadFromDisk("tmp" + uuid.ToString() + "_" + WIDTH + "_" + HEIGHT + ".jpg");
                if (www_local != null)
                {
                    using (www_local)
                    {
                        yield return www_local;
                        if (www_local.isDone && string.IsNullOrEmpty(www_local.error))
                        {
                            tex = www_local.texture;
                        }
                    }
                }
                if (tex != null)
                {
                    if (cb != null)
                    {
                        cb(tex);
                        load_local_ok = true;
                    }
                    else
                    {
                        GameObject.DestroyImmediate(tex);
                    }
                }
                tex = null;
            }
            if (load_local_ok == false)
            {
                download_ok = true;
                string x = LocalStorageMapTmp.ins.LoadTextFromDisk(StaticData.uuid + "_" + uuid.ToString() + ".json");
                if (string.IsNullOrEmpty(x) || !Serializable.Map.IsMapJson(x))
                {
                    download_ok = false;
                }
                if (download_ok)
                {
                    if (showWeapon)
                    {
                        ShowWeaponSpawnPoints();
                    }
                    var serializer = this.GetComponent<MapEditor.RuntimeSerialize>();
                    //先把地图加载出来
                    serializer.LoadFromJson(x);
                    transform.position = new Vector3(0f, 1000f, 1000f);

                    //销毁所有MapObject脚本 防止以外情况
                    //  yield return new WaitForEndOfFrame();
                    yield return new WaitForEndOfFrame();

                    if (showWeapon)
                    {
                        CheckWeaponSpawnPoints();
                    }

                    foreach (var p in this.GetComponentsInChildren<MonoBehaviour>())
                    {
                        if (p != null)
                        {
                            if (p.gameObject == this.gameObject) continue;
                            GameObject.Destroy(p);
                        }
                    }

                    //开始捕捉图像
                    RenderTexture tex = new RenderTexture(WIDTH, HEIGHT, 20);
                    camera.targetTexture = tex;
                    camera.Render();
                    RenderTexture.active = tex;

                    Texture2D tex2d = new Texture2D(WIDTH, HEIGHT);

                    tex2d.ReadPixels(new Rect(0, 0, WIDTH, HEIGHT), 0, 0);
                    tex2d.Apply();
                    byte[] save_data = tex2d.EncodeToJPG();

                    File.WriteAllBytes(file_name, save_data);
                    RenderTexture.active = null;
                    camera.targetTexture = null;
                    if (cb != null)
                    {
                        cb(tex2d);
                    }
                    else
                    {
                        GameObject.DestroyImmediate(tex2d);
                    }
                    GameObject.DestroyImmediate(tex);
                    tex2d = null;
                    tex = null;
                }
                else
                {
                    cb(null);
                }
            }
            Application.UnloadLevel("MapRuntimeCapture");
        }


        //指定的json 截图
        IEnumerator AsyncCaptureCurrentEditMap(string json, VoidFuncObject cb, int WIDTH, int HEIGHT, bool showWeapon = false)
        {
            if (showWeapon)
            {
                ShowWeaponSpawnPoints();
            }
            var serializer = this.GetComponent<MapEditor.RuntimeSerialize>();
            //先把地图加载出来
            serializer.LoadFromJson(json);
            transform.position = new Vector3(0f, 1000f, 1000f);

            //销毁所有MapObject脚本 防止以外情况
            //  yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            if (showWeapon)
            {
                CheckWeaponSpawnPoints();
            }

            foreach (var p in this.GetComponentsInChildren<MonoBehaviour>())
            {
                if (p != null)
                {
                    if (p.gameObject == this.gameObject) continue;
                    GameObject.Destroy(p);
                }
            }

            //开始捕捉图像
            RenderTexture tex = new RenderTexture(WIDTH, HEIGHT, 20);
            camera.targetTexture = tex;
            camera.Render();
            RenderTexture.active = tex;

            Texture2D tex2d = new Texture2D(WIDTH, HEIGHT);

            tex2d.ReadPixels(new Rect(0, 0, WIDTH, HEIGHT), 0, 0);
            tex2d.Apply();
            RenderTexture.active = null;
            camera.targetTexture = null;
            if (cb != null)
            {
                cb(tex2d);
            }
            else
            {
                GameObject.DestroyImmediate(tex2d);
            }
            GameObject.DestroyImmediate(tex);
            tex2d = null;
            tex = null;
            Application.UnloadLevel("MapRuntimeCapture");

        }

        private void ShowWeaponSpawnPoints()
        {
            var weaponSpawn = transform.Find("WeaponSpawn");
            if (weaponSpawn)
            {
                weaponSpawn.gameObject.SetActive(true);
            }
        }

        private void CheckWeaponSpawnPoints()
        {
            var spawnPointList = transform.GetComponentsInChildren<EditorWeaponSpawnPointTag>();
            if (spawnPointList != null)
            {
                foreach (var point in spawnPointList)
                {
                    bool canSpawnWeapons = false;
                    if(point.weapon_ids != null)
                    {
                        foreach(int id in point.weapon_ids)
                        {
                            if(id > 0)
                            {
                                canSpawnWeapons = true;
                                break;
                            }
                        }
                    }
                    point.gameObject.SetActive(canSpawnWeapons);
                }
            }
        }
    }
}