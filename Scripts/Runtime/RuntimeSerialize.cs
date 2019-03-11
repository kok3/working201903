/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MapEditor
{//运行模式下 顶级控制脚本
    public class RuntimeSerialize : MonoBehaviour
    {
        public Serializable.Map CurrentSerializeMap = null;
        bool loaded = false;
        //info 是因为要传递 额外的数据 到MapInfo里面
        public void Load(long customs = 0)
        {
            if (loaded) return;
            loaded = true;
            string file = "";
            if (customs > 0)
            {
                file = customs.ToString() + ".json";
            }
            else
            {
                Debug.LogError("can not find curstom id=" + customs);
            }
            var str = Serializable.LoadFromFile(file);
            Serializable.Map map = null;
            try
            {
                map = Serializable.ToObject<Serializable.Map>(str);
            }
            catch (System.Exception e)
            {

            }
            if (map == null)
            {
                //TODO BUGLY 出现了  https://bugly.qq.com/v2/crash-reporting/errors/f4a2f0d471/17667?pid=1
#if UNITY_EDITOR
                Debug.LogError(" de-serialize map error  " + customs);
#endif
                return;
            }
            var serializes = this.transform.GetComponentsInChildren<EditorSerializeBase>();
            foreach (var p in serializes)
            {
                p.DeSerializeObject(map);
            }
            this.CurrentSerializeMap = map;

            LoadBackGround(map.theme);

#if UNITY_EDITOR
            //       Debug.Log(str);
#endif
        }

        public void LoadFromJson(string json)
        {
            if (loaded) return;
            loaded = true;
#if UNITY_EDITOR
            Debug.Log(json);
#endif
            Serializable.Map map = Serializable.ToObject<Serializable.Map>(json);

            var serializes = this.transform.GetComponentsInChildren<EditorSerializeBase>();
            foreach (var p in serializes)
            {
                p.DeSerializeObject(map);
            }
            this.CurrentSerializeMap = map;
            LoadBackGround(map.theme);
        }
        void Start()
        {
            //             if (loaded) return;
            //             this.Load();
            //             loaded = true;
        }

        private void LoadBackGround(int theme)
        {
            var back = this.GetComponentInChildren<BackGround>();
            if (back != null)
            {
                var spp = back.GetComponent<SpriteRenderer>();

                var tex = MapLoader.ins.LoadEdotorImageThemeBgV1(theme); ;// (Texture2D)PrefabsMgr.Load<Object>("Map/Image/theme_bg/" + map.theme.ToString());
                if (tex == null)
                {
                    Debug.Assert(false);

                }
                var sp = Sprite.Create(tex, new Rect(new Vector2(0, 0), new Vector2(tex.width, tex.height)), new Vector2(0.5f, 0.5f));
                spp.sprite = sp;

                var bgEffectTransform = back.transform.Find("BgEffect");
                if (bgEffectTransform != null)
                {
                    GameObject.Destroy(bgEffectTransform.gameObject);
                }
                var needBgEffect = MapEditorConfig.GetNeedBgEffect(theme);
                if (needBgEffect)
                {
                    back.SetDefaultBgEffect(false);
                    var effectParent = back.transform;
                    var bgEffectPrefab = MapLoader.ins.LoadBgEffectV1(theme);
                    if (bgEffectPrefab != null)
                    {
                        var bgEffect = GameObject.Instantiate<GameObject>(bgEffectPrefab, effectParent);
                        bgEffect.name = "BgEffect";
                        bgEffect.transform.localPosition = Vector3.zero;
                    }
                }
                else
                {
                    back.SetDefaultBgEffect(true);
                }
            }
        }
    }
}