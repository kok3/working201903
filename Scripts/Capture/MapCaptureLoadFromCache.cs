/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
//MapRuntimeCapture 要加载场景 如果缓存图片存在的话 是没必要的 因此 优先用这个  内部自动处理
//外部不要调用这里的接口 请调用 MapRuntimeCapture提供的接口
namespace MapEditor
{
    public class MapCaptureLoadFromCache : MonoBehaviour
    {
        static MapCaptureLoadFromCache ins = null;
        void Start()
        {
            ins = this;
        }
        void OnDestroy()
        {
            if (ins == this)
            {
                ins = null;
            }
        }
        public static void ClearAll()
        {
            if (ins != null)
            {
                ins.StopAllCoroutines();
            }
        }
        public static bool LoadFromCacheAsyncCapture(string uuid, VoidFuncObject cb, int WIDTH, int HEIGHT, bool showWeapon = false)
        {
            if (ins == null || string.IsNullOrEmpty(uuid) || cb == null || WIDTH <= 0 || HEIGHT <= 0)
            {
                return false;
            }
            string weaponExtension = showWeapon ? "_weapon" : "";
            string file_name = LocalStorageMapCaptureImage.ins.GetRootDirectory() + "/" + uuid.ToString() + "_" + WIDTH + "_" + HEIGHT + weaponExtension + ".jpg";
            if (File.Exists(file_name))
            {
                //缓存存在 直接读取
                ins.StartCoroutine(ins.LoadFromCache_AsyncCapture(file_name, uuid, cb, WIDTH, HEIGHT, showWeapon));
                return true;
            }
            return false;
        }
        IEnumerator LoadFromCache_AsyncCapture(string file_name, string uuid, VoidFuncObject cb, int WIDTH, int HEIGHT, bool showWeapon = false)
        {
            //  throw new NullReferenceException();
            Texture2D tex = null;
            //try load from disk
            string weaponExtension = showWeapon ? "_weapon" : "";
            var www_local = LocalStorageMapCaptureImage.ins.LoadFromDisk(uuid.ToString() + "_" + WIDTH + "_" + HEIGHT + weaponExtension + ".jpg");
            if (www_local != null)
            {
                using (www_local)
                {
                    yield return www_local;
                    if (www_local.isDone && string.IsNullOrEmpty(www_local.error))
                    {
                        tex = www_local.texture;
                    }
                }
            }
            if (tex != null)
            {
                if (cb != null)
                {
                    cb(tex);
                }
                else
                {
                    GameObject.DestroyImmediate(tex);
                }
            }
            tex = null;
        }

        public static bool LoadFromCacheAsyncCaptureLocalMapTmp(string uuid, VoidFuncObject cb, int WIDTH, int HEIGHT)
        {
            if (ins == null || string.IsNullOrEmpty(uuid) || cb == null || WIDTH <= 0 || HEIGHT <= 0)
            {
                return false;
            }
            string file_name = LocalStorageMapCaptureImage.ins.GetRootDirectory() + "/" + "tmp" + uuid.ToString() + "_" + WIDTH + "_" + HEIGHT + ".jpg";
            if (File.Exists(file_name))
            {
                //缓存存在 直接读取
                ins.StartCoroutine(ins.LoadFromCache_AsyncCaptureLocalMapTmp(file_name, uuid, cb, WIDTH, HEIGHT));
                return true;
            }
            return false;
        }
        IEnumerator LoadFromCache_AsyncCaptureLocalMapTmp(string file_name, string uuid, VoidFuncObject cb, int WIDTH, int HEIGHT)
        {
            //本地的都带了tmp字样
            //  throw new NullReferenceException();
            Texture2D tex = null;
            //try load from disk
            var www_local = LocalStorageMapCaptureImage.ins.LoadFromDisk("tmp" + uuid.ToString() + "_" + WIDTH + "_" + HEIGHT + ".jpg");
            if (www_local != null)
            {
                using (www_local)
                {
                    yield return www_local;
                    if (www_local.isDone && string.IsNullOrEmpty(www_local.error))
                    {
                        tex = www_local.texture;
                    }
                }
            }
            if (tex != null)
            {
                if (cb != null)
                {
                    cb(tex);
                }
                else
                {
                    GameObject.DestroyImmediate(tex);
                }
            }
            tex = null;
        }
    }
}