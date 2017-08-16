using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// </summary>
public static class Trig
{

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

    public enum Half { front, back };
    public static Half GetHalf(Vector2 origin, Vector2 direction, Vector2 pnt, float frontage, float sideage)
    {
        return GetHalf(new Ray2D(origin, direction), pnt, frontage, sideage);
    }
    public static Half GetHalf(Ray2D dir, Vector2 pnt, float frontage, float sideage)
    {
        Half ret;

        Vector2 v = pnt - dir.origin;
        float ang = Vector2.SignedAngle(v, dir.direction);
        float absAng = Mathf.Abs(ang);
        if (absAng <= 90f)
            ret = Half.front;
        else
            ret = Half.back;

        return ret;
    }

    public enum Quarter {front, back, left, right};
    public static Quarter GetQuarter(Vector2 origin, Vector2 direction, Vector2 pnt, float frontage, float sideage)
    {
        return GetQuarter(new Ray2D(origin, direction), pnt, frontage, sideage);
    }
    public static Quarter GetQuarter(Ray2D dir, Vector2 pnt, float frontage, float sideage)
    {
        Quarter ret;

        Vector2 v = pnt - dir.origin;
        float ang = Vector2.SignedAngle(v, dir.direction);
        float absAng = Mathf.Abs(ang);
        if (absAng <= 45.5f)
            ret = Quarter.front;
        else if(absAng >= 135f)
            ret = Quarter.back;
        else if(ang < 0)
            ret = Quarter.left;
        else
            ret = Quarter.right;

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
        if(GetQuarter(dir, pnt, 0, 0) != Quarter.front)
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

    public static void DrawLineRend(LineRenderer lr, Vector3[] pnts)
    {
        lr.positionCount = pnts.Length;
        lr.SetPositions(pnts);
    }

    // TODO Factory??
    public static GameObject MakeGhost(GameObject par)
    {
        GameObject g = new GameObject();
        g.name = "Ghost";
        g.transform.parent = par.transform;
        g.transform.localPosition = Vector3.zero + Vector3.back;
        g.transform.localRotation = Quaternion.identity;

        // Get parent stuff
        Vector2 size = par.GetComponent<BoxCollider2D>().size;
        Sprite sprite = par.GetComponent<SpriteRenderer>().sprite;
        SelectComponent sel = par.GetComponent<SelectComponent>();

        // Box Collider Init
        g.AddComponent<BoxCollider2D>().size = size;

        // Custom Component Init
        g.AddComponent<ClickComponent>().Init();
        g.AddComponent<SpriteRenderer>().sprite = sprite;
        g.AddComponent<SelectComponent>().Init(sel);

        return g;
    }

}