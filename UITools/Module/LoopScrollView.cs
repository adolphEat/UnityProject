using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 循环列表 Version 1.0
/// </summary>
[RequireComponent(typeof(ScrollRect))]
public class LoopScrollView : MonoBehaviour
{
    private enum ELoopType
    {
        Honrizition,
        Vertical
    }

    private ELoopType _LoopScrollType;
    
    private ScrollRect scrollRect;
    private RectTransform contentRectTra;

    private List<GameObject> goList; //当前显示的go列表
    private Queue<GameObject> freeGoQueue; //空闲的go队列，存放未显示的go
    private Dictionary<GameObject, int> goIndexDic; //key:所有的go value:真实索引

    private Vector2 cellSize;
    private Vector2 scrollRectSize;

    private int startIndex; //起始索引
    private int maxCount; //创建的最大数量
    private int createCount; //当前显示的数量

    private const int cacheCount = 2; //缓存数目
    private const int invalidStartIndex = -1; //非法的起始索引

    private int lastIndex;
    private int dataCount;
    private GameObject prefab;
    private float cellPadding;
    private float NextCellPadding;

    private Action<GameObject, int> updateCellCB;

    public void Init(int count, GameObject prefabGo, Action<GameObject, int> updateCell, float padding = 0f)
    {
        prefab = prefabGo;
        dataCount = count;
        updateCellCB = updateCell;
        cellPadding = padding;

        goList = new List<GameObject>();
        freeGoQueue = new Queue<GameObject>();
        goIndexDic = new Dictionary<GameObject, int>();
        scrollRect = GetComponent<ScrollRect>();

        contentRectTra = scrollRect.content;
        scrollRectSize = scrollRect.GetComponent<RectTransform>().sizeDelta;
        cellSize = prefab.GetComponent<RectTransform>().sizeDelta;

        InitViewPara();
        AddEventListener();
        ResetSize(dataCount);

        NextCellPadding = cellSize.y + cellPadding;
    }

    /// <summary>
    /// 重置数量 回到初始位置
    /// </summary>
    public void ResetSize(int count)
    {
        dataCount = count;
        contentRectTra.sizeDelta = GetContentSize();



        for (int i = 0; i < createCount; i++)
        {
            CreateItem(i);
        }

        //刷新数据
        startIndex = -1;
        contentRectTra.anchoredPosition = Vector3.zero;
        OnValueChanged(Vector2.zero);
    }

    private void OnResetView()
    {
        //回收显示的go
        for (int i = goList.Count - 1; i >= 0; i--)
        {
            GameObject go = goList[i];
            RecoverItem(go);
        }

        //创建或显示需要的go
        createCount = Mathf.Min(dataCount, maxCount);
        
        
        
        
        
        
    }

    public void UpdateList()
    {
        for (int i = 0; i < goList.Count; i++)
        {
            GameObject go = goList[i];
            int index = goIndexDic[go];
            updateCellCB?.Invoke(go, index);
        }
    }

    private void InitViewPara()
    {
        startIndex = 0;
        maxCount = GetMaxCount();
        createCount = 0;
        lastIndex = startIndex + 2;

        if (scrollRect.horizontal)
        {
            contentRectTra.anchorMin = new Vector2(0, 0);
            contentRectTra.anchorMax = new Vector2(0, 1);
        }
        else
        {
            contentRectTra.anchorMin = new Vector2(0, 1);
            contentRectTra.anchorMax = new Vector2(1, 1);
        }
    }

    private void AddEventListener()
    {
        scrollRect.onValueChanged.RemoveAllListeners();
        scrollRect.onValueChanged.AddListener(OnValueChanged);
    }
    
