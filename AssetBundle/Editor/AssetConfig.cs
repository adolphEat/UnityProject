/*****************************************
*ScriptName: #ScriptName#
*UnityVerSion: #UNITYVERSION#
*Author: #AUTHOR#
*Date:  #DATE#
*Description: AssetBundle配置类
 
******************************************/
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu]
public class AssetConfig : ScriptableObject
{
    [NonSerialized]
    public string assetconfigPath = "Code/AssetBundle/Editor/AssetConfig.asset";

    [Serializable]
    public class Bundles
    {
        /// <summary>
        /// 文件资源路径
        /// </summary>
        public string SourceFilePath;

        /// <summary>
        /// 目标文件位置
        /// </summary>
        public string TargetFilePath;

        /// <summary>
        /// 文件后缀
        /// </summary>
        public string FileSuffix;
    }

    public List<Bundles> filesConfig;

    /// <summary>
    /// 获取配置项
    /// </summary>
    public List<Bundles> GetAssetBundleConfig()
    {
        List<Bundles> config = UnityEditor.AssetDatabase.LoadAssetAtPath<AssetConfig>(assetconfigPath).filesConfig;
        return config;
    }
}
