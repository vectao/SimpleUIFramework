using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VerticalLoopScrollRect : LoopScrollRectEx
{
    protected float mTime = 0f;

    protected override Vector2 GetOffsetVector(float size)
    {
        return new Vector2(0, size);
    }

    private void Update()
    {
        if (mTime > 0)
        {
            mTime -= Time.deltaTime;
            UpdateItems();
        }
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);

        mTime = 2f;
    }

    public override void FillData(int offset = 0)
    {
        ClearItems();

        StopMovement();

        int index = GetLine(offset) * GridLayoutGroup.constraintCount;
        index = index <= 0 ? 0 : index > m_TotalCount - 1 ? m_TotalCount : index;
        mCurFirstIndex = index;
        mCurLastIndex = mCurFirstIndex;

        OffsetFix();
        float size2Fill = viewRect.sizeDelta.y;
        while (mCurLastIndex <= m_TotalCount - 1 && size2Fill > 0)
        {
            size2Fill -= GetSize(null, true);
            AppendLineAtEnd();
        }
        OffsetFix();
        while (size2Fill > 0 && mCurFirstIndex > 0)
        {
            size2Fill -= GetSize(null, true);
            AppendLineAtStart();
        }
        OffsetFix();
    }

    public override float GetSpaceValue()
    {
        return GridLayoutGroup.spacing.y;
    }

    public override float GetSize(RectTransform item = null, bool withSpace = true)
    {
        float size = withSpace ? GridLayoutGroup.spacing.y : 0;
        if (item == null && GridLayoutGroup != null)
        {
            size += GridLayoutGroup.cellSize.y;
        }
        else
        {
            size += LayoutUtility.GetPreferredHeight(item);
        }
        return size;
    }

    public override void UpdateItems()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        if (mCurLastIndex < m_TotalCount && m_ViewBounds.min.y < m_ContentBounds.min.y)
        {
            AppendLineAtEnd();
            OffsetFix();
        }
        else if ((mCurLastIndex >= 0) && (m_ViewBounds.min.y > m_ContentBounds.min.y + GetSize(null) * mScaleSize1) && (m_ViewBounds.max.y < m_ContentBounds.max.y))
        {
            RemoveLineAtEnd();
            OffsetFix();
        }

        if (m_ViewBounds.max.y > m_ContentBounds.max.y && mCurFirstIndex > 0)
        {
            AppendLineAtStart();
            OffsetFix();
        }
        else if (m_ViewBounds.max.y < m_ContentBounds.max.y - GetSize(null) * mScaleSize1 && m_ViewBounds.min.y >= m_ContentBounds.min.y)
        {
            RemoveLineAtStart();
            OffsetFix();
        }
    }
}
