/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MapEditor
{
    public class UIPanelUp : MonoBehaviour
    {
        public enum LoadType
        {
            MapObject,//物件
            Weapon,//武器
        }

        public GameObject img_add;
        public GameObject img_delete;
        public GameObject img_selected;

        public static UIPanelUp ins = null;
        private GameObject _obj_content;

        UIRoot root;
        void Awake()
        {
            ins = this;
            this.root = this.GetComponentInParent<UIRoot>();
        }
        void Start()
        {
            _obj_content = transform.Find("ListUp/Viewport/ListContent").gameObject;
            this.LoadListView();
        }

        public void ClearSelected()
        {
            this.OnCellClick(currentSelect);
        }
        List<GameObject> _child = new List<GameObject>();
        public void LoadListView(LoadType type = LoadType.MapObject)
        {
            img_selected.transform.SetParent(transform, true);

            img_selected.transform.position = Vector3.one * 100000f;
            foreach (var p in _child)
            {
                GameObject.Destroy(p.gameObject);
            }
            _child.Clear();

            if (type == LoadType.Weapon)
            {
                var list = MapEditorConfig.GetWeapons();
                foreach (var id in list)
                {
                    var obj =
                        MapLoader.ins.LoadEditorV1("OneMapObjectWeapon");

                        //PrefabsMgr.Load("Map/Prefabs/Editor/OneMapObjectWeapon");
                    if (obj == null) break;
                    obj = GameObject.Instantiate<GameObject>(obj);
                    //   Debug.LogError("load id111111111111111111111111111111111111 = " + id);
                    obj.transform.SetParent(_obj_content.transform, false);
                    bool ok = obj.GetComponentFully<OneMapObjectWeapon>().InitData(id);
                    if (!ok)
                    {
                        GameObject.DestroyImmediate(obj);
                        break;
                    }
                    _child.Add(obj);
                }
            }
            else if (type == LoadType.MapObject)
            {

                var list = MapEditorConfig.GetMapObject(MapEditorConfig.CurrentSelectTheme);
                //加载物件
                foreach (var p in list)
                {
                    var obj = MapLoader.ins.LoadEditorV1("OneMapObject");

                        //PrefabsMgr.Load("Map/Prefabs/Editor/OneMapObject");
                    if (obj == null) break;
                    obj = GameObject.Instantiate<GameObject>(obj);
                    obj.transform.SetParent(_obj_content.transform, false);
                    bool ok = obj.GetComponentFully<OneMapObjectBase>().InitData(p);
                    if (!ok)
                    {
                        GameObject.DestroyImmediate(obj);
                        break;
                    }
                    _child.Add(obj);
                }
            }

            this.ResizeContent(_child.Count);
        }
          
        void ResizeContent(int num_of_one)
        {
            var trans = _obj_content.GetComponent<RectTransform>();
            var size = trans.sizeDelta;

            size.x = 95 * num_of_one + 20;//left pading is 20 
            trans.sizeDelta = size;
        }
        public void OnBtnDeleteClick()
        {
            root.OnClickDelete();
            this.ClearSelected();
        }

        public void OnBtnUndo()
        {
            EditorUndo.Undo();
        }

        public void OnBtnRedo()
        {
            EditorUndo.Redo();
        }

        public void Clear()
        {
            foreach(var p in _child)
            {
                GameObject.Destroy(p);
            }
            _child.Clear();
        }
        public void OnBtnAddClick()
        {
            root.OnClickAdd();
        }
        public void OnCellClick(OneMapObjectBase who)
        {
            if (who == null) return;
            if (who == currentSelect)
            {
                currentSelect = null;
                img_selected.transform.SetParent(transform, true);

                img_selected.transform.position = Vector3.one * 100000f;
                return;
            }
            if (currentSelect != null)
            {
                currentSelect.SetBright(false);
            }
            currentSelect = who;
            currentSelect.SetBright(true);
            img_selected.transform.SetParent( currentSelect.transform,true);
            img_selected.transform.localPosition = Vector3.zero;

            //  img_selected.transform.position = currentSelect.transform.position;
        }
        public void OnCellWeaponClick(OneMapObjectWeapon who)
        {
            if (currentWeapon != null && root != null && root.CurrentStep == MapEditorStep.WeaponSpawn)
            {
                currentWeapon.OnClick(who);
            }
            else
            {
                if (UITips.ins != null)
                {
                    UITips.ins.ShowTips("请先选择一个武器出生点!");
                }
            }
        }
        public void Sync(MapObjectWeaponSpawnPoint who)
        {
            if (root != null && root.CurrentStep == MapEditorStep.WeaponSpawn && currentWeapon != who && who != null)
            {
                if (currentWeapon != null)
                {
                    //last 
                    currentWeapon.FillEmpty();
                }
                // this step will sync weapon view if click
                currentWeapon = who;
                //sync
                foreach (var p in _child)
                {
                    var c = p.GetComponentFully<OneMapObjectWeapon>();
                    int idx = 0;

                    c.SetNumber(0);
                }
                foreach (var p in _child)
                {
                    var c = p.GetComponentFully<OneMapObjectWeapon>();
                    int idx = 0;
                    foreach (var id in who._weapon_ids)
                    {
                        if (id != -1 && id == c.id)
                        {
                            c.SetNumber(idx + 1);
                        }
                        idx++;
                    }
                }
            }
        }
        private MapObjectWeaponSpawnPoint currentWeapon = null;
        public OneMapObjectBase currentSelect = null;
    }
}