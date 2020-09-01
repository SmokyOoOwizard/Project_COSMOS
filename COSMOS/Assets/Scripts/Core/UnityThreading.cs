using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace COSMOS.Core
{
    public static class UnityThreading
    {
        public enum Queue
        {
            Update,
            LateUpdate,
            FixedUpdate
        }
        class ExecuteObject : MonoBehaviour
        {
            Queue<Action> updateActions = new Queue<Action>();
            Queue<Action> lateUpdateActions = new Queue<Action>();
            Queue<Action> fixedUpdateActions = new Queue<Action>();

            public int CountOfQueueExecutePerUpdate = 100;
            public bool UseClampForUpdate = false;

            public int CountOfQueueExecutePerLateUpdate = 100;
            public bool UseClampForLateUpdate = false;

            public int CountOfQueueExecutePerFixedUpdate = 100;
            public bool UseClampForFixedUpdate = false;


            private void Update()
            {
                int maxI = UseClampForUpdate ? Mathf.Min(updateActions.Count, CountOfQueueExecutePerUpdate) : updateActions.Count;
                for (int i = 0; i < maxI; i++)
                {
                    updateActions.Dequeue()?.Invoke();
                }
            }
            private void LateUpdate()
            {
                int maxI = UseClampForLateUpdate ? Mathf.Min(lateUpdateActions.Count, CountOfQueueExecutePerLateUpdate) : lateUpdateActions.Count;
                for (int i = 0; i < maxI; i++)
                {
                    lateUpdateActions.Dequeue()?.Invoke();
                }
            }
            private void FixedUpdate()
            {
                int maxI = UseClampForFixedUpdate ? Mathf.Min(fixedUpdateActions.Count, CountOfQueueExecutePerFixedUpdate) : fixedUpdateActions.Count;
                for (int i = 0; i < maxI; i++)
                {
                    fixedUpdateActions.Dequeue()?.Invoke();
                }
            }

            public void Execute(Action action, Queue queue = Queue.Update)
            {
                //обеспечиваем потокобезопасность записи в лист
                switch (queue)
                {
                    case Queue.Update:
                        updateActions.Enqueue(action);
                        break;
                    case Queue.LateUpdate:
                        lateUpdateActions.Enqueue(action);
                        break;
                    case Queue.FixedUpdate:
                        fixedUpdateActions.Enqueue(action);
                        break;
                }
            }

        }
        static Dictionary<GameObject, ExecuteObject> ExecuteObjects = new Dictionary<GameObject, ExecuteObject>();
        static ExecuteObject MainObject;

        public static void Init()
        {
            ExecuteObjects.Clear();
            MainObject = new GameObject("Main threading object").AddComponent<ExecuteObject>();
            GameObject.DontDestroyOnLoad(MainObject.gameObject);
        }

        public static void Execute(Action action, Queue queue = Queue.Update)
        {
            MainObject.Execute(action, queue);
        }
        public static void Execute(GameObject go, Action action, Queue queue = Queue.Update)
        {
            if (go == null)
            {
                Execute(action, queue);
                return;
            }
            if (!ExecuteObjects.ContainsKey(go))
            {
                Execute(() => { ExecuteObjects.Add(go, go.AddComponent<ExecuteObject>()); ExecuteObjects[go].Execute(action, queue); });
                return;
            }
            ExecuteObjects[go].Execute(action, queue);
        }
        public static void WaitExecute(Action action, Queue queue = Queue.Update)
        {
            Thread thread = Thread.CurrentThread;
            if (Task.CurrentId.HasValue || !GameData.IsMainThread)
            {
                ManualResetEvent resetEvent = new ManualResetEvent(false);

                MainObject.Execute(() => { action?.Invoke(); resetEvent.Set(); }, queue);

                resetEvent.WaitOne();
            }
            else
            {
                MainObject.Execute(() => { action?.Invoke(); thread.Interrupt(); });
            }
        }

        public static void StartCoroutine(IEnumerator coroutine)
        {
            if (MainObject != null)
            {
                MainObject.StartCoroutine(coroutine);
            }
        }
        public static void StartCoroutine(IEnumerator coroutine, Action callback)
        {
            if (MainObject != null && coroutine != null)
            {
                MainObject.StartCoroutine(executeCoroutine(coroutine, callback));
            }
        }
        private static IEnumerator executeCoroutine(IEnumerator coroutine, Action callback)
        {
            if (coroutine != null)
            {
                while (coroutine.MoveNext())
                {
                    yield return coroutine.Current;
                }

                callback?.Invoke();
            }
        }
    }
}
