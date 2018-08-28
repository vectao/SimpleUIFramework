using UnityEngine;
using UnityEditor;

namespace UnityEngine.UI {

	[CanEditMultipleObjects, CustomEditor(typeof(ButtonEx))]
	public class ButtonExInspector : UnityEditor.UI.ButtonEditor {

		public override void OnInspectorGUI() {
			base.OnInspectorGUI();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("m_SfxId"));
			serializedObject.ApplyModifiedProperties();
		}

	}

}
