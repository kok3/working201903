/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MapEditor
{
    //mono script for editor .  use this to serializable
    //该类 是所有序列化到MAP 中对象的基类 统一接口 个泛型化
    //该挂载在需要序列化的物体上 可 动态添加 
    public class EditorSerializeBase : MonoBehaviour
    {
        //editor 下的物体 序列化到 map 对象
        public virtual bool SerializeObject(Serializable.Map map)
        {
            //when ok return true
            return true;
        }
        // map 中的对象 反序列化到 runtime 下的物体
        public virtual bool DeSerializeObject(Serializable.Map map)
        {
            //when ok return true
            return true;
        }
    }

}
