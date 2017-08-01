﻿using System;
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
        mLrMove = Draw.CreateLineRend(gameObject, "MovementLine", Color.red);

        mLrLGuide = Draw.CreateLineRend(gameObject, "LeftGuide", Color.yellow);
        mLrLGuide.useWorldSpace = false;
        Draw.DrawLine(mLrLGuide, new Vector2(0, 0), new Vector2(100, 100));

        mLrRGuide = Draw.CreateLineRend(gameObject, "RightGuide", Color.yellow);
        mLrRGuide.useWorldSpace = false;
        Draw.DrawLine(mLrRGuide, new Vector2(0, 0), new Vector2(-100, 100));

        mLrCGuide = Draw.CreateLineRend(gameObject, "CenterGuide", Color.green);
        mLrCGuide.useWorldSpace = false;
        Draw.DrawLine(mLrCGuide, new Vector2(0, 0), new Vector2(0, 100));

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
        Vector3 pnt = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pnt.z = transform.position.z; // Set Z
        Ray2D dir = new Ray2D(transform.position, transform.up);

        // Check Invalid Arc
        myGhost.Show(true);
        if (!Trig.WithinFrontArc(dir, pnt, 0))
        {
            mLrMove.positionCount = 0;
            myGhost.Show(false);
        }
        // Check if Within Line tolerance
        else if(Trig.DistToLine(dir, pnt) < .25f)
        {
            Trig.Line line = new Trig.Line(dir.origin, Trig.NearestPointOnLine(dir, pnt));
            Draw.DrawLine(mLrMove, line);

            // Place Ghost
            myGhost.SetPos(line.End, Quaternion.identity);
        }
        // Else Arc
        else
        {
            Trig.Arc arc = Trig.GetArc(dir, pnt);
            Draw.DrawArc(mLrMove, arc, 20);
            float ghRot = Vector2.SignedAngle(dir.direction, arc.FinalDir);

            // Place Ghost
            myGhost.SetPos(pnt, Quaternion.AngleAxis(ghRot, Vector3.forward));
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
            mSelector.ChainSelect();
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
            mSelector.ChainSelect();
        }

        // TODO Rotate
    }
    /////////////////////////////////////////////////////////////////////
    #endregion
}
