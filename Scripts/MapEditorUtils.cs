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

//编辑器模式下读写excel
#if UNITY_EDITOR
using Aspose.Cells;
#endif

namespace MapEditor
{
    public static class MapEditorUtils
    {
        //检查是否有自动备份 需要处理
        public static bool HasRecover()
        {
            if (string.IsNullOrEmpty(PlayerPrefsExt.GetString("___auto_save_tmp_id")))
            {//不存在上次操作 因此无需回滚
                //try delete file
                DeleteAutoFile();
                return false;
            }
            var json = LocalStorageMapTmp.ins.LoadTextFromDisk(StaticData.uuid + "_" + "auto_save_tmp.json");
            if (string.IsNullOrEmpty(json))
            {   //try delete file
                DeleteAutoFile();
                return false;
            }
            if (Serializable.Map.IsMapJson(json))
            {
                try
                {
                    Serializable.Map map = Serializable.ToObject<Serializable.Map>(json);
                    //valid json
                    return true;
                }
                catch (Exception e)
                {

                }
            }
            //try delete file
            DeleteAutoFile();
            return false;
        }
        //恢复自动备份文件
        public static bool RecoverAuto()
        {
            string last_id = PlayerPrefsExt.GetString("___auto_save_tmp_id");
            if (string.IsNullOrEmpty(last_id))
            {//不存在上次操作 因此无需回滚
                //try delete file
                DeleteAutoFile();
                return false;
            }
            string name = LocalStorageMapTmp.ins.GetRootDirectory() + "/" + StaticData.uuid + "_" + last_id + ".json";

            /* if (File.Exists(name))
             {
                 //  try
                 {
                     //    File.Delete(name);
                 }
                 //    catch (Exception e)
                 {
                 }
             }
             else
             {
                 //目标旧文件不存在 因此不需要恢复
                 Debug.LogError("111111111111");

                 return false;
             }*/
            //直接覆盖
            var json = LocalStorageMapTmp.ins.LoadTextFromDisk(StaticData.uuid + "_" + "auto_save_tmp.json");
            if (string.IsNullOrEmpty(json))
            {   //try delete file
                DeleteAutoFile();
                return false;
            }
            try
            {
                using (FileStream f = new FileStream(name, FileMode.Create))
                {
                    var ss = new StreamWriter(f);
                    ss.Write(json);
                    ss.Flush();
                    f.Flush();
                    ss.Close();
                    f.Close();
                }
            }
            catch (Exception e)
            {
                return false;
            }
            DeleteAutoFile();
            return true;
        }
        public static void DeleteAutoFile()
        {
            string name = LocalStorageMapTmp.ins.GetRootDirectory() + "/" + StaticData.uuid + "_" + "auto_save_tmp.json";
            if (File.Exists(name))
            {
                try
                {
                    File.Delete(name);
                }
                catch (Exception e)
                {
                }
            }
            //删除备份文件 始终是 要删除记录的
            PlayerPrefsExt.DeleteKey("___auto_save_tmp_id");
        }
        //将一张下架的地图文件 移动到 本地地图文件夹里面
        //地图id 沿用老id
        public static bool SwapToLocalMap(string id)
        {
            if (string.IsNullOrEmpty(id)) return false;

            string name = LocalStorageMap.ins.GetRootDirectory() + "/" + id + ".json";
            var json = LocalStorageMap.ins.LoadTextFromDisk(id.ToString() + ".json");
            if (string.IsNullOrEmpty(json))
            {
                return false;
            }
            Serializable.Map map = null;
            if (Serializable.Map.IsMapJson(json))
            {
                try
                {
                    map = Serializable.ToObject<Serializable.Map>(json);
                    var x = map.theme;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            //执行写入到操作
            try
            {
                using (FileStream f = new FileStream(LocalStorageMapTmp.ins.GetRootDirectory() + "/" + StaticData.uuid + "_" + id + ".json", FileMode.Create))
                {
                    var ss = new StreamWriter(f);
                    ss.Write(json);
                    ss.Flush();
                    f.Flush();
                    ss.Close();
                    f.Close();
                }
            }
            catch (Exception e)
            {
                return false;
            }
            //写入成功 删除旧文件
            try
            {
                File.Delete(name);
            }
            catch (Exception e)
            {
                return false;
            }
            //尝试删除 地图截图
            try
            {
                //   LocalStorageMapCapture
                LocalStorageMapCaptureImage.ins.DeleteFiles(id.ToString());
            }
            catch (Exception e)
            {
            }
            UpdateLocalMapTime(id.ToString());
            return true;
        }
        //请求编辑一张本地地图  未发布 
        public static bool RequestEditorLocalMap(string id)
        {
            if (string.IsNullOrEmpty(id)) return false;

            //___auto_save_tmp_id
            var json = LocalStorageMapTmp.ins.LoadTextFromDisk(StaticData.uuid + "_" + id.ToString() + ".json");
            if (string.IsNullOrEmpty(json))
            {
                return false;
            }
            Serializable.Map map = null;
            if (Serializable.Map.IsMapJson(json))
            {
                try
                {
                    map = Serializable.ToObject<Serializable.Map>(json);
                    var x = map.theme;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            MapEditorStroageData.Clear();

            // 
            MapEditor.MapEditorConfig.CurrentSelectTheme = map.theme;
            MapEditor.MapEditorConfig.CurrentMapGameMode = (MapGameMode)(map.MapInfoMode);
            if (MapEditor.MapEditorConfig.CurrentMapGameMode == MapGameMode.Normal)
            {
                //经典模式  边界重置
                MapEditorStroageData.ResetEdgeBox();
            }
            var xxx = new MapEditor.MapEditorStroageData();
            xxx.ReloadJson(json);
            MapEditor.MapObjectRoot.data = xxx;
            MapEditor.MapObjectRoot.record_json = json;
            PlayerPrefsExt.SetString("___auto_save_tmp_id", id);
            MapEditor.MapEditorRunner.auto_id = 0;
            long lid = 0;
            if (long.TryParse(id, out lid))
            {
                MapEditor.MapEditorRunner.auto_id = lid;
            }
            Application.LoadLevel("MapEditor");
            return true;
        }
        //新建一张地图
        public static void GoMapEditor(int theme_id, int mode)
        {
            MapEditor.MapEditorRunner.auto_id = Utils.GetTimestampMiliseconds() + Utils.GetServerTimestampSeconds(); // Utils.;
            PlayerPrefsExt.SetString("___auto_save_tmp_id", MapEditor.MapEditorRunner.auto_id.ToString());
            MapEditor.MapEditorConfig.CurrentSelectTheme = theme_id;
            MapEditor.MapEditorConfig.CurrentMapGameMode = (MapGameMode)(mode);
            if (MapEditor.MapEditorConfig.CurrentMapGameMode == MapGameMode.Normal)
            {
                //经典模式  边界重置
                MapEditorStroageData.ResetEdgeBox();
            }
            //新建地图需要清理 静态 数据
            MapEditorStroageData.current_map_name = "";
            MapEditorStroageData.current_map_brief = "";
            Application.LoadLevel("MapEditor");
        }

        //更新本地地图的 修改时间
        public static void UpdateLocalMapTime(string id)
        {
            //更新地图的修改时间
            string str = PlayerPrefsExt.GetString("__local_map_version_time");
            var js = Json.Decode(str);
            if (js == null)
            {
                js = HashTable.Create();
            }
            js.Set(id.ToString(), Utils.GetServerTimestampSeconds().ToString());
            PlayerPrefsExt.SetString("__local_map_version_time", Json.Encode(js));
        }

        public static void UpdateLocalMapTargetTime(string id, string time)
        {
            //更新地图的修改时间
            string str = PlayerPrefsExt.GetString("__local_map_version_time");
            var js = Json.Decode(str);
            if (js == null)
            {
                js = HashTable.Create();
            }
            js.Set(id.ToString(), time);
            PlayerPrefsExt.SetString("__local_map_version_time", Json.Encode(js));
        }

        //设置界面退出 按钮操作
        public static void SettingQuit()
        {
            MapEditorStroageData.Clear();
            MapEditor.MapEditorRunner.auto_id = 0;
            //删除上一次操作的 地图记录 
            //删除临时保存的文件
            DeleteAutoFile();
            //回到gamelogin 界面
            Application.LoadLevel("GameLogin");
        }
        //退出并且 保存
        public static void SettingQuitAndSave()
        {
            //删除上一次操作的 地图记录 
            string id = PlayerPrefsExt.GetString("___auto_save_tmp_id");
            if (string.IsNullOrEmpty(id))// || MapEditor.MapObjectRoot.ins == null)
            {
                return;
            }
            MapEditorStroageData.Clear();
            MapEditor.MapEditorRunner.auto_id = 0;
            //保存修改到正式的地图文件
            string json = MapEditor.MapObjectRoot.record_json;//.ins.SerializeToJson();;
            if (MapEditor.MapObjectRoot.ins != null)
            {
                //当前在 编辑器内 强制重新生一下
                json = MapEditor.MapObjectRoot.ins.SerializeToJson();
            }
            else
            {
                //不在编辑器内  那就是在编辑器中的预览了 那么可能修改了 需要更新一下json内容
                Serializable.Map map = Serializable.Map.FromJson(json);
                if (map == null) return;
                bool will_save = false;
                if (!string.IsNullOrEmpty(MapEditorStroageData.current_map_brief))
                {
                    map.brief = MapEditorStroageData.current_map_brief;
                    will_save = true;
                }
                if (!string.IsNullOrEmpty(MapEditorStroageData.current_map_name))
                {
                    map.name = MapEditorStroageData.current_map_name;
                    will_save = true;
                }
                if (will_save)
                {
                    json = map.ToJson();
                }
            }
            var file = LocalStorageMapTmp.ins.GetRootDirectory() + "/" + StaticData.uuid + "_" + id + ".json";
            try
            {
                using (FileStream f = new FileStream(LocalStorageMapTmp.ins.GetRootDirectory() + "/" + StaticData.uuid + "_" + id + ".json", FileMode.Create))
                {
                    var ss = new StreamWriter(f);
                    ss.Write(json);
                    ss.Flush();
                    f.Flush();
                    ss.Close();
                    f.Close();
                }
            }
            catch (Exception e)
            {
                return;
            }
            //删除临时保存的文件
            DeleteAutoFile();
            //删除老地图的 预览图 因为地图更新了
            LocalStorageMapCaptureImage.ins.DeleteFiles("tmp" + id.ToString());

            //更新地图的修改时间
            UpdateLocalMapTime(id);
            //回到gamelogin 界面
            Application.LoadLevel("GameLogin");
        }

        //发布地图 地图编辑器中使用 自动处理 是 预览 还是编辑器下
        //地图编辑器中的 设置按钮
        //编辑器内已经可以保证地图合法性了
        public static void SettingPublish(VoidFuncN<bool, string> cb)
        {
            if (//MapObjectRoot.ins == null ||
                cb == null)
            {
                return;
            }

            string json = MapEditor.MapObjectRoot.record_json;//.ins.SerializeToJson();;
            if (MapEditor.MapObjectRoot.ins != null)
            {
                //当前在 编辑器内 强制重新生一下
                json = MapEditor.MapObjectRoot.ins.SerializeToJson();
            }

            string map_name = MapEditorStroageData.current_map_name; //MapObjectRoot.ins.map_name;
            string map_brief = MapEditorStroageData.current_map_brief; // MapObjectRoot.ins.map_brief;
            var auto_id = MapEditorRunner.auto_id;
            if (string.IsNullOrEmpty(json) || auto_id <= 0)
            {
                cb(false, "");
                return;
            }
            //先上传地图 成功后 再删除本地备份

            //先请求uuid  因为服务器现在没有校验机制  因此需要请求2次 设置地图基本信息，等接入后 客户端只需要上传json即可 uuid什么的都服务器设置
            RpcClient.ins.SendRequest("map", "upload_map", "uuid:" + StaticData.uuid + ",", (RpcRespone msg1) =>
            {
                if (msg1.ok)
                {
                    var kv = Json.Decode(msg1.protocol.json);
                    if (kv != null)
                    {
                        var id = kv.Get("id");
                        var t = new rpc.MapUpload();
                        t.uuid = id;
                        t.data = json;
                        t.name = map_name;
                        t.brief = map_brief;
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
                                        //  this.OnUploadDone(true, "");
                                        //先处理 数据 

                                        if (LuaInterface.LuaMgr.ins != null)
                                        {
                                            LuaInterface.LuaMgr.ins.CallGlobalFunction("_____DeleteOneLocalMap", auto_id.ToString());
                                        }
                                        //只要是上传成功了都要尝试一下删除 自动备份
                                        MapEditor.MapEditorUtils.DeleteAutoFile();
                                        //清理 共享数据
                                        MapEditorStroageData.Clear();
                                        cb(true, id);
                                        return;
                                    }
                                    else
                                    {
                                        cb(false, "");
                                        // this.OnUploadDone(false, "上传失败,请稍后重试:" + kv1.Get("msg"));
                                    }
                                }
                                else
                                {
                                    cb(false, "");
                                    //   this.OnUploadDone(false, "上传失败,请稍后重试");
                                }

                                // Debug.LogError(ss.protocol.json);
                            }
                            else
                            {
                                cb(false, "");
                            }
                        });
                    }
                    else
                    {
                        cb(false, "");
                    }
                }
                else
                {
                    cb(false, "");
                }
            });
        }


