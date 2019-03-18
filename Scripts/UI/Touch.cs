/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MapEditor
{
    public class Touch : MonoBehaviour
    {
        public LayerMask mask;
        UIRoot root;
        UIPanelUp _panel_up;

        private bool isTouchDown = false;
        private Bounds boundingBox;

        const int MAX_NUM = 9999;

        void Awake()
        {
            root = this.GetComponent<UIRoot>();
            _panel_up = this.GetComponentInChildren<UIPanelUp>();
        }
        void Start()
        {

        }

#if (UNITY_ANDROID || UNITY_IOS|| UNITY_IPHONE ) && !UNITY_EDITOR
        public bool GetTouchDown()
        {
            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)return true;
            }
            return false;
        }
        public bool GetTouchUp()
        {
            if (Input.touchCount >0)
            {
                var touch = Input.GetTouch(0);
                if (  touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved) return false;
            }
            return true;
        }
        public Vector3 GetTouchPosition()
        {
            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);
                return touch.position;
            }
            return Vector3.zero;
        }
#else
        public bool GetTouchDown()
        {
            return Input.GetMouseButtonDown(0);
        }
        public bool GetTouchUp()
        {
            return Input.GetMouseButtonUp(0);
        }
        public Vector3 GetTouchPosition()
        {
            return Input.mousePosition;
        }
