using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class NHelper
{
    public static string ParseObjectToString(object table)
    {
        if( table == null )
        {
            return "";
        }
        return table.ToString();
    }

    public static int ParseInt(string lineData)
    {
        if( string.IsNullOrEmpty(lineData) )
        {
            return 0;
        }
        return int.Parse(lineData);
    }

    public static float ParseFloat(string lineData)
    {
        if( lineData.Trim().Equals("") )
        {
            return 0f;
        }
        return float.Parse(lineData);
    }

    public static bool ParseBool(string lineData)
    {
        return ParseInt(lineData) != 0;
    }


    public static string[] ToStringArray(string dataStr, string delim = "|")
    {
        if (dataStr == "")
        {
            return new string[1] { "" };
        }
        return dataStr.Split(delim[0]);
    }

    public static bool[] ToBoolArray(string dataStr, string delim = ",")
    {
        if( dataStr == "" )
        {
            return new bool[1] { false };
        }

        string[] strArr = dataStr.Split(delim[0]);
        bool[] boolArr = new bool[strArr.Length];

        for( int i = 0; i < strArr.Length; i++ )
        {
            boolArr[i] = ParseBool(strArr[i]);
        }
        return boolArr;
    }
	
	public static int[] ToIntArray(string dataStr, string delim = ",")
	{
		if (dataStr == "")
		{
			return new int[0] {  };
		}
		
		string[] strArr = dataStr.Split(delim[0]);
		int[] numArr = new int[strArr.Length];
		
		for (int i = 0; i < strArr.Length; i++)
		{
            try
            {
                numArr[i] = int.Parse(strArr[i]);
            }
            catch( Exception )
            { 
                
            }
		}
		return numArr;
	}
	
	public static float[] ToFloatArray(string dataStr, string delim = ",", float rate = 1.0f)
	{
		if (string.IsNullOrEmpty(dataStr))
		{
			return new float[0] {};
		}
		
		string[] strArr = dataStr.Split(delim[0]);
		float[] numArr = new float[strArr.Length];
		for (int i = 0; i < strArr.Length; i++)
		{
			numArr[i] = float.Parse(strArr[i]) * rate;
		}
		return numArr;
	}

    public static Vector3 ToVector3(string dataStr, Vector3 defaultVector)
    {
        var floatArr = ToFloatArray( dataStr );
        if( floatArr.Length < 3 )
        {
            return defaultVector;
        }
        return new Vector3( floatArr[0], floatArr[1], floatArr[2] );
    }
	
	public static int[][] ToIntTable(string dataStr)
	{
		string[] strTableArr = dataStr.Split(";"[0]);
		int[][] numTable = new int[strTableArr.Length][];
		for (int i = 0; i < strTableArr.Length; i++)
		{
			numTable[i] = ToIntArray(strTableArr[i]);
		}
		return numTable;
	}
	
	public static Vector2[] ToVector2Array(string dataStr)
	{
		string[] strTableArr = dataStr.Split(";"[0]);
		Vector2[] vec2Arr = new Vector2[strTableArr.Length];
		for (int i = 0; i < strTableArr.Length; i++)
		{
			string[] strArr = strTableArr[i].Split(","[0]);
			vec2Arr[i] = new Vector2(int.Parse(strArr[0]), int.Parse(strArr[1]));
		}
		return vec2Arr;
	}
  
}

