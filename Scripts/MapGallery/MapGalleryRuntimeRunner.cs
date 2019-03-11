/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MapEditor
{
    public class MapGalleryRuntimeRunner : MonoBehaviour
    {
        public void LoadFromJson(string json)
        {
            var s = this.GetComponent<MapEditor.RuntimeSerialize>();
            s.LoadFromJson(json);
        }
    }
}