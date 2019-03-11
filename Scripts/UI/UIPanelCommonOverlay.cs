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
    public class UIPanelCommonOverlay : MonoBehaviour
    {
        UIRoot root;
        public GameObject tips;
        public static UIPanelCommonOverlay ins;
        void Awake()
        {
            this.root = this.GetComponentInParent<UIRoot>();
            ins = this;
        }
        //下一步
        public void OnClickNext()
        {
            root.OnClickNext();
        }
        //上一步
        public void OnClickBack()
        {
            root.OnClickBack();
        }
        //返回
        public void OnClickReturn()
        {
            if (Base.Events.ins != null)
            {
                Base.Events.ins.FireLua("map_editor", "open_settings");
            }
            return;
            //
            if (UICommonDialog.ins != null)
            {
                UICommonDialog.ins.ShowYesNo("是否放弃本次编辑直接返回到主界面？", () =>
                {

                }, () =>
                {
                    Application.LoadLevel("GameLogin");
                }, "继续编辑", "放弃编辑");
            }
        }
        //帮助
        public void OnClickHelp()
        {

        }

        public void SetVisible(bool visible)
        {
            this.gameObject.SetActive(visible);
        }

        public static void ShowTip(string txt)
        {
            if (ins != null && ins.gameObject.activeSelf)
            {
                ins.StartCoroutine(ShowTips(txt));
            }
        }

        private static IEnumerator ShowTips(string txt)
        {
            ins.tips.SetActive(true);
            ins.tips.GetComponentInChildren<Text>().text = txt;
            yield return new WaitForSeconds(1.0f);
            ins.tips.SetActive(false);
        }
    }

}