using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RectTransform))]
public class MyTest : DecoratorEditor
{
    public MyTest() : base("RectTransformEditor") { }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Adding this button"))
        {
            Debug.Log("Adding this button");
        }
    }
}