#endif
#if UNITY_EDITOR
        //编辑器下允许连续 增加物件
        void UpdateWithAdded()
        {
            if (!IsTouchUI._IsTouchUI && this.GetTouchDown())
            {
                var ray = Camera.main.ScreenPointToRay(this.GetTouchPosition());
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 1000f, mask))
                {// hit somethig
                    if (CurrentSelectObject != null)
                    {
                        CurrentSelectObject.GetComponentFully<MapObjectBase>().SetBright(false);
                    }
                    if (hit.collider.gameObject.GetComponentFully<MapObjectBase>() != null)
                    {
                        CurrentSelectObject = hit.collider.gameObject.GetComponentFully<MapObjectBase>().gameObject;
                    }
                    /* if (hit.collider.gameObject.GetComponentFully<MapObjectDecorate>() != null)
                     {
                         CurrentSelectObject = hit.collider.gameObject;
                     }*/

                    _position_delta = hit.collider.gameObject.GetComponentFully<MapObjectBase>().gameObject.transform.position - Camera.main.ScreenToWorldPoint(this.GetTouchPosition());
                    pos_begin_touch = hit.collider.gameObject.GetComponentFully<MapObjectBase>().gameObject.transform.position;
                    // Debug.LogError("hit " + hit.collider.gameObject.name + "    " + CurrentSelectObject.gameObject.name);

                    this.CurrentSelectWeapon = CurrentSelectObject.GetComponent<MapObjectWeaponSpawnPoint>();
                    if (this.CurrentSelectWeapon != null)
                    {//sync view
                        root._panel_up.Sync(this.CurrentSelectWeapon);
                        foreach (var p in CurrentSelectObject.transform.parent.GetComponentsInChildren<MapObjectWeaponSpawnPoint>())
                        {
                            p.transform.GetChild(0).gameObject.SetActive(false);
                        }
                        CurrentSelectWeapon.transform.GetChild(0).gameObject.SetActive(true);
                        MapObjectRoot.ins.CheckAllConflict();
                        if (UITips.ins != null)
                        {
                            int order = CurrentSelectWeapon.GetComponent<MapObjectWeaponSpawnPoint>().Order;
                            if (order > 0)
                            {
                                UITips.ins.ShowTips("您正在设置武器" + order + "出生点枪支，请点击顶部枪械设置。");
                            }
                            else
                            {
                                UITips.ins.ShowTips("您正在设置武器出生点枪支，请点击顶部枪械设置。");
                            }
                        }
                    }
                }
                else
                {
                    this.CurrentSelectWeapon = null;// if hit nothing  will cancel weapon spawn point select status
                    if (CurrentSelectObject != null)
                    {
                        CurrentSelectObject.GetComponent<MapObjectBase>().SetBright(false);
                    }
                    // not hit some thind  -   if select  then added
                    if (_panel_up.currentSelect != null)
                    {
                        if (MapObjectRoot.ins.TotalMapObjectCount > MAX_NUM)
                        {
                            if (UICommonDialog.ins != null)
                            {
                                UICommonDialog.ins.ShowOK("数量达到上限");
                            }
                        }
                        else
                        {
                            //   if (root.CurrentStep == MapEditorStep.MapObject)
                            {
                                ///////////////////////////////////////////////
                                //点击空地时创建
                                var obj = MapObjectRoot.ins.CreateObject(_panel_up.currentSelect.id.ToString(), LayerMgr.ins.GetCurLayerTransform());
                                var pos = Camera.main.ScreenToWorldPoint(this.GetTouchPosition());
                                pos.x = 0f;
                                //      Debug.LogError(pos);
                                obj.transform.position = pos;
                                CurrentSelectObject = obj;
                                //计算包围盒
                                boundingBox = CurrentSelectObject.CalculateBounds();
                                _position_delta = Vector3.zero;
                                // create one will cancel selected 
                                root._panel_up.ClearSelected();
                                root._panel_left.ClearSelected();

                                return;
                            }
                        }
                    }
                }
            }
            var tmpcurrent = CurrentSelectObject;

            if (this.GetTouchUp())
            {
                this.CurrentSelectWeapon = null;// if hit nothing  will cancel weapon spawn point select status
                if (CurrentSelectObject != null && (CurrentSelectObject.GetComponent<MapObject>() != null || CurrentSelectObject.GetComponent<MapObjectDecorate>() != null))
                {
                    if (MapObjectRoot.ins.CheckConflict(CurrentSelectObject))
                    {
                        CurrentSelectObject.transform.position = pos_begin_touch;
                    }
                    CurrentSelectObject.GetComponent<MapObjectBase>().SetBright(false);
                    MapObjectRoot.ins.CheckAllConflict();

                    //编辑器模式下 允许连续点击物件  鼠标右键取消
                    var obj = MapObjectRoot.ins.CreateObject(CurrentSelectObject.name, LayerMgr.ins.GetCurLayerTransform());
                    var pos = Camera.main.ScreenToWorldPoint(this.GetTouchPosition());
                    pos.x = 0f;
                    //      Debug.LogError(pos);
                    obj.transform.position = pos;
                    CurrentSelectObject = obj;
                    _position_delta = Vector3.zero;
                    // create one will cancel selected 
                    root._panel_up.ClearSelected();
                    root._panel_left.ClearSelected();

                    return;
                }
                else
                {
                    if (CurrentSelectObject != null)
                    {
                        if (MapObjectRoot.ins.CheckConflict(CurrentSelectObject))
                        {
                            CurrentSelectObject.transform.position = pos_begin_touch;
                        }
                        CurrentSelectObject.GetComponent<MapObjectBase>().SetBright(false);
                        MapObjectRoot.ins.CheckAllConflict();
                    }
                    CurrentSelectObject = null;
                }
            }
            if (Input.GetMouseButtonDown(1) && CurrentSelectObject != null && (CurrentSelectObject.GetComponent<MapObject>() != null || CurrentSelectObject.GetComponent<MapObjectDecorate>() != null))
            {
                MapObjectRoot.ins.DestroyObjectImmediate(CurrentSelectObject);
                CurrentSelectObject = null;

                root.touchBehaviour = TouchBehaviour.Select;
                return;
            }
            if (CurrentSelectObject != null)
            {
                var pos_pre = CurrentSelectObject.transform.position;

                var pos = Camera.main.ScreenToWorldPoint(this.GetTouchPosition()) + _position_delta;
                pos.x = 0f;
                CurrentSelectObject.transform.position = pos;
                if (MapObjectRoot.ins.CheckConflict(CurrentSelectObject))
                {
                    MapObjectRoot.ins.SetBrightAll(true);
                    CurrentSelectObject.GetComponent<MapObjectBase>().SetBright(true);
                }
                else
                {
                    MapObjectRoot.ins.SetBrightAll(false);
                }
                //move 
                if (CurrentSelectObject.transform.hasChanged)
                {
                    //   MapObjectRoot.ins.CheckAllConflict();
                    CurrentSelectObject.transform.hasChanged = false;
                }
            }
            // CurrentSelectObject = tmpcurrent;
            if (Input.GetMouseButtonDown(1))
            {
                //   CurrentSelectObject = null;

            }
           
            //拖动连续生成
            if (isTouchDown && !IsTouchUI._IsTouchUI)
            {
                if (CurrentSelectObject != null)
                {                   
                    if (MapObjectRoot.ins.TotalMapObjectCount > MAX_NUM)
                    {
                        if (UICommonDialog.ins != null)
                        {
                            UICommonDialog.ins.ShowOK("数量达到上限");
                        }
                    }
                    else
                    {                      
                        var pos = Camera.main.ScreenToWorldPoint(this.GetTouchPosition());
                        pos.x = 0f;
                        if (boundingBox != null)
                        {
                            //离上个物体足够距离了
                            if ((Mathf.Abs(boundingBox.center.y-pos.y) >= boundingBox.size.y) || (Mathf.Abs(boundingBox.center.z - pos.z) >= boundingBox.size.z))
                            {
                                var obj = MapObjectRoot.ins.CreateObject(CurrentSelectObject.name, LayerMgr.ins.GetCurLayerTransform());
                                obj.transform.position = pos;

                                CurrentSelectObject = obj;
                                boundingBox.center = pos;

                                return;
                            }
                        }
                                              
                    }
                }
            }


        }