        //发布一张本地地图到服务器   和SettingPublish不一样 这个是可以在非编辑器环境处理的
        //cb param is int
        // 0 is ok
        // 1 general error code
        // 2 is map is invalid
        public static void GeneralPublishLocal(string map_id, VoidFuncN<int, string> cb, bool isInnerEditor)
        {
            if (//MapObjectRoot.ins == null ||
                cb == null)
            {
                return;
            }

            string name = StaticData.uuid + "_" + map_id + ".json";
            var json = LocalStorageMapTmp.ins.LoadTextFromDisk(name);
            if (json == null || !Serializable.Map.IsMapJson(json))
            {
                //invalid file ignore
                cb(1, null);
                return;
            }
            // 
            Serializable.Map map = null;
            try
            {
                map = Serializable.Map.FromJson(json);
                //目前地图非法规则 只有 物体为空
                if (map.objects == null || map.objects.Count <= 0)
                {
                    cb(2, null);
                    return;
                }
                //valid map file
            }
            catch (Exception e)
            {
                cb(2, null);
                return;
            }

            //先上传地图 成功后 再删除本地备份

            //先请求uuid  因为服务器现在没有校验机制  因此需要请求2次 设置地图基本信息，等接入后 客户端只需要上传json即可 uuid什么的都服务器设置
            RpcClient.ins.SendRequest("map", "upload_map", "uuid:" + StaticData.uuid + ",", (RpcRespone msg1) =>
            {
                if (msg1.ok)
                {
                    var kv = Json.Decode(msg1.protocol.json);
                    if (kv != null)
                    {
                        var id = kv.Get("id");
                        var t = new rpc.MapUpload();
                        t.uuid = id;
                        t.data = json;
                        t.name = map.name;
                        t.brief = map.brief;
                        t.creator = StaticData.uuid;

                        MapEditorStroageData.current_map_name = map.name;
                        MapEditorStroageData.current_map_brief = map.brief;
                        //再上传数据
                        string uploadMethod = "upload_map_info";
                        if (isInnerEditor)
                        {
                            //内部编辑器模式使用接口使用接口一键发布地图，区别是这条发布的地图不进入最新地图列表
                            //服务器暂时忽略地图背包检查 目前只是客户端处理
                            uploadMethod = "upload_map_info_inner";
                        }
                        RpcClient.ins.SendRequest<rpc.MapUpload>("map", uploadMethod, t, (RpcRespone ss) =>
                        {
                            if (msg1.ok)
                            {
                                var kv1 = Json.Decode(ss.protocol.json);
                                if (kv1 != null)
                                {
                                    if (kv1.Get("ret") == "ok")
                                    {
                                        //  this.OnUploadDone(true, "");
                                        //先处理 数据 

                                        if (LuaInterface.LuaMgr.ins != null)
                                        {
                                            LuaInterface.LuaMgr.ins.CallGlobalFunction("_____DeleteOneLocalMap", map_id.ToString());
                                        }
                                        //只要是上传成功了都要尝试一下删除 自动备份
                                        MapEditor.MapEditorUtils.DeleteAutoFile();
                                        //清理 共享数据
                                        MapEditorStroageData.Clear();
                                        cb(0, id);
                                        return;
                                    }
                                    else
                                    {
                                        cb(1, null);
                                        // this.OnUploadDone(false, "上传失败,请稍后重试:" + kv1.Get("msg"));
                                    }
                                }
                                else
                                {
                                    cb(1, null);
                                    //   this.OnUploadDone(false, "上传失败,请稍后重试");
                                }

                                // Debug.LogError(ss.protocol.json);
                            }
                            else
                            {
                                cb(1, null);
                            }
                        });
                    }
                    else
                    {
                        cb(1, null);
                    }
                }
                else
                {
                    cb(1, null);
                }
            });
        }


