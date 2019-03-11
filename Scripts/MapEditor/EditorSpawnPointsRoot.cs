/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MapEditor
{
    public class EditorSpawnPointsRoot : EditorSerializeBase
    {
        public override bool SerializeObject(Serializable.Map map)
        {
            var p = new Serializable.SpawnPoints();

            if (transform.childCount != 4 || p == null) return false;

            List<Vector3> list = new List<Vector3>();
            list.Add(this.transform.GetChild(0).position);
            list.Add(this.transform.GetChild(1).position);
            list.Add(this.transform.GetChild(2).position);
            list.Add(this.transform.GetChild(3).position);
            p.Fill<List<Vector3>>(list);
            map.spawn = p;
            return base.SerializeObject(map);//default ok
        }
        public override bool DeSerializeObject(Serializable.Map map)
        {
            var points = map.spawn;
            List<Transform> list = new List<Transform>();
            if (points.points.Count != transform.childCount || points.points.Count != 4) return false;
            for (int i = 0; i < points.points.Count; i++)
            {
                var child = transform.GetChild(i);
                list.Add(child);
            }
            map.spawn.Emplace<List<Transform>>(list);
     
            return base.DeSerializeObject(map);//default ok
        }
    }

}