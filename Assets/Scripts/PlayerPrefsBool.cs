using UnityEngine;

public class PlayerPrefsBool
{
    public static void SetBool(string name, bool booleanValue) 
        => PlayerPrefs.SetInt(name, booleanValue ? 1 : 0);

    public static bool GetBool(string name) 
        => PlayerPrefs.GetInt(name) == 1;

    public static bool GetBool(string name, bool defaultValue) 
        => PlayerPrefs.HasKey(name) ? GetBool(name) : defaultValue;
}

