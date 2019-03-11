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
    public class MapEditorPreviewRunner : GameMgrExtensionBase
    {
        public static MapEditorPreviewRunner ins = null;
        GameMgr mgr = null;
        void Awake()
        {
            ins = this;
        }
        public override void OnStart(GameMgr mgr)
        {
            if (!BattleServer.IsNetwork)
            {
                //init base info

               // mgr.latestLoadedMap = GameObject.FindObjectOfType<MapInfo>();
                this.StopAllCoroutines();
                this.StartCoroutine(StartWithIsNetworkFalse());

                mgr.latestLoadedMapInfo.gameObject.SetActive(true);

                mgr.latestLoadedMapInfo.GetComponent<Maps>().AutoLoadSyncable(mgr);

            }
            this.mgr = mgr;

        }

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
        public override void OnIsNetworkFalseKillPlayerDone()
        {
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
            //    UIRoot.ins.EndPreView();
            //  UIRoot.ins.StartPreView();
            Application.LoadLevel("MapEditorPreview");
            Application.LoadLevelAdditive("MapEditorPreviewRuntime");
        }
        GameObject player = null;
        IEnumerator StartWithIsNetworkFalse()
        {
            yield return new WaitForSecondsRealtime(1.0f);
            if (this.player != null)
            {
                GameObject.DestroyImmediate(this.player);
                this.player = null;
            }
            mgr.GameHasStartForMap = true;
            if (mgr.latestLoadedMapInfo == null)
            {
                //没有任何地图信息 那么需要 
                Application.LoadLevel("GameLogin");
                yield break;
            }
            // play count down
            if (mgr.latestLoadedMapInfo.CurrentSerializeMap != null && mgr.latestLoadedMapInfo.CurrentSerializeMap.MapInfoMode == (int)MapGameMode.Parkour)
                {
                    mgr.EnableParkourCameraFollowCharacter();
                    mgr.LazyCheckEdgeForParkourCameraSlider();

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
            GameObject player1 = null;
            GameObject obj = null;
            if (MiscLoader.ins != null)
            {//修改加载点
                obj = MiscLoader.ins.Load<GameObject>("Game/Character", "Game/Character/Character.prefab");
            }
            else
            {
                player1 = Resources.Load<GameObject>("Prefabs/Game/Character"); ;
                obj = player1;
            }
            obj = GameObject.Instantiate<GameObject>(obj);
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
                info.myLayer = 20;
                info.uuid = StaticData.uuid;
                info.luuid = StaticData.luuid;
                info.characterName = StaticData.name;
                info.teamID = PlayerPrefs.GetInt("teamid");
            }

            //默认颜色是黄色 无需手动修改
            /* if (info != null)
             {
                 foreach (var p in obj.GetComponentsInChildren<SetLinePositions>())
                 {
                     p.SetColor(info.myColor);
                 }
                 obj.GetComponentInChildren<HeadRenderer>().SetColor(info.myColor,info.id_in_room);
                 //   var sync = obj.GetComponent<SyncablePlayer>();
                 //    GameObject.DestroyObject(sync as Component);
                 info.myLayer = 20;
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
             }*/
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