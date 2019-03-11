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
    public class OneMapObject : OneMapObjectBase
    {
        void Start()
        {
            this.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (_panel_up != null)
                {
                    _panel_up.OnCellClick(this);
                }
            });
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