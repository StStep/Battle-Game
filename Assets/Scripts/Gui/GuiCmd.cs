using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuiCmd : IClickRef
{
    // Static Objects
    private GameObject mParent;
    private GuiGhost mMoveGhost;
    private LineRenderer mLrLGuide;
    private LineRenderer mLrRGuide;
    private LineRenderer mLrCGuide;

    // Dynamic Objects
    private List<LineRenderer> mLrMoves;

    public GuiCmd(GameObject obj)
    {
        mLrMoves = new List<LineRenderer>();
        mParent = obj;

        mLrLGuide = Draw.CreateLineRend(obj, "LeftGuide", Color.yellow);
        mLrRGuide = Draw.CreateLineRend(obj, "RightGuide", Color.yellow);
        mLrCGuide = Draw.CreateLineRend(obj, "CenterGuide", Color.green);

        mMoveGhost = Draw.MakeGhost(obj);
        mMoveGhost.Show(false);
    }

    public void Reset(Ray2D dir)
    {
        foreach (LineRenderer lr in mLrMoves)
        {
            UnityEngine.Object.Destroy(lr);
        }
        mLrMoves.Clear();

        LockIn(dir);

        mMoveGhost.Neutral();
        mMoveGhost.Show(false);
    }

    public void LockIn(Ray2D dir)
    {
        LineRenderer curMove = Draw.CreateLineRend(mParent, "MovementLine", Color.red);
        mLrMoves.Add(curMove);

        MoveGuides(dir);
        mMoveGhost.Show(true);
    }

    public void Fin()
    {
        mMoveGhost.Final();
    }

    public void Retract()
    {
        mLrMoves[mLrMoves.Count - 1].positionCount = 0;
    }

    public void Stretch(Vector3[] pnts)
    {
        Draw.DrawLineRend(mLrMoves[mLrMoves.Count - 1], pnts);
    }

    public void EnableGuides(bool en)
    {
        mLrLGuide.enabled = en;
        mLrRGuide.enabled = en;
        mLrCGuide.enabled = en;
    }

    private void MoveGuides(Ray2D dir)
    {
        Vector3[] line = new Vector3[2];
        line[0] = dir.origin;

        line[1] = 100 * dir.direction;
        line[1] += line[0];
        Draw.DrawLineRend(mLrCGuide, line);

        Vector3 rightGuideDir = Quaternion.AngleAxis(-45f, Vector3.forward) * dir.direction;
        line[1] = 100 * rightGuideDir + line[0];
        Draw.DrawLineRend(mLrRGuide, line);

        Vector3 leftGuideDir = Quaternion.AngleAxis(45f, Vector3.forward) * dir.direction;
        line[1] = 100 * leftGuideDir + line[0];
        Draw.DrawLineRend(mLrLGuide, line);

        float ghRot = Vector2.SignedAngle(Vector2.up, dir.direction);
        mMoveGhost.SetPos(dir.origin, Quaternion.AngleAxis(ghRot, Vector3.forward));
    }

    #region IClickRef
    //////////////////////// IClickRef ///////////////////////////////

    public void SetLeft(Click del)
    {
        mMoveGhost.SetLeft(del);
    }

    public void SetRight(Click del)
    { }

    /////////////////////////////////////////////////////////////////
    #endregion
}
