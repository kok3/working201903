﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapSerializeData
{
    [System.Serializable]
    public class Prefab40SerializeData : Serializable.SerializeBase
    {
        public Serializable.Vector3 start;
        public float edgebox_left;
        public Serializable.Vector3 end;
        public float edgebox_right;
        public float edgebox_down;
        public float edgebox_up;
    }

    [System.Serializable]
    public class Prefab37SerializeData : Serializable.SerializeBase
    {
        public Serializable.Vector3 left;
        public Serializable.Vector3 right;
    }
}


