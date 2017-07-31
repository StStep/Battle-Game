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
    private Selector mSelector;

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
        // Prefab items
        mySpriteRend = GetComponent<SpriteRenderer>();
        this.myGhost = GetComponentInChildren(typeof(GuiGhost)) as GuiGhost;
        if(this.myGhost == null)
        {
            Debug.LogError("No ghost object found.");
        }
        this.myGhost.Show(false);

        // Selector
        mSelector = new Selector(gameObject.name, GameManager.instance.mSelector, this);

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
            myGhost.Show(false);
            return;
        }

        // Draw  Path
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = transform.position.z; // Set Z
        Ray2D dir = new Ray2D(transform.position, transform.up);
        Trig.LinePos lp = Trig.GetLine(dir, mousePosition, .25f);
        myGhost.Show(true);
        if (lp.Valid)
        {
            Trig.DrawLine(mLrMove, lp.Line);

            // Place Ghost
            myGhost.SetPos(lp.Line.End, Quaternion.identity);
        }
        else
        {
            Trig.ArcPos ap = Trig.GetArc(dir, mousePosition);
            if (ap.Valid)
            {
                Trig.DrawArc(mLrMove, ap.Arc, 20);
                float ghRot = Vector2.SignedAngle(dir.direction, ap.Arc.FinalDir);

                // Place Ghost
                myGhost.SetPos(mousePosition, Quaternion.AngleAxis(ghRot, Vector3.forward));
            }
            else
            {
                mLrMove.positionCount = 0;
                myGhost.Show(false);
            }
        }

        // TODO If left Click Finalize state
    }

    #endregion


    #region ISelectable
    //////////////////////// ISelectable ///////////////////////////////

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

    #region MouseIF
    //////////////////////// MouseIF ///////////////////////////////

    public void OnMouseOver()
    {
        // Ignore UI click
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        // Left Click
        if (Input.GetMouseButtonDown(0))
            LeftClick();
 
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
            mSelector.ParentSelect();
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
            mSelector.ParentSelect();
        }

        // TODO Rotate
    }
    /////////////////////////////////////////////////////////////////////
    #endregion
}
