using UnityEngine;
using System;

public class UIUtil
{
    //public static void SetSortingLayer(Renderer renderer, string lay)
    //{
    //    renderer.sortingLayerID = SortingLayer.NameToID(lay);
    //}
    //public static T Get<T>(GameObject go) where T : MonoBehaviour
    //{
    //    T t = null;
    //    T[] arr = go.GetComponentsInChildren<T>(true);
    //    if (arr.Length > 0)
    //    {
    //        t = arr[0];
    //    }
    //    return t;
    //}
    //ui
    //public static void SetAllLayer(Transform transform, int layer)
    //{
    //    transform.gameObject.layer = layer;
    //    for (int i = 0; i < transform.childCount; ++i)
    //    {
    //        Transform childTranform = transform.GetChild(i);
    //        childTranform.gameObject.layer = layer;
    //        SetAllLayer(childTranform, layer);
    //    }
    //}
    public static void AddChildTfAllDefault(Transform tC, Transform tP)
    {
        tC.SetParent(tP);
        tC.localScale = Vector3.one;
        tC.localPosition = Vector3.zero;
        tC.localRotation = Quaternion.identity;
    }
    static void AddChildTf(Transform tC, Transform tP)
    {
        tC.SetParent(tP);
        //tC.localScale = Vector3.one;
        tC.localPosition = Vector3.zero;
        tC.localRotation = Quaternion.identity;
    }
    public static void AddChild(Transform tC, Transform tP)
    {
        if (tC != null)
        {
            AddChildTf(tC, tP);
            return;
        }
        Debug.LogError("AddChild tf null.");
    }
    public static void AddChildEffect(Transform tC, Transform tP)
    {
        if (tC != null)
        {
            AddChildTf(tC, tP);
            return;
        }
        Debug.LogError("AddChild tf null.");
    }
    public static Transform FindInChildByName(Transform parent, string childName)
    {
        Transform child = parent.Find(childName);
        if (child == null)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                child = FindInChildByName(parent.GetChild(i), childName);
                if (child != null)
                {
                    break;
                }
            }
        }
        return child;
    }

    internal static Color GetColor(int r, int g, int b)
    {
        return new Color(r / 255f, g / 255f, b / 255f);
    }

    //UNITY_EDITOR
    //UNITY_STANDALONE_OSX
    //UNITY_STANDALONE_WIN
    //UNITY_STANDALONE_LINUX
    //UNITY_STANDALONE
    //UNITY_WEBPLAYER
    //UNITY_WII
    //UNITY_IPHONE
    //UNITY_ANDROID
    //UNITY_PS3
    //UNITY_XBOX360
    //public static string GetTimeHHMMSS(float seconds)
    //{
    //    int h = (int)(seconds / 3600);
    //    int m = (int)((seconds - h * 3600) / 60);
    //    int s = (int)(seconds - h * 3600 - m * 60);
    //    return h.ToString("00") + ":" + m.ToString("00") + ":" + s.ToString("00");
    //}
    //public static bool IsOverUI(Vector3 mousePosition)
    //{
    //    return UICamera.isOverUI;
    //GameObject hoverobject = UICamera.Raycast(mousePosition) ? UICamera.lastHit.collider.gameObject : null;
    //return hoverobject != null;
    //}

}