        /*  //地图预览模式下 发布地图
          //地图预览
          public static void PreviewPublish(VoidFuncN<bool, string> cb)
          {
              if (//MapObjectRoot.ins == null || 

                  cb == null || string.IsNullOrEmpty(MapObjectRoot.record_json))
              {
                  return;
              }
              //预览界面无需重建 json
              string json = MapObjectRoot.record_json;  // json111; //  MapObjectRoot.ins.SerializeToJson();
              string map_name = MapEditorStroageData.current_map_name; //MapObjectRoot.ins.map_name;
              string map_brief = MapEditorStroageData.current_map_brief; // MapObjectRoot.ins.map_brief;
              var auto_id = MapEditorRunner.auto_id;
              if (string.IsNullOrEmpty(json) || auto_id <= 0)
              {
                  cb(false,"");
                  return;
              }
              //先上传地图 成功后 再删除本地备份

              //先请求uuid  因为服务器现在没有校验机制  因此需要请求2次 设置地图基本信息，等接入后 客户端只需要上传json即可 uuid什么的都服务器设置
              RpcClient.ins.SendRequest("rpc", "request_upload_map", "uuid:" + StaticData.uuid + ",", (RpcRespone msg1) =>
              {
                  if (msg1.ok)
                  {
                      var kv = Json.Decode(msg1.protocol.json);
                      if (kv != null)
                      {
                          var id = kv.Get("id");
                          var t = new rpc.MapUpload();
                          t.uuid = id;
                          t.data = json;
                          t.name = map_name;
                          t.brief = map_brief;
                          t.creator = StaticData.uuid;
                          //再上传数据
                          RpcClient.ins.SendRequestProto("rpc", "upload_map_info", RpcClient.Encode(t), (RpcRespone ss) =>
                          {
                              if (msg1.ok)
                              {
                                  var kv1 = Json.Decode(ss.protocol.json);
                                  if (kv1 != null)
                                  {
                                      if (kv1.Get("ret") == "ok")
                                      {
                                          //  this.OnUploadDone(true, "");
                                          //先处理 数据 

                                          if (LuaInterface.LuaMgr.ins != null)
                                          {
                                              LuaInterface.LuaMgr.ins.CallGlobalFunction("_____DeleteOneLocalMap", auto_id.ToString());
                                          }
                                          //只要是上传成功了都要尝试一下删除 自动备份
                                          MapEditor.MapEditorUtils.DeleteAutoFile();
                                          //清理 共享数据
                                          MapEditorStroageData.Clear();
                                          cb(true,id);
                                          return;
                                      }
                                      else
                                      {
                                          // this.OnUploadDone(false, "上传失败,请稍后重试:" + kv1.Get("msg"));
                                      }
                                  }
                                  else
                                  {
                                      //   this.OnUploadDone(false, "上传失败,请稍后重试");
                                  }

                                  // Debug.LogError(ss.protocol.json);
                              }
                              cb(false,"");
                          });
                      }
                      else
                      {
                          cb(false,"");
                      }
                  }
                  else
                  {
                      cb(false,"");
                  }
              });
          }*/

