using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateGizmoController : MonoBehaviour
{
    public Renderer leftArrow;
    public Renderer rightArrow;

    public Color baseArrowColor;
    public Color focusArrowColor;

    public delegate void ArrowClickedDelegate();
    public event ArrowClickedDelegate leftArrowClicked;
    public event ArrowClickedDelegate rightArrowClicked;

    void Start()
    {
        m_leftPropertyBlock = new MaterialPropertyBlock();
        m_rightPropertyBlock = new MaterialPropertyBlock();

        m_leftPropertyBlock.SetColor("_Color", baseArrowColor);
        m_rightPropertyBlock.SetColor("_Color", baseArrowColor);

        leftArrow.SetPropertyBlock(m_leftPropertyBlock);
        rightArrow.SetPropertyBlock(m_rightPropertyBlock);
    }

    public void OnLeftArrowClicked()
    {
        if (leftArrowClicked != null) leftArrowClicked();
    }

    public void OnLeftArrowFocus()
    {
        m_leftPropertyBlock.SetColor("_Color", focusArrowColor);
        leftArrow.SetPropertyBlock(m_leftPropertyBlock);
    }

    public void OnLeftArrowLoseFocus()
    {
        m_leftPropertyBlock.SetColor("_Color", baseArrowColor);
        leftArrow.SetPropertyBlock(m_leftPropertyBlock);
    }

    public void OnRightArrowClicked()
    {
        if (rightArrowClicked != null) rightArrowClicked();
    }

    public void OnRightArrowFocus()
    {
        m_rightPropertyBlock.SetColor("_Color", focusArrowColor);
        rightArrow.SetPropertyBlock(m_rightPropertyBlock);
    }

    public void OnRightArrowLoseFocus()
    {
        m_rightPropertyBlock.SetColor("_Color", baseArrowColor);
        rightArrow.SetPropertyBlock(m_rightPropertyBlock);
    }

    private MaterialPropertyBlock m_leftPropertyBlock;
    private MaterialPropertyBlock m_rightPropertyBlock;
}
