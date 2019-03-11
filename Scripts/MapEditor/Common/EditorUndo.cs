using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using System.Collections;
using System.Reflection;


namespace MapEditor
{
    public delegate bool ApplyCallback(Record record);
    public delegate void PurgeCallback(Record record);

    public class Record
    {
        private object m_state;
        private object m_target;
        private ApplyCallback m_applyCallback;
        private PurgeCallback m_purgeCallback;

        public object Target
        {
            get { return m_target; }
        }

        public object State
        {
            get
            {
                return m_state;
            }
        }

        public Record(object target, object state, ApplyCallback applyCallback, PurgeCallback purgeCallback)
        {
            if (applyCallback == null)
            {
                throw new System.ArgumentNullException("callback");
            }

            m_target = target;
            m_applyCallback = applyCallback;
            m_purgeCallback = purgeCallback;
            if (state != null)
            {
                m_state = state;
            }
        }

        public bool Apply()
        {
            return m_applyCallback(this);
        }

        public void Purge()
        {
            m_purgeCallback(this);
        }
    }

    public class UndoStack<T> : IEnumerable
    {
        private int m_tosIndex;
        private T[] m_buffer;
        private int m_count;
        private int m_totalCount;

        public int Count
        {
            get
            {
                return m_count;
            }
        }

        public bool CanPop
        {
            get { return m_count > 0; }
        }

        public bool CanRestore
        {
            get { return m_count < m_totalCount; }
        }

        public UndoStack(int size)
        {
            if (size == 0)
            {
                throw new System.ArgumentException("size should be greater than 0", "size");
            }
            m_buffer = new T[size];
        }

        public T Push(T item)
        {
            T purge = m_buffer[m_tosIndex];
            m_buffer[m_tosIndex] = item;
            m_tosIndex++;
            m_tosIndex %= m_buffer.Length;
            if (m_count < m_buffer.Length)
            {
                m_count++;
                purge = default(T);
            }
            m_totalCount = m_count;
            return purge;
        }

        public T Restore()
        {
            if (!CanRestore)
            {
                throw new System.InvalidOperationException("nothing to restore");
            }
            if (m_count < m_totalCount)
            {
                m_tosIndex++;
                m_tosIndex %= m_buffer.Length;
                m_count++;
            }
            return Peek();
        }

        public T Peek()
        {
            if (m_count == 0)
            {
                throw new System.InvalidOperationException("Stack is empty");
            }

            int index = m_tosIndex - 1;
            if (index < 0)
            {
                index = m_buffer.Length - 1;
            }

            return m_buffer[index];
        }

        public T Pop()
        {
            if (m_count == 0)
            {
                throw new System.InvalidOperationException("Stack is empty");
            }

            m_count--;
            m_tosIndex--;
            if (m_tosIndex < 0)
            {
                m_tosIndex = m_buffer.Length - 1;
            }
            return m_buffer[m_tosIndex];
        }

        public void Clear()
        {
            m_tosIndex = 0;
            m_count = 0;
            m_totalCount = 0;

            for (int i = 0; i < m_buffer.Length; ++i)
            {
                m_buffer[i] = default(T);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_buffer.GetEnumerator();
        }
    }

    public delegate void RuntimeUndoEventHandler();

    /// <summary>
    /// Undo/Redo
    /// </summary>
    public static class EditorUndo
    {
        public static event RuntimeUndoEventHandler BeforeUndo;
        public static event RuntimeUndoEventHandler UndoCompleted;
        public static event RuntimeUndoEventHandler BeforeRedo;
        public static event RuntimeUndoEventHandler RedoCompleted;
        public static event RuntimeUndoEventHandler StateChanged;

        private static List<Record> m_group;
        private static UndoStack<Record[]> m_stack;
        private static Stack<UndoStack<Record[]>> m_stacks;

        public const int Limit = 1024;

        public static bool Enabled
        {
            get;
            set;
        }

        public static bool CanUndo
        {
            get { return m_stack.CanPop; }
        }

        public static bool CanRedo
        {
            get { return m_stack.CanRestore; }
        }

        static EditorUndo()
        {
            Reset();
        }

        public static void Reset()
        {
            Enabled = true;
            m_group = null;
            m_stack = new UndoStack<Record[]>(Limit);
            m_stacks = new Stack<UndoStack<Record[]>>();
        }

        public static bool IsRecording
        {
            get { return m_group != null; }
        }

        public static void BeginRecord()
        {
            if (!Enabled)
            {
                return;
            }

            m_group = new List<Record>();
        }

        public static void EndRecord()
        {
            if (!Enabled)
            {
                return;
            }

            if (m_group != null)
            {
                Record[] purgeItems = m_stack.Push(m_group.ToArray());
                if (purgeItems != null)
                {
                    for (int i = 0; i < purgeItems.Length; ++i)
                    {
                        purgeItems[i].Purge();
                    }
                }

                if (StateChanged != null)
                {
                    StateChanged();
                }
            }
            m_group = null;
        }

        private class BoolState
        {
            public bool value;
            public BoolState(bool v)
            {
                value = v;
            }
        }

