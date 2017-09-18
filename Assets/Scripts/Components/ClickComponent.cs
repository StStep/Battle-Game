using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public delegate void ClickDel();

public class ClickComponent : MonoBehaviour
{
    public ClickDel OnRightClick = null;
    public ClickDel OnLeftClick = null;
    public ClickDel OnMiddleClick = null;
    public ClickDel OnDoubleRightClick = null;
    public ClickDel OnDoubleLeftClick = null;
    public ClickDel OnDoubleMiddleClick = null;
    public ClickDel OnHoldRightClick = null;
    public ClickDel OnHoldLeftClick = null;
    public ClickDel OnHoldMiddleClick = null;

    public ClickComponent Init()
    {
        return this;
    }

    #region MouseIF
    //////////////////////// MouseIF ///////////////////////////////

    public void OnMouseOver()
    {
        // Ignore UI click
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        // Left Click
        if (Input.GetMouseButtonUp(0))
        {
            if (OnLeftClick != null) OnLeftClick();
        }

        // Right Click
        if (Input.GetMouseButtonUp(1))
        {
            if (OnRightClick != null) OnRightClick();
        }

        // Middle Click
        if (Input.GetMouseButtonUp(2))
        {
            if (OnMiddleClick != null) OnMiddleClick();
        }
    }

    /////////////////////////////////////////////////////////////////////
    #endregion
}
