using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Prefab40Property : CustomerPropertyBase
{
    public MapSerializeData.Prefab40SerializeData serialization;

    public override void OnDeseriazlie(string json)
    {
        serialization = Serializable.ToObject<MapSerializeData.Prefab40SerializeData>(json);
    }

    void Start()
    {

    }

    void Update()
    {

    }

}





