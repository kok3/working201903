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
    public static class MapEditorConfig
    {
        /*       //主题1 可创建的武器 id ， 填入数字即可 该数字表示unity面板里面的武器数字前缀
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
               //主题1的排序顺序
               public static string[] theme1 =
               {		"36","37","38","39","40","41","42","43","44","45","46","11","21","25","2","27","28","29","6","7","8","9","10","15","16","20","22","23","24","1","5"
               };
               //主题3的排序顺序
               public static string[] theme3 =
               {         "39","40","41","42","43","44","45","46","47","48","49","33","14","17","9","8","1","2","4","5","6","10","11","12","13","18","19","20","22","24","28","32"
               };
               //主题8的排序顺序
               public static string[] theme8 =
               {    "62","63","64","65","50","51","52","53","54","55","56","57","58","59","60","61","32","33","34","21","27","14","1","2","3","4","8","9","15","16","22","23","25","26","28","31","35","36","37","38","39"
               };
               //主题1的排序顺序
               public static string[] theme_d1 =
               {
                   "30","31","32","33","34","35"
               };


               //主题3的排序顺序
               public static string[] theme_d3 =
               {
                   "35","36","37","38"
               };
               //主题8的排序顺序
               public static string[] theme_d8 =
               {
                   "46","47","48","49"
               };



               //拥有的主题 id  ，1表示主题1 ， 4表示主题4
               public static int[] theme =
               {
                   1,3,8
               };

               */

        //获取主题下的武器
        public static int[] GetWeapons(int theme)
        {
            // var type = typeof(MapEditorConfig);
            // FieldInfo field = type.GetField("weapon" + theme.ToString());
            //  var ret = (int[])field.GetValue(null);
            //   return ret;
#if UNITY_EDITOR
            var config = ConfigLoader.ins.GetConfig<ConfigMapEditorTheme>(theme.ToString() + "_gm");
#else
            var config = ConfigLoader.ins.GetConfig<ConfigMapEditorTheme>(theme.ToString());
#endif
            return config.weapon_id;
        }
        public static int[] GetWeapons()
        {
            return GetWeapons(CurrentSelectTheme);
        }


        //获取主题下的 物件
        public static int[] GetMapObject(int theme)
        {
            //  var type = typeof(MapEditorConfig);
            // FieldInfo field = type.GetField("theme" + theme.ToString());
            // var ret = (string[])field.GetValue(null);
            //    return ret;
#if UNITY_EDITOR
            var config = ConfigLoader.ins.GetConfig<ConfigMapEditorTheme>(theme.ToString() + "_gm");
#else
            var config = ConfigLoader.ins.GetConfig<ConfigMapEditorTheme>(theme.ToString());
#endif
            return config.object_id;
        }
        public static int[] GetMapObject()
        {
            return GetMapObject(CurrentSelectTheme);
        }


        //获取主题下的 装饰物件
        public static int[] GetMapObjectD(int theme)
        {
            // var type = typeof(MapEditorConfig);
            //  FieldInfo field = type.GetField("theme_d" + theme.ToString());
            // var ret = (string[])field.GetValue(null);
            //  return ret;
#if UNITY_EDITOR
            var config = ConfigLoader.ins.GetConfig<ConfigMapEditorTheme>(theme.ToString() + "_gm");
#else
            var config = ConfigLoader.ins.GetConfig<ConfigMapEditorTheme>(theme.ToString());
#endif
            return config.decorate_id;
        }
        public static int[] GetMapObjectD()
        {
            return GetMapObjectD(CurrentSelectTheme);
        }

        public static int CurrentSelectTheme = 1;

        //当前地图编辑器的模式
        public static MapGameMode CurrentMapGameMode = MapGameMode.Normal;
        
        public static bool GetNeedBgEffect(int theme)
        {
#if UNITY_EDITOR
            var config = ConfigLoader.ins.GetConfig<ConfigMapEditorTheme>(theme.ToString() + "_gm");
#else
            var config = ConfigLoader.ins.GetConfig<ConfigMapEditorTheme>(theme.ToString());
#endif
            return config.need_bg_effect;
        }

    }

}