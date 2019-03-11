/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MapEditor
{
    //all map object base class .such as MapObject MapObjectDecorate
    //if want to use type  use  is   sucn as if(obj is MapObject) not enum ,
    //just for extensin easy
    public class MapObjectBase : MonoBehaviour
    {
        public virtual bool CheckConflict()
        {
            return false;
        }
        public virtual void SetBright(bool bright)
        {

        }
    /*    public void Awake()
        {
           var list = this.GetComponentsFully<BoxCollider>();
            foreach (var p in list)
            {
                this._bounds.Add(p.FetchComponent<AABBCollider>());
            }
       //     Debug.LogError(_bounds.Count + " ........  " + gameObject.name);
            transform.hasChanged = false;
            
        }*/
      /*  public bool CheckAABBColliderAll(MapObjectBase other)
        {
            foreach (var p in _bounds)
            {
                foreach (var pp in other._bounds)
                {
                    if (p.Intersects(pp))
                    {
                        cache = true;
                        return true;
                    }
                }
            }
            return false;
        }*/
      //  bool cache = false;
    //    protected List<AABBCollider> _bounds = new List<AABBCollider>();
    }

}