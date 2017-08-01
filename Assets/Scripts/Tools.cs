using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO Make renderer composite
// Make workable without render

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

        // TODO Check validity?
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
        if(dist > mArcLength)
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

/// <summary>
/// </summary>
public static class Trig
{

    public struct Line
    {
        public Vector2 Start, End;
        public Line(Vector2 strt, Vector2 end)
        {
            Start = strt;
            End = end;
        }

    }

    public struct Arc
    {
        public Vector2 Start, End, Center;
        public Vector2 FinalDir;
        public Arc(Vector2 strt, Vector2 end, Vector2 cntr, Vector2 dir)
        {
            Start = strt;
            End = end;
            Center = cntr;
            FinalDir = dir;
        }

    }

    public static float DistToLine(Ray2D dir, Vector2 pnt)
    {
        Vector2 linepnt = NearestPointOnLine(dir, pnt);
        return Vector2.Distance(linepnt, pnt);
    }

    public static Vector2 NearestPointOnLine(Ray2D dir, Vector2 pnt)
    {
        Vector2 v = pnt - dir.origin;
        float d = Vector2.Dot(v, dir.direction);
        return dir.GetPoint(d);
    }

    // TODO use frontage
    public static bool WithinFrontArc(Ray2D dir, Vector2 pnt, float frontage)
    {
        bool ret = true;

        Vector2 v = pnt - dir.origin;
        float ang = Vector2.Angle(v, dir.direction);
        if (ang > 45F)
            ret = false;

        return ret;
    }

    public static Vector2 LineIntersectionPoint(Ray2D l1, Ray2D l2)
    {
        return LineIntersectionPoint(l1.origin, l1.origin + l1.direction, l2.origin, l2.origin + l2.direction);
    }

    public static Vector2 LineIntersectionPoint(Vector2 ps1, Vector2 pe1, Vector2 ps2, Vector2 pe2)
    {
        // Get A,B,C of first line - points : ps1 to pe1
        float A1 = pe1.y - ps1.y;
        float B1 = ps1.x - pe1.x;
        float C1 = A1 * ps1.x + B1 * ps1.y;

        // Get A,B,C of second line - points : ps2 to pe2
        float A2 = pe2.y - ps2.y;
        float B2 = ps2.x - pe2.x;
        float C2 = A2 * ps2.x + B2 * ps2.y;

        // Get delta and check if the lines are parallel
        float delta = A1 * B2 - A2 * B1;
        if (delta == 0)
            throw new System.Exception("Lines are parallel");

        // now return the Vector2 intersection point
        return new Vector2(
            (B2 * C1 - B1 * C2) / delta,
            (A1 * C2 - A2 * C1) / delta
        );
    }


    /// <summary>
    /// Returns an arc, if valid, starting at the given line direction
    /// and ending at the given point.
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="pnt"></param>
    /// <returns></returns>
    public static Arc GetArc(Ray2D dir, Vector2 pnt)
    {
        // Invalid if pnt is in back half of dir
        if(!WithinFrontArc(dir, pnt, 0))
            throw new System.Exception("Outside bounds");

        // LegA of isolese triangle is perp to dir
        Vector2 triBaseOut = pnt - dir.origin; // base direction
        triBaseOut.Normalize();
        Vector2 triBaseIn = new Vector2(-triBaseOut.x, -triBaseOut.y);

        Ray2D legA = dir;
        bool clockwise = (Vector2.SignedAngle(dir.direction, triBaseOut) > 0);
        if (clockwise)
        {
            legA.direction = new Vector2(-legA.direction.y, legA.direction.x);
        }
        else
        {
            legA.direction = new Vector2(legA.direction.y, -legA.direction.x);
        }

        float legAng = Vector2.Angle(legA.direction, triBaseOut);

        Vector2 newDir;
        Vector2 finalDir; // Perp to lebB is final facing direction
        if (clockwise)
        {
            newDir = Quaternion.AngleAxis(-legAng, Vector3.forward) * triBaseIn;
            finalDir = Quaternion.AngleAxis(-90, Vector3.forward) * newDir;
        }
        else
        {
            newDir = Quaternion.AngleAxis(legAng, Vector3.forward) * triBaseIn;
            finalDir = Quaternion.AngleAxis(90, Vector3.forward) * newDir;
        }
        Ray2D legB = new Ray2D(pnt, newDir);

        Vector2 center = LineIntersectionPoint(legA, legB);
        Arc arc = new Arc(dir.origin, pnt, center, finalDir);

        return arc;
    }

}

public static class Draw
{
    public static LineRenderer CreateLineRend(GameObject parent, String name, Color color)
    {
        GameObject myLine = new GameObject();
        myLine.name = name;
        myLine.transform.parent = parent.transform;
        myLine.transform.position = parent.transform.position;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;
        lr.positionCount = 0;
        return lr;
    }

    public static void DrawLine(LineRenderer lr, Trig.Line line)
    {
        DrawLine(lr, line.Start, line.End);
    }

    public static void DrawLine(LineRenderer lr, Vector2 strt, Vector2 end)
    {
        lr.positionCount = 2;
        lr.SetPosition(0, new Vector3(strt.x, strt.y));
        lr.SetPosition(1, new Vector3(end.x, end.y));
    }

    public static void DrawArc(LineRenderer lr, Trig.Arc arc, int seg)
    {
        int segments = 20;
        lr.positionCount = segments + 1;
        float rawAngle = Vector2.SignedAngle(arc.Start - arc.Center, arc.End - arc.Center);
        bool clockwise = (rawAngle < 0);
        float angle = (clockwise) ? -90 : 90;
        float arcLength = Mathf.Abs(rawAngle);
        float radius = Mathf.Abs(Vector2.Distance(arc.Start, arc.Center));
        for (int i = 0; i <= segments; i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            float y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

            lr.SetPosition(i, new Vector3(x + arc.Center.x, y + arc.Center.y));

            angle += (clockwise) ? (arcLength / segments) : -(arcLength / segments);
        }
        lr.SetPosition(segments, arc.End);
    }
}