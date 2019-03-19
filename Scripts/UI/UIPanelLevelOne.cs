using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace MapEditor
{
    public class UIPanelLevelOne : MonoBehaviour
    {
        public GameObject img_selected;

        public static UIPanelLevelOne ins = null;
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
        public void LoadListView()
        {
            img_selected.transform.SetParent(transform, true);

            img_selected.transform.position = Vector3.one * 100000f;
            foreach (var p in _child)
            {
                GameObject.Destroy(p.gameObject);
            }
            _child.Clear();

            if (true)
            {
                var list = GameConfig.instance.GetComponentGroupInfo();
                foreach (var id in list)
                {
                    var obj = MapLoader.ins.LoadEditorV1("OneMapObjectLevelOne");

                    if (obj == null) break;
                    obj = GameObject.Instantiate<GameObject>(obj);

                    obj.transform.SetParent(_obj_content.transform, false);
                    bool ok = obj.GetComponentFully<MapObjectLevelOne>().InitData(id);
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

        public void Clear()
        {
            foreach (var p in _child)
            {
                GameObject.Destroy(p);
            }
            _child.Clear();
        }

        public void OnCellClick(MapObjectLevelOne who)
        {
            if (who == null) return;
            if (who == currentSelect)
            {
                currentSelect = null;
                img_selected.transform.SetParent(transform, true);

                img_selected.transform.position = Vector3.one * 100000f;
                return;
            }
            currentSelect = who;

            img_selected.transform.SetParent(currentSelect.transform, true);
            img_selected.transform.localPosition = Vector3.zero;


            root._panel_level_two.LoadListView(currentSelect.id);
        }

        public MapObjectLevelOne currentSelect = null;
    }
}