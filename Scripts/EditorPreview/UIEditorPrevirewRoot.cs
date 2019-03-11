/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MapEditor
{
    //when in map editor  ,
    public class UIEditorPrevirewRoot : MonoBehaviour
    {
        void Start()
        {
            Time.timeScale = 1f;
        }

        public void OnClickReturn()
        {

        }
        public void OnClickDone()
        {

        }
        public void OnClickEditAgain()
        {
            Application.LoadLevel("MapEditor");
         //   UIRoot.ins.EndPreView();
        }
    }

}