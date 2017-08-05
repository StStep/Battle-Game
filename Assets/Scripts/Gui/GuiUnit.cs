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
    private List<LineRenderer> mLrMoves;
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
        mLrMoves = new List<LineRenderer>();

        // Make lines
        LineRenderer curMove = Draw.CreateLineRend(gameObject, "MovementLine", Color.red);
        mLrMoves.Add(curMove);

        Vector3[] line = new Vector3[2];
        line[0] = Vector3.zero;
        mLrLGuide = Draw.CreateLineRend(gameObject, "LeftGuide", Color.yellow);
        mLrLGuide.useWorldSpace = false;
        line[1] = 100 * (Vector3.up + Vector3.left);
        Draw.DrawLineRend(mLrLGuide, line);

        mLrRGuide = Draw.CreateLineRend(gameObject, "RightGuide", Color.yellow);
        mLrRGuide.useWorldSpace = false;
        line[1] = 100 * (Vector3.up + Vector3.right);
        Draw.DrawLineRend(mLrRGuide, line);

        mLrCGuide = Draw.CreateLineRend(gameObject, "CenterGuide", Color.green);
        mLrCGuide.useWorldSpace = false;
        line[1] = 100 * (Vector3.up);
        Draw.DrawLineRend(mLrCGuide, line);

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
            mLrMoves[mLrMoves.Count - 1].positionCount = 0;
            myGhost.Show(false);
            return;
        }

        // Draw  Path
        Vector3 pnt = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pnt.z = transform.position.z; // Set Z
        Ray2D dir = new Ray2D(transform.position, transform.up);

        // If in Back Arc, nothing
        myGhost.Show(true);
        Trig.Quarter qrt = Trig.GetQuarter(dir, pnt, 0, 0);
        if (qrt == Trig.Quarter.back)
        {
            mLrMoves[mLrMoves.Count - 1].positionCount = 0;
            myGhost.SetPos(pnt, Quaternion.identity);
        }
        // If left, bend to edge
        else if (qrt == Trig.Quarter.left)
        {
            // TODO
            mLrMoves[mLrMoves.Count - 1].positionCount = 0;
            myGhost.SetPos(pnt, Quaternion.identity);
        }
        // If right, bend to edge
        else if (qrt == Trig.Quarter.right)
        {
            // TODO
            mLrMoves[mLrMoves.Count - 1].positionCount = 0;
            myGhost.SetPos(pnt, Quaternion.identity);
        }
        // Check if Within Line tolerance
        else if(Trig.DistToLine(dir, pnt) < .25f)
        {
            LinePath line = new LinePath(dir, Trig.NearestPointOnLine(dir, pnt));
            Draw.DrawLineRend(mLrMoves[mLrMoves.Count - 1], line.RenderPoints());

            // Place Ghost
            myGhost.SetPos(line.End, Quaternion.identity);
        }
        // Else Arc
        else
        {
            ArcPath arc = new ArcPath(dir, pnt);
            Draw.DrawLineRend(mLrMoves[mLrMoves.Count - 1], arc.RenderPoints());
            float ghRot = Vector2.SignedAngle(dir.direction, arc.EndDir);

            // Place Ghost
            myGhost.SetPos(pnt, Quaternion.AngleAxis(ghRot, Vector3.forward));
        }

        // If left Click Add Movement Segment
        if (Input.GetMouseButtonDown(0))
        {
            LineRenderer curMove = Draw.CreateLineRend(gameObject, "MovementLine", Color.red);
            mLrMoves.Add(curMove);
        }
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
        if (Input.GetMouseButtonUp(0))
        {
            LeftClick();
        }
 
        // Right Click
        if (Input.GetMouseButtonUp(1))
        {
            RightClick();
        }

        // Middle Click
        if (Input.GetMouseButtonUp(2))
        {
            Debug.Log("Pressed middle click.");
        }
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
