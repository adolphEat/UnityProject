/*****************************************
*ScriptName: #ScriptName#
*UnityVerSion: #UNITYVERSION#
*Author: #AUTHOR#
*Date:  #DATE#
*Description: AssetBundle生成类
 
******************************************/
using UnityEngine;
using UnityEditor;
using System.IO;

public static class AssetBulid
{
    private static string outputPath = Application.streamingAssetsPath + "/" + "AssetBundle";
    private class AssetInfo
    {
        public int RefNum = 0;
        public bool IsDenpend = true;
        public string FullPath = string.Empty;
        public string AssetPath = string.Empty;
    }

    /// <summary>
    /// Set AssetBundle Name
    /// </summary>
    private static void SetBundlesName()
    {
       


    }

    /// <summary>
    /// Bulid AssetBundle
    /// </summary>
    public static void BulidAB()
    {
        UtilTools.CreateDirectory(outputPath);

        BuildAssetBundleOptions assetOptions = 
            BuildAssetBundleOptions.ChunkBasedCompression |
            BuildAssetBundleOptions.DeterministicAssetBundle;

        BuildPipeline.BuildAssetBundles(outputPath, assetOptions, EditorUserBuildSettings.activeBuildTarget);

        string srcManifest = outputPath + "/" + Path.GetFileName(outputPath);
        if (File.Exists(srcManifest))
        {
            string destManifest = outputPath + "/" + "AB";
            File.Copy(srcManifest, destManifest, true);
            File.Copy(srcManifest + ".manifest", destManifest + ".manifest", true);

            File.Delete(srcManifest);
        }

        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 设置单个bundle名字
    /// </summary>
    private static void SetAssetBundleName(string path, string bundlename)
    {
        //重新导入资源函数
        AssetImporter assetImporter = AssetImporter.GetAtPath(path);
        if (assetImporter != null)
        {
            assetImporter.assetBundleName = bundlename;
            // 区分不同版本
            assetImporter.assetBundleVariant = null;
        }
    }

    /// <summary>
    /// 删除所有Bundle Name
    /// </summary>
    private static void DeleteAllBundleName()
    {
        string[] names = AssetDatabase.GetAllAssetBundleNames();
        for (int i = 0; i < names.Length; i++)
        {
            AssetDatabase.RemoveAssetBundleName(names[i], true);
        }
    }

    /// <summary>
    /// 清除当前选中的Bundle名字
    /// </summary>
    public static void ClearSelectBundlesName()
    {
        GameObject[] objs = Selection.gameObjects;
        for (int i = 0; i < objs.Length; i++)
        {
            string path = AssetDatabase.GetAssetPath(objs[i]);
            AssetImporter assetimporter = AssetImporter.GetAtPath(path);
            if (assetimporter != null)
            {
                assetimporter.assetBundleName = null;
            }
        }
    }

    /// <summary>
    /// 设置当前选中的Bundle名字
    /// </summary>
    public static void SetSelectBundlesName()
    {
        GameObject[] objs = Selection.gameObjects;
        for (int i = 0; i < objs.Length; i++)
        {
            string path = AssetDatabase.GetAssetPath(objs[i]);
            AssetImporter assetimporter = AssetImporter.GetAtPath(path);
            if (assetimporter != null)
            {
                assetimporter.assetBundleName = objs[i].name;
            }
        }
    }
}
