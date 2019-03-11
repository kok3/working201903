/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

// map http task uploader and downloader
//   other http task ,use HttpTaskMgr instead
public class MapHttpTask : MonoBehaviour
{
    public const string upload_url = "127.0.0.1:7013/upload.php";

#if UNITY_EDITOR
    public const string download_url = "p3k0b7ial.bkt.clouddn.com";
#else
        public const string download_url = "download.ldhcr.com";
#endif

    //"127.0.0.1/map/";
    public static MapHttpTask ins = null;
    private bool isWorking = false;

    //记录当前正在运行的 任务 防止一张地图重复请求(在运行中的时候)
    private HashSet<string> _running_task = new HashSet<string>();
    void Awake()
    {
        ins = this;
    }
    void OnDestroy()
    {
        ins = null;
    }
    GameObject host;// cb is valid tag, cb should is a member of MonoScript when gameobject destroy host will set-to null cb will not be call, so need host to check is valid
    public static bool UploadMap(string json, VoidFuncN<bool> cb, long file_uuid, GameObject host)
    {
        if (ins == null) return false;
        if (string.IsNullOrEmpty(json)) return false;
        if (file_uuid < 0) return false;
        if (cb == null) return false;
        if (host == null) return false;
        return ins.AddUploadTask(json, cb, file_uuid, host);
    }
    VoidFuncN<bool> cb;// if ok param bool is ok
    long file_uuid = -1;
    //return run ok ?
    public bool AddUploadTask(string json, VoidFuncN<bool> cb, long file_uuid, GameObject host)
    {
        if (string.IsNullOrEmpty(json)) return false;
        if (isWorking) return false;
        if (cb == null) return false;
        if (this.cb != null) return false;
        if (file_uuid < 0) return false;
        if (host == null) return false;
        isWorking = true;
        this.cb = cb;
        this.file_uuid = file_uuid;
        StartCoroutine(RunUploadTask(json));
        return true;
    }
    IEnumerator RunUploadTask(string json)
    {
        WWWForm form = new WWWForm();
        form.AddField("json_data", json);
        form.AddField("file_name", file_uuid.ToString());

        WWW www = new WWW(upload_url, form);

        yield return www;
        if (www.text == "ok")
        {
            if (host != null)
            {
                cb.Invoke(true);
            }
        }
        else if (www.text == "error")
        {
            if (host != null)
            {
                cb.Invoke(false);
            }
        }
        else
        {
            // upload php error
            Debug.LogError(www.text);
            if (host != null)
            {
                cb.Invoke(false);
            }
        }
#if UNITY_EDITOR
        Debug.Log("upload map response:" + www.text);
#endif
        cb = null;
        isWorking = false;
    }


    //for map editor
    //async download or load in local file cache
    public static IEnumerator Download(string name, VoidFuncString ok, VoidFuncVoid error = null, bool ignore_error = false)
    {        //load from local file cache

        var time = Utils.GetTimestampMiliseconds();

        string json = Serializable.LoadFromFileEx(name + ".json");
        bool save_file_ok = true;
        if (string.IsNullOrEmpty(json))
        {
#if UNITY_EDITOR
            Debug.Log(" missing map file cache  start download with www " + name);
#endif
            int try_times = 3;
            while (try_times > 0)
            {
                string url = Serializable.GetDownloadWWWUrl(name);
                var www = new WWW(url);
                yield return www;
#if UNITY_EDITOR
                Debug.Log(url + "  " + www.text);
#else
                Debug.Log(url);
#endif
                if (Serializable.Map.IsMapJson(www.text))
                {
                    save_file_ok = Serializable.SaveToFile(name + ".json", www.text);
                    //ok
                    json = www.text;
                    break;
                }
                else
                {
                    if (www.text.IndexOf("Document not found") > 0 && www.text.Length < 150)
                    {
                        Debug.LogError("MapHttpTask Fatal error:This Map HasNot exist in web server " + name + " url=" + url);
                        try_times = -1;
                        break;
                    }
                    //error
                    try_times--;
                    continue;
                }
            }
        }
        if (Serializable.Map.IsMapJson(json) == false || !save_file_ok)
        {
            BattleServer.TryDisconnected();
            if (!ignore_error)
            {
                if (UICommonDialog.ins != null)
                {
                    UICommonDialog.ins.ShowOKCancle("地图下载失败 [error " + name + "] ", () =>
                    {
                        Application.LoadLevel("GameLogin");
                    }, () =>
                    {
                        Application.LoadLevel("GameLogin");
                    });
                }
            }
            if (error != null)
            {
                error();
            }
        }
        else
        {
            if (ok != null)
            {
                ok(json);
            }
        }
        time = (Utils.GetTimestampMiliseconds() - time);
#if UNITY_EDITOR
        if (time > 2000)
        {
            Debug.LogError("  MapHttpTask.Download load   " + name + ".json time =" + time + "  ms");
        }
        else
        {
            Debug.Log("  MapHttpTask.Download load   " + name + ".json time =" + time + "  ms");
        }
#endif
    }
    public static IEnumerator AddDownloadTask(string uuid)
    {
        string name = uuid;
        //load from local file cache
        bool save_file_ok = true;
        var time = Utils.GetTimestampMiliseconds();
        string json = Serializable.LoadFromFileEx(name + ".json");
        if (string.IsNullOrEmpty(json))
        {
#if UNITY_EDITOR
            Debug.Log(" missing map file cache  start download with www " + name);
#endif
            int try_times = 3;
            while (try_times > 0)
            {
                var www = new WWW(Serializable.GetDownloadWWWUrl(name));
                yield return www;

#if UNITY_EDITOR
                Debug.Log(www.url + "  " + www.text);
#else
                Debug.Log(www.url);
#endif
                if (Serializable.Map.IsMapJson(www.text))
                {
                    save_file_ok = Serializable.SaveToFile(name + ".json", www.text);
                    json = www.text; ;
                    break;
                }
                else
                {
                    if (www.text.IndexOf("Document not found") > 0 && www.text.Length < 150)
                    {
                        Debug.LogError("MapHttpTask Fatal error:This Map HasNot exist in web server " + name + " url=" + www.url);
                        try_times = -1;
                        break;
                    }
                    //error 
                    try_times--;
                    continue;
                }
            }
        }
        if (Serializable.Map.IsMapJson(json) == false || !save_file_ok)
        {
            BattleServer.TryDisconnected();
            if (UICommonDialog.ins != null)
            {
                UICommonDialog.ins.ShowOKCancle("地图下载失败 [error " + uuid + "]", () =>
                {
                    Application.LoadLevel("GameLogin");
                }, () =>
                {
                    Application.LoadLevel("GameLogin");
                });
            }
        }
        time = (Utils.GetTimestampMiliseconds() - time);
#if UNITY_EDITOR
        if (time > 2000)
        {
            Debug.LogError("  MapHttpTask.AddDownloadTask load   " + name + ".json time =" + time + "  ms");
        }
        else
        {
            Debug.Log("  MapHttpTask.AddDownloadTask load   " + name + ".json time =" + time + "  ms");
        }
#endif
    }


