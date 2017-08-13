using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


[RequireComponent(typeof(Collider2D))]
public abstract class ClickObject : MonoBehaviour, IClickable, ISelectable
{
    // Status Members
    protected bool mSel = false;
    protected GuiRender mGuiRender = null;
    protected Selector mSelector = null;
    protected GameObject mPar = null;

    public virtual void Init(GameObject par, Selector sel, GuiRender ren)
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

    public void Render()
    {
        mGuiRender.Render();
    }

    #region ISelectable
    //////////////////////// ISelectable ///////////////////////////////

    public virtual bool Select()
    {
        mSel = true;
        return true;
    }

    public virtual bool Deselect()
    {
        mSel = false;
        return true;
    }
    /////////////////////////////////////////////////////////////////////
    #endregion

    #region IClickable
    //////////////////////// IClickable ///////////////////////////////

    public virtual void Click(ClickType t) { }

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
            Click(ClickType.LeftClick);
        }

        // Right Click
        if (Input.GetMouseButtonUp(1))
        {
            Click(ClickType.RightClick);
        }

        // Middle Click
        if (Input.GetMouseButtonUp(2))
        {
            Click(ClickType.MiddleClick);
        }
    }

    /////////////////////////////////////////////////////////////////////
    #endregion
}
