using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using XLua;

namespace com.vt
{
    public class GameMain : MonoBehaviour
    {
        /// <summary>
        /// Gets the UIRoot.
        /// </summary>
        /// <value>The UIR oot.</value>
        public RectTransform UIRoot { get; private set; }

        /// <summary>
        /// Gets the UICamera.
        /// </summary>
        /// <value>The UIC amera.</value>
        public Camera UICamera { get; private set; }

        internal static LuaEnv luaEnv = new LuaEnv(); //all lua behaviour shared one luaenv only!
        internal static float lastGCTime = 0;
        internal const float GCInterval = 1;//1 second 

        private Action luaStart;
        private Action luaUpdate;
        private Action luaOnDestroy;

        private LuaTable scriptEnv;

        private static GameMain instance;
        public static GameMain GetInstance()
        {
            return instance;
        }

        public LuaTable GetGlobalTable()
        {
            return scriptEnv;
        }

        private void Awake()
        {
            instance = this;
            InitDontDestroy();
            InitGamelogic();
        }

        /// <summary>
        /// Inits the dont destroy.
        /// </summary>
        private void InitDontDestroy()
        {
            GameObject uiroot = GameObject.Find("UIRoot");
            if(uiroot != null)
            {
                UIRoot = uiroot.transform as RectTransform;
            }
                
            GameObject uicamera = GameObject.Find("UICamera");
            if (UICamera != null)
            {
                UICamera = uicamera.GetComponent<Camera>();
            }
        }

        /// <summary>
        /// Inits the gamelogic.
        /// </summary>
        private void InitGamelogic()
        {
            scriptEnv = luaEnv.NewTable();

            LuaTable meta = luaEnv.NewTable();
            meta.Set("__index", luaEnv.Global);
            scriptEnv.SetMetaTable(meta);
            meta.Dispose();

            scriptEnv.Set("self", this);

            luaEnv.DoString("require 'Scripts/Lua/LuaMain'", "LuaBehaviour", scriptEnv);

            Action luaAwake = scriptEnv.Get<Action>("awake");
            scriptEnv.Get("start", out luaStart);
            scriptEnv.Get("update", out luaUpdate);
            scriptEnv.Get("ondestroy", out luaOnDestroy);

            if (luaAwake != null)
            {
                luaAwake();
            }
        }

        void Start()
        {
            if (luaStart != null)
            {
                luaStart();
            }
        }

        void Update()
        {
            if (luaUpdate != null)
            {
                luaUpdate();
            }
            if (Time.time - LuaBehaviour.lastGCTime > GCInterval)
            {
                luaEnv.Tick();
                LuaBehaviour.lastGCTime = Time.time;
            }
        }

        void OnDestroy()
        {
            if (luaOnDestroy != null)
            {
                luaOnDestroy();
            }
            luaOnDestroy = null;
            luaUpdate = null;
            luaStart = null;
            scriptEnv.Dispose();
        }
    }
}
