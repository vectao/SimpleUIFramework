using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

public abstract class LoopScrollRectEx : ScrollRectEx
{
    protected float mScaleSize1 = 1.1f;
    protected float mScaleSize2 = 0.1f;

    public RectTransform m_Item;
    protected int mCurFirstIndex = 0;
    protected int mCurLastIndex = 0;
    private GridLayoutGroup m_GridLayoutGroup = null;
    [SerializeField] protected int m_TotalCount = 0;
    protected Dictionary<int, RectTransform> m_List = new Dictionary<int, RectTransform>();

    public Action<int, RectTransform> FillItem;

    /// <summary>
    /// 获取Item间隔;
    /// </summary>
    /// <returns>返回Item对象的间隔距离</returns>
    public abstract float GetSpaceValue();

    /// <summary>
    /// 刷新Items，进行Item对象的新增或裁剪;
    /// </summary>
    public abstract void UpdateItems();

    /// <summary>
    /// 获取Item对象的大小
    /// </summary>
    /// <param name="item"></param>
    /// <param name="withSpace"></param>
    /// <returns>基于排版的格式，返回对应的size.x或size.y</returns>
    public abstract float GetSize(RectTransform item = null, bool withSpace = true);

    public abstract void FillData(int index = 0);

    /// <summary>
    /// 获取GridLayoutGroup;
    /// </summary>
    public GridLayoutGroup GridLayoutGroup
    {
        get
        {
            if (m_GridLayoutGroup == null)
            {
                m_GridLayoutGroup = content.GetComponent<GridLayoutGroup>();
            }
            return m_GridLayoutGroup;
        }
    }

    /// <summary>
    /// 获取当前索引所在的行或列;
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    protected int GetLine(int index)
    {
        int line = index / GridLayoutGroup.constraintCount;
        return line;
    }

