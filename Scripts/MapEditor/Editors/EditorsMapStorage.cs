using UnityEngine;

namespace MapEditor
{
    public class EditorsMapStorage : MonoBehaviour
    {
        public const string EditorsMapPrefabName = "MapEditorsMap";
        //[HideInInspector]
        public GameObject[] Editors;

        [HideInInspector]
        public bool IsDefaultMaterialEditorEnabled;
        [HideInInspector]
        public GameObject DefaultMaterialEditor;
        [HideInInspector]
        public Shader[] Shaders;
       // [HideInInspector]
        public GameObject[] MaterialEditors;
       // [HideInInspector]
        public bool[] IsMaterialEditorEnabled;
     
    }
}
