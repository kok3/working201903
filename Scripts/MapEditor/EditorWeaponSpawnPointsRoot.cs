/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MapEditor
{
    public class EditorWeaponSpawnPointsRoot : EditorSerializeBase
    {
        public override bool SerializeObject(Serializable.Map map)
        {
            var p = new Serializable.WeaponSpawnPoints();

            if (transform.childCount <= 0 || p == null) return false;

            List<EditorWeaponSpawnPointTag> list = new List<EditorWeaponSpawnPointTag>();

            foreach (var pp in transform.GetComponentsInChildren<EditorWeaponSpawnPointTag>())
            {
                list.Add(pp);
            }
            p.Fill<List<EditorWeaponSpawnPointTag>>(list);
            map.weapon_spawn_points = p;
            return base.SerializeObject(map);//default ok
        }
        public override bool DeSerializeObject(Serializable.Map map)
        {
            var points = map.weapon_spawn_points;
            List<EditorWeaponSpawnPointTag> list = new List<EditorWeaponSpawnPointTag>();

            while (transform.childCount < points.points.Count)
            {
                //实际武器点大于阈值 需要扩容  editor 也要处理这个
                GameObject x = GameObject.Instantiate<GameObject>(transform.GetChild(0).gameObject, transform, false);
                x.name = "p" + (transform.childCount + 1);
            }

            if (points == null || points.points.Count > transform.childCount || points.points.Count <= 0) return false;
            int i = 0;
            for (i = 0; i < points.points.Count; i++)
            {
                var child = transform.GetChild(i);
                var c = child.FetchComponent<EditorWeaponSpawnPointTag>();
                list.Add(c);
            }
            //emplace
            map.weapon_spawn_points.Emplace<List<EditorWeaponSpawnPointTag>>(list);


            /*   //destroy un-needed
               List<Transform> need_destroy = new List<Transform>();
               for (; i < 4; i++)
               {
                   need_destroy.Add(transform.GetChild(i));
               }
               foreach (var p in need_destroy)
               {
                   GameObject.DestroyImmediate(p.gameObject);
               }
               need_destroy.Clear();
               */

            return base.DeSerializeObject(map);//default ok
        }
    }

}