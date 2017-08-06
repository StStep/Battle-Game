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
    private SimCmd mPaths;
    private GuiCmd mLrMoves;

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
        mLrMoves = new GuiCmd(gameObject);
        mLrMoves.SetLeft(SelectMove);
        mPaths = new SimCmd();

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
        mPaths.Reset(dir);

        // Reset Cmd Gui
        mLrMoves.Reset(dir);
    }

    public void SelectMove()
    {
        if (!mSel)
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
            mLrMoves.Retract();
            mCursorGhost.Show(false);
            return;
        }

        // Draw  Path
        Vector3 pnt = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pnt.z = transform.position.z; // Set Z
        Ray2D dir = mPaths.FinalDir;
        Path curPath = null;

        // If in Back Arc, nothing
        mCursorGhost.Show(true);
        mCursorGhost.Bad();
        Trig.Quarter qrt = Trig.GetQuarter(dir, pnt, 0, 0);
        if (qrt == Trig.Quarter.back)
        {
        }
        // If left, bend to edge
        else if (qrt == Trig.Quarter.left)
        {
        }
        // If right, bend to edge
        else if (qrt == Trig.Quarter.right)
        {
        }
        // Check if Within Line tolerance
        else if(Trig.DistToLine(dir, pnt) < .25f)
        {
            curPath = new LinePath(dir, Trig.NearestPointOnLine(dir, pnt));
        }
        // Else Arc
        else
        {
            curPath = new ArcPath(dir, pnt);
        }

        if(curPath != null)
        {
            // Render
            mLrMoves.Stretch(curPath.RenderPoints());

            // Place Ghost
            mCursorGhost.Good();
            float ghRot = Vector2.SignedAngle(Vector2.up, curPath.EndDir);
            mCursorGhost.SetPos(curPath.End, Quaternion.AngleAxis(ghRot, Vector3.forward));
        }
        else
        {
            mLrMoves.Retract();
            mCursorGhost.SetPos(pnt, Quaternion.identity);
        }

        // Check total time
        if (curPath != null && mPaths.TimeSpent + curPath.Time > GameManager.TIME_PER_TURN)
        {
            curPath = null;
            mCursorGhost.Bad();
            mLrMoves.Retract();
            mCursorGhost.SetPos(pnt, Quaternion.identity);
        }

        // If left Click and Path, Add Movement Segment
        if (curPath != null && Input.GetMouseButtonDown(0))
        {
            mPaths.Add(curPath);
            mLrMoves.LockIn(mPaths.FinalDir);
        }
    }

    #endregion

    #region ISelectable
    //////////////////////// ISelectable ///////////////////////////////

    public bool Select()
    {
        mSel = true;
        mLrMoves.EnableGuides(true);
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
        mLrMoves.EnableGuides(false);
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
            SelectMove();
            ResetPath();
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
