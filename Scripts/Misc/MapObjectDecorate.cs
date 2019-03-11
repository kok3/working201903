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
    public class MapObjectDecorate : MapObjectBase
    {
        List<Collider> _colliders = new List<Collider>();

        void Awake()
        {
            _colliders = this.GetComponentsFully<Collider>();
        }
        public void DisableColliders()
        {
            for (int i = 0;i<_colliders.Count ;i++ )
            {
                _colliders[i].isTrigger = true;
            }
        }

        public void EnabledCollider()
        {
            for (int i = 0; i < _colliders.Count; i++)
            {
                _colliders[i].isTrigger = false;
            }
        }
    }

}