    protected abstract Vector2 GetOffsetVector(float size);

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        UpdateItems();
    }

    protected virtual void ClearItems()
    {
        foreach (KeyValuePair<int, RectTransform> item in m_List)
        {
            GameObject obj = item.Value.gameObject;
            obj.SetActive(false);
            Destroy(obj);
        }
        m_List.Clear();
    }

    /// <summary>
    /// 偏移值修复;
    /// </summary>
    protected void OffsetFix()
    {
        Canvas.ForceUpdateCanvases();
        m_ContentBounds = GetBounds();

        Vector3 contentSize = m_ContentBounds.size;
        Vector3 contentPos = m_ContentBounds.center;
        Vector3 excess = m_ViewBounds.size - contentSize;
        if (excess.x > 0)
        {
            contentPos.x -= excess.x * (m_Content.pivot.x - 0.5f);
            contentSize.x = m_ViewBounds.size.x;
        }
        if (excess.y > 0)
        {
            contentPos.y -= excess.y * (m_Content.pivot.y - 0.5f);
            contentSize.y = m_ViewBounds.size.y;
        }

        m_ContentBounds.size = contentSize;
        m_ContentBounds.center = contentPos;
    }

    protected void AppendLineAtStart()
    {
        if (mCurFirstIndex <= 0) return;

        int line1 = GetLine(mCurFirstIndex);
        int line2 = line1 - 1;
        if (line2 < 0) return;

        int count = (mCurFirstIndex - 1) % GridLayoutGroup.constraintCount;

        RectTransform obj = null;
        //Debug.LogError("AppendLineAtStart");

        for (int i = count; i >= 0; i--)
        {
            if (mCurFirstIndex >= 0)
            {
                RectTransform _obj = AddItemAtStart();
                obj = _obj != null ? _obj : obj;
            }
        }

        AppendLineAtStartOffsetFix(obj);
    }

    protected void AppendLineAtStartOffsetFix(RectTransform obj = null)
    {
        if (obj != null)
        {
            float size = GetSize(obj);
            Vector2 offset = GetOffsetVector(size);
            content.anchoredPosition += offset;
            m_PrevPosition += offset;
            m_ContentStartPosition += offset;
        }
    }

    protected float AppendLineAtEnd()
    {
        int line1 = GetLine(mCurLastIndex);
        int line2 = line1;
        int count = GridLayoutGroup.constraintCount - mCurLastIndex % GridLayoutGroup.constraintCount;

        //Debug.LogError("AppendLineAtEnd");
        for (int i = 0; i < count; i++)
        {
            if (line1 == line2 && mCurLastIndex < m_TotalCount)
            {
                AddItemAtEnd();
                line2 = GetLine(mCurLastIndex);
            }
        }
        return 0;
    }

    protected RectTransform AddItemAtStart()
    {
        mCurFirstIndex--;
        if (mCurFirstIndex < 0) return null;

        //Debug.LogError(mCurFirstIndex);
        RectTransform obj = AddItem(mCurFirstIndex);
        if (obj != null)
        {
            obj.SetAsFirstSibling();
        }
        return obj;
    }

    protected void AddItemAtEnd()
    {
        if (mCurLastIndex > m_TotalCount - 1) return;

        RectTransform obj = AddItem(mCurLastIndex);
        if (obj != null)
        {
            obj.SetAsLastSibling();
        }
        //Debug.LogError(mCurLastIndex);
        mCurLastIndex++;
    }

    protected virtual RectTransform AddItem(int index)
    {
        RectTransform obj = PullFromPool();//Instantiate (m_Item).GetComponent<RectTransform> ();
        obj.SetParent(content.transform);
        if (m_List.ContainsKey(index))
        {
            m_List[index] = obj;
        }
        else
        {
            m_List.Add(index, obj);
        }
        obj.transform.localScale = Vector3.one;
        obj.gameObject.SetActive(true);
        obj.gameObject.name = "item" + index;
        obj.GetComponentInChildren<Text>().text = "item" + index;
        return obj;
    }

    protected void RemoveLineAtStart()
    {
        int line = GetLine(mCurFirstIndex);

        //Debug.LogError("RemoveLineAtStart");
        for (int i = 0; i < GridLayoutGroup.constraintCount; i++)
        {
            int index = line * GridLayoutGroup.constraintCount + i;
            //Debug.LogError(index);
            mCurFirstIndex++;
            if (m_List.ContainsKey(index) && m_List[index] != null)
            {
                RemoveItem(m_List[index]);
                m_List.Remove(index);
            }
        }

        RemoveLineAtStartOffsetFix(null);
    }

    protected void RemoveLineAtStartOffsetFix(RectTransform obj = null)
    {
        Vector2 offset = GetOffsetVector(GetSize(null));
        content.anchoredPosition -= offset;
        m_PrevPosition -= offset;
        m_ContentStartPosition -= offset;
    }

    protected void RemoveLineAtEnd()
    {
        if (mCurLastIndex < 0) return;
        mCurLastIndex--;
        //Debug.LogError("RemoveLineAtEnd");
        int line = GetLine(mCurLastIndex);

        int count = (mCurLastIndex) % GridLayoutGroup.constraintCount;

        for (int i = count; i >= 0; i--)
        {
            if (m_List.ContainsKey(mCurLastIndex) && m_List[mCurLastIndex] != null)
            {
                RemoveItem(m_List[mCurLastIndex]);
                //DestroyImmediate(m_List[mCurLastIndex].gameObject);
                m_List.Remove(mCurLastIndex);
            }
            //Debug.LogError(mCurLastIndex);
            mCurLastIndex--;
        }
        mCurLastIndex++;
    }

    protected virtual void RemoveItem(RectTransform obj)
    {
        PushToPool(obj);
    }


    



    //------------------------------------Pool---------------------------------
    public List<RectTransform> m_PoolList = new List<RectTransform>();
    private GameObject PoolManager;
    public void PushToPool(RectTransform obj)
    {
        if (PoolManager == null)
        {
            PoolManager = new GameObject("PoolManager");
            DontDestroyOnLoad(PoolManager);
        }
        obj.gameObject.SetActive(false);
        obj.SetParent(PoolManager.transform);
        m_PoolList.Add(obj);
    }

    public RectTransform PullFromPool()
    {
        if (m_PoolList.Count > 0 && m_PoolList[m_PoolList.Count - 1] != null)
        {
            RectTransform obj = m_PoolList[m_PoolList.Count - 1];
            m_PoolList.RemoveAt(m_PoolList.Count - 1);
            return obj;
        }
        return Instantiate(m_Item).GetComponent<RectTransform>();
    }

    protected override void OnDestroy()
    {
        ClearItems();
        ClearPool();
    }
    private void ClearPool()
    {
        for (int i = 0; i < m_PoolList.Count; i++)
        {
            RectTransform rt = m_PoolList[i];
            if (rt != null)
                DestroyImmediate(rt);
        }
        m_PoolList.Clear();
    }

    /*
    #if UNITY_EDITOR
        private string skipNum = "0";
        void OnGUI()
        {
            GUILayout.BeginHorizontal();
            skipNum = GUILayout.TextField(skipNum);
            if (GUILayout.Button("跳转"))
            {
                int num = 0;
                int.TryParse(skipNum, out num);
                FillData(num);
            }
            GUILayout.EndHorizontal();
        }

    #endif
    */
    [ContextMenu("Grid填充")]
    private void Skip2Num()
    {
        Skip2Num(0);
    }

    private void Skip2Num(int num)
    {
        FillData(num);
    }
}
