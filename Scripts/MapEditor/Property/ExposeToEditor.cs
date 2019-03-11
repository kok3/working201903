using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MapEditor
{
    public delegate void ExposeToEditorEvent(ExposeToEditor obj);

    [DisallowMultipleComponent]
    public class ExposeToEditor : MonoBehaviour
    {
        public static event ExposeToEditorEvent MarkAsDestroyedChanged;

        public static event ExposeToEditorEvent NameChanged;
        public static event ExposeToEditorEvent TransformChanged;

        public static event ExposeToEditorEvent Enabled;
        public static event ExposeToEditorEvent Disabled;

        //11
        private bool m_markAsDestroyed;
        public bool MarkAsDestroyed
        {
            get { return m_markAsDestroyed; }
            set
            {
                if (m_markAsDestroyed != value)
                {
                    m_markAsDestroyed = value;
                    gameObject.SetActive(!m_markAsDestroyed);
                    if (MarkAsDestroyedChanged != null)
                    {
                        MarkAsDestroyedChanged(this);
                    }
                }
            }
        }

    }
}
