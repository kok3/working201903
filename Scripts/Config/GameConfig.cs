using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LuaInterface;

public class GameConfig
{
    public static class ConfigName
    {
        public const string COMPONENT_CONFIG_NAME = "TBX.component_config";
    }

    private static GameConfig s_instance;
    public static GameConfig instance
    {
        get
        {
            if( s_instance == null )
            {
                s_instance = new GameConfig();
            }
            return s_instance;
        }
    }

    private Dictionary<string, Object> _allConfigDict;

    private GameConfig()
    {
        //这里写入k-v
        _allConfigDict = new Dictionary<string, Object>();
        _allConfigDict.Add(ConfigName.COMPONENT_CONFIG_NAME, new Dictionary<int, ComponentConfig>());
    }

    public void ReloadAllConfig()
    {
        _allConfigDict.Clear();
        this.InitAllConfig();
    }

    public void InitAllConfig()
    {
        this.InitConfig<ComponentConfig>(LuaMgr.ins.GetTable(ConfigName.COMPONENT_CONFIG_NAME));
    }

    public Dictionary<int, T> GetAllConfig<T>() where T : IConfigItem, new()
    {
        string typeName = typeof(T).Name;
        return ( _allConfigDict[typeName] as Dictionary<int, T>);
    }

    public T GetConfig<T>(int key) where T : IConfigItem, new()
    {
        Dictionary<int, T> allConfig = GetAllConfig<T>();
        if( allConfig.ContainsKey(key) == true )
        {
            return ( T )( allConfig[key] );
        }
        return default(T);
    }

   

    public ComponentConfig GetComponentConfig(int key)
    {
        string typeName = "ComponentConfig";
        Dictionary<int, ComponentConfig> allConfig = _allConfigDict[typeName] as Dictionary<int, ComponentConfig>;
        if (allConfig.ContainsKey(key) == true)
        {
            return (allConfig[key]);
        }
        return default(ComponentConfig);
    }


    /// <summary>
    /// 获取所有分组信息
    /// </summary>
    /// <returns></returns>
    public int[] GetComponentGroupInfo()
    {
        Dictionary<int, int> groupDic = new Dictionary<int, int>();

        string typeName = "ComponentConfig";
        Dictionary<int, ComponentConfig> allConfig = _allConfigDict[typeName] as Dictionary<int, ComponentConfig>;
        if (allConfig != null)
        {
            foreach(var kvp in allConfig)
            {
                if (!groupDic.ContainsKey(kvp.Value.group_id))
                {
                    groupDic.Add(kvp.Value.group_id, kvp.Value.group_id);
                }
            }
        }

        return groupDic.Values.ToArray();

    }

    /// <summary>
    /// 根据分组Id返回所有的组件
    /// </summary>
    /// <param name="groupId"></param>
    /// <returns></returns>
    public ComponentConfig[] GetComponentsByGroup(int groupId)
    {
        List<ComponentConfig> list = new List<ComponentConfig>();
        string typeName = "ComponentConfig";
        Dictionary<int, ComponentConfig> allConfig = _allConfigDict[typeName] as Dictionary<int, ComponentConfig>;
        if (allConfig != null)
        {
            foreach (var kvp in allConfig)
            {
                if (kvp.Value.group_id == groupId)
                {
                    list.Add(kvp.Value);
                }
            }
        }

        return list.ToArray();
    }

    private void InitConfig<T>(LuaTable luaTable) where T : IConfigItem, new()
    {
        string typeName = typeof( T ).Name;
        if( _allConfigDict.ContainsKey( typeName ) )
        {
            return;
        }

        try
        {
            bool hasRecord = false;
            Dictionary<int, T> dict = new Dictionary<int, T>();
            LuaDictTable table = luaTable.ToDictTable();
            luaTable.Dispose();
            var iter2 = table.GetEnumerator();
            while (iter2 != null)
            {
                var one = iter2.Current.Key;
                LuaTable content = iter2.Current.Value as LuaTable;
                if (content != null)
                {
                    T configItem = new T();
                    configItem.SetKey(int.Parse(one.ToString()));
                    configItem.CreateByLuaTable(content);
                    if (dict.ContainsKey(configItem.GetKey()))
                    {
                        UnityEngine.Debug.LogError(string.Format("[{0}][{1}]配置表key重复：{2}", typeof(T), one, configItem.GetKey()));
                    }
                    else
                    {
                        hasRecord = true;
                        dict.Add(configItem.GetKey(), configItem);
                    }
                }

                //临时解决读表结束不退出循环
                if ((one == null) && hasRecord)
                {
                    break;
                }
                else
                {
                    iter2.MoveNext();
                }

            }
            _allConfigDict.Add(typeName, dict);
            table.Dispose();
        }
        catch(Exception e)
        {
            UnityEngine.Debug.LogError(e);
        }
     
    }

}