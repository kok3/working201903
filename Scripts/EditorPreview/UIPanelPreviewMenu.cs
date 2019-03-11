/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MapEditor
{
    public class UIPanelPreviewMenu : MonoBehaviour
    {

        public void OnUploadErrror(string error = "上传地图错误")
        {

        }
        bool upload_lock = false;
        private void Upload(string json)
        {
            if (upload_lock) return;
            upload_lock = true;
            //先请求uuid  因为服务器现在没有校验机制  因此需要请求2次 设置地图基本信息，等接入后 客户端只需要上传json即可 uuid什么的都服务器设置
            RpcClient.ins.SendRequest("map", "upload_map", "uuid:" + StaticData.uuid + ",", (RpcRespone msg1) =>
            {
                Debug.Log(msg1.protocol.json);
                if (msg1.ok)
                {
                    var kv = Json.Decode(msg1.protocol.json);
                    if (kv != null)
                    {
                        var id = kv.Get("id");
                        var t = new rpc.MapUpload();
                        t.uuid = id;
                        t.data = json;
                        t.creator = StaticData.uuid;
                        //再上传数据
                        RpcClient.ins.SendRequest<rpc.MapUpload>("map", "upload_map_info", t, (RpcRespone ss) =>
                        {
                            if (msg1.ok)
                            {
                                var kv1 = Json.Decode(ss.protocol.json);
                                if (kv1 != null)
                                {
                                    if (kv1.Get("ret") == "ok")
                                    {
                                        this.OnUploadDone(true, "");
                                    }
                                    else
                                    {
                                        this.OnUploadDone(false, "上传失败,请稍后重试:" + kv1.Get("msg"));
                                    }
                                }
                                else
                                {
                                    this.OnUploadDone(false, "上传失败,请稍后重试");
                                }

                                // Debug.LogError(ss.protocol.json);
                            }
                            upload_lock = false;
                        });
                    }
                    else
                    {
                        upload_lock = false;
                    }
                }
                else
                {
                    upload_lock = false;
                }
            });
        }

        private void OnUploadDone(bool ok, string error)
        {

            return;

            if (UICommonDialog.ins != null)
            {
                if (ok)
                {
                    MapEditorStroageData.Clear();
                    UICommonDialog.ins.ShowOK("恭喜您设计的地图已上传至服务器，请点击确定返回游戏主界面。", () =>
                    {
                        Application.LoadLevel("GameLogin");
                    });
                }
                else
                {
                    UICommonDialog.ins.ShowOK(error);
                }
            }
        }
        float time = 0;
        //click done , can upload
        public void OnClickDone()
        {
            //防止过快点击
            float c = Time.time;
            if (c - time <1f)
            {
                return;
            }
            time = c;

            //改为回调 lua   来触发 上传逻辑
            if (LuaInterface.LuaMgr.ins != null)
            {
                LuaInterface.LuaMgr.ins.CallGlobalFunction("MAP_EDITOR_PREVIEW_ON_CLICK_UPLOAD");
            }
            //      Base.Events.ins.FireLua("map_editor", "preview_upload_map_click");
            //ignore any errro
            return;
            //完成上传
            //
          /*  Base.Events.ins.FireLua("UI", "MapEditorPreviewShowDialogYesNo",
              new VoidFuncVoid(() =>
                {
                    //yes
                    MapEditor.MapEditorUtils.PreviewPublish((bool ok) =>
                        {
                            if (ok)
                            {
                                try
                                {
                                    // 通知lua处理后续
                                    LuaInterface.LuaMgr.ins.CallGlobalFunctionMust("MAP_EDITOR_PREVIEW_UPLOAD_DONE");
                                }
                                catch (System.Exception e)
                                {
                                    //未知错误 直接跳到主界面
                                    Application.LoadLevel("GameLogin");
                                }
                            }
                            else
                            {
                                try
                                {
                                    // 通知lua处理后续
                                    LuaInterface.LuaMgr.ins.CallGlobalFunctionMust("MAP_EDITOR_PREVIEW_UPLOAD_ERROR");
                                }
                                catch (System.Exception e)
                                {
                                    //未知错误 直接忽略
                                }
                            }
                        });
                }));*/
            return;
            if (upload_lock) return;
            if (UICommonDialog.ins != null)
            {
                UICommonDialog.ins.ShowYesNo("是否确定将关卡上传至服务器供所有人玩？", () =>
                {
                    //yes
                    // do upload 
                    // request server
                    //   if (MapObjectRoot.ins != null)
                    {
                        this.Upload(MapObjectRoot.record_json);
                    }
                },
                    () =>
                    {
                    });
            }
        }
        //click edit map again
        public void OnClickEditAgain()
        {
            Application.LoadLevel("MapEditor");
            //   UIRoot.ins.EndPreView();
        }
        //return to GameLogin
        public void OnClickReturn()
        {
            //打开设置界面
            if (Base.Events.ins != null)
            {
                Base.Events.ins.FireLua("map_editor", "open_settings");
            }
            return;
            if (UICommonDialog.ins != null)
            {
                UICommonDialog.ins.ShowYesNo("您编辑的关卡仍未上传至服务器，是否放弃本次编辑直接返回到主界面？", () =>
                {

                }, () =>
                    {
                        // 初始化 共享数据
                        MapEditorStroageData.Clear();
                        Application.LoadLevel("GameLogin");
                    }, "继续编辑", "放弃编辑");
            }
        }
    }
}