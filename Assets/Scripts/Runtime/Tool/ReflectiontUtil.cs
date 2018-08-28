#if UNITY_EDITOR
using System;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;

public class ReflectiontUtil
{
    private static Assembly editorAssembly = null;
    public static Assembly EditorAssembly
    {
        get
        {
            if (editorAssembly == null)
            {
                editorAssembly = Assembly.Load("UnityEditor");
            }
            return editorAssembly;
        }
    }

    public static Assembly engineAssembly = null;
    public static Assembly EngineAssembly
    {
        get
        {
            if (engineAssembly == null)
            {
                engineAssembly = Assembly.GetAssembly(typeof(MonoBehaviour));
            }
            return engineAssembly;
        }
    }

    public static Assembly engineUIAssembly = null;
    public static Assembly EngineUIAssembly
    {
        get
        {
            if (engineUIAssembly == null)
            {
                engineUIAssembly = Assembly.GetAssembly(typeof(UIBehaviour));
            }
            return engineUIAssembly;
        }
    }

    private static Dictionary<string, Type> editorClassTypesDic = null;
    public static Dictionary<string, Type> EditorClassTypesDic
    {
        get
        {
            if (editorClassTypesDic == null || editorClassTypesDic.Count == 0)
            {
                editorClassTypesDic = new Dictionary<string, Type>();
                var editorTypes = EditorAssembly.GetTypes();
                foreach (var item in editorTypes)
                {
                    if (item.IsPublic)
                    {
                        editorClassTypesDic.Add(item.FullName, item);
                    }
                }

                var engineTypes = EngineAssembly.GetTypes();
                foreach (var item in engineTypes)
                {
                    if (item.IsPublic)
                    {
                        editorClassTypesDic.Add(item.FullName, item);
                    }
                }

                var engineUITypes = EngineUIAssembly.GetTypes();
                foreach (var item in engineUITypes)
                {
                    if (item.IsPublic)
                    {
                        editorClassTypesDic.Add(item.FullName, item);
                    }
                }
            }
            return editorClassTypesDic;
        }
    }

    public static Type GetTypeByFullName(string fullName)
    {
        return EditorClassTypesDic[fullName];
    }
}
#endif