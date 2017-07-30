using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D), typeof(Sprite))]
public class GuiUnit : MonoBehaviour, ISelectable
{
    enum State {None, Moving};

    private bool mSel = false;
    private State mState = State.None;

    // Prefab items
    private SpriteRenderer mySpriteRend;
    private GuiGhost myGhost;

    // Lines, dynamic
    private LineRenderer mLrMove;
    private LineRenderer mLrLGuide;
    private LineRenderer mLrRGuide;
    private LineRenderer mLrCGuide;


    // Use this for initialization
    public void Start()
    {
        mySpriteRend = GetComponent<SpriteRenderer>();
        this.myGhost = GetComponentInChildren(typeof(GuiGhost)) as GuiGhost;
        if(this.myGhost == null)
        {
            Debug.LogError("No ghost object found.");
        }
        this.myGhost.Hide();

        // Make lines
        mLrMove = Create.LineRender(gameObject, "MovementLine", Color.red);

        mLrLGuide = Create.LineRender(gameObject, "LeftGuide", Color.yellow);
        mLrLGuide.useWorldSpace = false;
        Trig.DrawLine(mLrLGuide, new Vector2(0, 0), new Vector2(100, 100));

        mLrRGuide = Create.LineRender(gameObject, "RightGuide", Color.yellow);
        mLrRGuide.useWorldSpace = false;
        Trig.DrawLine(mLrRGuide, new Vector2(0, 0), new Vector2(-100, 100));

        mLrCGuide = Create.LineRender(gameObject, "CenterGuide", Color.green);
        mLrCGuide.useWorldSpace = false;
        Trig.DrawLine(mLrCGuide, new Vector2(0, 0), new Vector2(0, 100));

        EnableGuides(false);

        Deselect();
    }

    private void EnableGuides(bool en)
    {
        mLrLGuide.enabled = en;
        mLrRGuide.enabled = en;
        mLrCGuide.enabled = en;

    }

    // Update is called once per frame
    public void Update()
    {
        switch (mState)
        {
            case State.Moving:
                StMove();
                break;
            default:
                break;
        }
    }

    #region States

    private void StMove()
    {
        // If right Click exit state
        if (Input.GetMouseButtonDown(1))
        {
            mState = State.None;
            mLrMove.positionCount = 0;
            return;
        }

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Ray2D dir = new Ray2D(transform.position, transform.up);
        Trig.LinePos lp = Trig.GetLine(dir, mousePosition, .25f);
        if (lp.Valid)
        {
            Trig.DrawLine(mLrMove, lp.Line);
        }
        else
        {
            Trig.ArcPos ap = Trig.GetArc(dir, mousePosition);
            if (ap.Valid)
            {
                Trig.DrawArc(mLrMove, ap.Arc, 20);
            }
            else
            {
                mLrMove.positionCount = 0;
            }

        }
    }

    #endregion


    #region ISelectable
    //////////////////////// ISelectable ///////////////////////////////

    public string Name()
    {
        return this.gameObject.name;
    }

    public bool Select()
    {
        mSel = true;
        EnableGuides(true);
        mySpriteRend.color = Color.yellow;

        return true;
    }

    public bool Deselect()
    {
        // Only deslect if doing nothing
        if (mState != State.None)
            return false;

        mSel = false;
        mySpriteRend.color = Color.blue;
        EnableGuides(false);
        return true;
    }
    /////////////////////////////////////////////////////////////////////
    #endregion

    #region IClickObject
    //////////////////////// IClickObject ///////////////////////////////

    public void OnMouseOver()
    {
        // Ignore UI click
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        // Left Click
        if (Input.GetMouseButtonDown(0))
        {
            LeftClick();
        }

        // Right Click
        if (Input.GetMouseButtonDown(1))
            RightClick();

        // Middle Click
        if (Input.GetMouseButtonDown(2))
            Debug.Log("Pressed middle click.");
    }

    public void LeftClick()
    {
        if(!mSel)
        {
            GameManager.instance.SelectItem(this);
        }

        if (mSel)
        {
            if (mState == State.None)
            {
                mState = State.Moving; // Defualt to moving after selecting
            }
        }
    }

    public void RightClick()
    {
        if (!mSel)
        {
            GameManager.instance.SelectItem(this);
        }

        Vector3 p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        myGhost.Rotate(new Vector2(p.x, p.y));
        myGhost.Show(this.gameObject.transform.position, this.gameObject.transform.rotation);
    }
    /////////////////////////////////////////////////////////////////////
    #endregion
}
