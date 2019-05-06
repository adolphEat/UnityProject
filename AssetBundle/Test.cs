/*****************************************
*ScriptName: #ScriptName#
*UnityVerSion: #UNITYVERSION#
*Author: #AUTHOR#
*Date:  #DATE#
*Description:
 
******************************************/

using UnityEngine;

public class Test : MonoBehaviour
{
    AssetFileMgr assetfiles;
    AssetBusiness assetbusiness;

    [ContextMenu("TestLoad")]
    public void TestScript()
    {
        /*string[] depend = UnityEditor.AssetDatabase.GetDependencies(new[] { Application.dataPath + "/Resource/" + "cube" }, true);

        Debug.LogError(depend.Length);

        for (int i = 0; i < depend.Length; i++)
        {
            Debug.LogError(depend[i]);
        }*/
    }

    private void Start()
    {
        assetfiles = new AssetFileMgr(this);
        assetbusiness = new AssetBusiness();

        object obj = assetbusiness.SyncLoadAsset<GameObject>("cube");

        GameObject.Instantiate<GameObject>((GameObject)obj,
                                            Vector3.zero,
                                            Quaternion.identity);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            assetbusiness.UnLoadOneAsset("cube");
        }
    }
}
