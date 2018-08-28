using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Com.VT.Editor
{
    class UILuaCreater : EditorWindow
    {
        private static UILuaCreater _creater;

        private GameObject mGo = null;
        private string mLuaViewPathRoot = "StreamingAssets/Lua/UI/ViewLayer";
        private string mLuaCotrollerPathRoot = "StreamingAssets/Lua/UI/Controller";
        private const string mLuaRequirPath = "StreamingAssets/Lua/Require/UIRequire.lua";

        private const string mLuaViewTemplatePath = "Assets/Editor/UITool/Template/lua_view_template.txt";
        private const string mLuaControllerTemplatePath = "Assets/Editor/UITool/Template/lua_controller_template.txt";

        private bool mShowPosition = true;
        private Vector2 mScrollPosition = Vector2.zero;
        private bool isExportController = false;

        [MenuItem("VT/UITool/UI Lua Create")]
        public static void ShowOrHide()
        {
            if (_creater == null)
            {
                _creater = GetWindow<UILuaCreater>();
            }
            _creater.Show();
        }

        [MenuItem("VT/UITool/UI Lua Create", true)]
        public static bool Validate()
        {
            if (Selection.activeGameObject != null)
            {
                string path;
                if (PrefabUtility.GetPrefabType(Selection.activeGameObject) == PrefabType.PrefabInstance)
                {
                    UnityEngine.Object parentObject = PrefabUtility.GetPrefabParent(Selection.activeGameObject);
                    path = AssetDatabase.GetAssetPath(parentObject);
                }
                else if (PrefabUtility.GetPrefabType(Selection.activeGameObject) == PrefabType.Prefab)
                {
                    path = AssetDatabase.GetAssetPath(Selection.activeGameObject);
                }
                else
                {
                    return false;
                }

                return true;
                /*
                if (path.StartsWith(UIPrefabPostProcess.mUIPrefabFolderPath) && path.EndsWith(UIPrefabPostProcess.mObjExtensionName))
                    return true;
                else
                    return false;
                */
            }
            return false;
        }

        UILuaCreater()
        {
            titleContent = new GUIContent("Lua脚本生成器", "Lua代码生成工具，用于自动化生成View层代码");
        }

        void OnGUI()
        {
            RefreshGUI();
        }

		private List<ObjectContainer> mContainers = new List<ObjectContainer>();
		private List<string> mContainerPaths = new List<string>();
		private List<List<string>> mObjectPaths = new List<List<string>>();
		private int mTotalComponents = 0;
		//private Dictionary<string, string> names;
		private void InitSelectionData()
        {
            mGo = Selection.activeGameObject;
			mContainers.Clear();
			mContainerPaths.Clear();
			mObjectPaths.Clear();
			mTotalComponents = 0;
			if (mGo == null) { return; }
			mGo.GetComponentsInChildren<ObjectContainer>(true, mContainers);
			for (int i = 0; i < mContainers.Count; i++) {
				ObjectContainer oc = mContainers[i];
				mContainerPaths.Add(UIPrefabPostProcess.GetObjectPath(oc.transform, mGo.transform));
				List<string> paths = new List<string>();
				for (int j = 0; j < oc.GetObjectItems().Count; j++) {
					ObjectContainer.single_obj_item so = oc.GetObjectItems()[j];
					string p = so.component == null ? string.Format("{0}(null)", so.name) : UIPrefabPostProcess.GetObjectPath(so.component.transform, mGo.transform);
					paths.Add(string.Format("控件路径 {0}  ", p));
					mTotalComponents++;
				}
				mObjectPaths.Add(paths);
			}
		}

        private void Export()
        {
            StringBuilder sb1 = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();
            StringBuilder sb3 = new StringBuilder();

            string name = Selection.activeGameObject.name;
            string viewName = UIViewUtils.GetLuaViewName(name);
            string controllerName = UIViewUtils.GetLuaControllerName(name);

            var oc = mGo.GetComponent<ObjectContainer>();
            if (oc == null)
            {
                Debug.LogError("请先使用UI/ProcessPrefab进行序列化操作！！！");
                return;
            }
            for (int i = 0; i < oc.GetObjectItems().Count; i++)
            {
                var item = oc.GetObjectItems()[i];
                if (i == oc.GetObjectItems().Count - 1)
                {
                    sb1.AppendFormat("    self.{0} = self.objectContainer:GetObjComponent(\"{1}\");", item.name, item.name);
                    if (item.component is ButtonEx)
                    {
                        sb2.AppendFormat("    self.view.{0}.onClick:AddListener(function() self:{1}Handler(); end);", item.name, UIViewUtils.FormatObjectName(item.name));
                        sb3.AppendFormat("function {0}:{1}Handler()\n\nend", controllerName, UIViewUtils.FormatObjectName(item.name));
                    }
                }
                else
                {
                    sb1.AppendLine(string.Format("    self.{0} = self.objectContainer:GetObjComponent(\"{1}\");", item.name, item.name));
                    if (item.component is ButtonEx)
                    {
                        sb2.AppendFormat("    self.view.{0}.onClick:AddListener(function() self:{1}Handler(); end);\n", item.name, UIViewUtils.FormatObjectName(item.name));
                        sb3.AppendFormat("function {0}:{1}Handler()\n  \nend\n\n", controllerName, UIViewUtils.FormatObjectName(item.name));
                    }
                }
            }
			StringBuilder itemView = new StringBuilder();
			StringBuilder itemCtrl = new StringBuilder();
			ObjectContainer[] ocs = mGo.GetComponentsInChildren<ObjectContainer>(true);
			for (int i = 0, imax = ocs.Length; i < imax; i++) {
				ObjectContainer c = ocs[i];
				if (c == oc) { continue; }
				string itemViewName = string.Concat(viewName, ".", UIViewUtils.GetLuaItemViewName(c.name));
				string itemCtrlName = string.Concat(controllerName, ".", UIViewUtils.GetLuaItemControllerName(c.name));
				itemCtrl.AppendLine(string.Format("{0} = class(\"{0}\");", itemCtrlName));
				itemCtrl.AppendLine();
				itemCtrl.AppendLine(string.Format("function {0}:ctor(index, view)", itemCtrlName));
				itemCtrl.AppendLine("\tself.view = view;");
				itemCtrl.AppendLine("\tself:Init();");
				itemCtrl.AppendLine("end");
				itemCtrl.AppendLine();
				itemCtrl.AppendLine(string.Format("function {0}:Init()", itemCtrlName));
				itemCtrl.AppendLine("\t-- initialize ...");
				itemCtrl.AppendLine("end");
				itemCtrl.AppendLine();
				itemCtrl.AppendLine(string.Format("function {0}:OnUpdateItem(index, go)", itemCtrlName));
				itemCtrl.AppendLine("\t-- OnUpdateItem ...");
				itemCtrl.AppendLine("end");
				itemCtrl.AppendLine();
				itemView.AppendLine(string.Format("{0} = class(\"{0}\");", itemViewName));
				itemView.AppendLine();
				itemView.AppendLine(string.Format("function {0}:ctor(index, go)", itemViewName));
				itemView.AppendLine(string.Format("\tself.name = \"{0}\";", c.name));
				itemView.AppendLine("\tself.gameObject = go;");
				itemView.AppendLine("\tself.objectContainer = go:GetComponent(\"ObjectContainer\");");
				itemView.AppendLine();
				for (int j = 0; j < c.GetObjectItems().Count; j++) {
					ObjectContainer.single_obj_item obj = c.GetObjectItems()[j];
					itemView.AppendLine(string.Format("\tself.{0} = self.objectContainer:GetObjComponent(\"{0}\");", obj.name));
				}
				itemView.AppendLine();
				itemView.AppendLine(string.Format("\tself.controller = {0}.New(index, self);", itemCtrlName));
				itemView.AppendLine("end");
				itemView.AppendLine();
				itemView.AppendLine(string.Format("function {0}:OnUpdateItem(index, go)", itemViewName));
				itemView.AppendLine("\tself.controller:OnUpdateItem(index, go);");
				itemView.AppendLine("end");
				itemView.AppendLine();
			}
            string viewScriptStr = string.Format(mLuaViewTemplate.Replace("#viewClsName#", viewName).Replace("#controllerClsName#", controllerName), DateTime.Now.ToString(), name, sb1.ToString());
            string controllerScriptStr = string.Format(mLuaControllerTemplate.Replace("#controllerClsName#", controllerName), DateTime.Now.ToString(), sb2.ToString(), sb3.ToString());
			if (itemView.Length > 0) {
				viewScriptStr = string.Concat(viewScriptStr, "\n\n\n", itemView.ToString());
			}
			if (itemCtrl.Length > 0) {
				controllerScriptStr = string.Concat(controllerScriptStr, "\n\n\n", itemCtrl.ToString());
			}

            string viewRelativePath = mLuaViewPathRoot + "/" + UIViewUtils.GetLuaViewName(name);
            string viewPath = Application.dataPath + "/" + viewRelativePath + ".lua";

            string controllerRelativePath = mLuaCotrollerPathRoot + "/" + UIViewUtils.GetLuaControllerName(name);
            string controllerPath = Application.dataPath + "/" + controllerRelativePath + ".lua";

            CreateDirectory(viewPath, true);
            File.WriteAllText(viewPath, viewScriptStr);
            Debug.Log("成功导出View Script --> " + viewPath);
            if (isExportController)
            {
                CreateDirectory(controllerPath, true);
                File.WriteAllText(controllerPath, controllerScriptStr);
                Debug.Log("成功导出Controller Script --> " + controllerPath);
            }

            Add2Requir(viewRelativePath, controllerRelativePath);

            AssetDatabase.Refresh();
        }

        private void Add2Requir(string viewPath, string controllerPath)
        {
            viewPath = viewPath.Substring(("StreamingAssets/").Length, viewPath.Length - ("StreamingAssets/").Length);
            controllerPath = controllerPath.Substring("StreamingAssets/".Length, controllerPath.Length - ("StreamingAssets/").Length);
            string LuaRequirPath = Application.dataPath + "/" + mLuaRequirPath;
            if (File.Exists(LuaRequirPath))
            {
                StringBuilder sb = new StringBuilder();
                string[] strs = File.ReadAllLines(LuaRequirPath);
                bool isContainsV = false;
                bool isContainsC = false;
                foreach (var item in strs)
                {
                    if (item.Contains(viewPath))
                    {
                        isContainsV = true;
                    }
                    else if (item.Contains(controllerPath))
                    {
                        isContainsC = true;
                    }
                }
                if (isContainsC && isContainsV) return;
                sb.AppendLine();
                if (!isContainsV)
                    sb.AppendLine("require \'" + viewPath + "\';");
                if (!isContainsC && isExportController)
                    sb.AppendLine("require \'" + controllerPath + "\';");
                File.AppendAllText(LuaRequirPath, sb.ToString());
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("require \'" + viewPath + "\';");
                sb.AppendLine("require \'" + controllerPath + "\';");
                Directory.CreateDirectory(LuaRequirPath.Substring(0, LuaRequirPath.LastIndexOf('/')));
                File.WriteAllText(LuaRequirPath, sb.ToString());
            }
        }

        private void RefreshGUI()
        {
            if (Validate())
            {
                string name = Selection.activeGameObject.name;
                GUILayout.Label(string.Format("找到{0}个ObjectContainer，{1}个UI控件", mContainers.Count, mTotalComponents));

                EditorGUILayout.BeginHorizontal();
                //mLuaViewPathRoot = EditorGUILayout.TextField("View层代码目录", mLuaViewPathRoot);
                EditorGUILayout.LabelField("View层代码目录: " + mLuaViewPathRoot);
                /*
                if (GUILayout.Button("选择", GUILayout.MaxWidth(120f)))
                {
                    mLuaViewPathRoot = EditorUtility.SaveFolderPanel("选择代码存储目录", "", Application.dataPath);
                }
                */
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                //mLuaCotrollerPathRoot = EditorGUILayout.TextField("Control层代码目录", mLuaCotrollerPathRoot);
                EditorGUILayout.LabelField("Control层代码目录: " + mLuaCotrollerPathRoot);
                /*
                if (GUILayout.Button("选择", GUILayout.MaxWidth(120f)))
                {
                    mLuaCotrollerPathRoot = EditorUtility.SaveFolderPanel("选择代码存储目录", "", Application.dataPath);
                }
                */
                EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				GUILayout.Label("UI初始化Lua文件:" + UIViewUtils.GetLuaViewName(name) + "." + "lua");
				if (GUILayout.Button("ProcessPrefab", GUILayout.MaxWidth(120f))) {
					EditorApplication.ExecuteMenuItem("Assets/UI/ProcessPrefab");
					InitSelectionData();
				}
				EditorGUILayout.EndHorizontal();

				if (mShowPosition = EditorGUILayout.Foldout(mShowPosition, "控件概要"))
                {
                    mScrollPosition = EditorGUILayout.BeginScrollView(mScrollPosition);
					for (int i = 0; i < mContainers.Count; i++) {
						ObjectContainer oc = mContainers[i];
						EditorGUILayout.LabelField(string.Format("ObjectContainer : {0}, 控制数量 : {1}", mContainerPaths[i], oc.GetObjectItems().Count));
						List<string> paths = mObjectPaths[i];
						for (int j = 0; j < oc.GetObjectItems().Count; j++) {
							ObjectContainer.single_obj_item so = oc.GetObjectItems()[j];
							EditorGUILayout.BeginHorizontal();
							GUILayout.Space(16f);
							EditorGUILayout.LabelField(paths[j]);
							GameObject go = so.component == null ? null : so.component.gameObject;
							EditorGUILayout.ObjectField(GUIContent.none, go, typeof(GameObject), true, GUILayout.MaxWidth(240f));
							EditorGUILayout.EndHorizontal();
						}
						GUILayout.Space(4f);
					}
                    //GUILayout.Space(150);
                    /*foreach (var i in names)
                    {
                        EditorGUILayout.BeginHorizontal();
                        GameObject _g = Selection.activeGameObject.transform.Find(i.Value.Substring(Selection.activeGameObject.name.Length + 1, i.Value.Length - Selection.activeGameObject.name.Length - 1)).gameObject;
                        GUILayout.Label(string.Format("控件路径 {0}  ", i.Value));
                        EditorGUILayout.ObjectField("", _g, typeof(GameObject), true, GUILayout.Width(200));
                        //EditorGUILayout.ObjectField(string.Format("控件路径 {1}  控件名称：{0} ", i.Key.Substring(0), i.Value), _g, typeof(GameObject), true);
                        //GUILayout.Label(string.Format("控件路径 {1}  控件名称：{0} ", i.Key.Substring(0), i.Value));
                        EditorGUILayout.EndHorizontal();
                    }*/
                    EditorGUILayout.EndScrollView();
                }

                isExportController = EditorGUILayout.BeginToggleGroup("是否导出Controller层结构代码", isExportController);
                EditorGUILayout.EndToggleGroup();

                string path = Path.Combine(Application.dataPath + "/" + mLuaCotrollerPathRoot, UIViewUtils.GetLuaControllerName(name) + ".lua");
                if (File.Exists(path) && isExportController)
                {
                    GUILayout.Label(string.Format("Control层结构代码已经存在，选择导出将会被覆盖"));
                }

                if (GUILayout.Button("生成Lua"))
                {
                    if (UIPrefabPostProcess.LogRepeatNames(mGo))
                    {
                        ShowNotification(new GUIContent("含有重名，不可导出"));
                    }
                    else
                    {
                        Export();
                    }
                }
            }
            else
            {
                EditorGUILayout.Separator();
                EditorGUILayout.Separator();
                EditorGUILayout.Separator();
                EditorGUILayout.Separator();
                EditorGUILayout.Separator();
                EditorGUILayout.Separator();
                GUILayout.Label("当前对象不符合规则，不可以生成Lua脚本，请检查...");
            }
        }

        private string mLuaViewTemplate
        {
            get
            {
                return AssetDatabase.LoadAssetAtPath<TextAsset>(mLuaViewTemplatePath).text;
            }
        }

        private string mLuaControllerTemplate
        {
            get
            {
                return AssetDatabase.LoadAssetAtPath<TextAsset>(mLuaControllerTemplatePath).text;
            }
        }
        public static void CreateDirectory(string path, bool isFile)
        {
            string direcoryPath = isFile == true ? Path.GetDirectoryName(path) : path;
            if (Directory.Exists(direcoryPath))
            {
                return;
            }
            Directory.CreateDirectory(direcoryPath);
        }

        private void OnFocus()
        {
            InitSelectionData();
        }
        public void OnSelectionChange()
        {
            InitSelectionData();
        }
    }
}
