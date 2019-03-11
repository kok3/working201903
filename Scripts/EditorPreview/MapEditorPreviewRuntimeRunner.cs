/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MapEditor
{
    public class MapEditorPreviewRuntimeRunner : MonoBehaviour
    {
        void Awake()
        {
            Debug.LogError("11111111111111111111111111111111");
            var s = this.GetComponent<MapEditor.RuntimeSerialize>();
            s.LoadFromJson(MapObjectRoot.record_json);
        }

        void Start()
        {

        }
    }
}