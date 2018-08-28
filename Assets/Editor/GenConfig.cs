using XLua;
using System.Collections.Generic;
using System;

//配置的详细介绍请看Doc下《XLua的配置.doc》
public static class GenConfig
{
    [ReflectionUse]
    public static List<Type> LuaReflectionUseCSharp = new List<Type>()
    {
        typeof(UnityEngine.RuntimePlatform),
        typeof(UnityEngine.Application),
        typeof(UnityEngine.iOS.Device),
        typeof(UnityEngine.iOS.DeviceGeneration),
        typeof(System.IO.Directory),
        typeof(UnityEngine.Screen),
        typeof(UnityEngine.SleepTimeout), 
    };

        //lua中要使用到C#库的配置，比如C#标准库，或者Unity API，第三方库等。
    [LuaCallCSharp]
    public static List<Type> LuaCallCSharp = new List<Type>() {
        typeof(System.Object),
        typeof(System.Type),
		typeof(System.DateTime),
        typeof(System.TimeSpan),
		typeof(EventArgs),
		typeof(System.Reflection.BindingFlags),
		typeof(System.IO.SearchOption),
		

        //-------------------UnityEngine-------------------//
        typeof(UnityEngine.Object),
        typeof(UnityEngine.Behaviour),
        typeof(UnityEngine.MonoBehaviour),
        typeof(UnityEngine.Component),
        typeof(UnityEngine.GameObject),
        typeof(UnityEngine.Transform),
        typeof(UnityEngine.RectTransform),
        typeof(UnityEngine.RectTransform.Axis),
        typeof(UnityEngine.RectTransform.Edge),
        typeof(UnityEngine.Vector2),
        typeof(UnityEngine.Vector3),
        typeof(UnityEngine.Vector4),
        typeof(UnityEngine.Quaternion),
        typeof(UnityEngine.Color),
        typeof(UnityEngine.Ray),
        typeof(UnityEngine.Bounds),
        typeof(UnityEngine.Ray2D),
        typeof(UnityEngine.Time),
        typeof(UnityEngine.Material),
        typeof(UnityEngine.TextAsset),
        typeof(UnityEngine.Keyframe),
        typeof(UnityEngine.AnimationCurve),
        typeof(UnityEngine.AnimationClip),
        typeof(UnityEngine.ParticleSystem),
        typeof(UnityEngine.SkinnedMeshRenderer),
        typeof(UnityEngine.Renderer),
        typeof(UnityEngine.WWW),
        typeof(UnityEngine.WaitForSeconds),
        typeof(UnityEngine.Debug),
        typeof(UnityEngine.RectTransformUtility),
        typeof(UnityEngine.Camera),
        typeof(UnityEngine.Camera.MonoOrStereoscopicEye),
        typeof(UnityEngine.Camera.StereoscopicEye),
        typeof(UnityEngine.Events.UnityEvent),
        typeof(UnityEngine.Events.UnityEventBase),
        typeof(UnityEngine.HingeJoint),
        typeof(UnityEngine.Space),
        typeof(UnityEngine.PlayerPrefs),
        typeof(UnityEngine.SceneManagement.SceneManager),
        typeof(UnityEngine.Mathf),
        typeof(UnityEngine.Sprite),
        typeof(UnityEngine.WrapMode),
		typeof(UnityEngine.Random),
		typeof(UnityEngine.AsyncOperation),
		typeof(UnityEngine.Font),
		typeof(UnityEngine.Screen),
		typeof(UnityEngine.RuntimePlatform),
		typeof(UnityEngine.Application),
		typeof(UnityEngine.Screen),
		typeof(UnityEngine.NetworkReachability),
		typeof(UnityEngine.AudioListener),
		typeof(UnityEngine.SystemLanguage),
		typeof(UnityEngine.AudioClip),
		
		
        

        //-------------------UGUI-------------------//
        typeof(UnityEngine.UI.Graphic),
        typeof(UnityEngine.UI.Text),
        typeof(UnityEngine.UI.Image),
        typeof(UnityEngine.UI.Button),
        typeof(UnityEngine.UI.Slider),
        typeof(UnityEngine.UI.MaskableGraphic),
        typeof(UnityEngine.UI.InputField),
        typeof(UnityEngine.Canvas),
        typeof(UnityEngine.UI.Image),
        typeof(UnityEngine.UI.Slider.Direction),
        typeof(UnityEngine.UI.Image.Origin360),
        typeof(UnityEngine.UI.Image.Origin180),
        typeof(UnityEngine.UI.Image.Origin90),
        typeof(UnityEngine.UI.Image.OriginVertical),
        typeof(UnityEngine.UI.Image.OriginHorizontal),
        typeof(UnityEngine.UI.Image.FillMethod),
        typeof(UnityEngine.UI.Image.Type),
        typeof(UnityEngine.UI.InputField.LineType),
        typeof(UnityEngine.UI.InputField.CharacterValidation),
        typeof(UnityEngine.UI.InputField.InputType),
        typeof(UnityEngine.UI.InputField.ContentType),
        
    };



