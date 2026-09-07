using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Client
{

    public class MessageSystemParam
    {
    }
    public class MessageManager : MonoBehaviour, IEventSystemHandler
    {
        #region Singleton
        static MessageManager _instance;
        public static MessageManager Instance { get { Init(); return _instance; } }
        MessageManager() { }

        private static void Init()
        {
            if (_instance == null)
            {
                GameObject gm = GameObject.Find("MessageManager");
                if (gm == null)
                {
                    gm = new GameObject { name = "MessageManager" };
                    gm.AddComponent<MessageManager>();
                }

                _instance = gm.GetComponent<MessageManager>();
                DontDestroyOnLoad(gm);
            }
        }

        #endregion


        private Dictionary<Type, Action<MessageSystemParam>> _messageSystemMap = new();
        private Dictionary<object, Dictionary<Type, Action<MessageSystemParam>>> _messageCache = new();
        internal Dictionary<object, Dictionary<Type, Action<MessageSystemParam>>> MessageCache => _messageCache;
        

        // Start is called before the first frame update
        void Start()
        {
            Init();
        }

        public static void SendMessage<T>(T messageBase) where T : MessageSystemParam
        {
            if (Instance._messageSystemMap.ContainsKey(messageBase.GetType()))
            {
                if (Instance._messageSystemMap[messageBase.GetType()] == null)
                    return;

                Instance._messageSystemMap[messageBase.GetType()].Invoke(messageBase);
            }
        }

        public static void SubscribeMessage<T>(object recver ,Action<T> action) where T : MessageSystemParam
        {
            if (action == null || recver == null)
                return;

            Action<MessageSystemParam> convertedAction = param => action((T)param);

            if (Instance._messageSystemMap.ContainsKey(typeof(T)) == false)
            {
                Instance._messageSystemMap.Add(typeof(T), convertedAction);
            }
            else
            {
                Instance._messageSystemMap[typeof(T)] += convertedAction;
            }


            if (Instance._messageCache.ContainsKey(recver) == false)
            {
                Instance._messageCache.Add(recver, new());
            }

            if (Instance._messageCache[recver].ContainsKey(typeof(T)) == false)
            {
                Instance._messageCache[recver].Add(typeof(T), convertedAction);
            }
            else
            {
                Instance._messageCache[recver][typeof(T)] += convertedAction;
            }

        }

        //public static void RomoveMessage<T>(object recver) where T : MessageSystemParam
        //{
        //    if (recver == null)
        //        return;
        //    if (Instance._messageSystemMap.ContainsKey(typeof(T)) == false || Instance._messageCache.ContainsKey(recver) == false)
        //        return;
        //    if (Instance._messageCache[recver].ContainsKey(typeof(T)) == false)
        //        return;
        //
        //    Instance._messageSystemMap[typeof(T)] -= Instance._messageCache[recver][typeof(T)];
        //}

        public static void RemoveMessageAll(object recver)
        {
            if (recver == null)
                return;
            List<Type> removeType = new();
            foreach(var type in Instance._messageCache[recver].Keys)
            {
                if (type == null)
                    return;
                removeType.Add(type);
            }
            foreach (var type in removeType)
            {
                Instance._messageSystemMap[type] -= Instance._messageCache[recver][type];
                if (Instance._messageSystemMap[type] == null)
                {
                    Instance._messageSystemMap.Remove(type);
                }
            }
            Instance._messageCache.Remove(recver);
        }

    }
}