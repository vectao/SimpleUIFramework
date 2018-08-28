using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.VT
{
    public class UIViewUtils
    {
        public static string FormatObjectName(string name)
        {
            string[] strs = name.Split('_');
            for (int i = 0; i < strs.Length; i++)
            {
                if (i == 0)
                {
                    strs[i] = strs[i].ToUpper();
                }
                else
                {
                    strs[i] = strs[i].Substring(0, 1).ToUpper() + strs[i].Substring(1);
                }
            }
            return string.Concat(strs);
        }

        public static string GetLuaViewName(string name)
        {
            return FormatObjectName(name) + "View";
        }

        public static string GetLuaControllerName(string name)
        {
            return FormatObjectName(name) + "Controller";
        }

		public static string GetLuaItemViewName(string name) {
			return string.Concat("Item", FormatObjectName(name).Substring(1), "View");
		}

		public static string GetLuaItemControllerName(string name) {
			return string.Concat("Item", FormatObjectName(name).Substring(1), "Controller");
		}
    }
}
