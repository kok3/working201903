/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorWeaponSpawnPointTag : MonoBehaviour
{//武器出生点 的具体信息
    // 武器刷新的id 顺序就是index
 //   [HideInInspector]
    public List<int> weapon_ids = new List<int>();

    //return -1 will dont spawn with this method called
    public int GetNextWeaponIndex()
    {
        if (weapon_ids.Count <= 0) return -1;
        if (idx >= weapon_ids.Count) idx = 0;

        for (int i = idx; i < weapon_ids.Count; i++)
        {
            if (weapon_ids[i] != -1)
            {
                idx = i + 1;
                return weapon_ids[i];
            }
        }
        for (int i = 0; i < idx; i++)
        {
            if (weapon_ids[i] != -1)
            {
                idx = i + 1;
                return weapon_ids[i];
            }
        }
        return -1;
    }
    private int idx = 0;
}