/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

namespace MapEditor
{
    public static class ConfigPanelUpSort
    {
        //主题1 可创建的武器 id ， 填入数字即可 该数字表示unity面板里面的武器数字前缀
        public static int[] weapon1 =
         {
            0,1,2,3,4,5,6,7,10,12,14,15,16,19,22,24,26,32,38,41
         };

        //主题2 可创建的武器id ， 填入数字即可 该数字表示unity面板里面的武器数字前缀
        public static int[] weapon3 =
         {
             0,1,2,3,4,5,6,7,10,12,14,15,16,19,22,24,26,32,38,41
         };


        //主题5 可创建的武器id ， 填入数字即可 该数字表示unity面板里面的武器数字前缀
        public static int[] weapon8 =
         {
            0,1,2,3,4,5,6,7,10,12,14,15,16,19,22,24,26,32,38,41
         };


        //拥有的主题 id  ，1表示主题1 ， 4表示主题4
        public static int[] theme =
        {
            1,3,8
        };




        public static int[] GetWeapons(int theme)
        {
            var type = typeof(MapEditorConfig);
            FieldInfo field = type.GetField("weapon" + theme.ToString());
            var ret = (int[])field.GetValue(null);

            return ret;
        }
        public static int[] GetWeapons()
        {
            return GetWeapons(CurrentSelectTheme);
        }
        public static int CurrentSelectTheme = 1;
    }

}