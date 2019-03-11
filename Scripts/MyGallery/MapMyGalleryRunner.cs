/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using System.Collections;
using System.Collections.Generic;
using Skin;
using UnityEngine;
namespace MapEditor
{
    //浏览地图 打分 主runner
    //TODO把 个人地图数据 缓存在本地 通过时间戳 来标记出来 是有上传成功才更新本地缓存 否则都读取本地缓存 而非服务器请求
    //缓存要注意多账号问题
    public class MapMyGalleryRunner : GameMgrExtensionBase
    {
        public static MapMyGalleryRunner ins = null;
        void Awake()
        {
            ins = this;
        }

        GameMgr mgr = null;
        //     public static HashTable kv_cache = null;
        //       public static bool just_reload = false;
        //缓存当前自己的地图 数据
        static Dictionary<long, long> _cache_hash = new Dictionary<long, long>();
        static List<long> _cache = new List<long>();
        static int currentIndex = -1;
        static long currentId = 0;
        public static bool just_reload = false;
        public static long GetNext()
        {
            if (_cache.Count <= 0) return 0;
            currentIndex = ++currentIndex % _cache.Count;
            var id = _cache[currentIndex];
            currentId = id;
            if (_cache_hash.ContainsKey(id))
            {
                return id;
            }
            else
            {
                Debug.LogError("can not exist in hash table check this");
            }
            return 0;
        }

        void LoadNext(long id = 0)
        {
            //  long id =GetNext();
            if (just_reload)
            {
                id = currentId;
            }
            else
            {
                id = GetNext();
            }
            if (id > 0)
            {
                currentId = id;
                this.StopAllCoroutines();
                StartCoroutine(Download(id.ToString()));
                AsyncSyncMapInfo(id);
            }
            else
            {
                if (UICommonDialog.ins != null)
                {
                    UICommonDialog.ins.ShowOK("未知错误001", () =>
                    {
                        Application.LoadLevel("MapEditorEntry");
                    });
                }
                else
                {
                    Application.LoadLevel("MapEditorEntry");
                }
            }
        }
        void AsyncSyncMapInfo(long id)
        {
            if (UIPanelMyGalleryMenu.ins != null)
            {
                UIPanelMyGalleryMenu.ins.Sync(0);
            }
            if (id <= 0) return;
            //先在cache中查找
            if (_cache_hash.ContainsKey(id) && _cache_hash[id] != 0)
            {
                if (UIPanelMyGalleryMenu.ins != null)
                {
                    UIPanelMyGalleryMenu.ins.Sync(_cache_hash[id]);
                }
            }
            else
            {
                RpcClient.ins.SendRequest("map", "map_info_public", "uuid:" + id.ToString() + ",", (RpcRespone ss) =>
                {//不需要重试
                    if (ss != null && ss.ok)
                    {
                        var kv = Json.Decode(ss.protocol.json);
                        if (kv != null && kv.Get("ret") == "ok")
                        {
                            if (UIPanelMyGalleryMenu.ins != null)
                            {
                                long like = kv.GetLong("like");

                                if (_cache_hash.ContainsKey(id))
                                {
                                    _cache_hash[id] = like;
                                }
                                else
                                {
                                    _cache_hash.Add(id, like);
                                }
                                UIPanelMyGalleryMenu.ins.Sync(like);
                            }
                        }
                    }
                });
            }
        }
        void _ReqMyGallery()
        {
            // 拉取 个人地图先
            RpcClient.ins.SendRequest("map", "map_info_my_gallery_all", "uuid:" + //      
                StaticData.uuid.ToString()
                //   "110"
                +
                ",", (RpcRespone ss) =>
                {
                    if (ss != null && ss.ok)
                    {
                        Debug.Log(ss.protocol.json);
                        var list = ss.protocol.json.Split(',');
                        if (list.Length >= 2)
                        {
                            long id = 0;
                            for (int i = 0; i < list.Length - 1; i++)
                            {
                                id = long.TryParse(list[i], out id) ? id : 0;
                                if (id <= 0)
                                {
                                    Debug.Log("parse list has reach end");
                                    break;
                                }
                                _cache_hash.Add(id, 0);
                                _cache.Add(id);
                            }
                            this.LoadNext();
                        }
                        else if (list.Length == 1)
                        {
                            UICommonDialog.ins.ShowOK("当前没有地图快去创建把!", () =>
                            {
                                Application.LoadLevel("MapEditorEntry");
                            });
                        }
                        else
                        {
                            UICommonDialog.ins.ShowYesNo("网络错误是否重试?", () =>
                            {
                                _ReqMyGallery();
                            }, () =>
                            {

                            });
                        }
                    }
                    else
                    {
                        UICommonDialog.ins.ShowYesNo("网络错误是否重试?", () =>
                        {
                            _ReqMyGallery();
                        }, () =>
                        {

                        });
                    }
                });
        }
        public override void OnStart(GameMgr mgr)
        {
            if (_cache.Count > 0)
            {
                this.LoadNext();
            }
            else
            {
                this._ReqMyGallery();
            }
            this.mgr = mgr;
        }

