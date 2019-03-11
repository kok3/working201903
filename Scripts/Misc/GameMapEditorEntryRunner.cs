/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMapEditorEntryRunner : MonoBehaviour
{
    //创作地图，进入地图编辑场景
    public void OnGoToMapEditor()
    {
        Application.LoadLevel("MapEditor");
    }
    void Start()
    {

    }
}
