using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class GuiUnit : MonoBehaviour, ICommandRef
{
    // Status Member
    private State mState;
    private SelectComponent mSelector;

    // Static Objects
    private GameObject mCursorGhost;
    private SimUnit mSim;
    private SimCmd mSimCmd;
    private GuiCmd mGuiCmd;

    // Use this for initialization
    public void Start()
    {
        mSelector = gameObject.AddComponent<SelectComponent>();
        mSelector.Init(GameManager.instance.mSelector);
        mSelector.OnSelect = () =>
        {
            mGuiCmd.EnableGuides(true);
            return true;
        };
        mSelector.OnDeselect = () =>
        {
            if (mState != State.None)
                return false;

            mGuiCmd.EnableGuides(false);
            return true;
        };

        // Status Members
        mState = State.None;

        // Static Init TODO Make only render
        mCursorGhost = Draw.MakeGhost(gameObject);
        mCursorGhost.GetComponent<RenderComponent>().Show(false);
        mSim = new SimUnit();
        mGuiCmd = new GuiCmd(gameObject);
        mSimCmd = new SimCmd();
        mGuiCmd.EnableGuides(false);

        // Startup Functions
        ResetPath();
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

    public void ResetPath()
    {
        Ray2D dir = new Ray2D(this.transform.position, this.transform.up);
        // Reset Cmd Sim
        mSimCmd.Reset(dir);

        // Reset Cmd Gui
        mGuiCmd.Reset(dir);
    }

    public bool SetState(State st)
    {
        if (mState != State.None && st != State.None)
            return false;

        bool ret = false;
        switch (st)
        {
            case State.Moving:
                if (mSimCmd.TimeLeft > float.Epsilon)
                {
                    mState = State.Moving;
                    ret = true;
                }
                break;
            default:
                mState = State.None;
                ret = true;
                break;
        }

        return ret;
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
            mCursorGhost.GetComponent<RenderComponent>().Show(false);
            return;
        }

        // Draw  Path
        Vector3 pnt = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pnt.z = transform.position.z; // Set Z
        Ray2D dir = mSimCmd.FinalDir;
        float timeLeft = mSimCmd.TimeLeft;

        mCursorGhost.GetComponent<RenderComponent>().Show(true);
        mCursorGhost.GetComponent<RenderComponent>().Renderer.BadRender();
        Path curPath = null;

        // Min Move Distance or Back Half
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
            mCursorGhost.GetComponent<RenderComponent>().Renderer.GoodRender();
            float ghRot = Vector2.SignedAngle(Vector2.up, curPath.EndDir);
            mCursorGhost.GetComponent<RenderComponent>().SetPos(curPath.End, Quaternion.AngleAxis(ghRot, Vector3.forward));
        }
        else
        {
            mGuiCmd.Retract();
            mCursorGhost.GetComponent<RenderComponent>().SetPos(pnt, Quaternion.identity);
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
                mCursorGhost.GetComponent<RenderComponent>().Show(false);
            }
        }
    }

    #endregion
}
