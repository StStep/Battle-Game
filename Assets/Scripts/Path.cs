using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Path
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

    public Path(Ray2D dir, Vector2 pnt)
    {
        // Set Minimum input
        Start = dir.origin;
        StartDir = dir.direction;
        End = pnt;

        // First Calculate
        Recalculate(pnt);
    }

    abstract public Vector3[] RenderPoints();

    abstract public Vector2 GetPoint(float dist);

    abstract protected void Translate(Ray2D dir);

    abstract protected void Recalculate(Vector2 pnt);
}

public class LinePath : Path
{
    float mLength;

    override public Vector2 EndDir
    {
        get { return StartDir; }
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
        if (Vector2.Distance(End, tmpEnd) > float.Epsilon)
            throw new Exception("Invalid line");
    }
}

public class ArcPath : Path
{
    protected Trig.Arc mArc;
    protected float mRawAngle;
    protected float mArcLength;
    protected float mRadius;

    override public Vector2 EndDir
    {
        get { return mArc.FinalDir; }
        protected set { }
    }

    public ArcPath(Ray2D dir, Vector2 pnt) : base(dir, pnt)
    { }

    public override Vector3[] RenderPoints()
    {
        const int segments = 20;
        Vector3[] pnts = new Vector3[segments + 1];

        bool clockwise = (mRawAngle < 0);
        float angle = (clockwise) ? -90 : 90;
        float arcLength = Mathf.Abs(mRawAngle);
        for (int i = 0; i <= segments; i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * mRadius;
            float y = Mathf.Cos(Mathf.Deg2Rad * angle) * mRadius;

            pnts[i] = new Vector3(x + mArc.Center.x, y + mArc.Center.y, 0f);

            angle += (clockwise) ? (arcLength / segments) : -(arcLength / segments);
        }
        pnts[segments] = End;

        return pnts;
    }

    public override Vector2 GetPoint(float dist)
    {
        if (dist > mArcLength)
            throw new Exception("Length outside bounds");

        float angle = (dist / mArcLength) * Mathf.Abs(mRawAngle);

        float x = Mathf.Sin(Mathf.Deg2Rad * angle) * mRadius;
        float y = Mathf.Cos(Mathf.Deg2Rad * angle) * mRadius;

        return new Vector2(x, y);
    }

    protected override void Translate(Ray2D dir)
    {
        throw new NotImplementedException();
    }

    protected override void Recalculate(Vector2 pnt)
    {
        mArc = Trig.GetArc(new Ray2D(Start, StartDir), pnt);
        End = pnt;

        mRawAngle = Vector2.SignedAngle(mArc.Start - mArc.Center, mArc.End - mArc.Center);
        mRadius = Mathf.Abs(Vector2.Distance(mArc.Start, mArc.Center));
        mArcLength = 2 * Mathf.PI * mRadius * (Mathf.Abs(mRawAngle) / 360f);
    }
}
