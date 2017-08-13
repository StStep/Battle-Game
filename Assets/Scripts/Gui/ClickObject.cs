using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public delegate void ClickDel();

[RequireComponent(typeof(Collider2D))]
public class ClickObject : MonoBehaviour, ISelectable
{
    // Status Members
    private bool mSel = false;
    private GuiRender mGuiRender = null;
    private Selector mSelector = null;
    private GameObject mPar = null;

    public ClickDel RightClick = null;
    public ClickDel LeftClick = null;
    public ClickDel MiddleClick = null;
    public ClickDel DoubleRightClick = null;
    public ClickDel DoubleLeftClick = null;
    public ClickDel DoubleMiddleClick = null;
    public ClickDel HoldRightClick = null;
    public ClickDel HoldLeftClick = null;
    public ClickDel HoldMiddleClick = null;

    public GuiRender Renderer
    {
        get { return mGuiRender; }
        protected set { }
    }

    public void Init(GameObject par, Selector sel, GuiRender ren)
    {
        mPar = par;
        mSelector = new Selector(gameObject.name, sel, this);
        mGuiRender = ren;
    }

    // Use this for initialization
    public void Awake()
    {
        ;
    }

    public void Start()
    {
        ;
    }

    public void Update()
    {
        ;
    }

    public void SetPos(Vector3 pos, Quaternion rot)
    {
        transform.position = pos;
        transform.rotation = rot;
    }

    public void Show(bool en)
    {
        gameObject.SetActive(en);
    }

    public bool ChainSelect()
    {
        return mSelector.ChainSelect();
    }

    #region ISelectable
    //////////////////////// ISelectable ///////////////////////////////

    public bool SelectSelf()
    {
        mSel = true;
        mGuiRender.SelectedRender(true);
        return true;
    }

    public bool DeselectSelf()
    {
        mSel = false;
        mGuiRender.SelectedRender(false);
        return true;
    }
    /////////////////////////////////////////////////////////////////////
    #endregion

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
            if (LeftClick != null) LeftClick();
        }

        // Right Click
        if (Input.GetMouseButtonUp(1))
        {
            if (RightClick != null) RightClick();
        }

        // Middle Click
        if (Input.GetMouseButtonUp(2))
        {
            if (MiddleClick != null) MiddleClick();
        }
    }

    /////////////////////////////////////////////////////////////////////
    #endregion
}
