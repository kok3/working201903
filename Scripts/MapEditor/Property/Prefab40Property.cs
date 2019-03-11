using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Prefab40Property : CustomerPropertyBase
{
    public MapSerializeData.Prefab40SerializeData serialization;

    //public void OnDeseriazlie(MapSerializeData.Prefab40SerializeData ss)
    //{
    //    serialization = ss;
    //}

    public override void OnDeseriazlie(string json)
    {
        serialization = Serializable.ToObject<MapSerializeData.Prefab40SerializeData>(json);
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


}





