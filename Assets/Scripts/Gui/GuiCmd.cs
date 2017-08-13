using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuiCmd
{
    // Static Objects
    private GameObject mParent;
    private ClickObject mStartGhost;
    private ClickObject mEndGhost;
    private LineRenderer mLrLGuide;
    private LineRenderer mLrRGuide;
    private LineRenderer mLrCGuide;
    private LineRenderer mGapLine;
    protected ICommandRef cmdRef;

    // Dynamic Objects
    private List<LineRenderer> mLrMoves;

    public GuiCmd(GameObject par, Selector sel)
    {
        mLrMoves = new List<LineRenderer>();
        mParent = par;
        cmdRef = par.GetComponent<GuiUnit>();

        mLrLGuide = Draw.CreateLineRend(par, "LeftGuide", Color.yellow);
        mLrRGuide = Draw.CreateLineRend(par, "RightGuide", Color.yellow);
        mLrCGuide = Draw.CreateLineRend(par, "CenterGuide", Color.green);
        mGapLine = Draw.CreateLineRend(par, "GapLine", Color.blue);

        mEndGhost = Draw.MakeCmdSegTip(par, sel);
        mEndGhost.Renderer.NeutralRender();
        mEndGhost.LeftClick = () => ClickStart(false);

        mStartGhost = Draw.MakeCmdSegTip(par, sel);
        mStartGhost.Renderer.SelectedRender(false);
        mStartGhost.LeftClick = () => ClickStart(true);
    }

    public void ClickStart(bool reset)
    {
        if (!mStartGhost.ChainSelect())
            return;

        if (reset)
            cmdRef.ResetPath();

        cmdRef.SetState(State.Moving);
    }

    public void Reset(Ray2D dir)
    {
        foreach (LineRenderer lr in mLrMoves)
        {
            UnityEngine.Object.Destroy(lr);
        }
        mLrMoves.Clear();

        LockIn(dir);

        mEndGhost.Show(false);
        mEndGhost.Renderer.NeutralRender();
    }

    public void LockIn(Ray2D dir)
    {
        LineRenderer curMove = Draw.CreateLineRend(mParent, "MovementLine", Color.red);
        mLrMoves.Add(curMove);

        MoveGuides(dir);
        mEndGhost.Show(true);
    }

    public void Fin()
    {
        mEndGhost.Renderer.LockRender();
    }

    public void Retract()
    {
        mLrMoves[mLrMoves.Count - 1].positionCount = 0;
        mGapLine.positionCount = 0;
    }

    public void Stretch(Vector3[] pnts, Vector3 mouse)
    {
        Draw.DrawLineRend(mLrMoves[mLrMoves.Count - 1], pnts);

        Vector3[] gap = new Vector3[2];
        gap[0] = pnts[pnts.Length - 1];
        gap[1] = mouse;
        Draw.DrawLineRend(mGapLine, gap);

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
        mEndGhost.SetPos(dir.origin, Quaternion.AngleAxis(ghRot, Vector3.forward));
    }
}