    /// <summary>
    /// 滑动回调
    /// </summary>
    private void OnValueChanged(Vector2 vec)
    {
        int curStartIndex = GetStartIndex();
        if (startIndex != curStartIndex && curStartIndex > invalidStartIndex)
        {
            startIndex = curStartIndex;

            //收集被移出去的go
            //索引的范围:[startIndex, startIndex + createCount - 1]
            for (int i = goList.Count - 1; i >= 0; i--)
            {
                GameObject go = goList[i];
                int index = goIndexDic[go];
                if (index < startIndex || index > (startIndex + createCount - 1))
                {
                    RecoverItem(go);
                }
            }

            //对移除出的go进行重新排列
            for (int i = startIndex; i < startIndex + createCount; i++)
            {
                if (i >= dataCount)
                {
                    break;
                }

                bool isExist = false;
                for (int j = 0; j < goList.Count; j++)
                {
                    GameObject go = goList[j];
                    int index = goIndexDic[go];
                    if (index == i)
                    {
                        isExist = true;
                        break;
                    }
                }

                if (isExist)
                {
                    continue;
                }

                CreateItem(i);
            }
        }

        PoolFunc();
    }

    private void PoolFunc()
    {
        while (contentRectTra.anchoredPosition.y > (startIndex + 1) * NextCellPadding && lastIndex != dataCount - 1)
        {
            var prefab = goList[0];
            var rectTrans = prefab.GetComponent<RectTransform>();
            goList.RemoveAt(0);
            goList.Add(prefab);

            //将这个物体移到最下方
            rectTrans.anchoredPosition = GetPosition(lastIndex - 1);
            startIndex += 1;
            lastIndex += 1;
        }

        //向上滚动
        while (contentRectTra.anchoredPosition.y < startIndex * NextCellPadding && startIndex != 0)
        {
            var prefab = goList[goList.Count - 1];
            var rectTrans = prefab.GetComponent<RectTransform>();
            goList.RemoveAt(goList.Count - 1);
            goList.Insert(0, prefab);
            rectTrans.anchoredPosition = GetPosition(startIndex - 1);
            startIndex -= 1;
            lastIndex -= 1;
        }
    }

    #region Tool Function
    
    private int GetMaxCount()
    {
        return scrollRect.horizontal
            ? Mathf.CeilToInt(scrollRectSize.x / (cellSize.x + cellPadding)) + cacheCount
            : Mathf.CeilToInt(scrollRectSize.y / (cellSize.y + cellPadding)) + cacheCount;
    }
   
    private int GetStartIndex()
    {
        return scrollRect.horizontal
            ? Mathf.FloorToInt(-contentRectTra.anchoredPosition.x / (cellSize.x + cellPadding))
            : Mathf.FloorToInt(contentRectTra.anchoredPosition.y / (cellSize.y + cellPadding));
    }
    
    private Vector3 GetPosition(int index)
    {
        return scrollRect.horizontal ? new Vector3(index * (cellSize.x + cellPadding), 0, 0)
            : new Vector3(0, index * -(cellSize.y + cellPadding), 0);
    }
     
    private Vector2 GetContentSize()
    {
        return scrollRect.horizontal
            ? new Vector2(cellSize.x * dataCount + cellPadding * (dataCount - 1), contentRectTra.sizeDelta.y)
            : new Vector2(contentRectTra.sizeDelta.x, cellSize.y * dataCount + cellPadding * (dataCount - 1));
    }

    #endregion    
    
    private void CreateItem(int index)
    {
        GameObject go;
        if (freeGoQueue.Count > 0)
        {
            go = freeGoQueue.Dequeue();
            goIndexDic[go] = index;
            go.SetActive(true);
        }
        else
        {
            go = Instantiate(prefab, contentRectTra.transform, true);
            go.gameObject.SetActive(true);
            goIndexDic.Add(go, index);
            var rect = go.GetComponent<RectTransform>();
            rect.pivot = new Vector2(0, 1);
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.localScale = Vector3.one;
        }

        goList.Add(go);
        go.transform.localPosition = GetPosition(index);
        updateCellCB?.Invoke(go, index);
    }

    private void RecoverItem(GameObject go)
    {
        go.SetActive(false);
        goList.Remove(go);
        freeGoQueue.Enqueue(go);
        goIndexDic[go] = invalidStartIndex;
    }
}