#else
        void UpdateWithAdded()
        {
            if (!IsTouchUI._IsTouchUI && this.GetTouchDown())
            {
                var ray = Camera.main.ScreenPointToRay(this.GetTouchPosition());
#if UNITY_EDITOR
                Debug.DrawLine(ray.origin, ray.GetPoint(100f), Color.red, 10f);
#endif
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 1000f, mask))
                {// hit somethig
                    if (CurrentSelectObject != null)
                    {
                        CurrentSelectObject.GetComponentFully<MapObjectBase>().SetBright(false);
                    }
                    if (hit.collider.gameObject.GetComponentFully<MapObjectBase>() != null)
                    {
                        CurrentSelectObject = hit.collider.gameObject.GetComponentFully<MapObjectBase>().gameObject;
                    }
                    /* if (hit.collider.gameObject.GetComponentFully<MapObjectDecorate>() != null)
                     {
                         CurrentSelectObject = hit.collider.gameObject;
                     }*/

                    _position_delta = hit.collider.gameObject.GetComponentFully<MapObjectBase>().gameObject.transform.position - Camera.main.ScreenToWorldPoint(this.GetTouchPosition());
                    pos_begin_touch = hit.collider.gameObject.GetComponentFully<MapObjectBase>().gameObject.transform.position;
                    // Debug.LogError("hit " + hit.collider.gameObject.name + "    " + CurrentSelectObject.gameObject.name);

                    this.CurrentSelectWeapon = CurrentSelectObject.GetComponent<MapObjectWeaponSpawnPoint>();
                    if (this.CurrentSelectWeapon != null)
                    {//sync view
                        root._panel_up.Sync(this.CurrentSelectWeapon);
                        foreach (var p in CurrentSelectObject.transform.parent.GetComponentsInChildren<MapObjectWeaponSpawnPoint>())
                        {
                            p.transform.GetChild(0).gameObject.SetActive(false);
                        }
                        CurrentSelectWeapon.transform.GetChild(0).gameObject.SetActive(true);
                        MapObjectRoot.ins.CheckAllConflict();
                        if (UITips.ins != null)
                        {
                            int order = CurrentSelectWeapon.GetComponent<MapObjectWeaponSpawnPoint>().Order;
                            if (order > 0)
                            {
                                UITips.ins.ShowTips("您正在设置武器" + order + "出生点枪支，请点击顶部枪械设置。");
                            }
                            else
                            {
                                UITips.ins.ShowTips("您正在设置武器出生点枪支，请点击顶部枪械设置。");
                            }
                        }
                    }
                    else if (CurrentSelectObject != null)//点中的是之前的物件(直接切换到选择模式)
                    {
                        var type = CurrentSelectObject.GetComponent<MapObjectBase>();
                        if ((type as MapObjectSpawnPoint == null) && (type as MapObjectWeaponSpawnPoint == null))
                        {
                            root.touchBehaviour = TouchBehaviour.Select;
                            SelectObject(CurrentSelectObject);
                        }                        
                    }

                }
                else
                {
                    this.CurrentSelectWeapon = null;// if hit nothing  will cancel weapon spawn point select status
                    if (CurrentSelectObject != null)
                    {
                        CurrentSelectObject.GetComponent<MapObjectBase>().SetBright(false);
                    }
                    // not hit some thind  -   if select  then added
                    if (_panel_up.currentSelect != null)
                    {
#if UNITY_EDITOR
                        if (MapObjectRoot.ins.TotalMapObjectCount > MAX_NUM)
#else
                        //#1000170  改为不限制碰撞和增加物件上限为50
                        if (MapObjectRoot.ins.TotalMapObjectCount > DevConfig.MapEditorMaxAllowMapObjectNumber)
#endif

                        {
                            if (UICommonDialog.ins != null)
                            {
                                UICommonDialog.ins.ShowOK("数量达到上限");
                            }
                        }
                        else
                        {
                            //   if (root.CurrentStep == MapEditorStep.MapObject)
                            {
                                var obj = MapObjectRoot.ins.CreateObject(_panel_up.currentSelect.id.ToString());
                                var pos = Camera.main.ScreenToWorldPoint(this.GetTouchPosition());
                                pos.x = 0f;
                                //      Debug.LogError(pos);
                                obj.transform.position = pos;
                                CurrentSelectObject = obj;
                                boundingBox = obj.CalculateBounds();
                                _position_delta = Vector3.zero;
                                // create one will cancel selected 
                                //root._panel_up.ClearSelected();
                                //root._panel_left.ClearSelected();

                                return;
                            }
                        }
                    }
                    else //没生成又没碰到组件
                    {
                        ClearSelectObject();
                    }
                }
            }
            if (this.GetTouchUp())
            {
                this.CurrentSelectWeapon = null;// if hit nothing  will cancel weapon spawn point select status
                //if (CurrentSelectObject != null)
                //{
                //    if (MapObjectRoot.ins.CheckConflict(CurrentSelectObject))
                //    {
                //        CurrentSelectObject.transform.position = pos_begin_touch;
                //    }
                //    CurrentSelectObject.GetComponent<MapObjectBase>().SetBright(false);
                //    MapObjectRoot.ins.CheckAllConflict();
                //}
                CurrentSelectObject = null;
            }

            //当前鼠标控制武器人物出生点移动
            if (CurrentSelectObject != null)
            {
                var type = CurrentSelectObject.GetComponent<MapObjectBase>();
                if ((type as MapObjectSpawnPoint != null) || (type as MapObjectWeaponSpawnPoint != null))
                {
                    var pos_pre = CurrentSelectObject.transform.position;

                    var pos = Camera.main.ScreenToWorldPoint(this.GetTouchPosition()) + _position_delta;
                    pos.x = 0f;
                    CurrentSelectObject.transform.position = pos;
                    if (MapObjectRoot.ins.CheckConflict(CurrentSelectObject))
                    {
                        MapObjectRoot.ins.SetBrightAll(true);
                        CurrentSelectObject.GetComponent<MapObjectBase>().SetBright(true);
                    }
                    else
                    {
                        MapObjectRoot.ins.SetBrightAll(false);
                    }
                    //move 
                    if (CurrentSelectObject.transform.hasChanged)
                    {
                        //   MapObjectRoot.ins.CheckAllConflict();
                        CurrentSelectObject.transform.hasChanged = false;
                    }
                }
            }

            //拖动连续生成
            if (Input.touchCount > 0)
            {
                UnityEngine.Touch th = Input.GetTouch(0);
                if ((th.phase == TouchPhase.Moved) && !IsTouchUI._IsTouchUI)
                {
                    if (CurrentSelectObject != null)
                    {

                        if (MapObjectRoot.ins.TotalMapObjectCount > MAX_NUM)
                        {
                            if (UICommonDialog.ins != null)
                            {
                                UICommonDialog.ins.ShowOK("数量达到上限");
                            }
                        }
                        else
                        {
                            var pos = Camera.main.ScreenToWorldPoint(this.GetTouchPosition());
                            pos.x = 0f;
                            if (boundingBox != null)
                            {
                                //离上个物体足够距离了
                                if ((Mathf.Abs(boundingBox.center.y - pos.y) >= boundingBox.size.y) || (Mathf.Abs(boundingBox.center.z - pos.z) >= boundingBox.size.z))
                                {
                                    var obj = MapObjectRoot.ins.CreateObject(CurrentSelectObject.name, LayerMgr.ins.GetCurLayerTransform());
                                    obj.transform.position = pos;

                                    CurrentSelectObject = obj;
                                    boundingBox.center = pos;

                                    return;
                                }
                            }

                        }
                    }
                }
            }

        }
