using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class GuiUnit : MonoBehaviour, ISelectable
{
    // Types
    enum State {None, Moving};

    // Status Member
    private bool mSel;
    private State mState;
    private Selector mSelector;

    // Components
    private SpriteRenderer mySpriteRend;

    // Static Objects
    private GuiGhost mCursorGhost;
    private SimUnit mSim;
    private SimCmd mSimCmd;
    private GuiCmd mGuiCmd;

    // Use this for initialization
    public void Start()
    {
        // Components
        mySpriteRend = GetComponent<SpriteRenderer>();

        // Status Members
        mSelector = new Selector(gameObject.name, GameManager.instance.mSelector, this);
        mState = State.None;
        mSel = false;

        // Static Init
        mCursorGhost = Draw.MakeGhost(gameObject);
        mCursorGhost.Show(false);
        mSim = new SimUnit();
        mGuiCmd = new GuiCmd(gameObject);
        mGuiCmd.SetLeft(SelectMove);
        mSimCmd = new SimCmd();

        // Startup Functions
        ResetPath();
        Deselect();
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

    #region Utility

    private void ResetPath()
    {
        Ray2D dir = new Ray2D(this.transform.position, this.transform.up);
        // Reset Cmd Sim
        mSimCmd.Reset(dir);

        // Reset Cmd Gui
        mGuiCmd.Reset(dir);
    }

    public void SelectMove()
    {
        if (!mSel)
        {
            mSelector.ChainSelect();
        }

        if (mSel)
        {
            if (mState == State.None && mSimCmd.TimeLeft > float.Epsilon)
            {
                mState = State.Moving; // Defualt to moving after selecting
            }
        }
    }

    public void SelectRotate()
    {
        if (!mSel)
        {
            mSelector.ChainSelect();
        }

        // TODO Rotate
    }

    #endregion

    #region States

    private void StMove()
    {
        // If right Click exit state
        if (Input.GetMouseButtonDown(1))
        {
            mState = State.None;
            mGuiCmd.Retract();
            mCursorGhost.Show(false);
            return;
        }

        // Draw  Path
        Vector3 pnt = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pnt.z = transform.position.z; // Set Z
        Ray2D dir = mSimCmd.FinalDir;
        float timeLeft = mSimCmd.TimeLeft;

        mCursorGhost.Show(true);
        mCursorGhost.Bad();
        Path curPath = null;

        // Min Move Distance or Back Hald
        if (Trig.GetHalf(dir, pnt, 0, 0) == Trig.Half.back
            || Vector3.Distance(dir.origin, pnt) < GameManager.MOVE_MIN_PNT_DIST)
        { }
        // Line if within Line tolerance, or within min straight distance of start
        else if(Trig.DistToLine(dir, pnt) < GameManager.MOVE_LINE_TOL
            || Vector2.Distance(dir.origin, Trig.NearestPointOnLine(dir, pnt)) < GameManager.MOVE_MIN_ARC_DIST)
        {
            curPath = new LinePath(timeLeft, dir, pnt);
        }
        // Else Arc
        else
        {
            curPath = new ArcPath(timeLeft, dir, pnt);
        }

        if(curPath != null)
        {
            // Render
            mGuiCmd.Stretch(curPath.RenderPoints(), pnt);

            // Place Ghost
            mCursorGhost.Good();
            float ghRot = Vector2.SignedAngle(Vector2.up, curPath.EndDir);
            mCursorGhost.SetPos(curPath.End, Quaternion.AngleAxis(ghRot, Vector3.forward));
        }
        else
        {
            mGuiCmd.Retract();
            mCursorGhost.SetPos(pnt, Quaternion.identity);
        }

        // If left Click and Path, Add Movement Segment
        if (curPath != null && Input.GetMouseButtonDown(0))
        {
            mSimCmd.Add(curPath);
            mGuiCmd.LockIn(mSimCmd.FinalDir);

            if(mSimCmd.TimeLeft < float.Epsilon)
            {
                mGuiCmd.Fin();
                mState = State.None;
                mGuiCmd.Retract();
                mCursorGhost.Show(false);
            }
        }
    }

    #endregion

    #region ISelectable
    //////////////////////// ISelectable ///////////////////////////////

    public bool Select()
    {
        mSel = true;
        mGuiCmd.EnableGuides(true);
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
        mGuiCmd.EnableGuides(false);
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
            ResetPath();
            SelectMove();
        }
 
        // Right Click
        if (Input.GetMouseButtonUp(1))
        {
            SelectRotate();
        }

        // Middle Click
        if (Input.GetMouseButtonUp(2))
        {
            Debug.Log("Pressed middle click.");
        }
    }

    /////////////////////////////////////////////////////////////////////
    #endregion
}
