using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System;

public class HorizontalloopScrollRect : LoopScrollRectEx
{
    protected float mTime = 0f;

    protected override Vector2 GetOffsetVector(float size)
    {
        return new Vector2(-size, 0);
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

        float size2Fill = viewRect.sizeDelta.x;
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

    IEnumerator Wait4Frame(int num, Action action)
    {
        while (num > 0)
        {
            num--;
            yield return new WaitForEndOfFrame();
        }
        if (action != null)
        {
            action();
        }
    }

    public override float GetSpaceValue()
    {
        return GridLayoutGroup.spacing.x;
    }

    public override float GetSize(RectTransform item = null, bool withSpace = true)
    {
        float size = withSpace ? GridLayoutGroup.spacing.x : 0;
        if (item == null && GridLayoutGroup != null)
        {
            size += GridLayoutGroup.cellSize.x;
        }
        else
        {
            size += LayoutUtility.GetPreferredWidth(item);
        }
        return size;
    }

    public override void UpdateItems()
    {
        if (!Application.isPlaying)
        {
            return;
        }
        if (mCurLastIndex < m_TotalCount && m_ViewBounds.max.x > m_ContentBounds.max.x)
        {
            AppendLineAtEnd();
            OffsetFix();
        }
        else if ((mCurLastIndex >= 0) && m_ViewBounds.max.x < m_ContentBounds.max.x - GetSize(null) * mScaleSize1 && m_ViewBounds.min.x >= m_ContentBounds.min.x)
        {
            RemoveLineAtEnd();
            OffsetFix();
        }

        if (mCurFirstIndex > 0 && m_ViewBounds.min.x < m_ContentBounds.min.x)
        {
            AppendLineAtStart();
            OffsetFix();
        }
        else if ((m_ViewBounds.min.x > m_ContentBounds.min.x + GetSize(null) * mScaleSize1) && (m_ViewBounds.max.x < m_ContentBounds.max.x))
        {
            RemoveLineAtStart();
            OffsetFix();
        }
    }

    protected override void UpdateBounds()
    {
        base.UpdateBounds();
        UpdateItems();
    }
}