    public void TryDownloadOnly(string uuid, VoidFuncVoid cb_ok, VoidFuncVoid cb_error)
    {
        if (string.IsNullOrEmpty(uuid)) return;
        if (this._running_task.Contains(uuid))
        {
#if UNITY_EDITOR
            Debug.LogWarning("MapHttpTask this map has downloading " + uuid);
#endif
            //has running
            return;
        }
        StartCoroutine(_CoTryDownloadOnly(uuid, cb_ok, cb_error));
    }
    IEnumerator _CoTryDownloadOnly(string uuid, VoidFuncVoid cb_ok, VoidFuncVoid cb_error)
    {
        string name = uuid;
        //load from local file cache
        bool save_file_ok = true;
        var time = Utils.GetTimestampMiliseconds();
        string json = Serializable.LoadFromFileEx(name + ".json");
        if (string.IsNullOrEmpty(json))
        {
#if UNITY_EDITOR
            Debug.Log(" missing map file cache  start download with www " + name);
#endif
            if (this._running_task.Contains(uuid))
            {

            }
            else
            {
                this._running_task.Add(uuid);
            }
            int try_times = 3;
            while (try_times > 0)
            {
                var www = new WWW(Serializable.GetDownloadWWWUrl(name));
                yield return www;

#if UNITY_EDITOR
                Debug.Log(www.url + "  " + www.text);
#else
              //  Debug.Log(www.url);
#endif
                if (Serializable.Map.IsMapJson(www.text))
                {
                    save_file_ok = Serializable.SaveToFile(name + ".json", www.text);
                    json = www.text; ;
                    break;
                }
                else
                {
                    if (www.text.IndexOf("Document not found") > 0 && www.text.Length < 150)
                    {
                        Debug.LogError("MapHttpTask Fatal error:This Map HasNot exist in web server " + name + " url=" + www.url);
                        try_times = -1;
                        break;
                    }
                    //error 
                    try_times--;
                    continue;
                }
            }
            if (this._running_task.Contains(uuid))
            {
                this._running_task.Remove(uuid);
            }
        }
        if (Serializable.Map.IsMapJson(json) == false || !save_file_ok)
        {
            BattleServer.TryDisconnected();
            if (cb_error != null)
            {
                cb_error();
            }
        }
        else
        {
            if (cb_ok != null)
            {
                cb_ok();
            }
        }
        time = (Utils.GetTimestampMiliseconds() - time);
#if UNITY_EDITOR
        if (time > 2000)
        {
            Debug.LogError("  MapHttpTask.AddDownloadTask load   " + name + ".json time =" + time + "  ms");
        }
        else
        {
            Debug.Log("  MapHttpTask.AddDownloadTask load   " + name + ".json time =" + time + "  ms");
        }
#endif
    }
}