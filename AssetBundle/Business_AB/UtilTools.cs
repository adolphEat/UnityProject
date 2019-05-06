/*****************************************
*ScriptName: #ScriptName#
*UnityVerSion: #UNITYVERSION#
*Author: #AUTHOR#
*Date:  #DATE#
*Description: 工具类
 
******************************************/
using System.IO;

public class UtilTools
{
    public const string URL = "";

    public bool OpenHotUpdate = false;

    /// <summary>
    /// 创建字典
    /// </summary>
    /// <param name="path"></param>
    public static void CreateDirectory(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    /// <summary>
    /// 删除文件下的内容
    /// </summary>
    public static void DeleteDirectoryContext(string path)
    {
        if (Directory.Exists(path))
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            FileSystemInfo[] files = dir.GetFileSystemInfos();

            foreach (var item in files)
            {
                item.Delete();
            }
        }
    }

    /// <summary>
    /// 删除当前文件夹
    /// </summary>
    public static void DeleteDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }

    }
   
}
