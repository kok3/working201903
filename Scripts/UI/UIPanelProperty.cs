using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MapEditor
{
    public class UIPanelProperty : MonoBehaviour, IBeginDragHandler, IDragHandler
    {
        public static UIPanelProperty ins = null;

        private RectTransform curRecTran;
        private Vector3 offsetPos = Vector3.zero;

        public void Show()
        {
            transform.gameObject.SetActive(true);
        }

        public void Hide()
        {
            transform.gameObject.SetActive(false);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Vector3 globalMousePos;

            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(curRecTran, eventData.position, eventData.pressEventCamera, out globalMousePos))
            {
                offsetPos = curRecTran.position - globalMousePos;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector3 globalMousePos;

            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(curRecTran, eventData.position, eventData.pressEventCamera, out globalMousePos))
            {
                curRecTran.position = globalMousePos + offsetPos;
            }
        }

        void Awake()
        {
            ins = this;

            curRecTran = transform.GetComponent<RectTransform>();

            Hide();
        }

    }
}