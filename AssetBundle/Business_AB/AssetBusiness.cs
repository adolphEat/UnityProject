/*****************************************
*ScriptName: #ScriptName#
*UnityVerSion: #UNITYVERSION#
*Author: #AUTHOR#
*Date:  #DATE#
*Description:  资源业务层
 
******************************************/

using UnityEngine;
using System.Collections.Generic;

public class AssetBusiness
{
    /// <summary>
    /// 资源类型
    /// </summary>
    public enum AssetType
    {
        HoldOnAsset, //常驻
        UIAsset,     //非战斗资源
        BattleAsset, //战斗资源
    }

    private Dictionary<string, AssetFile> uiFile;
    private Dictionary<string, AssetFile> battleFile;
    private Dictionary<string, AssetFile> holdonFile;

    public AssetBusiness()
    {
        uiFile = new Dictionary<string, AssetFile>();
        battleFile = new Dictionary<string, AssetFile>();
        holdonFile = new Dictionary<string, AssetFile>();
    }

    /// <summary>
    /// 同步加载资源
    /// </summary>
    public object SyncLoadAsset<T>(string assetname, AssetType assetType= AssetType.HoldOnAsset)
    {
        AssetFile assetFile = AssetFileMgr.Instance.OpenAsset(assetname, null, false);
        object obj = assetFile.LoadAssetFile(assetname, typeof(T));
        if (obj != null)
        {
            switch (assetType)
            {
                case AssetType.HoldOnAsset:
                    holdonFile[assetname] = assetFile;
                    break;
                case AssetType.UIAsset:
                    uiFile[assetname] = assetFile;
                    break;
                case AssetType.BattleAsset:
                    battleFile[assetname] = assetFile;
                    break;
            }
        }

        return obj;
    }

    /// <summary>
    /// 异步加载资源
    /// </summary>
    public void AsyncLoadAsset(string assetname, AssetType assetType)
    {
        AssetFileMgr.Instance.OpenAsset
            (assetname,
            (files) =>
            {
                switch (assetType)
                {
                    case AssetType.HoldOnAsset:
                        holdonFile[assetname] = files;
                        break;
                    case AssetType.UIAsset:
                        uiFile[assetname] = files;
                        break;
                    case AssetType.BattleAsset:
                        battleFile[assetname] = files;
                        break;
                }
            },
            true);
    }

    /// <summary>
    /// 卸载单个资源
    /// </summary>
    public void UnLoadOneAsset(string assetname, AssetType assetType = AssetType.HoldOnAsset)
    {
        bool ishave = false;
        AssetFile assetfile = null;
        switch (assetType)
        {
            case AssetType.HoldOnAsset:
                ishave = holdonFile.TryGetValue(assetname, out assetfile);
                break;
            case AssetType.UIAsset:
                ishave = uiFile.TryGetValue(assetname, out assetfile);
                break;
            case AssetType.BattleAsset:
                ishave = battleFile.TryGetValue(assetname, out assetfile);
                break;
        }

        if (ishave)
        {
            assetfile.UnLoadBundle();
            //TODO 确实卸载当前这个资源 不应该卸载相关的所有资源
            //assetfile.Destory();
        }
    }

    /// <summary>
    /// 卸载某一类资源
    /// </summary>
    public void UnLoadByAssetType(AssetType assetType)
    {
        switch (assetType)
        {
            case AssetType.HoldOnAsset:
                UnLoadDirctionary(ref holdonFile);
                break;
            case AssetType.UIAsset:
                UnLoadDirctionary(ref uiFile);
                break;
            case AssetType.BattleAsset:
                UnLoadDirctionary(ref battleFile);
                break;
        }
    }

    private void UnLoadDirctionary(ref Dictionary<string, AssetFile> files)
    {
        var it = files.GetEnumerator();

        while (it.MoveNext())
        {
            it.Current.Value.Destory();
        }

        it.Dispose();
        files.Clear();
    }
}
