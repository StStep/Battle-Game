using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Path : ICommandSegment
{
    private Vector2 _end;
    public Vector2 End
    {
        get { return _end; }

        protected set { _end = value; }
    }

    private Vector2 _start;
    public Vector2 Start
    {
        get { return _start; }

        protected set { _start = value; }
    }

    private Vector2 _stDir;
    public Vector2 StartDir
    {
        get { return _stDir; }

        protected set { _stDir = value; }
    }

    abstract public Vector2 EndDir
    {
        get;
        protected set;
    }

    abstract public float Length
    {
        get;
        protected set;
    }

    // TODO Assumes same speed across whole thing
    public float Time
    {
        get { return Length / GameManager.DEFAULT_SPEED; }

        protected set { }
    }

    public Path(Ray2D dir, Vector2 pnt)
    {
        // Set Minimum input
        Start = dir.origin;
        StartDir = dir.direction;
        End = pnt;

        // First Calculate
        Recalculate(pnt);
    }

    public virtual Vector3[] DebugRenderPoints()
    {
        return RenderPoints();
    }

    abstract public Vector3[] RenderPoints();

    abstract public Vector2 GetPoint(float dist);

    abstract protected void Translate(Ray2D dir);

    abstract protected void Recalculate(Vector2 pnt);

    public virtual void LocalToWorld(Transform refT)
    {
        StartDir = refT.TransformDirection(StartDir);
        Start = refT.TransformPoint(Start);
        End = refT.TransformPoint(End);
    }

    public virtual void WorldToLocal(Transform refT)
    {
        StartDir = refT.InverseTransformDirection(StartDir);
        Start = refT.InverseTransformPoint(Start);
        End = refT.InverseTransformPoint(End);
    }
}

public class PointPath : Path
{
    override public Vector2 EndDir
    {
        get { return StartDir; }
        protected set { }
    }

    override public float Length
    {
        get { return 0; }
        protected set { }
    }

    public PointPath(Ray2D dir, Vector2 pnt) : base(dir, pnt)
    { }

    public override Vector3[] RenderPoints()
    {
        Vector3[] pnts = new Vector3[1];
        pnts[0] = new Vector3(Start.x, Start.y, 0f);
        return pnts;
    }

    public override Vector2 GetPoint(float dist)
    {
        if (dist > float.Epsilon)
            throw new Exception("Length outside bounds");
        return Start;
    }

    protected override void Translate(Ray2D dir)
    {
        Start = dir.origin;
        StartDir = dir.direction;
        End = Start;
    }

    protected override void Recalculate(Vector2 pnt)
    {
        End = Start;
    }
}

public class LinePath : Path
{
    // Transform Agnostic Info
    float mLength;

    override public Vector2 EndDir
    {
        get { return StartDir; }
        protected set { }
    }

    override public float Length
    {
        get { return mLength; }
        protected set { }
    }

    public LinePath(Ray2D dir, Vector2 pnt) : base(dir, pnt)
    { }

    public override Vector3[] RenderPoints()
    {
        Vector3[] pnts = new Vector3[2];
        pnts[0] = new Vector3(Start.x, Start.y, 0f);
        pnts[1] = new Vector3(End.x, End.y, 0f);
        return pnts;
    }

    public override Vector2 GetPoint(float dist)
    {
        if (dist > Vector2.Distance(Start, End))
            throw new Exception("Length outside bounds");
        return Start + StartDir * dist;
    }

    protected override void Translate(Ray2D dir)
    {
        Start = dir.origin;
        StartDir = dir.direction;
        End = Start + StartDir * mLength;
    }

    protected override void Recalculate(Vector2 pnt)
    {
        End = pnt;
        mLength = Vector2.Distance(Start, End);

        Vector2 tmpEnd = Start + mLength * StartDir;
        if (tmpEnd != End)
            throw new Exception(String.Format("Invalid line Calc {0} vs Actual {1}", End, tmpEnd));
    }
}

public class ArcPath : Path
{
    // Transform Agnostic Info
    protected float mRawAngle;
    protected float mArcLength;
    protected float mRadius;

    // Transform dependent info
    protected Vector2 mCenter;
    protected Vector2 mFinalDir;

    override public Vector2 EndDir
    {
        get { return mFinalDir; }
        protected set { }
    }

    override public float Length
    {
        get { return mArcLength; }
        protected set { }
    }

    public ArcPath(Ray2D dir, Vector2 pnt) : base(dir, pnt)
    { }

    public override Vector3[] RenderPoints()
    {
        const int segments = 20;
        Vector3[] pnts = new Vector3[segments + 1];

        float cDist = 0;
        float diff = mArcLength / segments;
        for (int i = 0; i < segments; i++)
        {
            pnts[i] = GetPoint(cDist);
            cDist += diff;
        }
        pnts[segments] = End;

        return pnts;
    }

    public override Vector3[] DebugRenderPoints()
    {
        Vector3[] pnts = new Vector3[3];

        pnts[0] = Start;
        pnts[1] = mCenter;
        pnts[2] = End;

        return pnts;
    }

    public override Vector2 GetPoint(float dist)
    {
        if (dist > mArcLength)
            throw new Exception("Length outside bounds");

        float angle = (dist / mArcLength) * mRawAngle;

        float x = Mathf.Cos(Mathf.Deg2Rad * angle) * mRadius;
        float y = Mathf.Sin(Mathf.Deg2Rad * angle) * mRadius;
        Vector2 pnt = new Vector2(x, y);

        // Rotate appropriately
        Vector2 cord = End - Start;
        float bodyAng = Vector2.SignedAngle(Vector2.up, StartDir);
        float pntAng = Vector2.SignedAngle(StartDir, cord);
        if(pntAng > 0)
        {
            pnt = Quaternion.AngleAxis(bodyAng, Vector3.forward) * pnt;
        }
        else
        {
            pnt = Quaternion.AngleAxis(bodyAng - 180, Vector3.forward) * pnt;
        }

        // Add center offset
        pnt.x += mCenter.x;
        pnt.y += mCenter.y;

        return pnt;
    }

    protected override void Translate(Ray2D dir)
    {
        throw new NotImplementedException();
    }

    protected override void Recalculate(Vector2 pnt)
    {
        Trig.Arc arc = Trig.GetArc(new Ray2D(Start, StartDir), pnt);
        mCenter = arc.Center;
        mFinalDir = arc.FinalDir;
        End = pnt;

        mRawAngle = Vector2.SignedAngle(Start - mCenter, End - mCenter);
        mRadius = Mathf.Abs(Vector2.Distance(Start, mCenter));
        mArcLength = 2 * Mathf.PI * mRadius * (Mathf.Abs(mRawAngle) / 360f);
    }

    public override void LocalToWorld(Transform refT)
    {
        base.LocalToWorld(refT);
        mFinalDir = refT.TransformDirection(mFinalDir);
        mCenter = refT.TransformPoint(mCenter);
    }

    public override void WorldToLocal(Transform refT)
    {
        base.WorldToLocal(refT);
        mFinalDir = refT.InverseTransformDirection(mFinalDir);
        mCenter = refT.InverseTransformPoint(mCenter);
    }
}