        /*   //发布一张本地地图 在非编辑器下使用
           public static void PublishLocalMap(string map_id, VoidFunc1<bool> cb)
           {
               throw new System.NotSupportedException();
               //暂时不能 上传本地地图 在非编辑器下 因为无法校验 逻辑合法性 比如是否有重叠

               if (cb == null) return;
               if (string.IsNullOrEmpty(map_id))
               {
                   cb(false);
               }
               // var file = LocalStorageMapTmp.ins.GetRootDirectory() + "/" + StaticData.uuid + "_" + id + ".json";

               string name = StaticData.uuid + "_" + map_id + ".json";
               var json = LocalStorageMapTmp.ins.LoadTextFromDisk(name);
               if (string.IsNullOrEmpty(json) == false && Serializable.Map.IsMapJson(json))
               {
                   string map_name = MapEditorStroageData.current_map_name;  //MapObjectRoot.ins.map_name;
                   string map_brief = MapEditorStroageData.current_map_brief; // MapObjectRoot.ins.map_brief;
                   var auto_id = MapEditorRunner.auto_id;
                   //id 是本地 存储的 需要删掉
                   if (string.IsNullOrEmpty(json) || auto_id <= 0)
                   {
                       cb(false);
                       return;
                   }
                   //先上传地图 成功后 再删除本地备份

                   //先请求uuid  因为服务器现在没有校验机制  因此需要请求2次 设置地图基本信息，等接入后 客户端只需要上传json即可 uuid什么的都服务器设置
                   RpcClient.ins.SendRequest("rpc", "request_upload_map", "uuid:" + StaticData.uuid + ",", (RpcRespone msg1) =>
                   {
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
                               RpcClient.ins.SendRequestProto("rpc", "upload_map_info", RpcClient.Encode(t), (RpcRespone ss) =>
                               {
                                   if (msg1.ok)
                                   {
                                       var kv1 = Json.Decode(ss.protocol.json);
                                       if (kv1 != null)
                                       {
                                           if (kv1.Get("ret") == "ok")
                                           {
                                               //  this.OnUploadDone(true, "");
                                               //先处理 数据 

                                               if (LuaInterface.LuaMgr.ins != null)
                                               {
                                                   LuaInterface.LuaMgr.ins.CallGlobalFunction("_____DeleteOneLocalMap", auto_id.ToString());
                                               }
                                               //只要是上传成功了都要尝试一下删除 自动备份
                                               MapEditor.MapEditorUtils.DeleteAutoFile();
                                               //清理 共享数据
                                               MapEditorStroageData.Clear();
                                               cb(true);
                                               return;
                                           }
                                           else
                                           {
                                               // this.OnUploadDone(false, "上传失败,请稍后重试:" + kv1.Get("msg"));
                                           }
                                       }
                                       else
                                       {
                                           //   this.OnUploadDone(false, "上传失败,请稍后重试");
                                       }

                                       // Debug.LogError(ss.protocol.json);
                                   }
                                   cb(false);
                               });
                           }
                           else
                           {
                               cb(false);
                           }
                       }
                       else
                       {
                           cb(false);
                       }
                   });
               }
               else
               {
                   cb(false);
               }

           }*/

