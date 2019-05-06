/*****************************************
*ScriptName: #ScriptName#
*UnityVerSion: #UNITYVERSION#
*Author: #AUTHOR#
*Date:  #DATE#
*Description: AssetBundle路径类
 
******************************************/
using System.IO;
using UnityEngine;

public static class AssetBundlePath
{
    /// <summary>
    /// 随包压缩目录
    /// </summary>
    public static string ResourcePath { get; private set; }

    /// <summary>
    /// 二进制存放目录
    /// </summary>
    public static string SteamingAssetPath { get; private set; }

    /// <summary>
    /// 个人用户目录
    /// </summary>
    public static string PersistentDataPath { get; private set; }

    static AssetBundlePath()
    {
        ResourcePath = Application.dataPath.Replace("\\", "/");
        SteamingAssetPath = Application.streamingAssetsPath.Replace("\\", "/");
        PersistentDataPath = Application.persistentDataPath.Replace("\\", "/");
    }

    public static string GetFilePersistentPath(string path)
    {
        string relativelyPath = string.Empty;
        relativelyPath = GetFileRealPath(string.Format("{0}/{1}", PersistentDataPath, path));
        return relativelyPath;
    }

    public static string GetFileSteamingAssetPath(string path)
    {
        string relativelyPath = string.Empty;
        relativelyPath = GetFileRealPath(string.Format("{0}/{1}", SteamingAssetPath, path));
        return relativelyPath;
    }

    public static string GetAssetBundlePath(string path)
    {
        string relativelyPath = string.Empty;
        relativelyPath = GetFilePersistentPath(path);

        if (!File.Exists(relativelyPath))
        {
            relativelyPath = string.Format("{0}/{1}/{2}", SteamingAssetPath, "AssetBundle", path);
        }

        return relativelyPath;
    }

    /// <summary>
    /// 获得文件真实路径
    /// </summary>
    /// <param name="relativelyPath"></param>
    public static string GetFileRealPath(string relativelyPath)
    {
        string path = string.Empty;
#if UNITY_ANDROID
        path = relativelyPath;
#elif UNITY_IPHONE
        path = "file:///" + relativelyPath;
#elif UNITY_STANDLONE_WIN
        path = "file:///" + relativelyPath;
#elif UNITY_EDITOR
        path = relativelyPath;
#endif
        return path;
    }
}
