/*****************************************
*ScriptName: #ScriptName#
*UnityVerSion: #UNITYVERSION#
*Author: #AUTHOR#
*Date:  #DATE#
*Description: AssetBundle 文件管理类
 
 AssetBundle   Unload false 已经实例的物体不会删除 bundle存储的序列化数据释放 今后不能加载数据
 AssetBundle   Unload true  所有的空间释放， 已经实例的物体引用会丢失

******************************************/
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class AssetFileMgr
{
    private int num;

    public static AssetFileMgr Instance { get; private set; }

    /// <summary>
    /// 驱动者
    /// </summary>
    private MonoBehaviour drive;
    private AssetBundle manifestBundle = null;
    private AssetBundleManifest manifest = null;

    /// <summary>
    /// 所有的文件字典
    /// </summary>
    private Dictionary<string, AssetFile> files;

    public AssetFileMgr(MonoBehaviour mono)
    {
        drive = mono;
        Instance = this;
        files = new Dictionary<string, AssetFile>();

        LoadManiFest("manifest");
       /// WarmupAllShaders();
    }

    /// <summary>
    /// 加载manifest（依赖关系）
    /// </summary>
    /// <param name="manifestUrl"></param>
    public void LoadManiFest(string manifestUrl)
    {
        manifestUrl = Application.streamingAssetsPath + "/" + "AssetBundle" + "/" + "AB";
        manifestBundle = AssetBundle.LoadFromFile(manifestUrl);
        manifest = manifestBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        manifestBundle.Unload(false);
    }

    /// <summary>
    /// 加载所有shader的bundle 以及 引用
    /// </summary>
    public void WarmupAllShaders()
    {
        AssetFile shaderAsset = OpenAsset("shaderlibs.ab", null, false);
        shaderAsset.LoadBundleAll();
        Shader.WarmupAllShaders();
    }

    /// <summary>
    /// 加载资源入口
    /// </summary>
    public AssetFile OpenAsset(string assetname, Action<AssetFile> Actionfile, bool isAsync)
    {
        AssetFile file = null;
        
        //????
        //assetname = AssemblyAssetName(assetname);

        if (files.TryGetValue(assetname, out file))
        {
            file.AddRefNum();
        }

        file = AssemblyAssetFile(assetname, isAsync);

        if (isAsync)
        {
            drive.StartCoroutine(AsyncLoad(file, Actionfile));
        }
        else
        {
            SyncLoad(file);
        }

        return file;
    }

    /// <summary>
    /// 组装一个AssetFile
    /// </summary>
    private AssetFile AssemblyAssetFile(string filepath, bool isAsync)
    {
        AssetFile assetfile = new AssetFile(num++, filepath);
        assetfile.onDestory += RemoveElement;
        //将文件添加进加载过的files字典
        files.Add(filepath, assetfile);
        //查找到所有的依赖资源
        string[] dps = manifest.GetAllDependencies(filepath);

        for (int i = 0; i < dps.Length; i++)
        {
            AssetFile file = null;
            //假设已经加载过了 引用计数加一 当前的AssetFile依赖文件加一
            if (files.TryGetValue(dps[i], out file))
            {
                file.AddRefNum();
                assetfile.Filelist.Add(file);
            }
            else
            {
                AssetFile newfile = AssemblyAssetFile(filepath, isAsync);
                assetfile.Filelist.Add(file);
                drive.StartCoroutine(AsyncLoad(newfile, null));
            }
        }

        return assetfile;
    }

    /// <summary>
    /// 同步加载AssetBundle
    /// </summary>
    private void SyncLoad(AssetFile asset)
    {
        string filepath = AssetBundlePath.GetAssetBundlePath(asset.Name);
        asset.assetbundle = AssetBundle.LoadFromFile(filepath);

        if (asset.assetbundle == null)
        {
            Debug.LogError(" path no found file " + filepath);
        }

        asset.IsReady = true;
    }

    /// <summary>
    /// 异步加载AssetBundle 
    /// </summary>
    private IEnumerator AsyncLoad(AssetFile asset, Action<AssetFile> Actionfile)
    {
        if (manifestBundle == null)
        {
            Debug.LogError("manifest is null  wait！");
            yield return null;
        }

        string filepath = AssetBundlePath.GetAssetBundlePath(asset.Name);
        AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(filepath);

        while (!req.isDone)
        {
            asset.LoadProgress = req.progress * 100;
            yield return null;
        }

        /// yield return asset.LoadAssetFileAsync(filepath);
        
        asset.LoadProgress = 100;
        asset.assetbundle = req.assetBundle;

        if (Actionfile != null)
        {
            while (asset.IsReadyAll)
            {
                yield return null;
            }

            Actionfile(asset);
        }

        yield return null;
    }

    /// <summary>
    /// 组装资源名称
    /// </summary>
    private string AssemblyAssetName(string assetname)
    {
        if (!assetname.EndsWith(".ab"))
        {
            assetname += ".ab";
        }

        return assetname;
    }

    /// <summary>
    /// 销毁移除
    /// </summary>
    /// <param name="asset"></param>
    private void RemoveElement(AssetFile asset)
    {
        if (files.ContainsKey(asset.Name))
        {
            files.Remove(asset.Name);
        }
    }

    /// <summary>
    /// 所有卸载
    /// </summary>
    public void OnDestory()
    {
        var it = files.GetEnumerator();
        while (it.MoveNext())
        {
            it.Current.Value.UnLoadBundle();
        }
        it.Dispose();

        files.Clear();
        Instance = null;
    }
}