        private static void RecordActivateDeactivate(GameObject g, BoolState value)
        {
            RecordObject(g, value,
               record =>
               {
                   GameObject target = (GameObject)record.Target;
                   BoolState activate = (BoolState)record.State;
                   if (target && target.activeSelf != activate.value)
                   {
                       ExposeToEditor exposeToEditor = target.GetComponent<ExposeToEditor>();
                       if (exposeToEditor)
                       {
                           exposeToEditor.MarkAsDestroyed = !activate.value;
                       }
                       else
                       {
                           target.SetActive(activate.value);
                       }
                       return true;
                   }
                   return false;
               },
               record =>
               {
                   BoolState activate = (BoolState)record.State;
                   if (activate.value)
                   {
                       return;
                   }
                   GameObject target = (GameObject)record.Target;
                   if (target)
                   {
                       ExposeToEditor exposeToEditor = target.GetComponent<ExposeToEditor>();
                       if (exposeToEditor)
                       {
                           if (exposeToEditor.MarkAsDestroyed)
                           {
                               Object.DestroyImmediate(target);
                           }
                       }
                       else
                       {
                           if (!target.activeSelf)
                           {
                               Object.DestroyImmediate(target);
                           }
                       }
                   }
               });
        }

        public static void BeginRegisterCreateObject(GameObject g)
        {
            if (!Enabled)
            {
                return;
            }
            RecordActivateDeactivate(g, new BoolState(false));
        }

        public static void RegisterCreatedObject(GameObject g)
        {
            if (!Enabled)
            {
                return;
            }

            ExposeToEditor exposeToEditor = g.GetComponent<ExposeToEditor>();
            if (exposeToEditor)
            {
                exposeToEditor.MarkAsDestroyed = false;
            }
            else
            {
                g.SetActive(true);
            }

            RecordActivateDeactivate(g, new BoolState(true));

        }

        public static void BeginDestroyObject(GameObject g)
        {
            if (!Enabled)
            {
                return;
            }
            RecordActivateDeactivate(g, new BoolState(true));
        }

        public static void DestroyObject(GameObject g)
        {
            if (!Enabled)
            {
                return;
            }

            ExposeToEditor exposeToEditor = g.GetComponent<ExposeToEditor>();
            if (exposeToEditor)
            {
                exposeToEditor.MarkAsDestroyed = true;
            }
            else
            {
                g.SetActive(false);
            }
            RecordActivateDeactivate(g, new BoolState(false));
        }


        public static void RecordObject(object target, object state, ApplyCallback applyCallback, PurgeCallback purgeCallback = null)
        {
            if (!Enabled)
            {
                return;
            }

            if (purgeCallback == null)
            {
                purgeCallback = record => { };
            }

            if (m_group != null)
            {
                m_group.Add(new Record(target, state, applyCallback, purgeCallback));
            }
            else
            {
                Record[] purgeItems = m_stack.Push(new[] { new Record(target, state, applyCallback, purgeCallback) });
                if (purgeItems != null)
                {
                    for (int i = 0; i < purgeItems.Length; ++i)
                    {
                        purgeItems[i].Purge();
                    }
                }

                if (StateChanged != null)
                {
                    StateChanged();
                }
            }
        }



        public static void Redo()
        {
            if (!Enabled)
            {
                return;
            }

            if (!m_stack.CanRestore)
            {
                return;
            }

            if (BeforeRedo != null)
            {
                BeforeRedo();
            }

            bool somethingHasChanged;
            do
            {
                somethingHasChanged = false;
                Record[] records = m_stack.Restore();
                for (int i = 0; i < records.Length; ++i)
                {
                    Record record = records[i];
                    somethingHasChanged |= record.Apply();
                }
            }
            while (!somethingHasChanged && m_stack.CanRestore);

            if (RedoCompleted != null)
            {
                RedoCompleted();
            }
        }

        public static void Undo()
        {
            if (!Enabled)
            {
                return;
            }

            if (!m_stack.CanPop)
            {
                return;
            }

            if (BeforeUndo != null)
            {
                BeforeUndo();
            }

            bool somethingHasChanged;
            do
            {
                somethingHasChanged = false;
                Record[] records = m_stack.Pop();
                for (int i = 0; i < records.Length; ++i)
                {
                    Record record = records[i];
                    somethingHasChanged |= record.Apply();
                }
            }
            while (!somethingHasChanged && m_stack.CanPop);

            if (UndoCompleted != null)
            {
                UndoCompleted();
            }
        }

        public static void Purge()
        {
            if (!Enabled)
            {
                return;
            }

            foreach (Record[] records in m_stack)
            {
                if (records != null)
                {
                    for (int i = 0; i < records.Length; ++i)
                    {
                        Record record = records[i];
                        record.Purge();
                    }
                }
            }
            m_stack.Clear();

            if (StateChanged != null)
            {
                StateChanged();
            }
        }

        public static void Store()
        {
            if (!Enabled)
            {
                return;
            }
            m_stacks.Push(m_stack);
            m_stack = new UndoStack<Record[]>(Limit);
            if (StateChanged != null)
            {
                StateChanged();
            }
        }

        public static void Restore()
        {
            if (!Enabled)
            {
                return;
            }

            if (m_stack != null)
            {
                m_stack.Clear();
            }

            if (m_stacks.Count > 0)
            {
                m_stack = m_stacks.Pop();
                if (StateChanged != null)
                {
                    StateChanged();
                }
            }
        }


    }
}
