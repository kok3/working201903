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
    public class MapGalleryRunner : GameMgrExtensionBase
    {
        public static MapGalleryRunner ins = null;
        GameMgr mgr = null;
        public static HashTable kv_cache = null;
        public static bool just_reload = false;
        MapGalleryRunner self = null;
        void Awake()
        {
            ins = this;
            self = this;
        }
#if !UNITY_EDITOR
        //非编辑器模式 不开放
        void _ReqOneGallery()
        {
            RpcClient.ins.SendRequest("map", "map_info_gallery_one", "uuid:88,", (RpcRespone ss) =>
            {
                if (self == null) return;
                if (ss != null && ss.ok)
                {
                    var kv = Json.Decode(ss.protocol.json);
                    if (kv == null)
                    {
                        if (UICommonDialog.ins != null)
                        {
                            UICommonDialog.ins.ShowOK("网络,请求失败!请稍后重试", () =>
                            {
                                Application.LoadLevel("GameLogin");
                            });
                        }
                    }
                    else
                    {
                        if (kv.Get("msg") == "none")
                        {
                            if (UICommonDialog.ins != null)
                            {
                                UICommonDialog.ins.ShowOK("当前暂无地图!", () =>
                                {
                                    Application.LoadLevel("MapEditorEntry");
                                });
                            }
                        }
                        else if (kv.Get("ret") == "ok")
                        {
                            if (UIPanelGalleryMenu.ins != null)
                            {
                                UIPanelGalleryMenu.ins.Sync(kv.GetLong("like"));
                            }
                            MapGalleryRunner.kv_cache = kv;
                            Debug.Log(ss.protocol.json);
                            StartCoroutine(Download(""));
                        }
                        else
                        {
                            if (UICommonDialog.ins != null)
                            {
                                UICommonDialog.ins.ShowOK("网络,请求失败!请稍后重试", () =>
                                {
                                    Application.LoadLevel("GameLogin");
                                });
                            }
                        }
                    }
                }
                else
                {
                    if (UICommonDialog.ins != null)
                    {
                        UICommonDialog.ins.ShowOK("网络,请求失败!请稍后重试", () =>
                        {
                            Application.LoadLevel("GameLogin");
                        });
                    }
                }
            });
        }
#else
        //地图 人工审核
        void _ReqOneGallery()
        {

            if (UIPanelGalleryMenu.uuid_override == 0)
            {
                RpcClient.ins.SendRequest("map", "map_info_gallery_one1", "uuid:88,", (RpcRespone ss) =>
                {
                    if (self == null) return;
                    if (ss != null && ss.ok)
                    {
                        var kv = Json.Decode(ss.protocol.json);

                        Debug.LogError(ss.protocol.json);
                        if (kv == null)
                        {
                            if (UICommonDialog.ins != null)
                            {
                                UICommonDialog.ins.ShowOKCancle("网络,请求失败!请稍后重试", () =>
                                {
                                    Application.LoadLevel("GameLogin");
                                }, () =>
                                {
                                    Application.LoadLevel("GameLogin");
                                });
                            }
                        }
                        else
                        {
                            if (kv.Get("msg") == "none")
                            {
                                if (UICommonDialog.ins != null)
                                {
                                    UICommonDialog.ins.ShowOKCancle("当前暂无地图!", () =>
                                    {
                                        Application.LoadLevel("MapEditorEntry");
                                    }, () =>
                                    {
                                        Application.LoadLevel("MapEditorEntry");
                                    });
                                }
                            }
                            else if (kv.Get("ret") == "ok")
                            {
                                if (UIPanelGalleryMenu.ins != null)
                                {
                                    UIPanelGalleryMenu.ins.Sync(kv.GetLong("like"));
                                }
                                MapGalleryRunner.kv_cache = kv;
                                Debug.Log(ss.protocol.json);
                                StartCoroutine(Download(""));


                                string str = "地图uuid:" + kv.Get("uuid");
                                str += "  鲜花数:" + kv.Get("like");
                                str += "  鸡蛋数:" + kv.Get("hate");
                                str += "  权重:" + kv.Get("weight");
                                str += "  退出数:" + kv.Get("quit");
                                str += "  曝光量:" + kv.Get("use");
                                str += "  创建者:" + kv.Get("creator");
                                Debug.LogError(str);

                            }
                            else
                            {
                                if (UICommonDialog.ins != null)
                                {
                                    UICommonDialog.ins.ShowOKCancle("网络,请求失败!请稍后重试", () =>
                                    {
                                        Application.LoadLevel("GameLogin");
                                    }, () =>
                                    {
                                        Application.LoadLevel("GameLogin");
                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        if (UICommonDialog.ins != null)
                        {
                            UICommonDialog.ins.ShowOKCancle("网络,请求失败!请稍后重试", () =>
                            {
                                Application.LoadLevel("GameLogin");
                            }, () =>
                            {
                                Application.LoadLevel("GameLogin");
                            });
                        }
                    }
                });
            }
            else
            {
                //req target

                var uuid = UIPanelGalleryMenu.uuid_override;
                UIPanelGalleryMenu.uuid_override = 0;
                RpcClient.ins.SendRequest("map", "map_info_gallery_one_detail", "uuid:" + uuid.ToString() + ",", (RpcRespone ss) =>
                {
                    if (self == null) return;
                    if (ss != null && ss.ok)
                    {
                        var kv = Json.Decode(ss.protocol.json);

                        Debug.LogError(ss.protocol.json);

                        if (kv == null)
                        {
                            if (UICommonDialog.ins != null)
                            {
                                UICommonDialog.ins.ShowOK("网络,请求失败!请稍后重试", () =>
                                {

                                });
                            }
                        }
                        else
                        {
                            if (kv.Get("msg") == "none")
                            {
                                if (UICommonDialog.ins != null)
                                {
                                    UICommonDialog.ins.ShowOK("当前暂无地图!", () =>
                                    {

                                    });
                                }
                            }
                            else if (kv.Get("ret") == "ok")
                            {
                                if (UIPanelGalleryMenu.ins != null)
                                {
                                    UIPanelGalleryMenu.ins.Sync(kv.GetLong("like"));
                                }
                                MapGalleryRunner.kv_cache = kv;
                                Debug.Log(ss.protocol.json);
                                StartCoroutine(Download(""));

                                string str = "地图uuid:" + kv.Get("uuid");
                                str += "  鲜花数:" + kv.Get("like");
                                str += "  鸡蛋数:" + kv.Get("hate");
                                str += "  权重:" + kv.Get("weight");
                                str += "  退出数:" + kv.Get("quit");
                                str += "  曝光量:" + kv.Get("use");
                                str += "  创建者:" + kv.Get("creator");
                                Debug.LogError(str);

                            }
                            else
                            {
                                if (UICommonDialog.ins != null)
                                {
                                    UICommonDialog.ins.ShowOK("网络,请求失败!请稍后重试", () =>
                                    {

                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        if (UICommonDialog.ins != null)
                        {
                            UICommonDialog.ins.ShowOK("网络,请求失败!请稍后重试", () =>
                            {

                            });
                        }
                    }
                });
            }
        }

#endif
        public override void OnStart(GameMgr mgr)
        {
            if (just_reload == false)
            {
                if (!BattleServer.IsNetwork)
                {
                    //init base info
                    //   mgr.latestLoadedMap = GameObject.FindObjectOfType<MapInfo>();
                    this.StopAllCoroutines();
                    //  this.StartCoroutine(StartWithIsNetworkFalse());
                    _ReqOneGallery();
                }
                this.mgr = mgr;
            }
            else
            {
                if (!BattleServer.IsNetwork)
                {
                    //init base info

                    if (UIPanelGalleryMenu.ins != null)
                    {
                        UIPanelGalleryMenu.ins.Sync(kv_cache.GetLong("like"));
                    }
                  //  mgr.latestLoadedMap = GameObject.FindObjectOfType<MapInfo>();
                    this.StopAllCoroutines();
                    //  this.StartCoroutine(StartWithIsNetworkFalse());
                    StartCoroutine(Download(""));
                }
                this.mgr = mgr;
            }
        }

        public static string json = "";
        IEnumerator Download(string name)
        {
            string json = string.Empty;
#if UNITY_EDITOR
            yield return MapHttpTask.Download(kv_cache.Get("uuid"), (string json1) =>
            {
                json = json1;
            }, () => { }, true);
#else
     yield return MapHttpTask.Download(kv_cache.Get("uuid"), (string json1) =>
            {
                json = json1;
            });
#endif
            if (!string.IsNullOrEmpty(json))
            {
                yield return Application.LoadLevelAdditiveAsync("MapGalleryRuntime");
             //   mgr.latestLoadedMap = GameObject.FindObjectOfType<MapInfo>();
                var run = GameObject.FindObjectOfType<MapGalleryRuntimeRunner>();
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
            Application.LoadLevel("MapGallery");

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