using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MoveCmd : MonoBehaviour, ICommandSegment
{
    private Vector2 _posDiff;
    public Vector2 PosDiff
    {
        get { return _posDiff; }

        protected set { _posDiff = value; }
    }

    abstract public float DirDiff
    {
        get;
        protected set;
    }

    abstract public float Length
    {
        get;
        protected set;
    }

    public Ray2D GetDir()
    {
        return new Ray2D(transform.position, transform.up);
    }

    abstract public MoveCmd Init(Vector2 pnt, float time);

    public void Remove()
    {
        Destroy(gameObject);
    }

    // TODO Assumes same speed across whole thing
    public float TimeDiff
    {
        get { return Length / GameManager.DEFAULT_SPEED; }

        protected set { }
    }

    abstract public Ray2D Translate(Ray2D init);

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

#if false
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
#endif
}

public class LineMoveCmd : MoveCmd
{
    // Transform Agnostic Info
    float mLength;

    // Render
    PathComponent mPath;

    override public float DirDiff
    {
        get { return 0; }
        protected set { }
    }

    override public float Length
    {
        get { return mLength; }
        protected set { }
    }

    private Vector3 End()
    {
        return transform.position + mLength * transform.up;
    }

    private Vector3 End(float dist)
    {
        return transform.position + dist * transform.up;
    }

    public void Start()
    {
        mPath = gameObject.AddComponent<PathComponent>();
        mPath.Init(Color.red);
    }

    public override MoveCmd Init(Vector2 pnt, float time)
    {
        Vector2 end = Trig.NearestPointOnLine(GetDir(), pnt);
        mLength = Vector2.Distance(transform.position, end);

        // Recalc if too long
        if (mLength / GameManager.DEFAULT_SPEED > time)
        {
            end = GetPointTime(time);
            mLength = Vector2.Distance(transform.position, end);
        }

        return this;
    }

    public override Vector3[] RenderPoints()
    {
        Vector3[] pnts = new Vector3[2];
        pnts[0] = new Vector3(transform.position.x, transform.position.y, 0f);
        pnts[1] = End();
        return pnts;
    }

    public override Vector2 GetPointDist(float dist)
    {
        float d = (dist >= mLength) ? mLength : dist;
        return End(d);
    }

    public override Ray2D Translate(Ray2D init)
    {
        transform.position = init.origin;
        float ang = Vector2.Angle(transform.up, init.direction);
        transform.Rotate(Vector3.forward, ang);
        return new Ray2D(End(), transform.up);
    }
}

#if false

public class ArcMoveCmd : MoveCmd
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

    public ArcMoveCmd(float time, Ray2D dir, Vector2 pnt) : base(time, dir, pnt)
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

#endif