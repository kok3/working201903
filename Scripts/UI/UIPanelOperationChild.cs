using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MapEditor
{
    public class UIPanelOperationChild : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {

        public int index;
        private RectTransform curRecTran;

        public void OnBeginDrag(PointerEventData eventData)
        {
            UIPanelOperation.ins.OnBeginChildDrag(index);
        }

        public void OnDrag(PointerEventData eventData)
        {

            Vector3 globalMousePos;

            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(curRecTran, eventData.position, eventData.pressEventCamera, out globalMousePos))
            {
                //curRecTran.position = globalMousePos;
                UIPanelOperation.ins.OnChildDrag(index, globalMousePos);
            }
        }


        public void OnEndDrag(PointerEventData eventData)
        {
            UIPanelOperation.ins.OnEndChildDrag(index);
        }

        void Awake()
        {
            curRecTran = transform.GetComponent<RectTransform>();
        }

    }
}