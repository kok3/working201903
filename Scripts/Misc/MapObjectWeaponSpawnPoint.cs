/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MapEditor
{
    //map object  which is decroate
    public class MapObjectWeaponSpawnPoint : MapObjectBase
    {
        public TextMesh txt = null;
        int order = -1;
        //武器id 数字对应顺序 内容是武器index
        public List<int> _weapon_ids = new List<int>();
        const int MAX_WEAPON_NUM_CAN_SELECT = 5;
        List<BoxCollider> _colliders = null;// = new List<BoxCollider>();
        public int Order
        {
            get
            {
                if (txt == null)
                {
                    return -1;
                }
                if (order == -1)
                {
                    order = int.TryParse(txt.text.Substring(2, txt.text.Length-2), out order) ? order : -1;
                }
                return order;
            }
        }
        public bool HasAnyWeapon()
        {
            for (int i = 0; i < _weapon_ids.Count; i++)
            {
                if (-1 != _weapon_ids[i]) return true;
            }
            return false;
        }
        void Awake()
        {
            this.txt = this.GetComponentInChildren<TextMesh>();

            //hide hight-light
            transform.GetChild(0).gameObject.SetActive(false);

            this._list_renderer = this.GetComponentsFully<Renderer>();
            foreach (var p in _list_renderer)
            {
                this._list_color_orign.Add(p.material.color);
            }
            _weapon_ids.Clear();
            for (int i = 0; i < MAX_WEAPON_NUM_CAN_SELECT; i++)
            {
                _weapon_ids.Add(-1);
            }
            _colliders = this.GetComponentsFully<BoxCollider>();
        }
        // remove un-select  index
        public void FillEmpty()
        {
            List<int> list = new List<int>();
            foreach (var p in _weapon_ids)
            {
                if (p != -1)
                {
                    list.Add(p);
                }
            }
            for (int i = list.Count; i < MAX_WEAPON_NUM_CAN_SELECT; i++)
            {
                list.Add(-1);
            }
            this._weapon_ids = list;
        }
        public bool IsValidForStartPreview()
        {
            //  int num = 0;
            foreach (var p in _weapon_ids)
            {
                if (p != -1)
                {
                    return true;
                }
            }
            //  if (num > 0) return true;
            return false;
        }
        public override bool CheckConflict()
        {
            //改为 不处理任何碰撞
            if (DevConfig.MapEditorDisableAllConflictCheck)
            {
                return false;
            }
          /*  //#1000170  改为不限制碰撞和增加物件上限为50
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
        public void OnClick(OneMapObjectWeapon who)
        {
            int id = who.id;
            bool find = false;
            //find
            for (int i = 0; i < MAX_WEAPON_NUM_CAN_SELECT; i++)
            {
                if (_weapon_ids[i] == id && _weapon_ids[i] != -1)
                {// current index is click and has exist weapon then  cancel it
                    who.SetNumber(0);
                    _weapon_ids[i] = -1;
                    find = true;
                }
            }
            if (find) return;
            //not find ,select a empty to place this id
            for (int i = 0; i < MAX_WEAPON_NUM_CAN_SELECT; i++)
            {
                if (_weapon_ids[i] == -1)
                {// current index is empty
                    _weapon_ids[i] = id;
                    who.SetNumber(i + 1);
                    break;
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

        List<Color> _list_color_orign = new List<Color>();
        List<Renderer> _list_renderer = new List<Renderer>();
    }

}