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
    private GuiGhost myGhost;
    private LineRenderer mLrLGuide;
    private LineRenderer mLrRGuide;
    private LineRenderer mLrCGuide;

    // Dynamic Objects
    private List<LineRenderer> mLrMoves;
    private List<Path> mPaths;

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
        mLrLGuide = Draw.CreateLineRend(gameObject, "LeftGuide", Color.yellow);
        mLrRGuide = Draw.CreateLineRend(gameObject, "RightGuide", Color.yellow);
        mLrCGuide = Draw.CreateLineRend(gameObject, "CenterGuide", Color.green);
        myGhost = MakeGhost();
        myGhost.Show(false);

        // Dyanimic Init
        mLrMoves = new List<LineRenderer>();
        mPaths = new List<Path>();

        // Startup Functions
        EnableGuides(false);
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

    private GuiGhost MakeGhost()
    {
        GameObject g = new GameObject();
        g.name = "Ghost";
        g.transform.parent = transform;
        g.transform.localPosition = Vector3.zero + Vector3.back;
        g.AddComponent<SpriteRenderer>();
        g.AddComponent<BoxCollider2D>();
        g.AddComponent<GuiGhost>();

        // Box Collider Init
        SpriteRenderer sr = g.GetComponent<SpriteRenderer>();
        SpriteRenderer mSr = this.GetComponent<SpriteRenderer>();
        sr.sprite = mSr.sprite;

        // Box Collider Init
        BoxCollider2D bc = g.GetComponent<BoxCollider2D>();
        BoxCollider2D mBc = this.GetComponent<BoxCollider2D>();
        bc.size = mBc.size;

        return g.GetComponent<GuiGhost>();
    }

    #endregion

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
        myGhost.Bad();
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
            Draw.DrawLineRend(mLrMoves[mLrMoves.Count - 1], curPath.RenderPoints());

            // Place Ghost
            myGhost.Good();
            float ghRot = Vector2.SignedAngle(Vector2.up, curPath.EndDir);
            myGhost.SetPos(curPath.End, Quaternion.AngleAxis(ghRot, Vector3.forward));
        }
        else
        {
            mLrMoves[mLrMoves.Count - 1].positionCount = 0;
            myGhost.SetPos(pnt, Quaternion.identity);
        }

        // If left Click and Path, Add Movement Segment
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
