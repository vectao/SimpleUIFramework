using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;
using XLua;

public class SimpleLoopScrollRect : ScrollRectEx
{
    public enum DirectionType
    {
        Horizontal,
        Vetical,
    }

    [SerializeField]
    protected DirectionType m_DirectionType = DirectionType.Vetical;

    [SerializeField]
    public RectTransform m_Item;

    protected int mCurFirstIndex = 0;
    protected int mCurLastIndex = 0;
    private GridLayoutGroup m_GridLayoutGroup = null;

    public int m_TotalCount = 0;
	
	private float mTime = 0f;
    private float mTimeCount = 3f;
    public float m_ScaleSize = 0.3f;

    protected Dictionary<int, RectTransform> m_RectDict = new Dictionary<int, RectTransform>();
    protected Dictionary<int, LuaTable> m_LuaTableDict = new Dictionary<int, LuaTable>();

    public Func<int, RectTransform, LuaTable> OnAddLuaItem;

    [ContextMenu("FillData")]
    public void FillData()
    {
        FillData(0);
    }

    public void FillData(int offset)
    {
        // 偏移校验修正;
        offset = offset >= 0 && offset < m_TotalCount ? offset : 0;

        ClearItems();
        StopMovement();

        // 获取起始创建对象Index;
        int startIndex = GetLine(offset) * GridLayoutGroup.constraintCount;

        mCurFirstIndex = startIndex;
        mCurLastIndex = mCurFirstIndex;

        int fillNum = m_TotalCount <= MaxItemNum ? m_TotalCount : MaxItemNum;


        bool isOffset = false;
        if (mCurLastIndex > m_TotalCount - fillNum)
        {
            mCurLastIndex = m_TotalCount - fillNum;
            mCurFirstIndex = mCurLastIndex;
            isOffset = true;
        }
        for (int i = 0 ; i < fillNum; i++)
        {
            AddItem(mCurLastIndex++);
            if (mCurLastIndex >= m_TotalCount) break;
        }
        if (isOffset)
        {
            Canvas.ForceUpdateCanvases();
            m_ContentBounds = GetBounds();

            RectTransform obj = m_RectDict[offset];
            Vector3 _pos = obj.transform.localPosition;
            Vector3 offsetVec = new Vector3(m_DirectionType == DirectionType.Horizontal ? _pos.x + ItemSize * 0.5f : 0, m_DirectionType == DirectionType.Vetical ? _pos.y + ItemSize * 0.5f : 0, 0);
            Vector3 pos = content.transform.localPosition;
            pos -= offsetVec;
            content.transform.localPosition = pos;
        }
    }

	public void ResetCurrent() {
		foreach (KeyValuePair<int, RectTransform> kv in m_RectDict) {
			int index = kv.Key;
			RectTransform item = kv.Value;
			LuaTable table = m_LuaTableDict[index];
			LuaFunction func = table.Get<LuaFunction>("OnUpdateItem");
			if (func != null) { func.Call(table, index, item); }
		}
		UpdateItems();
	}

    protected override void OnDestroy()
    {
        base.OnDestroy();
        ClearItems();
    }

    [ContextMenu("ClearItems")]
    public void ClearItems()
    {
        /*
        for (int i = 0; i < content.childCount; i++)
        {
            if (Application.isPlaying)
                Destroy(content.GetChild(i).gameObject);
            else
                DestroyImmediate(content.GetChild(i).gameObject);
        }
        */

        foreach (KeyValuePair<int, RectTransform> item in m_RectDict)
        {
            if (item.Value != null)
            {
                GameObject obj = item.Value.gameObject;
                if (Application.isPlaying)
                    Destroy(obj);
                else
                    DestroyImmediate(obj);
            }
        }
        m_RectDict.Clear();

        foreach (KeyValuePair<int, LuaTable> item in m_LuaTableDict)
        {
            LuaTable table = item.Value;
            table.Dispose();
        }
        m_LuaTableDict.Clear();
    }

    public RectTransform AddItem(int index)
    {
        RectTransform item = Instantiate<RectTransform>(m_Item);
        item.SetParent(content);
		item.transform.localPosition = Vector3.zero;
        item.transform.localScale = Vector3.one;

        if (!m_RectDict.ContainsKey(index))
            m_RectDict.Add(index, item);
        else
            m_RectDict[index] = item;

        item.name = index.ToString();
        item.gameObject.SetActive(true);

        if (OnAddLuaItem != null)
        {
            LuaTable table = OnAddLuaItem(index, item);
            m_LuaTableDict.Add(index, table);
            LuaFunction func = table.Get<LuaFunction>("OnUpdateItem");

            if (func != null)
                func.Call(table, index, item);
        }
        return item;
    }

