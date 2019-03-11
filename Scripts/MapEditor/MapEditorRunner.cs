/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;

/*
 
地图编辑器玩家流程
 1.选择主题
 2.拖拽物体
 3.选择出生点.
 4.选择武器刷新点和武器信息
 
 测试保存提交
 */

/*
 地图编辑器代码流程
 1.新建主题
 2.控制拖拽物体
 3.选择出生点
 4.武器基本信息
 
 5。每一步操作都校验一下合法性处理
 6.玩家地图编辑完成后,可以预览缩略图,可以自己跑一边(考虑新建场景MapPreview独立去MapRuntime跑地图
 7.玩家确认后,流程
 8.请求服务器分配一个地图id(可以是http上传凭证等信息)等信息
 9.拿到上传信息后 HTTP上传,http服务器返回成功后,主动通知服务器我上传成功
 10.等到服务器确认后即可表示成功
 
 
 */

/*
 
地图编辑器 服务器流程

1.客户端请求一个id后 id自增一下，返回
2.客户端二次请求上传成功后吧该地图数据写入redis 表示有效
3.服务器定期清理以及分配了的id 并且存在的文件，但是redis二次确认没有请求,需要从磁盘或者云存储删除这些数据
 由于是存文本地图信息,可以考虑写入云数据库而不是云文件存储 比如redis
 */

//地图编辑器 顶级控制器
namespace MapEditor
{
    public class MapEditorRunner : MonoBehaviour
    {

        public void ChangeBackGround(int theme)
        {
            var obj = GameObject.Find("Maps/BackGround");
            if (obj == null) return;
            var spp = obj.GetComponent<SpriteRenderer>();

            var tex = MapLoader.ins.LoadEdotorImageThemeBgV1(theme);   // (Texture2D)PrefabsMgr.Load<Object>("Map/Image/theme_bg/" + theme.ToString());
            if (tex == null)
            {
                Debug.Assert(false);

            }
            var sp = Sprite.Create(tex, new Rect(new Vector2(0, 0), new Vector2(tex.width, tex.height)), new Vector2(0.5f, 0.5f));
            spp.sprite = sp;

            var bgEffectTransform = obj.transform.Find("BgEffect");
            if(bgEffectTransform != null)
            {
                GameObject.Destroy(bgEffectTransform.gameObject);
            }
            var needBgEffect = MapEditorConfig.GetNeedBgEffect(theme);
            if(needBgEffect)
            {
                var effectParent = obj.transform;
                var bgEffectPrefab = MapLoader.ins.LoadBgEffectV1(theme);
                if(bgEffectPrefab != null)
                {
                    var bgEffect = GameObject.Instantiate<GameObject>(bgEffectPrefab, effectParent);
                    bgEffect.name = "BgEffect";
                    bgEffect.transform.localPosition = Vector3.zero;
                }
            }
        }
        ConfigMapEditor config = null;
        void Start()
        {
            Time.timeScale = 1f;
            this.ChangeBackGround(MapEditor.MapEditorConfig.CurrentSelectTheme);
            this.config = ConfigLoader.ins.GetConfig<ConfigMapEditor>();
            if (config != null)
            {
                this.AUTO_SAVE_TIME_INTERVAL = config.AutoSaveInterval;
            }

        }
        float AUTO_SAVE_TIME_INTERVAL = 15f;

        float auto_save_time = 0f;
        public static long auto_id = 0;
        void Update()
        {
            auto_save_time += Time.deltaTime;
            if (auto_save_time > AUTO_SAVE_TIME_INTERVAL)
            {
                auto_save_time = 0f;
                //TODO AUTO SAVE;
                if (MapEditor.MapObjectRoot.ins != null)
                {
                    if (auto_id <= 0)
                    {
                        return;
                    }
                    string json = MapEditor.MapObjectRoot.ins.SerializeToJson();
                    var file = LocalStorageMapTmp.ins.GetRootDirectory() + "/" + StaticData.uuid + "_" + "auto_save_tmp.json";
                    StartCoroutine(AsyncAutoSaveMap(file, json));
                }
            }
        }
        IEnumerator AsyncAutoSaveMap(string file, string json)
        {
            bool ok = false;
            yield return new Engine.Base.Async.WaitForThreadJob(() =>
            {
                try
                {
                    if (string.IsNullOrEmpty(file))
                    {
                        return;
                    }
                    using (FileStream f = new FileStream(file, FileMode.Create))
                    {
                        var ss = new StreamWriter(f);
                        ss.Write(json);
                        ss.Flush();
                        f.Flush();
                        ss.Close();
                        f.Close();
                    }
                    ok = true;
                }
                catch (Exception e)
                {
                    Debug.Log("save tmp file error  " + e.Message);
                }
            });

            if (ok)
            {
                LuaInterface.LuaMgr.TryFireLua("map_editor", "auto_save_ok");
            }
            else
            {
                LuaInterface.LuaMgr.TryFireLua("map_editor", "auto_save_error");
            }
        }
    }
}