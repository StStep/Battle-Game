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

    public Path(float time, Ray2D dir, Vector2 pnt)
    {
        // Set Minimum input
        Start = dir.origin;
        StartDir = dir.direction;

        // First Calculate
        Recalculate(time, pnt);
    }

    public virtual Vector3[] DebugRenderPoints()
    {
        return RenderPoints();
    }

    abstract public Vector3[] RenderPoints();

    abstract public Vector2 GetPointDist(float dist);

    public Vector2 GetPointTime(float time)
    {
        float maxDist = time * GameManager.DEFAULT_SPEED;
        return GetPointDist(maxDist);
    }

    abstract protected void Translate(Ray2D dir);

    abstract protected void Recalculate(float time, Vector2 pnt);

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

    public PointPath(Ray2D dir) : base(0, dir, Vector2.zero)
    { }

    public override Vector3[] RenderPoints()
    {
        Vector3[] pnts = new Vector3[1];
        pnts[0] = new Vector3(Start.x, Start.y, 0f);
        return pnts;
    }

    public override Vector2 GetPointDist(float dist)
    {
        return Start;
    }

    protected override void Translate(Ray2D dir)
    {
        Start = dir.origin;
        StartDir = dir.direction;
        End = Start;
    }

    protected override void Recalculate(float time, Vector2 pnt)
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

    public LinePath(float time, Ray2D dir, Vector2 pnt) : base(time, dir, pnt)
    { }

    public override Vector3[] RenderPoints()
    {
        Vector3[] pnts = new Vector3[2];
        pnts[0] = new Vector3(Start.x, Start.y, 0f);
        pnts[1] = new Vector3(End.x, End.y, 0f);
        return pnts;
    }

    public override Vector2 GetPointDist(float dist)
    {
        if (dist >= mLength)
            return End;
        return Start + StartDir * dist;
    }

    protected override void Translate(Ray2D dir)
    {
        Start = dir.origin;
        StartDir = dir.direction;
        End = Start + StartDir * mLength;
    }

    protected override void Recalculate(float time, Vector2 pnt)
    {
        End = Trig.NearestPointOnLine(new Ray2D(Start, StartDir), pnt);
        mLength = Vector2.Distance(Start, End);

        // Recalc if too long
        if (mLength / GameManager.DEFAULT_SPEED > time)
        {
            End = GetPointTime(time);
            mLength = Vector2.Distance(Start, End);
        }
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

    public ArcPath(float time, Ray2D dir, Vector2 pnt) : base(time, dir, pnt)
    { }

    public override Vector3[] RenderPoints()
    {
        const int segments = 20;
        Vector3[] pnts = new Vector3[segments + 1];

        float cDist = 0;
        float diff = mArcLength / segments;
        for (int i = 0; i < segments; i++)
        {
            pnts[i] = GetPointDist(cDist);
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

    public override Vector2 GetPointDist(float dist)
    {
        if (dist >= mArcLength)
            return End;

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

    protected override void Recalculate(float time, Vector2 pnt)
    {
        Ray2D dir = new Ray2D(Start, StartDir);
        Trig.Quarter q = Trig.GetQuarter(dir, pnt, 0, 0);
        if (q == Trig.Quarter.back)
        {
            throw new Exception("Invalid pnt for arc creation");
        }
        else if (q == Trig.Quarter.left)
        {
            Ray2D guide = new Ray2D(dir.origin, Quaternion.AngleAxis(45f, Vector3.forward) * dir.direction);
            Ray2D toPnt = new Ray2D(pnt, Quaternion.AngleAxis(-90f, Vector3.forward) * dir.direction);
            End = Trig.LineIntersectionPoint(guide, toPnt);
        }
        else if (q == Trig.Quarter.right)
        {
            Ray2D guide = new Ray2D(dir.origin, Quaternion.AngleAxis(-45f, Vector3.forward) * dir.direction);
            Ray2D toPnt = new Ray2D(pnt, Quaternion.AngleAxis(90f, Vector3.forward) * dir.direction);
            End = Trig.LineIntersectionPoint(guide, toPnt);
        }
        // Front
        else
        {
            End = pnt;
        }

        Trig.Arc arc = Trig.GetArc(dir, End);
        mCenter = arc.Center;
        mRadius = Mathf.Abs(Vector2.Distance(Start, mCenter));
        mFinalDir = arc.FinalDir;

        mRawAngle = Vector2.SignedAngle(Start - mCenter, End - mCenter);
        mArcLength = 2 * Mathf.PI * mRadius * (Mathf.Abs(mRawAngle) / 360f);

        // Recalc if too long
        if (mArcLength / GameManager.DEFAULT_SPEED > time)
        {
            End = GetPointTime(time);
            mRawAngle = Vector2.SignedAngle(Start - mCenter, End - mCenter);
            mArcLength = 2 * Mathf.PI * mRadius * (Mathf.Abs(mRawAngle) / 360f);
            Vector2 leg = End - mCenter;
            float ang = (mRawAngle < 0) ? -90 : 90;
            mFinalDir = Quaternion.AngleAxis(ang, Vector3.forward) * leg.normalized;
        }
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
