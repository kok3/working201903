/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MapEditor
{
    //map object  which is spawn point
    public class MapObjectSpawnPoint : MapObjectBase
    {
        List<BoxCollider> _colliders = null;// = new List<BoxCollider>();
        void Awake()
        {
            _colliders = this.GetComponentsFully<BoxCollider>();
        }

        void Start()
        {
            this._list_renderer = this.GetComponentsFully<Renderer>();
            foreach (var p in _list_renderer)
            {
                this._list_color_orign.Add(p.material.color);
            }
        }
        public override bool CheckConflict()
        {
            //改为 不处理任何碰撞
            if (DevConfig.MapEditorDisableAllConflictCheck)
            {
                return false;
            }
            /*     //#1000170  改为不限制碰撞和增加物件上限为50
                 return false;
     #if UNITY_EDITOR
                 //编辑器下  不检查碰撞
                 return false;
     #endif*/
            bool ok = false;
            foreach (var p in _colliders)
            {
                p.isTrigger = true;
            }
            foreach (var p in _colliders)
            {
                //  Debug.Log(p.gameObject.name);
                ok = Physics.CheckBox(p.transform.position, ((p.center + p.size) / 2f).Multiply(p.transform.lossyScale));
                if (ok)
                {
                    break;
                }
            }
            foreach (var p in _colliders)
            {
                p.isTrigger = false;
            }
            return ok;
        }

        public override void SetBright(bool bright)
        {
            if (bright)
            {
                foreach (var p in _list_renderer)
                {
                    var c = Color.red;
                    c.a = 0.5f;
                    p.material.color = c;
                }
            }
            else
            {
                for (int i = 0; i < _list_renderer.Count; i++)
                {
                    _list_renderer[i].material.color = _list_color_orign[i];
                }
            }
        }
        public void DisableColliders()
        {
            this.GetComponent<Collider>().isTrigger = true;
        }

        public void EnabledCollider()
        {
            this.GetComponent<Collider>().isTrigger = false;
        }
        List<Color> _list_color_orign = new List<Color>();
        List<Renderer> _list_renderer = new List<Renderer>();
    }

}