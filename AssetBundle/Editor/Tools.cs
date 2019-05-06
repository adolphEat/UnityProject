/*****************************************
*ScriptName: #ScriptName#
*UnityVerSion: #UNITYVERSION#
*Author: #AUTHOR#
*Date:  #DATE#
*Description:
 
******************************************/
using UnityEditor;
using UnityEngine;

public static class Tools
{
    [MenuItem("Tools/SetAssetBundleName", false, 1)]
    private static void BulidAssetBundle()
    {
        AssetBulid.SetSelectBundlesName();
    }

    [MenuItem("Tools/BulidAssetBundle", false, 2)]
    private static void BulidAsset()
    {
        AssetBulid.BulidAB();
    }
}
