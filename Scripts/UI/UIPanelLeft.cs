/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MapEditor
{
    public class UIPanelLeft : MonoBehaviour
    {

        public static UIPanelLeft ins = null;
        private GameObject _obj_content;

        UIRoot root;
        void Awake()
        {
            ins = this;
            this.root = this.GetComponentInParent<UIRoot>();
        }
        void Start()
        {
            _obj_content = transform.Find("ListLeft/Viewport/ListContent").gameObject;
            this.LoadListView();
        }

        public void Clear()
        {
            foreach (var p in _child)
            {
                GameObject.Destroy(p);
            }
            _child.Clear();
        }


        public void LoadListView()
        {
            foreach (var p in _child)
            {
                GameObject.Destroy(p.gameObject);
            }
            _child.Clear();
            var list = MapEditorConfig.GetMapObjectD(UISelectTheme.CurrentSelectedTheme);
            //加载装饰物
            foreach (var p in list)
            {
                var obj = MapLoader.ins.LoadEditorV1("OneMapObject");

                //    PrefabsMgr.Load("Map/Prefabs/Editor/OneMapObject");
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
            this.ResizeContent(_child.Count);
        }
        void ResizeContent(int num_of_one)
        {
            var trans = _obj_content.GetComponent<RectTransform>();
            var size = trans.sizeDelta;

            size.y = 100 * num_of_one + 20;//left pading is 20 
            trans.sizeDelta = size;
        }
        public void OnCellClick(OneMapObjectBase who)
        {
            if (currentSelect != null)
            {
                currentSelect.SetBright(false);
            }
            currentSelect = who;
            if (currentSelect != null)
            {
                currentSelect.SetBright(true);

                UIRoot.ins.touchBehaviour = TouchBehaviour.Added;
            }
        }
        public void ClearSelected()
        {
            this.OnCellClick(currentSelect);
        }

        List<GameObject> _child = new List<GameObject>();
        public OneMapObjectBase currentSelect = null;
    }
}