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
    public class OneMapObjectWeapon : OneMapObjectBase
    {
        public Text txt_num = null;
        public GameObject obj_num;
        protected override void OnAwake()
        {
            this.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (this._panel_up != null)
                {
                    this._panel_up.OnCellWeaponClick(this);
                }
            });
            this.obj_num = this.transform.parent.Find("num").gameObject;
            this.txt_num = obj_num.transform.Find("Text").GetComponent<Text>();

            this.obj_num.SetActive(false);
        }
        //TODO BUGLY #14422 NullReferenceException
        //check why is null
        /*
         
         # main(1)

NullReferenceException

A null value was found where an object instance was required.

1 MapEditor.OneMapObjectWeapon.SetNumber (Int32 num)
2 MapEditor.UIPanelUp.Sync (MapEditor.MapObjectWeaponSpawnPoint who)
3 MapEditor.Touch.UpdateWithAdded ()
         */
        public void SetNumber(int num)
        {
            if (this.obj_num == null || this.txt_num == null) return;

            if (num <= 0)
            {// hide
                this.obj_num.SetActive(false);
            }
            else
            {
                //show
                this.obj_num.SetActive(true);
                txt_num.text = num.ToString();
            }
        }

        public override bool InitData(int id)
        {
            this.id = id;
            var img = this.GetComponent<Image>();
            //  Debug.LogError("load  " + id.ToString());
            var tex = MapLoader.ins.LoadEdotorImageWeaponV1(id.ToString());

            // (Texture2D)PrefabsMgr.Load<Object>("Map/Image/weapon/" + id.ToString());
            if (tex == null)
            {
                Debug.Assert(false);
                return false;
            }
            var sp = Sprite.Create(tex, new Rect(new Vector2(0, 0), new Vector2(tex.width, tex.height)), new Vector2(0.5f, 0.5f));
            img.sprite = sp;
            return true;
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