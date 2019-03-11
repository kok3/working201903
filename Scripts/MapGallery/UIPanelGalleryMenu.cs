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
    public class UIPanelGalleryMenu : MonoBehaviour
    {
        public static UIPanelGalleryMenu ins = null;
        public static long uuid_override = 0;
        [SerializeField]
        InputField input = null;

        [SerializeField]
        private GameObject PanelInner = null;//金手指
        void Awake()
        {
            ins = this;
            txt_like.text = "";
            last_click_time = Utils.GetTimestampSeconds();
#if UNITY_EDITOR
            PanelInner.SetActive(true);
#else
            GameObject.Destroy(PanelInner);
            input = null;
            PanelInner = null;
#endif
        }
        void OnDestroy()
        {
            ins = null;
        }
        public Text txt_like;
        public GameObject like_part;

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
#if UNITY_EDITOR
            if (long.TryParse(input.text, out uuid_override))
            {

            }
            else
            {
                uuid_override = 0;
            }
            curr += 5;
#else
                uuid_override = 0;
#endif
            if (curr - last_click_time > 2)
            {
                last_click_time = curr;
            }
            else
            {

                UIPopMsg.Show("请勿点击过快!");

                return;
            }
            HasLike = false;
            MapGalleryRunner.kv_cache = null;
            MapGalleryRunner.just_reload = false;

            Application.LoadLevel("MapGallery");
        }
        static bool HasLike = false;

        public void OnClickLike()
        {
            //disable
            return;
            if (MapGalleryRunner.kv_cache != null && HasLike == false)
            {
                if (MapGalleryRunner.kv_cache.Get("ret") == "ok")
                {
                    string uuid = MapGalleryRunner.kv_cache.Get("uuid");
                    if (!string.IsNullOrEmpty(uuid))
                    {
                        RpcClient.ins.SendRequest("rpc", "map_info_like", "uuid:" + uuid + ",", (RpcRespone ss) =>
                        {
                            if (ss != null && ss.ok)
                            {
                                //TODO play animation
                                HasLike = true;
                                Debug.Log(ss.protocol.json);

                                var kv = Json.Decode(ss.protocol.json);
                                if (kv != null && kv.Get("ret") == "ok")
                                {
                                    MapGalleryRunner.kv_cache = kv;
                                    this.Sync(kv.GetLong("like"));

                                    if (UITips.ins != null)
                                    {
                                        // UITips.ins.ShowTips("+1");
                                    }
                                    UIPopMsg.Show("点赞成功");
                                    var part = like_part.GetComponentInChildren<ParticleSystem>();
                                    if (part != null)
                                    {
                                        part.Play();
                                    }
                                }
                            }
                        });
                    }
                }
            }
            if (HasLike)
            {

                UIPopMsg.Show("请勿重复点赞!");

            }
        }
        public void OnClickReturn()
        {
            Application.LoadLevel("GameLogin");

        }
    }
}