using System.Reflection;
using System;
using UnityEditor;

public class DefaultHandles
{
    public static bool Hidden
    {
        get
        {
            Type type = typeof(Tools);
            FieldInfo field = type.GetField("s_Hidden", BindingFlags.NonPublic | BindingFlags.Static);
            return ((bool)field.GetValue(null));
        }
        set
        {
            Type type = typeof(Tools);
            FieldInfo field = type.GetField("s_Hidden", BindingFlags.NonPublic | BindingFlags.Static);
            field.SetValue(null, value);
        }
    }
}