#endif
        Vector3 pos_begin_touch;
        void UpdateWithDelete()
        {
            if (this.GetTouchDown() && !IsTouchUI._IsTouchUI)
            {
                var ray = Camera.main.ScreenPointToRay(this.GetTouchPosition());
#if UNITY_EDITOR
                Debug.DrawLine(ray.origin, ray.GetPoint(100f), Color.red, 10f);
#endif
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 1000f, mask))
                {// hit somethig
                    //hit map object
                    var obj = hit.collider.gameObject.GetComponentFully<MapObjectBase>();
                    if (obj != null)
                    {
                        CurrentSelectObject = obj.gameObject;
                    }
                    //hit map dec
                    //  if (hit.collider.gameObject.GetComponentFully<MapObjectDecorate>() != null)
                    {
                        //  CurrentSelectObject = hit.collider.gameObject;
                    }
                    //     Debug.LogError("hit " + hit.collider.gameObject.name);
                    if (CurrentSelectObject != null)
                    {
                        this.CurrentSelectWeapon = CurrentSelectObject.GetComponentFully<MapObjectWeaponSpawnPoint>();
                    }
                }
                else
                {
                    this.CurrentSelectWeapon = null;// if hit nothing  will cancel weapon spawn point select status
                    if (_panel_up.currentSelect != null)
                    {
                        var obj = MapObjectRoot.ins.CreateObject(_panel_up.currentSelect.id.ToString(), LayerMgr.ins.GetCurLayerTransform());
                        var pos = Camera.main.ScreenToWorldPoint(this.GetTouchPosition());
                        pos.x = 0f;
                        //      Debug.LogError(pos);
                        obj.transform.position = pos;
                        CurrentSelectObject = obj;
                        // create one will cancel selected 
                        root._panel_up.ClearSelected();
                        root._panel_left.ClearSelected();
                    }
                }
