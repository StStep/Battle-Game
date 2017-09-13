using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// TODO
// Add movement adjust hooks
// Missing Time limit when clicking

// Primary Unit GUI, Controls State
[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class GuiUnit : MonoBehaviour
{
    // Types
    private enum State { None, Moving };

    // Status Member
    private State mState = State.None;
    private bool busy = false;

    // Static Objects
    private GameObject mCursorGhost;
    private PathComponent mLGuide;
    private PathComponent mRGuide;
    private PathComponent mCGuide;
    private PathComponent mGapLine;
    private PathComponent mMovePreview;
    private GameObject mStartGhost;
    private GameObject mEndGhost;
    private SimCmd mCmds;

    // Use this for initialization
    public void Start()
    {
        // Add Self Components
        SelectComponent s = gameObject.AddComponent<SelectComponent>();
        s.Init(GameManager.instance.mSelector);
        s.OnSelect = () =>
        {
            EnableGuides(true);
            return true;
        };
        s.OnDeselect = () =>
        {
            if (busy)
                return false;

            EnableGuides(false);
            return true;
        };

        // Make Cursor Ghost
        mCursorGhost = Draw.MakeGhost(gameObject);
        mCursorGhost.SetActive(false);

        // Make Guides
        mLGuide = Draw.CreatePath(gameObject, "LeftGuide", Color.yellow);
        mRGuide = Draw.CreatePath(gameObject, "RightGuide", Color.yellow);
        mCGuide = Draw.CreatePath(gameObject, "CenterGuide", Color.green);
        mGapLine = Draw.CreatePath(gameObject, "GapLine", Color.blue);
        mMovePreview = Draw.CreatePath(gameObject, "MovePreview", Color.red);
        EnableGuides(false);
        mCmds = new SimCmd();

        // Make End-of-Turn Ghost
        mEndGhost = Draw.MakeGhost(gameObject);
        mEndGhost.GetComponent<SpriteRenderer>().color = Color.white;
        mEndGhost.GetComponent<ClickComponent>().OnLeftClick = () => ClickStart(false);

        // Make Start-of-turn Ghost
        mStartGhost = Draw.MakeGhost(gameObject);
        mStartGhost.GetComponent<SpriteRenderer>().color = Color.blue;
        mStartGhost.GetComponent<ClickComponent>().OnLeftClick = () => ClickStart(true);
        mStartGhost.GetComponent<SelectComponent>().OnSelect = () =>
        {
            mStartGhost.GetComponent<SpriteRenderer>().color = Color.yellow;
            return true;
        };
        mStartGhost.GetComponent<SelectComponent>().OnDeselect = () =>
        {
            mStartGhost.GetComponent<SpriteRenderer>().color = Color.blue;
            return true;
        };

        // Hook Together Components TODO StopGap
        mEndGhost.GetComponent<SelectComponent>().Init(mStartGhost.GetComponent<SelectComponent>());
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

    public void ResetCmds()
    {
        mCmds.Reset();

        mEndGhost.SetActive(false);
        mEndGhost.GetComponent<SpriteRenderer>().color = Color.white;

        MoveGuides(new Ray2D(transform.position, transform.up));
    }

    public void ClickStart(bool reset)
    {
        if (!mStartGhost.GetComponent<SelectComponent>().ChainSelect())
            return;

        if (reset)
            ResetCmds();

        SetMoving();
    }

    public void EnableGuides(bool en)
    {
        mLGuide.Active = en;
        mRGuide.Active = en;
        mCGuide.Active = en;
    }

    public bool SetMoving()
    {
        if (mState != State.None)
            return false;

        bool ret = false;
        if (mCmds.TimeLeft > float.Epsilon)
        {
            mState = State.Moving;
            busy = true;
            ret = true;
        }

        return ret;
    }

    private void MoveGuides(Ray2D dir)
    {
        Vector3[] line = new Vector3[2];
        line[0] = dir.origin;

        line[1] = 100 * dir.direction;
        line[1] += line[0];
        mCGuide.RenderPoints = line;

        Vector3 rightGuideDir = Quaternion.AngleAxis(-45f, Vector3.forward) * dir.direction;
        line[1] = 100 * rightGuideDir + line[0];
        mRGuide.RenderPoints = line;

        Vector3 leftGuideDir = Quaternion.AngleAxis(45f, Vector3.forward) * dir.direction;
        line[1] = 100 * leftGuideDir + line[0];
        mLGuide.RenderPoints = line;

        float ghRot = Vector2.SignedAngle(Vector2.up, dir.direction);
        mEndGhost.transform.position = dir.origin;
        mEndGhost.transform.rotation = Quaternion.AngleAxis(ghRot, Vector3.forward);
    }

    public void Fin()
    {
        mEndGhost.GetComponent<SpriteRenderer>().color = Color.green;
    }

    public void Retract()
    {
        mMovePreview.Zero();
        mGapLine.Zero();
    }

    public void Stretch(Vector3[] pnts, Vector3 mouse)
    {
        mMovePreview.RenderPoints = pnts;

        Vector3[] gap = new Vector3[2];
        gap[0] = pnts[pnts.Length - 1];
        gap[1] = mouse;
        mGapLine.RenderPoints = gap;

    }

    private void NotBusy()
    {
        busy = false;
    }

    #endregion

    #region States

    private void StMove()
    {
        // If right Click exit state
        if (Input.GetMouseButtonDown(1))
        {
            mState = State.None;
            Retract();
            mCursorGhost.SetActive(false);
            Invoke("NotBusy", .2f);
            return;
        }

        // Draw  Path
        Vector3 pnt = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pnt.z = transform.position.z; // Set Z
        Ray2D dir = mCmds.FinalDir(new Ray2D(transform.position, transform.up));
        float timeLeft = mCmds.TimeLeft;

        mCursorGhost.SetActive(true);
        mCursorGhost.GetComponent<SpriteRenderer>().color = Color.red - new Color(0, 0, 0, .7f);
        Path curPath = null;

        // Min Move Distance or Back Half
        if (Trig.GetHalf(dir, pnt, 0, 0) == Trig.Half.back
            || Vector3.Distance(dir.origin, pnt) < GameManager.MOVE_MIN_PNT_DIST)
        { }
        // Line if within Line tolerance, or within min straight distance of start
        else if(Trig.DistToLine(dir, pnt) < GameManager.MOVE_LINE_TOL
            || Vector2.Distance(dir.origin, Trig.NearestPointOnLine(dir, pnt)) < GameManager.MOVE_MIN_ARC_DIST)
        {
            curPath = new LinePath(dir, pnt);
        }
        // Else Arc
        else
        {
            curPath = new ArcPath(dir, pnt);
        }

        if(curPath != null)
        {
            // Render
            Stretch(curPath.RenderPoints(), pnt);

            // Place Ghost
            mCursorGhost.GetComponent<SpriteRenderer>().color = Color.green - new Color(0, 0, 0, .7f);
            float ghRot = Vector2.SignedAngle(Vector2.up, curPath.EndDir);
            mCursorGhost.transform.position = curPath.End;
            mCursorGhost.transform.rotation = Quaternion.AngleAxis(ghRot, Vector3.forward);
        }
        else
        {
            Retract();
            mCursorGhost.transform.position = pnt;
            mCursorGhost.transform.rotation = Quaternion.identity;
        }

        // If left Click and Path, Add Movement Segment
        if (curPath != null && Input.GetMouseButtonDown(0))
        {
            mEndGhost.SetActive(true);
            mCmds.Add(Draw.MakeMoveCmd(gameObject, curPath));
            MoveGuides(mCmds.FinalDir(new Ray2D(transform.position, transform.up)));

            if (mCmds.TimeLeft < float.Epsilon)
            {
                Fin();
                mState = State.None;
                Invoke("NotBusy", .2f);
                Retract();
                mCursorGhost.SetActive(false);
            }
        }
    }

#endregion
}
