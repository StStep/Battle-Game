using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public struct LinePos
    {
        public bool Valid;
        public Line Line;
    }

    public struct ArcPos
    {
        public bool Valid;
        public Arc Arc;
    }

    public static Vector2 NearestPointOnLine(Ray2D dir, Vector2 pnt)
    {
        Vector2 v = pnt - dir.origin;
        float d = Vector2.Dot(v, dir.direction);
        return dir.GetPoint(d);
    }


    /// <summary>
    /// Returns a line, if valid, starting at the given line direction
    /// and ending at the given point.
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="pnt"></param>
    /// <param name="tolerance"></param>
    /// <returns></returns>
    public static LinePos GetLine(Ray2D dir, Vector2 pnt, float tolerance)
    {
        LinePos ret = new LinePos();
        ret.Valid = false;

        // Invalid if pnt is in back half of dir
        Vector2 v = pnt - dir.origin;
        float ang = Vector2.Angle(v, dir.direction);
        if(ang > 90F)
        {
            return ret;
        }

        Vector2 linepnt = NearestPointOnLine(dir, pnt);
        float dist = Vector2.Distance(linepnt, pnt);

        if (dist <= tolerance)
        {
            ret.Valid = true;
            ret.Line = new Line(dir.origin, linepnt);
        }

        return ret;
    }

    public static Vector2 LineIntersectionPoint(Vector2 ps1, Vector2 pe1, Vector2 ps2,
       Vector2 pe2)
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
    public static ArcPos GetArc(Ray2D dir, Vector2 pnt)
    {
        ArcPos ret = new ArcPos();
        ret.Valid = false;

        // Invalid if pnt is in back half of dir
        Vector2 v = pnt - dir.origin;
        float ang = Vector2.Angle(v, dir.direction);
        if (ang > 90F)
        {
            return ret;
        }

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

        // Angles less than 45 degrees means point is outside circle quadrant
        if((legAng < 45f) || (legAng > 90f))
        {
            return ret;
        }

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

        Vector2 center = LineIntersectionPoint(legA.origin,  legA.origin + legA.direction, legB.origin, legB.origin + legB.direction);
        ret.Valid = true;
        ret.Arc = new Arc(dir.origin, pnt, center, finalDir);

        return ret;
    }

    public static void DrawLine(LineRenderer lr, Line line)
    {
        DrawLine(lr, line.Start, line.End);
    }

    public static void DrawLine(LineRenderer lr, Vector2 strt, Vector2 end)
    {
        lr.positionCount = 2;
        lr.SetPosition(0, new Vector3(strt.x, strt.y));
        lr.SetPosition(1, new Vector3(end.x, end.y));
    }

    public static void DrawArc(LineRenderer lr, Arc arc, int seg)
    {
        int segments = 20;
        lr.positionCount = segments + 1;
        float rawAngle = Vector2.SignedAngle(arc.Start - arc.Center, arc.End - arc.Center);
        Debug.Log(rawAngle);
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

public static class Create
{
    public static LineRenderer LineRender(GameObject parent, String name, Color color)
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
}