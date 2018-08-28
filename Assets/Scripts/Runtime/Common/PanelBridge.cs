using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace Com.VT
{
    public class PanelBridge : MonoBehaviour 
    {
        private static Dictionary<int, string> uiInfoDict = new Dictionary<int, string>();
        private static Dictionary<int, object[]> uiDataDict = new Dictionary<int, object[]>();

        public string UIName = "";

        public static void SetPanelData(int instanceId, string uiName, object[] args)
        {
            uiInfoDict.Add<int, string>(instanceId, uiName);
            uiDataDict.Add<int, object[]>(instanceId, args);
        }

        private LuaTable LuaViewCls;
        
        private LuaFunction LuaAwake;
        private LuaFunction LuaStart;
        private LuaFunction LuaOnEnable;
        private LuaFunction LuaOnDisable;
        private LuaFunction LuaPreDestroy;
        private LuaFunction LuaOnDestroy;
        private LuaFunction LuaOnFocus;

        private void Awake()
        {
            int instanceId = gameObject.GetInstanceID();

            string uiName = uiInfoDict[instanceId];
            uiInfoDict.RemoveKey(instanceId);
            UIName = uiName;

            object[] uiData = uiDataDict[instanceId];
            uiDataDict.RemoveKey(instanceId);

            string luaViewClsName = UIViewUtils.GetLuaViewName(uiName);
            Func<GameObject, object[], LuaTable> ctorFunc = GameMain.GetInstance().GetGlobalTable().GetInPath<Func<GameObject, object[], LuaTable >>(luaViewClsName + ".New");
            LuaViewCls = ctorFunc(gameObject, uiData);
            InitLuaFunction();

            if (LuaAwake != null)
                LuaAwake.Action<LuaTable>(LuaViewCls);
        }

        private void InitLuaFunction()
        {
            if (LuaViewCls != null)
            {
                LuaViewCls.Get("Awake", out LuaAwake);
                LuaViewCls.Get("Start", out LuaStart);
                LuaViewCls.Get("OnEnable", out LuaOnEnable);
                LuaViewCls.Get("OnDisable", out LuaOnDisable);
                LuaViewCls.Get("PreDestroy", out LuaPreDestroy);
                LuaViewCls.Get("OnDestroy", out LuaOnDestroy);
                LuaViewCls.Get("OnFocus", out LuaOnFocus);
            }
        }

        private void OnDestroy()
        {
            if (LuaOnDestroy != null)
                LuaOnDestroy.Action<LuaTable>(LuaViewCls);

            int instanceId = gameObject.GetInstanceID();
            uiInfoDict.RemoveKey(instanceId);
            uiDataDict.RemoveKey(instanceId);
        }

        private void OnDisable()
        {
            if (LuaOnDisable != null)
                LuaOnDisable.Action<LuaTable>(LuaViewCls);
        }

        private void OnEnable()
        {
            if (LuaOnEnable != null)
                LuaOnEnable.Action<LuaTable>(LuaViewCls);
        }

        private void Start()
        {
            if (LuaStart != null)
                LuaStart.Action<LuaTable>(LuaViewCls);
        }

        //当前焦点UI;
        public void OnFocus(bool focus)
        {
            if (LuaOnFocus != null)
                LuaOnFocus.Action<LuaTable, bool>(LuaViewCls, focus);
        }

        //预销毁，用于异步UI提前处理一些事件;
        public void PreDestroy()
        {
            if (LuaPreDestroy != null)
                LuaPreDestroy.Action<LuaTable>(LuaViewCls);
        }
    }
}