        //一般是UI调用
        public static void TrySetCurrentNameBrief(string name, string brief)
        {
            //设置 名字 和 简介 只有在编辑器下 时 才有效
            //  if (MapObjectRoot.ins != null)
            {
                // MapObjectRoot.ins.map_name = name;
                //  MapObjectRoot.ins.map_brief = brief;
            }
            MapEditorStroageData.current_map_name = name;
            MapEditorStroageData.current_map_brief = brief;
        }
        public static void TrySetCurrentName(string name)
        {
            //设置 名字 和 简介 只有在编辑器下 时 才有效
            //  if (MapObjectRoot.ins != null)
            {
                //  MapObjectRoot.ins.map_name = name;
            }
            MapEditorStroageData.current_map_name = name;
        }
        public static void TrySetCurrentBrief(string brief)
        {
            //设置 名字 和 简介 只有在编辑器下 时 才有效
            //  if (MapObjectRoot.ins != null)
            {
                //    MapObjectRoot.ins.map_brief = brief;
            }
            MapEditorStroageData.current_map_brief = brief;
        }
        //获取当前编辑的地图的名字
        public static string TyrGetCurrentEditName()
        {
            return MapEditorStroageData.current_map_name;
        }
        public static string TyrGetCurrentEditBrief()
        {
            return MapEditorStroageData.current_map_brief;
        }
        public static void RequestUploadCloudMap(string map_id, VoidFuncString cb)
        {
            string name = StaticData.uuid + "_" + map_id + ".json";
            var json = LocalStorageMapTmp.ins.LoadTextFromDisk(name);
            var t = new rpc.MapUpload();
            t.uuid = map_id;
            t.data = json;
            t.creator = StaticData.uuid;

            //再上传数据
            RpcClient.ins.SendRequest<rpc.MapUpload>("map", "request_map_cloud_upload", (t), (RpcRespone ss) =>
            {
                cb(ss.protocol.json);
            });
        }

        // 请求同步一张 地图 为下载
        public static void StartDownloadToLocalMapFile(string url, string save_name, VoidFunc2<string, int> cb)
        {
            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(save_name) || cb == null) return;

