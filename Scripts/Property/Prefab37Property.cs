using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prefab37Property : CustomerPropertyBase
{
    public MapSerializeData.Prefab37SerializeData serialization;


    public override void OnDeseriazlie(string json)
    {
        serialization = Serializable.ToObject<MapSerializeData.Prefab37SerializeData>(json);
    }

    //public void OnDeseriazlie(MapSerializeData.Prefab37SerializeData ss)
    //{
    //    serialization = ss;
    //}

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
