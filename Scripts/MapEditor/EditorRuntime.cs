using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapEditor
{
    public class EditorRuntime : MonoBehaviour
    {
        public static void Delete(GameObject selectionObj)
        {
            if (selectionObj == null)
            {
                return;
            }

            EditorUndo.BeginRecord();

            EditorUndo.BeginDestroyObject(selectionObj);

            EditorUndo.EndRecord();

            bool isEnabled = EditorUndo.Enabled;
            EditorUndo.Enabled = false;
            EditorSelection.objects = null;
            EditorUndo.Enabled = isEnabled;

            EditorUndo.BeginRecord();

            EditorUndo.DestroyObject(selectionObj);

            EditorUndo.EndRecord();
        }
    }

}
