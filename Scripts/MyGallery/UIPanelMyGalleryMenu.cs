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
    public class UIPanelMyGalleryMenu : MonoBehaviour
    {
        public static UIPanelMyGalleryMenu ins = null;
        void Awake()
        {
            ins = this;
            txt_like.text = "";
        }
        void Start()
        {
            last_click_time = Utils.GetTimestampSeconds();
        }
        void OnDestroy()
        {
            ins = null;
        }
        public Text txt_like;
        long like_cache = 0;
        public void Sync(long like)
        {
            txt_like.text = Utils.GetMapInfoLikeShowString(like);
            like_cache = like;
        }


        static long last_click_time = 0;
        public void OnClickNext()
        {
            long curr = Utils.GetTimestampSeconds();
            if (curr - last_click_time > 2)
            {
                last_click_time = curr;
            }
            else
            {
                if (UICommonDialog.ins != null)
                {
                    UICommonDialog.ins.ShowOK("请勿点击过快!");
                }
                return;
            }

            MapMyGalleryRunner.just_reload = false;
            Application.LoadLevel("MapMyGallery");
        }
        public void OnClickReturn()
        {
            Application.LoadLevel("GameLogin");
        }
    }
}



