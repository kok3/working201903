/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapEditor
{
    public class SerializeRoot : MonoBehaviour
    {
       /* public string Serialize()
        {
            Serializable.Map map = new Serializable.Map();
            int childCount = transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (child.GetComponent<DontSerializeThisToJson>() != null) continue;
                Serializable.MapObject obj = new Serializable.MapObject();
                obj.Fill<Transform>(child);
                obj.Fill<string>(child.gameObject.name);
                map.objects.Add(obj);
            }
            return map.ToJson();
            return string.Empty;
        }
        void Start()
        {
            Debug.LogError("111111111111111111111111111122222222222222222222222222222222");

            var str = Serialize();
            Debug.LogError(str);

            Serializable.SaveToFile("", str);
            DeSerialize();

        }
        public void DeSerialize()
        {
            Debug.LogError(Application.dataPath);

            var map = Serializable.Map.FromJson(Serializable.LoadFromFile(""));
            GameObject root = new GameObject("SerializeRoot");
            root.transform.parent = transform.parent;
            root.transform.localRotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));
            foreach (var p in map.objects)
            {
                GameObject obj1 = PrefabsMgr.LoadMapObjectWithoutTheme(map.theme + "/" + p.prefab);
                Debug.LogError("load " + obj1.name);


                var obj = GameObject.Instantiate<GameObject>(obj1, root.transform);
                p.Emplace<UnityEngine.Transform>(obj.transform);
                obj.name = p.prefab;
            }


        }
        */
    }

}