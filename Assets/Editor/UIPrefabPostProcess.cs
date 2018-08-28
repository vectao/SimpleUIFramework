using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Com.VT
{
    public class UIPrefabPostProcess : AssetPostprocessor
    {
        public static readonly string mUIPrefabFolderPath = "Assets/Resources/UI/Prefab/";
        public static readonly string mObjExtensionName = ".prefab";

        public static readonly string mGoRuleName = "m_";
        public static readonly string mUIRuleName = "ui_";
		private const string ui_item_rule_name = "i_";

        [MenuItem("Assets/UI/ProcessPrefab", true)]
        private static bool CanProcessPrefab()
        {
            GameObject go = Selection.activeGameObject;
            if (go == null) { return false; }
            string path = AssetDatabase.GetAssetPath(go);
            if (string.IsNullOrEmpty(path)) { return false; }
            return true;
        }
        [MenuItem("Assets/UI/ProcessPrefab", false)]
        private static void ProcessPrefab()
        {
            if (!CanProcessPrefab()) { return; }
            ResolveUIPrefab(Selection.activeGameObject);
        }

        private static void ResolveUIPrefab(GameObject go)
        {
            ObjectContainer oc = go.GetComponent<ObjectContainer>();
            if (oc == null)
            {
                oc = go.AddComponent<ObjectContainer>();
            }
            oc.GetObjectItems().Clear();
            if (UIPrefabPostProcess.LogRepeatNames(go))
            {
                EditorUtility.DisplayDialog("警告", "含有重复命名文件，请检查修改！", "确定");
            }
			Stack<Transform> children = new Stack<Transform>();
			Stack<ObjectContainer> containers = new Stack<ObjectContainer>();
			children.Push(oc.transform);
			containers.Push(oc);
			while (children.Count > 0) {
				Transform t = children.Pop();
				ObjectContainer c = containers.Pop();
				if (c.transform != t) {
					string node = t.name;
					ObjectContainer cc = c;
					bool flag = false;
					if (node.StartsWith(mGoRuleName)) {
						flag = true;
					} else if (node.StartsWith(ui_item_rule_name)) {
						flag = true;
						c = t.GetComponent<ObjectContainer>() ?? t.gameObject.AddComponent<ObjectContainer>();
						c.GetObjectItems().Clear();
					}
					if (flag) {
						Component component = t;
						cc.GetObjectItems().Add(new ObjectContainer.single_obj_item() { name = node, component = component });
					}
				}
				for (int i = t.childCount - 1; i >= 0; i--) {
					children.Push(t.GetChild(i));
					containers.Push(c);
				}
			}

            EditorUtility.SetDirty(oc);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static Component SearchComponent(Transform trans)
        {
            Component[] components = trans.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                Component cpt = components[i];
                if (cpt is ButtonEx || cpt is Text || cpt is ProgressBar)
                {
                    return cpt;
                }
            }
            return trans;
        }

        /// <summary>
        /// 如果有重复的命名返回true
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static bool LogRepeatNames(GameObject go)
        {
			bool repeated = false;
			Dictionary<Transform, Dictionary<string, List<string>>> nameDict = new Dictionary<Transform, Dictionary<string, List<string>>>();
			Stack<Transform> children = new Stack<Transform>();
			Stack<Transform> containers = new Stack<Transform>();
			children.Push(go.transform);
			containers.Push(go.transform);
			while (children.Count > 0) {
				Transform t = children.Pop();
				Transform c = containers.Pop();
				if (c != t) {
					string node = t.name;
					Transform cc = c;
					bool flag = false;
					if (node.StartsWith(mGoRuleName)) {
						flag = true;
					} else if (node.StartsWith(ui_item_rule_name)) {
						flag = true;
						c = t;
					}
					if (flag) {
						Dictionary<string, List<string>> dict;
						if (!nameDict.TryGetValue(cc, out dict)) {
							dict = new Dictionary<string, List<string>>();
							nameDict.Add(cc, dict);
						}
						List<string> list;
						if (!dict.TryGetValue(node, out list)) {
							list = new List<string>();
							dict.Add(node, list);
						} else {
							repeated = true;
						}
						list.Add(GetObjectPath(t, go.transform));
					}
				}
				for (int i = t.childCount - 1; i >= 0; i--) {
					children.Push(t.GetChild(i));
					containers.Push(c);
				}
			}
			if (repeated) {
				foreach (KeyValuePair<Transform, Dictionary<string, List<string>>> kv1 in nameDict) {
					foreach (KeyValuePair<string, List<string>> kv2 in kv1.Value) {
						if (kv2.Value.Count > 1) {
							for (int i = 0; i < kv2.Value.Count; i++) {
								Debug.LogError(kv2.Value[i]);
							}
						}
					}
				}
			}
			return repeated;
        }

        public static string GetObjectPath(Transform obj, Transform root)
        {
            List<string> _n = new List<string>() { obj.name };
			Transform _t = obj;
            while (_t != root && _t != null)
            {
                _n.Add(_t.name);
                _t = _t.parent;
            }
			if (_t != null) { _n.Add(_t.name); }
            _n.Reverse();
            return string.Join("/", _n.ToArray());
        }
    }
}
