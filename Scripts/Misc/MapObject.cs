/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
namespace MapEditor
{
    //map object 
    public class MapObject : MapObjectBase
    {
        //该数据用于存储 所有的地图物件用于处理碰撞规则问题 (物件之间不限制碰撞 但是和各种出生点之间有碰撞)
        public static HashSet<MapObject> _all_objs = new HashSet<MapObject>();

        List<BoxCollider> _colliders = null;// = new List<BoxCollider>();
        void Awake()
        {
            _colliders = this.GetComponentsFully<BoxCollider>();
            _all_objs.Add(this);
        }
        void OnDestroy()
        {
            _all_objs.Remove(this);
        }
        void Start()
        {
            foreach (var p in _colliders)
            {
                var mesh = p.GetComponent<MeshRenderer>();
                if (mesh != null)
                {
                    this._list_renderer.Add(mesh);
                }
            }
        }
        public override bool CheckConflict()
        {
            //改为 不处理任何碰撞
            if (DevConfig.MapEditorDisableAllConflictCheck)
            {
                return false;
            }
            /*    //#1000170  改为不限制碰撞和增加物件上限为50
                return false;
    #if UNITY_EDITOR
                //编辑器下  不检查碰撞
                return false;
    #endif*/
            //改为 物件之间不产生碰撞限制   出生点和武器出生点 和 物件之间 有碰撞限制
            //因此在判定时 需要禁用 所有的 物件  这个代价其实很大  暂时这么处理好了 因为这样改动最小
            if (!DevConfig.MapEditorEnableMapObjectToMapObjectConflictCheck)
            {
                foreach (var p in _all_objs)
                {
                    foreach (var pp in p._colliders)
                    {
                        pp.isTrigger = true;
                    }
                }
            }
            bool ok = false;
            foreach (var p in _colliders)
            {
                p.isTrigger = true;
            }
            foreach (var p in _colliders)
            {
                ok = Physics.CheckBox(p.transform.position, ((p.center + p.size) / 2f).Multiply(p.transform.lossyScale), transform.rotation);
                if (ok)
                {
                    //    Debug.LogError(p.gameObject.name);
                    break;
                }
            }
            foreach (var p in _colliders)
            {
                p.isTrigger = false;
            }

            //改为 物件之间不产生碰撞限制   出生点和武器出生点 和 物件之间 有碰撞限制
            //因此在判定时 需要禁用 所有的 物件  这个代价其实很大  暂时这么处理好了 因为这样改动最小
            if (!DevConfig.MapEditorEnableMapObjectToMapObjectConflictCheck)
            {
                foreach (var p in _all_objs)
                {
                    foreach (var pp in p._colliders)
                    {
                        pp.isTrigger = false;
                    }
                }
            }
            return ok;
        }

        public override void SetBright(bool bright)
        {
            foreach (var p in _list_renderer)
            {
                p.enabled = bright;
            }
        }
        List<MeshRenderer> _list_renderer = new List<MeshRenderer>();
    }

}