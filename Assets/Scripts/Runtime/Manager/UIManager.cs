using System;
using UnityEngine;
using System.Collections.Generic;

namespace Com.VT
{
    public class UIManager : MonoBehaviour
    {
        private int orderIndex = 1;
        private int orderInterval = 10;

        #region Instance;
        private static UIManager mInstance = null;

        public static UIManager Instance
        {
            get
            {
                if (mInstance == null)
                {
                    GameObject Uimanger = new GameObject("UIManager");
                    mInstance = Uimanger.AddComponent<UIManager>();
                    GameObject.DontDestroyOnLoad(Uimanger);
                }
                return mInstance;
            }
        }
        #endregion

        private Stack<PanelBridge> mPanelStack = new Stack<PanelBridge>();

        /// <summary>
        /// 初始化UI数据
        /// 调用Lua初始化;
        /// </summary>
        /// <param name="uiName"></param>
        /// <param name="go"></param>
        private void InitUI(string uiName, GameObject go, object[] args)
        {
            go.transform.SetParent(UIRoot);
            go.transform.ResetTransform();
            RectTransform rt = go.transform.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.pivot = Vector2.one * 0.5f;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            go.SetActive(true);
            var canvas = go.GetComponent<Canvas>();
            if (canvas != null)
            {
                orderIndex += orderInterval;
                canvas.overrideSorting = true;
                canvas.sortingLayerID = SortingLayer.NameToID("Default");
                canvas.sortingOrder = orderIndex;
            }
            string luaViewName = UIViewUtils.GetLuaViewName(uiName);

            PanelBridge.SetPanelData(go.GetInstanceID(), uiName, args);
            PanelBridge panelBridge = go.AddComponent<PanelBridge>();
            mPanelStack.Push(panelBridge);
        }

        public bool IsWindowOpen(string uiName)
        {
            foreach (var item in mPanelStack)
            {
                if (item.UIName.Equals(uiName))
                {
                    return true;
                }
            }
            return false;
        }

        public void OpenWindow(string uiName)
        {
            OpenWindow(uiName, null);
        }

        /// <summary>
        /// 打开某个UI界面;
        /// </summary>
        /// <param name="uiName"></param>
        public void OpenWindow(string uiName, params object[] args)
        {
            if (string.IsNullOrEmpty(uiName))
            {
                Debug.LogError("UI Name can not empty !");
                return;
            }
            GetPrefab(uiName, InitUI, args);
        }

        /// <summary>
        /// 获取某个UI的数据;
        /// </summary>
        /// <param name="uiName"></param>
        /// <returns></returns>
        public PanelBridge GetWindowData(string uiName)
        {
            foreach (var item in mPanelStack)
            {
                if (item.UIName.Equals(uiName))
                {
                    return item;
                }
            }
            return null;
        }

        /// <summary>
        /// 关闭最后一个页面;
        /// </summary>
        public void CloseWindow()
        {
            if (updating)
            {
                mToRemove.Add(null);
                return;
            }
            if (mPanelStack.Count > 0)
            {
                PanelBridge data = mPanelStack.Pop();
                GameObject.Destroy(data.gameObject);
            }
        }

        /// <summary>
        /// 关闭某个固定页面，并且一同关闭在它后面的所有UI界面;
        /// </summary>
        /// <param name="uiName"></param>
        public void CloseWindowByName(string uiName)
        {
            if (updating)
            {
                mToRemove.Add(uiName);
                return;
            }
            bool flag = false;
            foreach (var item in mPanelStack)
            {
                if (item.UIName.Equals(uiName))
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                while (mPanelStack.Count > 0)
                {
                    PanelBridge panelBridge = mPanelStack.Pop();
                    if (panelBridge.UIName.Equals(uiName))
                    {
                        GameObject.Destroy(panelBridge.gameObject);
                        break;
                    }
                    else
                    {
                        GameObject.Destroy(panelBridge.gameObject);
                    }
                }
            }
        }
        private bool isCloseAll = false;
        /// <summary>
        /// 关闭所有UI界面;
        /// </summary>
        public void CloseAllWindow()
        {
            orderIndex = 1;
            if (updating)
            {
                isCloseAll = true;
                return;
            }
            while (mPanelStack.Count > 0)
            {
                PanelBridge panelBridge = mPanelStack.Pop();
                GameObject.Destroy(panelBridge.gameObject);
            }
            isCloseAll = false;
        }

        public string GetCurUIName()
        {
            if (mPanelStack != null && mPanelStack.Count > 0)
            {
                return mPanelStack.Peek().UIName;
            }
            return "There is no UI here";
        }

        bool updating = false;
        List<string> mToRemove = new List<string>();
        /// <summary>
        /// 调用到Lua里面的Update;
        /// </summary>
        private void Update()
        {
            if (isCloseAll)
            {
                CloseAllWindow();
                mToRemove.Clear();
            }
            else
            {
                for (int i = 0; i < mToRemove.Count; i++)
                {
                    string ui = mToRemove[i];
                    if (string.IsNullOrEmpty(ui))
                    {
                        CloseWindow();
                    }
                    else
                    {
                        CloseWindowByName(ui);
                    }
                }
                mToRemove.Clear();
            }
        }

        #region Utils;
        private string mPrefabDirPath = "UI/Prefab/{0}";

        /// <summary>
        /// 获取prefab，采用回调策略，预支持异步加载;
        /// </summary>
        /// <param name="uiName"></param>
        /// <param name="action"></param>
        private void GetPrefab(string uiName, Action<string, GameObject, object[]> action, object[] args)
        {
            string path = GetPrefabPath(uiName);
            GameObject _tmpGo = Resources.Load(path) as GameObject;
            if (_tmpGo == null)
            {
                Debug.LogError("ui prefab path is error: " + path);
                return;
            }
            GameObject go = GameObject.Instantiate(_tmpGo);
            if (action != null)
            {
                action(uiName, go, args);
            }
            else
            {
                Debug.LogError("action is null!!!");
            }
        }

        private string GetPrefabPath(string uiName)
        {
            return string.Format(mPrefabDirPath, uiName);
        }

        private Transform uiRoot;
        private Transform UIRoot
        {
            get
            {
                if (uiRoot == null)
                {
                    GameObject go = GameObject.Find("UIRoot");
                    if (go != null)
                    {
                        uiRoot = go.transform;
                    }
                }
                return uiRoot;
            }
        }
        #endregion
    }
}