        public static string json = "";
        IEnumerator Download(string name)
        {
            string json = string.Empty;

            yield return MapHttpTask.Download(name, (string json1) =>
                {
                    json = json1;
                });
            if (!string.IsNullOrEmpty(json))
            {
                yield return Application.LoadLevelAdditiveAsync("MapMyGalleryRuntime");
              //  mgr.latestLoadedMap = GameObject.FindObjectOfType<MapInfo>();
                var run = GameObject.FindObjectOfType<MapMyGalleryRuntimeRunner>();

                run.LoadFromJson(json);
                this.StartCoroutine(StartWithIsNetworkFalse());
            }
        }
        public override void OnIsNetworkFalseKillPlayerDone()
        {
            just_reload = true;
            this.StopAllCoroutines();
            this.StartCoroutine(CoroOnIsNetworkFalseKillPlayerDone());
        }
        public IEnumerator CoroOnIsNetworkFalseKillPlayerDone()
        {
            yield return new WaitForSecondsRealtime(1.5f);
            Time.timeScale = 1f;
            //do some thing
            yield return new WaitForSecondsRealtime(0.5f);
            //reload
            Application.LoadLevel("MapMyGallery");

            // UIRoot.ins.EndPreView();
            //  UIRoot.ins.StartPreView();
        }
        GameObject player = null;
        void AddSkinToCharacter(GameObject player1)
        {
            CharacterSkin currentCharacterSkin = player1.GetComponent<CharacterSkin>();
            if (currentCharacterSkin != null && StaticData.CanLoadSkin())
            {
                Transform[] amounts = new Transform[(int)SkinPartType.Max];
                var rigbods = player1.transform.Find("Rigidbodies");
                if (rigbods != null)
                {
                    int max = (int)SkinPartType.Max;
                    amounts = new Transform[max];
                    for (int i = 1; i < max; i++)
                    {
                        SkinPartType partType = (SkinPartType)i;
                        amounts[i] = rigbods.Find(partType.ToString());
                        if (amounts[i] == null)
                        {
                            //Debug.LogError("CharacterSkin can not find this part " + partType);
                        }
                    }
                }
                var typetoskin = StaticData.GetSelfSkins();
                for (int i = 1; i < (int)Skin.SkinPartType.Max; i++)
                {
                    if (typetoskin[i] != null)
                    {
                        foreach (var id in typetoskin[i])
                        {
                            currentCharacterSkin.LoadSkinPart(i, id);
                        }
                    }
                }
            }
            Controller controller = player1.GetComponent<Controller>();
            controller.SetLineRendererVisible(StaticData.LineRendererVisible);
        }
        IEnumerator StartWithIsNetworkFalse()
        {
            yield return new WaitForSecondsRealtime(1.0f);
            if (this.player != null)
            {
                GameObject.DestroyImmediate(this.player);
                this.player = null;
            }
            mgr.GameHasStartForMap = true;

            // play count down

            if (mgr.latestLoadedMapInfo.CurrentSerializeMap != null && mgr.latestLoadedMapInfo.CurrentSerializeMap.MapInfoMode == (int)MapGameMode.Parkour)
            {
                if (UICountDownGo.ins != null)
                {
                    UICountDownGo.ins.Play();
                }
            }
            else
            {
                if (UICountDown.ins != null)
                {
                    UICountDown.ins.Play();
                }
            }

            //  yield return new WaitForSecondsRealtime(1.5f);

            //create player automatic

            var obj = MiscLoader.ins.LoadAndInstantiate<GameObject>("Game/Character", "Game/Character/Character.prefab");
            this.player = obj;
            obj.SetActive(true);
            //   yield return new WaitForEndOfFrame();
            obj.layer = 20;
            foreach (var p in obj.GetComponentsInChildren<Collider>(true))
            {
                p.gameObject.layer = 20;
            }
            var info = obj.GetComponent<CharacterInformation>();
            if (info != null)
            {
                foreach (var p in obj.GetComponentsInChildren<SetLinePositions>())
                {
                    p.SetColor(info.myColor);
                }
                obj.GetComponentInChildren<HeadRenderer>().SetColor(info.myColor, info.id_in_room);
                //   var sync = obj.GetComponent<SyncablePlayer>();
                //    GameObject.DestroyObject(sync as Component);
                info.myLayer = 20;
                info.uuid = StaticData.uuid;
                info.luuid = StaticData.luuid;
                info.characterName = PlayerPrefs.GetString("PlayerName");
                info.teamID = PlayerPrefs.GetInt("teamid");
            }
            else
            {
                foreach (var p in obj.GetComponentsInChildren<SetLinePositions>())
                {
                    p.SetColor(new Color(230f / 255f, 190f / 255f, 0f / 255f));
                }
                obj.GetComponentInChildren<HeadRenderer>().SetColor(new Color(230f / 255f, 190f / 255f, 0f / 255f));
                //   var sync = obj.GetComponent<SyncablePlayer>();
                //    GameObject.DestroyObject(sync as Component);
            }
            var controller = obj.GetComponent<Controller>();
            controller.TakeLocalControl();
            if (mgr != null)
            {
                mgr.mPlayersAll.Add(controller);
            }
            //set player position
            Vector3 target = mgr.latestLoadedMapInfo.spawnPoints[UnityEngine.Random.Range(0, mgr.latestLoadedMapInfo.spawnPoints.Length)].position;
            var rigs = obj.GetComponentsInChildren<Rigidbody>();
            Hip hip = obj.GetComponentInChildren<Hip>(true);
            var delta = target - hip.transform.position;
            //   obj.transform.position = target;
            foreach (var pp in rigs)
            {
                if (pp == null) continue;//TODO when player remove this coroutine will missing reference
                pp.transform.position += delta;
            }
            AddSkinToCharacter(this.player);
            //TODO why this code do not work ?
            foreach (Rigidbody rig in rigs)
            {
                rig.isKinematic = true;// disable physics              
            }
            yield return new WaitForEndOfFrame();
            yield return new WaitForSecondsRealtime(1f);

            foreach (Rigidbody rig in rigs)
            {
                rig.isKinematic = false;// disable physics      
            }

        }
        void OnDestroy()
        {
            if (ins == this)
            {
                ins = null;
            }
            //do something clean up
            foreach (var p in GameObject.FindObjectsOfType<RemoveOnLevelChange>())
            {
                GameObject.DestroyImmediate(p.gameObject);
            }
            if (this.player != null)
            {
                GameObject.DestroyImmediate(this.player);
                this.player = null;
            }

        }
    }
}