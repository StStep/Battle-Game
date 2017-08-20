using System.Collections.Generic;
using UnityEngine;

public class GuiCmd
{
    // Static Objects
    private GameObject mParent;
    private GameObject mStartGhost;
    private GameObject mEndGhost;
    private LineRenderer mLGuide;
    private LineRenderer mRGuide;
    private LineRenderer mCGuide;
    private LineRenderer mGapLine;
    protected ICommandRef cmdRef;

    // Dynamic Objects
    private List<PathComponent> mMoves;

    public GuiCmd(GameObject par)
    {
        mMoves = new List<PathComponent>();
        mParent = par;
        cmdRef = par.GetComponent<GuiUnit>();

        mLGuide = Draw.CreateLineRend(par, "LeftGuide", Color.yellow);
        mRGuide = Draw.CreateLineRend(par, "RightGuide", Color.yellow);
        mCGuide = Draw.CreateLineRend(par, "CenterGuide", Color.green);
        mGapLine = Draw.CreateLineRend(par, "GapLine", Color.blue);

        mEndGhost = Draw.MakeGhost(par);
        mEndGhost.GetComponent<SpriteRenderer>().color = Color.white;
        mEndGhost.GetComponent<ClickComponent>().OnLeftClick = () => ClickStart(false);

        mStartGhost = Draw.MakeGhost(par);
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

        // TODO StopGap
        mEndGhost.GetComponent<SelectComponent>().Init(mStartGhost.GetComponent<SelectComponent>());
    }

    public void ClickStart(bool reset)
    {
        if (!mStartGhost.GetComponent<SelectComponent>().ChainSelect())
            return;

        if (reset)
            cmdRef.ResetPath();

        cmdRef.SetMoving();
    }

    public void Reset(Ray2D dir)
    {
        foreach (PathComponent o in mMoves)
        {
            UnityEngine.Object.Destroy(o.gameObject);
        }
        mMoves.Clear();

        LockIn(dir);

        mEndGhost.SetActive(false);
        mEndGhost.GetComponent<SpriteRenderer>().color = Color.white;
    }

    public void LockIn(Ray2D dir)
    {
        PathComponent curMove = Draw.CreatePathObject(mParent);
        mMoves.Add(curMove);

        MoveGuides(dir);
        mEndGhost.SetActive(true);
    }

    public void Fin()
    {
        mEndGhost.GetComponent<SpriteRenderer>().color = Color.green;
    }

    public void Retract()
    {
        mMoves[mMoves.Count - 1].Zero();
        mGapLine.positionCount = 0;
    }

    public void Stretch(Vector3[] pnts, Vector3 mouse)
    {
        mMoves[mMoves.Count - 1].RenderPoints = pnts;

        Vector3[] gap = new Vector3[2];
        gap[0] = pnts[pnts.Length - 1];
        gap[1] = mouse;
        Draw.DrawLineRend(mGapLine, gap);

    }

    public void EnableGuides(bool en)
    {
        mLGuide.enabled = en;
        mRGuide.enabled = en;
        mCGuide.enabled = en;
    }

    private void MoveGuides(Ray2D dir)
    {
        Vector3[] line = new Vector3[2];
        line[0] = dir.origin;

        line[1] = 100 * dir.direction;
        line[1] += line[0];
        Draw.DrawLineRend(mCGuide, line);

        Vector3 rightGuideDir = Quaternion.AngleAxis(-45f, Vector3.forward) * dir.direction;
        line[1] = 100 * rightGuideDir + line[0];
        Draw.DrawLineRend(mRGuide, line);

        Vector3 leftGuideDir = Quaternion.AngleAxis(45f, Vector3.forward) * dir.direction;
        line[1] = 100 * leftGuideDir + line[0];
        Draw.DrawLineRend(mLGuide, line);

        float ghRot = Vector2.SignedAngle(Vector2.up, dir.direction);
        mEndGhost.transform.position = dir.origin;
        mEndGhost.transform.rotation = Quaternion.AngleAxis(ghRot, Vector3.forward);
    }
}
