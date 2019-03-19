using UnityEngine;
using System.Collections;
using LuaInterface;

public class ComponentConfig : IConfigItem
{
    public int id;
    public string name;
    public string brief;
    public int theme_id;
    public int group_id;
    public string prefab_name;

    public int GetKey()
    {
        return this.id;
    }

    public void SetKey(int key)
    {
        this.id = key;
    }

    public void CreateByLuaTable(LuaTable luaTable)
    {
        this.name = NHelper.ParseObjectToString(luaTable["name"]);
        this.brief = NHelper.ParseObjectToString(luaTable["brief"]);
        this.theme_id = NHelper.ParseInt(NHelper.ParseObjectToString(luaTable["theme_id"]));
        this.group_id = NHelper.ParseInt(NHelper.ParseObjectToString(luaTable["group_id"]));
        this.prefab_name = NHelper.ParseObjectToString(luaTable["prefab_name"]);
        //this.desc = NHelper.ParseObjectToString(luaTable["desc"]);
    }


}
