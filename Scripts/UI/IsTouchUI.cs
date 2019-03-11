/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//just for diff  touch is UI or GameObject
public class IsTouchUI : UnityEngine.UI.Graphic, ICancelHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public static bool _IsTouchUI = true;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _IsTouchUI = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _IsTouchUI = true;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        _IsTouchUI = false;
    }
    public void OnEndDrag(PointerEventData data)
    {
        _IsTouchUI = true;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        _IsTouchUI = false;
    }
    public void OnCancel(BaseEventData eventData)
    {
        _IsTouchUI = true;
    }
}