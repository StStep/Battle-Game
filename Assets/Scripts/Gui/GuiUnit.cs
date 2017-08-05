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
    private List<LineRenderer> mLrMoves;
    private LineRenderer mLrLGuide;
    private LineRenderer mLrRGuide;
    private LineRenderer mLrCGuide;

    // Current Movement Info
    private List<Path> mPaths;

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
        mPaths = new List<Path>();

        mLrLGuide = Draw.CreateLineRend(gameObject, "LeftGuide", Color.yellow);
        mLrRGuide = Draw.CreateLineRend(gameObject, "RightGuide", Color.yellow);
        mLrCGuide = Draw.CreateLineRend(gameObject, "CenterGuide", Color.green);

        //  Set Startup info
        EnableGuides(false);
        ResetPath();
        Deselect();
    }

    private void ResetPath()
    {
        MoveGuides(this.transform.position, this.transform.up);

        // Make Paths
        mPaths.Clear();
        PointPath strPath = new PointPath(new Ray2D(this.transform.position, this.transform.up), this.transform.position);
        mPaths.Add(strPath);

        // Clear Existing objects
        foreach(LineRenderer lr in mLrMoves)
        {
            Destroy(lr);
        }
        mLrMoves.Clear();

        // Make Lines
        LineRenderer curMove = Draw.CreateLineRend(gameObject, "MovementLine", Color.red);
        mLrMoves.Add(curMove);
    }

    private void MoveGuides(Vector2 origin, Vector2 direction)
    {
        Vector3[] line = new Vector3[2];
        line[0] = origin;

        line[1] = 100 * direction;
        line[1] += line[0];
        Draw.DrawLineRend(mLrCGuide, line);

        Vector3 rightGuideDir = Quaternion.AngleAxis(-45f, Vector3.forward) * direction;
        line[1] = 100 * rightGuideDir + line[0];
        Draw.DrawLineRend(mLrRGuide, line);

        Vector3 leftGuideDir = Quaternion.AngleAxis(45f, Vector3.forward) * direction;
        line[1] = 100 * leftGuideDir + line[0];
        Draw.DrawLineRend(mLrLGuide, line);
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
        Ray2D dir = new Ray2D(mPaths[mPaths.Count - 1].End, mPaths[mPaths.Count - 1].EndDir);
        Path curPath = null;

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
            curPath = new LinePath(dir, Trig.NearestPointOnLine(dir, pnt));
            Draw.DrawLineRend(mLrMoves[mLrMoves.Count - 1], curPath.RenderPoints());

            // Place Ghost
            float ghRot = Vector2.SignedAngle(Vector2.up, curPath.EndDir);
            myGhost.SetPos(curPath.End, Quaternion.AngleAxis(ghRot, Vector3.forward));
        }
        // Else Arc
        else
        {
            curPath = new ArcPath(dir, pnt);
            Draw.DrawLineRend(mLrMoves[mLrMoves.Count - 1], curPath.RenderPoints());

            // Place Ghost
            float ghRot = Vector2.SignedAngle(Vector2.up, curPath.EndDir);
            myGhost.SetPos(pnt, Quaternion.AngleAxis(ghRot, Vector3.forward));
        }

        // If left Click Add Movement Segment
        if (curPath != null && Input.GetMouseButtonDown(0))
        {
            mPaths.Add(curPath);
            MoveGuides(curPath.End, curPath.EndDir);
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
            ResetPath();
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
