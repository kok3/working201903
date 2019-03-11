/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MapEditor
{
    public class RenderToTextureInfo : MonoBehaviour
    {
    //  public  Vector3 position;
  //      Vector3 rotation;
    public      Vector3 scale = Vector3.one;
        void Awake()
        {
          //  position = transform.position;
           // rotation = transform.eulerAngles;
            //  scale = transform.localScale;
          //  Debug.LogError("11111111111");

        }
        public void ResetTransform(Transform parent)
        {
            transform.parent = parent;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = scale;
            //   transform.localPosition = Vector3.zero;
            // transform.rotation = Quaternion.identity;
        }
    }

}