/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace MapEditor
{
    public class OneMapObjectBase : MonoBehaviour
    {
        public int id = 5;
        protected UIPanelUp _panel_up;

        public virtual bool InitData111(int id)
        {
            this.id = id;
            return true;
        }
        public virtual bool InitData(int id)
        {
            this.id = id;
            var img = this.GetComponent<Image>();
            var tex =// (Texture2D)PrefabsMgr.Load<Object>("Map/Image/" + MapEditor.MapEditorConfig.CurrentSelectTheme + "/" + id.ToString());
                MapLoader.ins.LoadEdotorImageV1(MapEditor.MapEditorConfig.CurrentSelectTheme, id.ToString()); 
            if (tex == null)
            {
                return false;
            }
            var sp = Sprite.Create(tex, new Rect(new Vector2(0, 0), new Vector2(tex.width, tex.height)), new Vector2(0.5f, 0.5f));
            img.sprite = sp;
            return true;
        }
        void Awake()
        {
            _panel_up = UIRoot.ins._panel_up;// this.transform.root.GetComponentInChildren<UIPanelUp>();
            this.OnAwake();
        }
        protected virtual void OnAwake()
        {

        }
        public virtual void SetBright(bool bright)
        {

        }

    }

    /* public class OneMapObject : Image, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public int id = 5;

        void Start()
        {

        }

        public void OnDrag(PointerEventData eventData)
        {
            if (obj != null)
            {
                Vector3 pos = eventData.position;
                pos.z = pos.x;
                transform.GetComponentInParent<Touch>().OnDrag(pos);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            obj = null;
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            obj = MapObjectRoot.ins.CreateObject(id);
            transform.GetComponentInParent<Touch>().CurrentSelectObject = obj;
        }
        GameObject obj;
    }*/
}