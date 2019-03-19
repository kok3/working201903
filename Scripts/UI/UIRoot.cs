/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace MapEditor
{
    public enum TouchBehaviour
    {
        Added,//添加物件
        Deleted, // 移除物件
        Select, // 选择物件
    }
    public enum MapEditorStep
    {
        MapObject,//摆放物件
        SpawnPoint,//设置出生点
        WeaponSpawn,//设置武器 信息
    }
    public class StepBase
    {
        public UIRoot ui_root = null;
        public MapObjectRoot obj_root;
        public string error_string = "";
        // next step  下一步
        public virtual bool OnEnter()
        {
            return false;
        }
        //last step 上一步
        public virtual bool OnExit()
        {
            return false;
        }
    }
    public class Step1_MapObject : StepBase
    {
        public override bool OnEnter()
        {
            //
            ui_root.CurrentStep = MapEditorStep.MapObject;
            //  ui_root._panel_show_hide.OnClickShowPanelUp();
            // ui_root._panel_show_hide.OnClickShowPanelLeft();
            obj_root.SetSpawnPointVisible(false);
            ui_root.touchBehaviour = TouchBehaviour.Added;
            ui_root._touch.CurrentSelectObject = null;
            obj_root.SetWeaponSpawnPointVisible(false);
            ui_root._panel_up.currentSelect = null;
            ui_root._panel_up.LoadListView(UIPanelUp.LoadType.MapObject);
            return false;
        }
        public override bool OnExit()
        {
            return false;
        }
    }

    public class Step2_SpawnPoint : StepBase
    {
        public override bool OnEnter()
        {
            if (UIEditorGuide.ins != null)
            {
                UIEditorGuide.ins.ShowGuideStep2();
            }
            ui_root.CurrentStep = MapEditorStep.SpawnPoint;
            //  ui_root._panel_show_hide.OnClickShowPanelUp();
            //  ui_root._panel_show_hide.OnClickShowPanelLeft();
            obj_root.SetSpawnPointVisible(true);
            ui_root._panel_up.currentSelect = null;
            ui_root.touchBehaviour = TouchBehaviour.Added;
            ui_root._touch.CurrentSelectObject = null;
            obj_root.SetWeaponSpawnPointVisible(false);
            ui_root._panel_up.LoadListView(UIPanelUp.LoadType.MapObject);
            return false;
        }
        public override bool OnExit()
        {
            return false;
        }
    }
    public class Step3_WeaponSpawn : StepBase
    {
        public override bool OnEnter()
        {
            if (UIEditorGuide.ins != null)
            {
                UIEditorGuide.ins.ShowGuideStep3();
            }
            ui_root.CurrentStep = MapEditorStep.WeaponSpawn;
            //   ui_root._panel_show_hide.OnClickShowPanelUp();
            //   ui_root._panel_show_hide.OnClickShowPanelLeft();
            obj_root.SetSpawnPointVisible(false);
            ui_root.touchBehaviour = TouchBehaviour.Added;
            ui_root._touch.CurrentSelectObject = null;
            ui_root._panel_up.currentSelect = null;
            obj_root.SetWeaponSpawnPointVisible(true);
            ui_root._panel_up.LoadListView(UIPanelUp.LoadType.Weapon);
            return false;
        }
        public override bool OnExit()
        {
            return false;
        }
    }

    public class UIRoot : MonoBehaviour
    {
        public static UIRoot ins = null;
        // tag for controller behaviour
        [HideInInspector]
        public TouchBehaviour touchBehaviour = TouchBehaviour.Added;
        public GameObject[] obj_hide_when_preview;

        public UIPanelLevelOne _panel_level_one = null;
        public UIPanelLevelTwo _panel_level_two = null;

        public UIPanelUp _panel_up = null;
        public UIPanelLeft _panel_left = null;
        public UIPanelCommonOverlay _panel_common_overlay = null;
        public UIPanelShowHide _panel_show_hide = null;
        public Touch _touch = null;
        List<StepBase> _list_step = new List<StepBase>();
        int CurrentStepIndex = 0;
        public MapEditorStep CurrentStep = MapEditorStep.MapObject;
        public GameObject _camera;
        void Awake()
        {
            ins = this;
            this._panel_left = this.GetComponentInChildren<UIPanelLeft>();
            this._panel_level_one = this.GetComponentInChildren<UIPanelLevelOne>();
            this._panel_level_two = this.GetComponentInChildren<UIPanelLevelTwo>();
            this._panel_up = this.GetComponentInChildren<UIPanelUp>();
            this._panel_common_overlay = this.GetComponentInChildren<UIPanelCommonOverlay>();
            this._panel_show_hide = this.GetComponentInChildren<UIPanelShowHide>();
            this._touch = this.GetComponent<Touch>();
        }
        void OnDestroy()
        {
            ins = null;
        }
        void Start()
        {
            _list_step.Add(new Step1_MapObject
            {
                ui_root = this,
                obj_root = MapObjectRoot.ins
            });

            _list_step.Add(new Step2_SpawnPoint
            {
                ui_root = this,
                obj_root = MapObjectRoot.ins
            });

            _list_step.Add(new Step3_WeaponSpawn
            {
                ui_root = this,
                obj_root = MapObjectRoot.ins
            });

        }

        public void SetPanelUpVisible(bool visible)
        {
            this._panel_up.gameObject.SetActive(visible);

        }
        public void SetPanelLeftVisible(bool visible)
        {
            this._panel_left.gameObject.SetActive(visible);
            this.SyncBtnDeleteStatus();
        }
        //下一步
        public void OnClickNext()
        {
            if (CurrentStepIndex < _list_step.Count - 1)
            {
                _list_step[CurrentStepIndex].OnExit();
                _list_step[CurrentStepIndex + 1].OnEnter();
                CurrentStepIndex++;
            }
            else
            {
                this.StartPreView();
            }
            this.SyncBtnDeleteStatus();
        }
        //上一步
        public void OnClickBack()
        {
            if (CurrentStepIndex > 0)
            {
                _list_step[CurrentStepIndex].OnExit();
                _list_step[CurrentStepIndex - 1].OnEnter();
                CurrentStepIndex--;
            }
            else
            {
                //忽略
              /*  if (UICommonDialog.ins != null)
                {
                    UICommonDialog.ins.ShowYesNo("是否放弃本次编辑直接返回到主界面？", () =>
                    {

                    }, () =>
                    {
                        // 初始化 共享数据
                        MapEditorStroageData.Clear();
                        Application.LoadLevel("GameLogin");
                    }, "继续编辑", "放弃编辑");
                }*/
            }
            this.SyncBtnDeleteStatus();
        }
        public void OnClickDelete()
        {
            if (touchBehaviour == TouchBehaviour.Added)
            {
                touchBehaviour = TouchBehaviour.Deleted;
            }
            else if (touchBehaviour == TouchBehaviour.Deleted)
            {
                touchBehaviour = TouchBehaviour.Added;
            }
            else if (touchBehaviour == TouchBehaviour.Select)
            {
                touchBehaviour = TouchBehaviour.Added;
            }
            this.SyncBtnDeleteStatus();
        }
        public void SyncBtnDeleteStatus()
        {
            _panel_up.img_add.SetActive(false);
            _panel_up.img_delete.SetActive(false);

            if (touchBehaviour == TouchBehaviour.Deleted)
            {
                _panel_up.img_delete.SetActive(true);

            }
            else if (touchBehaviour == TouchBehaviour.Added)
            {
                _panel_up.img_add.SetActive(true);
            }
        }
        public void OnClickAdd()
        {
            touchBehaviour = TouchBehaviour.Added;
        }
        //只要previw 过 该变了都是true  可用于UI状态显示比如 引导提示
        public static bool HasPreview = false;
        public void StartPreView()
        {
            //check current map is valid;
            string error = "";
            bool ok = MapObjectRoot.ins.CanGoStartPreview(out error);
            if (!ok)
            {
                Debug.Log("check for preview faild ：" + error);
                //show error msg
                if (UICommonDialog.ins != null)
                {
                    UICommonDialog.ins.ShowOK(error);
                }

                return;
            }

            // check has any weapon
            ok = false;
            {
                var list = MapObjectRoot.ins._list_weapon_spawn_points;
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].HasAnyWeapon())
                    {
                        ok = true;
                        break;
                    }
                }
            }
            if (!ok)
            {
                if (UICommonDialog.ins != null)
                {
                    UICommonDialog.ins.ShowYesNo("您的武器出生点未配置任何枪支掉落，确定本地图为肉搏战？", () =>
                        {
                            MapObjectRoot.data = new MapEditorStroageData();
                            MapObjectRoot.data.Save(MapObjectRoot.ins);
                            MapObjectRoot.ins.SerializeToJson();
                            HasPreview = true;
                            StartCoroutine(AsyncLoad());
                        }, () =>
                        {


                        });
                    return;
                }
            }
            MapObjectRoot.data = new MapEditorStroageData();
            MapObjectRoot.data.Save(MapObjectRoot.ins);
            MapObjectRoot.ins.SerializeToJson();
            HasPreview = true;
            StartCoroutine(AsyncLoad());


        }
        IEnumerator AsyncLoad()
        {
            MapObjectRoot.ins.SetPreviewColliderEnable(false);
            MapObjectRoot.ins.SetAllGameObjectActive(false);
            foreach (var p in obj_hide_when_preview)
            {
                p.SetActive(false);
            }
            _panel_up.Clear();
            _panel_left.Clear();
            _panel_common_overlay.SetVisible(false);
            Application.LoadLevel("MapEditorPreview");
            Application.LoadLevelAdditive("MapEditorPreviewRuntime");

            this.gameObject.SetActive(false);
            yield return new WaitForEndOfFrame();

        }
        public void EndPreView11111111()
        {
            /*UIRoot.ins.gameObject.SetActive(true);
            MapObjectRoot.ins.SetPreviewColliderEnable(true);
            Application.UnloadLevel("MapEditorPreviewRuntime");
            Application.UnloadLevel("MapEditorPreview");

            MapObjectRoot.ins.SetAllGameObjectActive(true);
            MapObjectRoot.ins.SetSpawnPointVisible(false);
            MapObjectRoot.ins.SetWeaponSpawnPointVisible(true);
            this.StopAllCoroutines();
            foreach (var p in obj_hide_when_preview)
            {
                p.SetActive(true);
            }
            _panel_up.LoadListView(UIPanelUp.LoadType.Weapon);
            _panel_left.LoadListView();
            _panel_common_overlay.SetVisible(true);

            */
            //  StartCoroutine(AsyncUnLoad());
        }
        IEnumerator AsyncUnLoad()
        {
            yield return new WaitForEndOfFrame();

        }
    }
}