    public void SwapItem(int addIndex, int removeIndex)
    {
        if (!m_RectDict.ContainsKey(removeIndex))
        {
            throw new Exception("data is error");
        }

        RectTransform item = m_RectDict[removeIndex];
        item.gameObject.SetActive(addIndex < m_TotalCount);
        m_RectDict.Remove(removeIndex);

        if (!m_RectDict.ContainsKey(addIndex))
            m_RectDict.Add(addIndex, item);
        else
            m_RectDict[addIndex] = item;

        LuaTable table = m_LuaTableDict[removeIndex];
        m_LuaTableDict.Remove(removeIndex);

        if (!m_LuaTableDict.ContainsKey(addIndex))
            m_LuaTableDict.Add(addIndex, table);
        else
            m_LuaTableDict[addIndex] = table;

        if (addIndex > removeIndex)
            item.SetAsLastSibling();
        else
            item.SetAsFirstSibling();

        item.name = addIndex.ToString();

        LuaFunction func = table.Get<LuaFunction>("OnUpdateItem");
        if (func != null)
            func.Call(table, addIndex, item);
    }

    public float FillSize
    {
        get {
			Rect rect = viewRect.rect;
			return m_DirectionType == DirectionType.Vetical ? Mathf.Abs(rect.height) : Mathf.Abs(rect.width);
        }
    }

    public float ItemSize
    {
        get
        {
            return m_DirectionType == DirectionType.Vetical ? GridLayoutGroup.cellSize.y : GridLayoutGroup.cellSize.x;
        }
    }

    public float ItemSpace
    {
        get
        {
            return m_DirectionType == DirectionType.Vetical ? GridLayoutGroup.spacing.y : GridLayoutGroup.spacing.x;
        }
    }

    public int expansionNum = 3;

    public int MaxItemNum
    {
        get
        {
            if (ItemSize <= 0) throw new Exception("item size can not <= 0");

            return (int)(Math.Ceiling(FillSize / (ItemSize + ItemSpace)) + expansionNum) * GridLayoutGroup.constraintCount;
        }
    }

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

    protected int GetLine(int index)
    {
        return index / GridLayoutGroup.constraintCount;
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);

        mTime = mTimeCount;
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        UpdateItems();
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();

        if (mTime > 0)
        {
            mTime -= Time.deltaTime;
            UpdateItems();
        }
    }

    protected void UpdateItems()
    {
        if (m_DirectionType == DirectionType.Horizontal)
        {
            if (mCurLastIndex < m_TotalCount && m_ContentBounds.max.x - ItemSize * m_ScaleSize <= m_ViewBounds.max.x)
            {
                MoveFirstLine2End();
                OffsetFix();
            }
            else if (mCurFirstIndex > 0 && m_ContentBounds.min.x + ItemSize * m_ScaleSize >= m_ViewBounds.min.x)
            {
                MoveEndLine2First();
                OffsetFix();
            }
        }
        else
        {
            if (mCurFirstIndex > 0 && m_ContentBounds.max.y - ItemSize * m_ScaleSize <= m_ViewBounds.max.y)
            {
                MoveEndLine2First();
                OffsetFix();
            }
            else if (mCurLastIndex < m_TotalCount && m_ContentBounds.min.y + ItemSize * m_ScaleSize >= m_ViewBounds.min.y)
            {
                MoveFirstLine2End();
                OffsetFix();
            }
        }
    }

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

    public void MoveEndLine2First()
    {
        int line = GetLine(mCurLastIndex);
        for (int i = 0; i < GridLayoutGroup.constraintCount; i++)
        {
            SwapItem(--mCurFirstIndex, --mCurLastIndex);
        }

        Vector2 offset = GetOffsetVector(ItemSize + ItemSpace);
        content.anchoredPosition += offset;
        m_PrevPosition += offset;
        m_ContentStartPosition += offset;
        return;
    }

    protected Vector2 GetOffsetVector(float size)
    {
        return m_DirectionType == DirectionType.Horizontal ? new Vector2(-size, 0) : new Vector2(0, size);
    }

    public void MoveFirstLine2End()
    {
        int line = GetLine(mCurFirstIndex);
        for (int i = 0; i < GridLayoutGroup.constraintCount; i++)
        {
            SwapItem(mCurLastIndex++, mCurFirstIndex++);
        }

        Vector2 offset = GetOffsetVector(ItemSize + ItemSpace);
        content.anchoredPosition -= offset;
        m_PrevPosition -= offset;
        m_ContentStartPosition -= offset;
    }

    /*
    protected override void Start()
    {
        base.Start();

        FillData();
    }
    */
}
