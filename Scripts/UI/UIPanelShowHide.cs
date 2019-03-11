/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MapEditor
{
    public class UIPanelShowHide : MonoBehaviour
    {
        UIRoot root;

        public GameObject _obj_up_hide;
        public GameObject _obj_up_show;
        public GameObject _obj_left_show;
        public GameObject _obj_left_hide;
        void Awake()
        {
            this.root = this.GetComponentInParent<UIRoot>();
        }
        void Start()
        {
            this.OnClickShowPanelLeft();
            this.OnClickShowPanelUp();
        }
        // bind events in unity inspector
        public void OnClickShowPanelUp()
        {
            root.SetPanelUpVisible(true);
            _obj_up_hide.SetActive(true);
            _obj_up_show.SetActive(false);
        }
        public void OnClickHidePanelUp()
        {
            root.SetPanelUpVisible(false);
            _obj_up_hide.SetActive(false);
            _obj_up_show.SetActive(true);
        }
        public void OnClickShowPanelLeft()
        {
            root.SetPanelLeftVisible(true);
            _obj_left_hide.SetActive(true);
            _obj_left_show.SetActive(false);
        }
        public void OnClickHidePanelLeft()
        {
            root.SetPanelLeftVisible(false);
            _obj_left_hide.SetActive(false);
            _obj_left_show.SetActive(true);
        }

        public void OnTestAddLayer()
        {
            LayerMgr.ins.CreateLayer();
        }
    }

}