#if UNITY_EDITOR
                //编辑器模式，不需要取消删除按钮的点击
#else
                //root._panel_up.OnBtnDeleteClick();
#endif
            }
            if (CurrentSelectObject != null)
            {
                var type = CurrentSelectObject.GetComponent<MapObjectBase>();
                if ((type as MapObjectSpawnPoint == null) && (type as MapObjectWeaponSpawnPoint == null))
                {// hit spasm will ignore
                    // GameObject.DestroyImmediate(CurrentSelectObject);
                    // MapObjectRoot.ins.DestroyObjectImmediate(CurrentSelectObject);
                    EditorRuntime.Delete(CurrentSelectObject);

                }
                MapObjectRoot.ins.CheckAllConflict();
                CurrentSelectObject = null;
                //cancle
                //  root._panel_up.OnBtnDeleteClick();
                return;

            }
            CurrentSelectObject = null;
            ClearSelectObject();

            //拖动连续删除
            if (isTouchDown && !IsTouchUI._IsTouchUI)
            {
                var ray = Camera.main.ScreenPointToRay(this.GetTouchPosition());

                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 1000f, mask))
                {
                    var obj = hit.collider.gameObject.GetComponentFully<MapObjectBase>();
                    if (obj != null)
                    {
                        CurrentSelectObject = obj.gameObject;
                        if (CurrentSelectObject != null)
                        {
                            var type = CurrentSelectObject.GetComponent<MapObjectBase>();
                            if ((type as MapObjectSpawnPoint == null) && (type as MapObjectWeaponSpawnPoint == null))
                            {
                                EditorRuntime.Delete(CurrentSelectObject);
                            }
                            MapObjectRoot.ins.CheckAllConflict();
                            CurrentSelectObject = null;
                            //cancle
                            //  root._panel_up.OnBtnDeleteClick();
                            return;

                        }
                    }
                }
            }
        }

        void UpdateWithSelected()
        {
            if (this.GetTouchDown() && !IsTouchUI._IsTouchUI)
            {
                var ray = Camera.main.ScreenPointToRay(this.GetTouchPosition());
#if UNITY_EDITOR
                Debug.DrawLine(ray.origin, ray.GetPoint(100f), Color.red, 10f);
#endif
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 1000f, mask))
                {
                    var obj = hit.collider.gameObject.GetComponentFully<MapObjectBase>();
                    if (obj != null)
                    {
                        var type = obj.gameObject.GetComponent<MapObjectBase>();
                        if ((type as MapObjectSpawnPoint == null) && (type as MapObjectWeaponSpawnPoint == null))
                        {
                            SelectObject(obj.gameObject);
                        }
                        else
                        {
                            ClearSelectObject();
                        }
                    }
                    else
                    {
                        ClearSelectObject();
                    }
                }
                else
                {
                    ClearSelectObject();
                }
            }
        }

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="obj"></param>
        public void SelectObject(GameObject obj)
        {
            if (obj != null)
            {
                CurrentSelectObject = obj;
                UIPanelProperty.ins.Show();
                EditorSelection.activeObject = CurrentSelectObject;
                UIPanelOperation.ins.InitData(CurrentSelectObject.transform);
            }
        }

        /// <summary>
        ///  取消选择
        /// </summary>
        public void ClearSelectObject()
        {
            CurrentSelectObject = null;
            UIPanelProperty.ins.Hide();
            EditorSelection.activeObject = null;
            UIPanelOperation.ins.Hide();
        }



        bool IsTouchPositionHasObject()
        {
            var ray = Camera.main.ScreenPointToRay(this.GetTouchPosition());
#if UNITY_EDITOR
            Debug.DrawLine(ray.origin, ray.GetPoint(100f), Color.red, 10f);
#endif
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000f, mask))
            {
                var obj = hit.collider.gameObject.GetComponentFully<MapObjectBase>();
                if (obj != null)
                {
                    return true;
                }
            }

            return false;
        }

        void Update()
        {
            isTouchDown = false;
#if (UNITY_ANDROID || UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR
            if (Input.touchCount >0)
            {
                var touch = Input.GetTouch(0);
                if (  touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved)
                {
                    isTouchDown = true;
                }
            }
#else
            if (Input.GetMouseButtonDown(0))
            {
                isTouchDown = true;
            }

    #endif

            if (root.touchBehaviour == TouchBehaviour.Added)
            {
                this.UpdateWithAdded();
            }
            else if (root.touchBehaviour == TouchBehaviour.Deleted)
            {
                this.UpdateWithDelete();
            }
            else if (root.touchBehaviour == TouchBehaviour.Select)
            {
                this.UpdateWithSelected();
            }
        }
        public GameObject CurrentSelectObject = null;
        //{ set { EditorSelection.activeGameObject = CurrentSelectObject; }
        //  get { return EditorSelection.activeGameObject; }
        //}
        private MapObjectWeaponSpawnPoint CurrentSelectWeapon = null; // if current select is weapon spaen point  , use to update  UIPanelUp 's view ui
        Vector3 _position_delta;
    }
}