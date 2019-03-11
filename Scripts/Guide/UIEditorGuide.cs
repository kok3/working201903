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
    public class UIEditorGuide : MonoBehaviour
    {
        public static UIEditorGuide ins = null;
        public Text txt_center;
        public Text txt_tip;

        void Awake()
        {
            ins = this;
        }
        void OnDestroy()
        {
            ins = null;
        }
        void Start()
        {
            this.ShowGuideStep1();
        }


        public void OnClickOk()
        {
            this.SetVisible(false);

        }
        public GameObject hide;

        void SetVisible(bool visible)
        {
            hide.SetActive(visible);
        }
        bool s1 = false;
        bool s2 = false;
        bool s3 = false;

        public void ShowGuideStep1()
        {
            if (s1 || UIRoot.HasPreview) return;
            s1 = true;

            txt_center.text = "教程：顶部横栏为场景物件，带碰撞。左边竖栏为装饰物件，无碰撞。点击选中顶部物件栏中物品再点击场景即可放置物件。右上角为返回按钮（点击后退出编辑关卡）。返回按钮左边为橡皮按钮，选中后点击场景内物件即可删除物件。关卡设计完成后点击下一步进入出生点设置界面。";

            txt_tip.text = "点击屏幕开始关卡设计";

            this.SetVisible(true);

        }
        public void ShowGuideStep2()
        {
            if (s2 || UIRoot.HasPreview) return;
            s2 = true;

            txt_center.text = "教程：你必须设置4个出生点位置，拖动图中1P，2P,3P,4P位置到你想设置的出生点位即可，尽量不要把出生点位设置在比较尴尬的位置哦。设置完成后点击下一步进入枪支设置模式。";

            txt_tip.text = "点击屏幕开始设置出生点";

            this.SetVisible(true);


        }
        public void ShowGuideStep3()
        {
            if (s3 || UIRoot.HasPreview) return;
            s3 = true;

            txt_center.text = "你可以设置4个枪支掉落点，每个枪支掉落点可以设置0-5把武器掉落（武器掉落时间间隔为10秒）；点击武器掉落点后在顶部点击你想要掉落的枪支，按顺序点击1-5把枪即设置完成。武器掉落设置完成后点击下一步进入关卡预览模式，就可以查看您编辑的关卡了哦。";

            txt_tip.text = "点击屏幕开始设置枪支掉落";


            this.SetVisible(true);

        }



    }
}