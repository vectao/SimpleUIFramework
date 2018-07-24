using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.vt
{
    public class ObjectContainer : MonoBehaviour
    {
        [Serializable]
        public class sinle_obj_item
        {
            public string name;
            public Component component;
        }

        [SerializeField]
        private List<sinle_obj_item> objects = new List<sinle_obj_item>();

        private Dictionary<string, Component> objDict = null;

        private void InitObjDic()
        {
            objDict = new Dictionary<string, Component>();
            for (int i = 0; i < objects.Count; i++)
            {
                objDict.Add(objects[i].name, objects[i].component);
            }
        }

        private void Awake()
        {
            InitObjDic();
        }

        private void OnDestroy()
        {
            objects.ExClear();
            objDict.ExClear();
            objects = null;
            objDict = null;
        }

        public List<sinle_obj_item> GetObjectItems()
        {
            return objects;
        }

        public Component GetObjComponent(string name)
        {
            if (objDict == null)
            {
                Debug.LogError("ObjectContainer is missing or not init");
                return null;
            }
            else if (objDict[name] == null)
            {
                Debug.LogError("ObjectContainer is not exist obj --> " + name);
                return null;
            }
            else
            {
                return objDict[name];
            }
        }
    }
}