    //C#静态调用Lua的配置（包括事件的原型），仅可以配delegate，interface;
    [CSharpCallLua]
    public static List<Type> CSharpCallLua = new List<Type>() {

				//-------------------System Func----------------//
				typeof(Func<float>),
                typeof(Func<double, double, double>),


				//-------------------UnityEngine Func----------------//
                typeof(Func<UnityEngine.Color>),

                typeof(Func<UnityEngine.GameObject, LuaTable>),
                typeof(Func<UnityEngine.GameObject, object[], LuaTable>),
                typeof(Func<int, UnityEngine.RectTransform, LuaTable>),


            	//-------------------System Action----------------//
                typeof(Action),
                typeof(Action<bool>),
                typeof(Action<string>),
                typeof(Action<int>),
                typeof(Action<float>),
                typeof(Action<long>),
                typeof(Action<double>),
                typeof(Action<string, object>),
				typeof(System.Collections.IEnumerator),
                typeof(Action<object>),
                typeof(Action<EventArgs>),

				//-------------------UnityEngine Action----------------//
                typeof(Action<UnityEngine.EventSystems.PointerEventData>),
                typeof(Action<UnityEngine.Object>),
                typeof(Action<UnityEngine.Object, LuaTable>),
                typeof(Action<UnityEngine.Color>),
                typeof(Action<UnityEngine.GameObject>),
                typeof(Action<string, UnityEngine.GameObject>),
                typeof(Action<string, UnityEngine.Object>),
                typeof(Action<int, UnityEngine.RectTransform>),
                typeof(UnityEngine.Events.UnityAction),
                typeof(UnityEngine.Events.UnityAction<float>),
            };

    //结构体优化列表
	[GCOptimize]
    public static List<Type> GCOptimize = new List<Type>()
	{

	};


    //黑名单;
    [BlackList]
    public static List<List<string>> BlackList = new List<List<string>>()  {
                new List<string>(){"UnityEngine.WWW", "movie"},
                new List<string>(){ "UnityEngine.WWW", "GetMovieTexture" },
    #if UNITY_WEBGL
                new List<string>(){"UnityEngine.WWW", "threadPriority"},
    #endif
                new List<string>(){"UnityEngine.Texture2D", "alphaIsTransparency"},
                new List<string>(){"UnityEngine.Security", "GetChainOfTrustValue"},
                new List<string>(){"UnityEngine.CanvasRenderer", "onRequestRebuild"},
                new List<string>(){"UnityEngine.Light", "areaSize"},
                new List<string>(){"UnityEngine.AnimatorOverrideController", "PerformOverrideClipListCleanup"},
    #if !UNITY_WEBPLAYER
                new List<string>(){"UnityEngine.Application", "ExternalEval"},
    #endif
                new List<string>(){"UnityEngine.GameObject", "networkView"}, //4.6.2 not support
                new List<string>(){"UnityEngine.Component", "networkView"},  //4.6.2 not support
                new List<string>(){"System.IO.FileInfo", "GetAccessControl", "System.Security.AccessControl.AccessControlSections"},
                new List<string>(){"System.IO.FileInfo", "SetAccessControl", "System.Security.AccessControl.FileSecurity"},
                new List<string>(){"System.IO.DirectoryInfo", "GetAccessControl", "System.Security.AccessControl.AccessControlSections"},
                new List<string>(){"System.IO.DirectoryInfo", "SetAccessControl", "System.Security.AccessControl.DirectorySecurity"},
                new List<string>(){"System.IO.DirectoryInfo", "CreateSubdirectory", "System.String", "System.Security.AccessControl.DirectorySecurity"},
                new List<string>(){"System.IO.DirectoryInfo", "Create", "System.Security.AccessControl.DirectorySecurity"},
                new List<string>(){"UnityEngine.MonoBehaviour", "runInEditMode"},
                new List<string>(){"UnityEngine.UI.Graphic", "OnRebuildRequested" },
                new List<string>(){"UnityEngine.UI.Text", "OnRebuildRequested"},
                new List<string>(){"UnityEngine.WWW", "LoadImageIntoTexture", "UnityEngine.Texture2D" }, 
            };
}
