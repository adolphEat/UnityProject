/*****************************************
*ScriptName: #ScriptName#
*UnityVerSion: #UNITYVERSION#
*Author: #AUTHOR#
*Date:  #DATE#
*Description:  单个资源相关
 
******************************************/
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class AssetFile
{
    public int Id;

    public string Name;

    public float LoadProgress;

    public AssetBundle assetbundle;

    public List<AssetFile> Filelist;

    public event Action<AssetFile> onDestory = delegate { };

    //当前Bundle加载是否完毕
    public bool IsReady;

    //引用计数
    public int Count = 1;

    //是否全部加载完毕
    public bool IsReadyAll
    {
        get
        {
            bool isreadyAll = true;

            for (int i = 0; i < Filelist.Count; i++)
            {
                if (!Filelist[i].IsReady || !IsReady)
                {
                    isreadyAll = false;
                    break;
                }
            }

            return isreadyAll;
        }
    }

    public AssetFile(int id ,string name)
    {
        Id = id;
        Name = name;
        LoadProgress = 0;
        Filelist = new List<AssetFile>();
    }

    /// <summary>
    /// 同步加载文件
    /// </summary>
    /// <param name="name">文件名称</param>
    /// <param name="type">文件类型</param>
    /// <returns></returns>
    public object LoadAssetFile(string name, Type type)
    {
        if (assetbundle == null)
        {
            return null;
        }

        return assetbundle.LoadAsset(name, type);
    }

    /// <summary>
    /// 异步加载文件
    /// </summary>
    /// <param name="name"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public IEnumerator LoadAssetFileAsync(string name)
    {
        if (assetbundle == null)
        {
            yield break;
        }

        AssetBundleRequest abr = assetbundle.LoadAssetAsync(name);

        while (!abr.isDone)
        {
            LoadProgress = abr.progress * 100;
            yield return null;
        }

        yield return abr.asset;
    }

    /// <summary>
    /// 引用计数增加
    /// </summary>
    public void AddRefNum()
    {
        Count++;

        for (int i = 0; i < Filelist.Count; i++)
        {
            AssetFile assetfile = Filelist[i];
            assetfile.AddRefNum();
        }
    }

    /// <summary>
    /// 引用计数减少 (正常卸载引用)
    /// </summary>
    public void SubRefNum()
    {
        Count--;

        for (int i = 0; i < Filelist.Count; i++)
        {
            AssetFile assetfile = Filelist[i];
            assetfile.SubRefNum();
        }

        if (Count <= 0)
        {
            if (assetbundle != null)
            {
                assetbundle.Unload(true);
                assetbundle = null;
            }

            Filelist.Clear();
            onDestory(this);
        }
    }

    /// <summary>
    /// 手动回收
    /// </summary>
    public void Destory()
    {
        if (assetbundle != null)
        {
            assetbundle.Unload(true);
            assetbundle = null;
        }

        while (Count-- > 0)
        {
            for (int i = 0; i < Filelist.Count; i++)
            {
                AssetFile assetfile = Filelist[i];
                assetfile.SubRefNum();
            }
        }
        Filelist.Clear();

        onDestory(this);
    }

    #region Tools

    public void UnLoadBundle()
    {
        if (assetbundle != null)
        {
            assetbundle.Unload(true);
        }

        assetbundle = null;
    }

    public void LoadBundleAll()
    {
        if (assetbundle != null)
        {
            assetbundle.LoadAllAssets();
        }
    }

    #endregion

}
