using System;

using LuaInterface;

public interface IConfigItem
{
    int GetKey();
    void SetKey(int key);
    void CreateByLuaTable(LuaTable content);
}