            if (HttpTaskMgr.ins != null)
            {
                HttpTaskMgr.ins.AddTask2(url, (string data, int code) =>
                    {
                        if (code == 404)
                        {
                            //未找到该地图
                            cb("", 404);
                            return;
                        }
                        if ((Serializable.Map.IsMapJson(data)))
                        {
                            try
                            {
                                using (FileStream f = new FileStream(LocalStorageMapTmp.ins.GetRootDirectory() + "/" + save_name, FileMode.Create))
                                {
                                    var ss = new StreamWriter(f);
                                    ss.Write(data);
                                    ss.Flush();
                                    f.Flush();
                                    ss.Close();
                                    f.Close();
                                }
                                cb("ok", code);
                            }
                            catch (Exception e)
                            {
                                cb("2", code);
                            }
                        }
                        else
                        {
                            cb("1", code);
                        }
                    });
            }
            else
            {
                cb("", 0);
            }
        }

        //预览一张地图   只能预览在线地图 玩家本地的 不能预览 只能进入编辑器 流程
        public static void GoMapPreview(string uuid)
        {
            if (string.IsNullOrEmpty(uuid))
            {
                return;
            }
            MapEditor.MapPreviewRunner.map_preview_uuid = uuid;
            //跳转场景
            Application.LoadLevel("MapPreview");
        }
        //进入预览模式作为引导
        public static void GoMapPreviewWithGuide(string uuid)
        {
            if (string.IsNullOrEmpty(uuid))
            {
                return;
            }
            GameMgr.mode = BattleMode.Guide;
            MapEditor.MapPreviewRunner.map_preview_uuid = uuid;
            //跳转场景
            Application.LoadLevel("MapPreview");
        }
        //修改本地地图的 名字 需要反序列化 然后再处理
        public static bool ModifyLocalMapName(string map_id, string map_name)
        {
            if (string.IsNullOrEmpty(map_id) || string.IsNullOrEmpty(map_name))
            {
                return false;
            }

            string name = StaticData.uuid + "_" + map_id + ".json";
            var json = LocalStorageMapTmp.ins.LoadTextFromDisk(name);
            if (!Serializable.Map.IsMapJson(json))
            {
                //invalid file ignore
                return false;
            }
            //   
            try
            {
                Serializable.Map map = Serializable.Map.FromJson(json);
                map.name = map_name;
                var write_back_json = map.ToJson();
                using (FileStream f = new FileStream(LocalStorageMapTmp.ins.GetRootDirectory() + "/" + name, FileMode.Create))
                {
                    var ss = new StreamWriter(f);
                    ss.Write(write_back_json);
                    ss.Flush();
                    f.Flush();
                    ss.Close();
                    f.Close();
                }
                //write done
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }
        //修改地图简介
        public static bool ModifyLocalMapBrief(string map_id, string map_brief)
        {
            if (string.IsNullOrEmpty(map_id) || string.IsNullOrEmpty(map_brief))
            {
                return false;
            }

            string name = StaticData.uuid + "_" + map_id + ".json";
            var json = LocalStorageMapTmp.ins.LoadTextFromDisk(name);
            if (!Serializable.Map.IsMapJson(json))
            {
                //invalid file ignore
                return false;
            }
            //   
            try
            {
                Serializable.Map map = Serializable.Map.FromJson(json);
                map.brief = map_brief;
                var write_back_json = map.ToJson();
                using (FileStream f = new FileStream(LocalStorageMapTmp.ins.GetRootDirectory() + "/" + name, FileMode.Create))
                {
                    var ss = new StreamWriter(f);
                    ss.Write(write_back_json);
                    ss.Flush();
                    f.Flush();
                    ss.Close();
                    f.Close();
                }
                //write done
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }
        //本地存档的地图是否是合法的  用于非编辑器环境 处理
        public static bool IsLocalMapValid(string map_id)
        {
            if (string.IsNullOrEmpty(map_id))
            {
                return false;
            }
            string name = StaticData.uuid + "_" + map_id + ".json";
            var json = LocalStorageMapTmp.ins.LoadTextFromDisk(name);
            if (!Serializable.Map.IsMapJson(json))
            {
                //invalid file ignore
                return false;
            }
            //   
            try
            {
                Serializable.Map map = Serializable.Map.FromJson(json);
                //目前地图非法规则 只有 物体为空
                if (map.objects == null || map.objects.Count <= 0)
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }
        //判断当前地图是否存在
        public static bool IsLocalMapExist(string map_id)
        {
            if (string.IsNullOrEmpty(map_id))
            {
                return false;
            }
            string name = StaticData.uuid + "_" + map_id + ".json";
            var json = LocalStorageMapTmp.ins.LoadTextFromDisk(name);
            return Serializable.Map.IsMapJson(json);
        }

        //获取本地地图的 名字
        public static string GetLocalMapName(string map_id)
        {
            if (string.IsNullOrEmpty(map_id))
            {
                return null;
            }

            string name = StaticData.uuid + "_" + map_id + ".json";
            var json = LocalStorageMapTmp.ins.LoadTextFromDisk(name);
            if (!Serializable.Map.IsMapJson(json))
            {
                //invalid file ignore
                return null;
            }
            //   
            try
            {
                Serializable.Map map = Serializable.Map.FromJson(json);
                return map.name;
                //write done
            }
            catch (Exception e)
            {
                return null;
            }

            return null;
        }
        //获取本地地图的 简介
        public static string GetLocalMapBrief(string map_id)
        {
            if (string.IsNullOrEmpty(map_id))
            {
                return null;
            }

            string name = StaticData.uuid + "_" + map_id + ".json";
            var json = LocalStorageMapTmp.ins.LoadTextFromDisk(name);
            if (!Serializable.Map.IsMapJson(json))
            {
                //invalid file ignore
                return null;
            }
            //   
            try
            {
                Serializable.Map map = Serializable.Map.FromJson(json);
                return map.brief;
                //write done
            }
            catch (Exception e)
            {
                return null;
            }
            return null;
        }

        //删除评论文件
        public static bool CreateMapCommentsTargetFile(string fileName, List<string> titleList)
        {
#if UNITY_EDITOR
            if (File.Exists(fileName))
            {
                try
                {
                    File.Delete(fileName);
                }
                catch(Exception e)
                {
                    Debug.LogError(e.Message);
                    return false;
                }
            }
            Workbook book = OpenExcel(fileName);
            if (book == null) return false;
            Worksheet sheet = book.Worksheets[0];
            //string[] titleList = { "GroupId", "ID", "赞", "踩", "NPS", "图片或者链接", "曝光数", "退出数", "平均一血时间", "平均结算时间", "平均多杀数", "武器出生数量", "第一轮武器", "第二轮武器", "第三轮武器", "第四轮武器", "第五轮武器" };
            for (int column = 0; column < titleList.Count; column++)
            {
                Cell itemCell = sheet.Cells[0, column];
                itemCell.PutValue(titleList[column]);
            }
            try
            {
                book.Save(fileName, SaveFormat.Xlsx);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }
            return true;
#else
            return false;
#endif
        }
        //编辑器一键导出评论功能(根据地图信息，导出文件）
        public static bool ExportOneMapComment(string fileName, int index, List<string> comments)
        {
#if UNITY_EDITOR
            if (comments != null)
            {
                Workbook book = OpenExcel(fileName);
                if (book == null) return false;
                Worksheet sheet = book.Worksheets[0];
                for (int column = 0; column < comments.Count; column++)
                {
                    Cell itemCell = sheet.Cells[index, column];
                    try
                    {
                        double value = double.Parse(comments[column]);
                        itemCell.PutValue(value);
                    }
                    catch
                    {
                        itemCell.PutValue(comments[column]);
                    }
                }
                try
                {
                    book.Save(fileName, SaveFormat.Xlsx);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                    return false;
                }
                return true;
            }
            else
            {
                return false;
            }
#else
            return false;
#endif
        }

        private static Dictionary<int, string> weaponNames;
        private static void InitWeapons()
        {
            if (weaponNames == null)
            {
                weaponNames = new Dictionary<int, string>();
                weaponNames.Add(0, "手枪");
                weaponNames.Add(1, "AK47");
                weaponNames.Add(2, "长剑");
                weaponNames.Add(3, "榴弹枪");
                weaponNames.Add(4, "幻影刀");
                weaponNames.Add(5, "狙击枪");
                weaponNames.Add(6, "左轮手枪");
                weaponNames.Add(7, "水滴枪");
                weaponNames.Add(10, "散弹枪");
                weaponNames.Add(11, "铅球");
                weaponNames.Add(12, "飞弹枪");
                weaponNames.Add(14, "速射机枪");
                weaponNames.Add(15, "光剑");
                weaponNames.Add(16, "微型冲锋枪");
                weaponNames.Add(18, "微型加特林");
                weaponNames.Add(19, "加特林重机枪");
                weaponNames.Add(20, "弹射枪");
                weaponNames.Add(21, "磁暴枪");
                weaponNames.Add(22, "火箭筒");
                weaponNames.Add(24, "小蛇手枪");
                weaponNames.Add(26, "大蛇火箭筒");
                weaponNames.Add(32, "半自动步枪");
                weaponNames.Add(35, "熔岩激光枪");
                weaponNames.Add(36, "岩浆枪");
                weaponNames.Add(38, "爆弹枪");
                weaponNames.Add(39, "光子炮");
                weaponNames.Add(41, "黑洞枪");
                weaponNames.Add(67, "拖鞋");
                weaponNames.Add(68, "板砖");
                weaponNames.Add(70, "花伞");
            }
        }

        public static void ExportWeaponInfo(string fileName, string mapId, int row, int startColumn)
        {
#if UNITY_EDITOR
            InitWeapons();
            string jsonStr = LocalStorageMap.ins.LoadTextFromDisk(mapId + ".json");
            if (string.IsNullOrEmpty(jsonStr))
            {
                Debug.LogError("Can not load weapon from " + mapId);
            }
            else
            {
                Serializable.Map map = Serializable.ToObject<Serializable.Map>(jsonStr);
                Serializable.WeaponSpawnPoints weapon_spawn_points = map.weapon_spawn_points;//武器刷新点 1-4个
                string[] DropWeaponList = { "", "", "", "", ""};
                int weaponNum = 0;
                for (int i = 0; i < weapon_spawn_points.points.Count; i++)
                {
                    var point = weapon_spawn_points.points[i];
                    var ids = weapon_spawn_points.ids[i];
                    bool hasWeapons = false;
                    foreach (int id in ids.ids)
                    {
                        if (id >= 0)
                        {
                            weaponNum++;
                            hasWeapons = true;
                            break;
                        }
                    }
                    if(hasWeapons)
                    {
                        for (int round = 0; round < ids.ids.Count; round++)
                        {
                            if (round < DropWeaponList.Length)
                            {
                                int id = ids.ids[round];
                                if (id >= 0)
                                {
                                    if (weaponNames.ContainsKey(id))
                                    {
                                        DropWeaponList[round] += weaponNames[id] + ",";
                                    }
                                    else
                                    {
                                        DropWeaponList[round] += id + ",";
                                    }
                                }
                            }
                        }
                    }
                }
                Workbook book = OpenExcel(fileName);
                if (book == null) return;
                Worksheet sheet = book.Worksheets[0];
                Cell itemCell = sheet.Cells[row, startColumn];
                itemCell.PutValue(weaponNum);
                for (int round = 0; round < DropWeaponList.Length; round ++)
                {
                    itemCell = sheet.Cells[row, startColumn + round + 1];
                    itemCell.PutValue(DropWeaponList[round]);
                }
                try
                {
                    book.Save(fileName, SaveFormat.Xlsx);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                    return;
                }
            }
#endif
        }

        public static void ClearBatchPublishInfo()
        {
            string path = LocalStorageMgr.GetCacheRootDirectory() + "/InnerEditorData";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string sourceFilePath = path + "/map_publish_result.txt";
            using (FileStream fsw = new FileStream(sourceFilePath, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fsw))
                {
                    sw.Flush();
                }
            }
        }

        public static void ExportBatchPublishInfo(string localMapId, string publishMapId)
        {
            if (string.IsNullOrEmpty(localMapId) || string.IsNullOrEmpty(publishMapId))
            {
                return;
            }
            else
            {
                string path = LocalStorageMgr.GetCacheRootDirectory() + "/InnerEditorData";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string sourceFilePath = path + "/map_publish_result.txt";
                if (!File.Exists(sourceFilePath))
                {
                    using (FileStream fsw = new FileStream(sourceFilePath, FileMode.Create, FileAccess.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(fsw))
                        {
                            sw.WriteLine(localMapId + "    >>>>>>>>    " + publishMapId);
                            sw.Flush();
                        }
                    }
                }
                else
                {
                    using (FileStream fsw = new FileStream(sourceFilePath, FileMode.Append, FileAccess.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(fsw))
                        {
                            sw.WriteLine(localMapId + "    >>>>>>>>    " + publishMapId);
                            sw.Flush();
                        }
                    }
                }
            }
        }

        public static string SaveMapToDir(string mapId, string dir, bool showWeapon)
        {
            if (string.IsNullOrEmpty(mapId) || string.IsNullOrEmpty(dir))
            {
                return "";
            }
            string weaponExtension = showWeapon ? "_weapon" : "";
            string sourcePicFile = LocalStorageMgr.GetCacheRootDirectory() + "/MapCapture/" + mapId + "_284_160" + weaponExtension + ".jpg";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            if (File.Exists(sourcePicFile))
            {
                string targetPath = dir + "/" + mapId + ".jpg";
                File.Delete(targetPath);
                File.Copy(sourcePicFile, targetPath);
                return targetPath;
            }
            else
            {
                return "";
            }
        }

        public static void ExportLuaMapConfig(string excelFile, string luaFile)
        {
#if UNITY_EDITOR
            Workbook book = OpenExcel(excelFile);
            if (book == null) return;
            
            using (FileStream fsw = new FileStream(luaFile, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fsw))
                {
                    string luaHeader = 
@"local t = { };


t.ENABLE_HOTFIX_WHOLE_FILE =true; --include hotfix whole file can be hot-fix


";
                    sw.WriteLine(luaHeader);
                    Worksheet sheet = book.Worksheets[0];
                    int lastGroupId = 0;
                    for (int row = 1; row <= sheet.Cells.MaxDataRow; row++)
                    {
                        Cell itemCell = sheet.Cells[row, 0];
                        if (itemCell.Value == null) break;
                        string groupId = itemCell.Value.ToString();
                        int curGroupId = int.Parse(groupId);
                        if (curGroupId > lastGroupId)
                        {
                            sw.WriteLine("t.Group" + curGroupId + " = " + row + ";");
                            lastGroupId = curGroupId;
                        }
                        else if (curGroupId < lastGroupId)
                        {
                            Debug.LogError("分组id不连续");
                        }
                    }
                    sw.WriteLine("");
                    sw.WriteLine("");
                    lastGroupId = 0;
                    for (int row = 1; row <= sheet.Cells.MaxDataRow; row++)
                    {
                        Cell itemCell = sheet.Cells[row, 0];
                        if (itemCell.Value == null) break;
                        string groupId = itemCell.Value.ToString();
                        itemCell = sheet.Cells[row, 1];
                        if (itemCell.Value == null) break;
                        string mapId = itemCell.Value.ToString();
                        int curGroupId = int.Parse(groupId);
                        if (curGroupId > lastGroupId)
                        {
                            sw.WriteLine("");
                            sw.WriteLine("");
                            lastGroupId = curGroupId;
                        }
                        sw.WriteLine("t[" + row + "] = \"" + mapId + "\";");
                    }
                    sw.WriteLine("");
                    sw.WriteLine("");
                    sw.WriteLine("return t;");
                    sw.Flush();
                }
            }
#endif
        }

#if UNITY_EDITOR
        private static Workbook OpenExcel(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return null;
            string path = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            Workbook book;
            try
            {
                book = File.Exists(fileName) ? new Workbook(fileName) : new Workbook();
                return book;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return null;
            }
        }
#endif
        public static void DeleteDir(string dir)
        {
            if (string.IsNullOrEmpty(dir))
            {
                return;
            }
            if (Directory.Exists(dir))
            {
                try
                {
                    Directory.Delete(dir, true);
                }
                catch(Exception e)
                {
                    Debug.LogError(e.Message);
                }
            }
        }

        public static void SaveMapJsonToDir(string mapId, string dir)
        {
            if (string.IsNullOrEmpty(mapId) || string.IsNullOrEmpty(dir))
            {
                return;
            }
            string sourceJsonFile = LocalStorageMgr.GetCacheRootDirectory() + "/Map/" + mapId + ".json";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            if (File.Exists(sourceJsonFile))
            {
                string targetPath = dir + "/" + mapId + ".json";
                File.Delete(targetPath);
                File.Copy(sourceJsonFile, targetPath);
            }
        }

        public static string ReadStringFromFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return "";
            }
            if (File.Exists(fileName))
            {
                using (FileStream fsw = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader sw = new StreamReader(fsw))
                    {
                        return sw.ReadToEnd();
                    }
                }
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 根据类索引获取类名字-后期查表
        /// </summary>
        /// <param name="classIndex"></param>
        /// <returns></returns>
        public static string GetClassName(int classIndex)
        {
            return "Prefab" + classIndex.ToString()+ "SerializeData";
        }

        /// <summary>
        /// 根据类名字获取类索引-后期查表
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public static int GetClassIndex(string className)
        {
            string result = System.Text.RegularExpressions.Regex.Replace(className, @"[^0-9]+", "");
            return int.Parse(result);
        }
    }
}