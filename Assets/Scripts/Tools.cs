﻿using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// DouglasPeuckerReduction Source:
/// https://www.codeproject.com/Articles/18936/A-C-Implementation-of-Douglas-Peucker-Line-Appro
/// </summary>
public static class Tools
{
    public struct Point
    {
        public Int32 X, Y;
        public Point(Int32 px, Int32 py)
        {
            X = px;
            Y = py;
        }

    }

    /// <summary>
    /// Uses the Douglas Peucker algorithm to reduce the number of points.
    /// </summary>
    /// <param name="Points">The points.</param>
    /// <param name="Tolerance">The tolerance.</param>
    /// <returns></returns>
    public static List<Point> DouglasPeuckerReduction
        (List<Point> Points, Double Tolerance)
    {
        if (Points == null || Points.Count < 3)
            return Points;

        Int32 firstPoint = 0;
        Int32 lastPoint = Points.Count - 1;
        List<Int32> pointIndexsToKeep = new List<Int32>();

        //Add the first and last index to the keepers
        pointIndexsToKeep.Add(firstPoint);
        pointIndexsToKeep.Add(lastPoint);

        //The first and the last point cannot be the same
        while (Points[firstPoint].Equals(Points[lastPoint]))
        {
            lastPoint--;
        }

        DouglasPeuckerReduction(Points, firstPoint, lastPoint,
        Tolerance, ref pointIndexsToKeep);

        List<Point> returnPoints = new List<Point>();
        pointIndexsToKeep.Sort();
        foreach (Int32 index in pointIndexsToKeep)
        {
            returnPoints.Add(Points[index]);
        }

        return returnPoints;
    }

    /// <summary>
    /// Douglases the peucker reduction.
    /// </summary>
    /// <param name="points">The points.</param>
    /// <param name="firstPoint">The first point.</param>
    /// <param name="lastPoint">The last point.</param>
    /// <param name="tolerance">The tolerance.</param>
    /// <param name="pointIndexsToKeep">The point index to keep.</param>
    private static void DouglasPeuckerReduction(List<Point>
        points, Int32 firstPoint, Int32 lastPoint, Double tolerance,
        ref List<Int32> pointIndexsToKeep)
    {
        Double maxDistance = 0;
        Int32 indexFarthest = 0;

        for (Int32 index = firstPoint; index < lastPoint; index++)
        {
            Double distance = PerpendicularDistance
                (points[firstPoint], points[lastPoint], points[index]);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                indexFarthest = index;
            }
        }

        if (maxDistance > tolerance && indexFarthest != 0)
        {
            //Add the largest point that exceeds the tolerance
            pointIndexsToKeep.Add(indexFarthest);

            DouglasPeuckerReduction(points, firstPoint,
            indexFarthest, tolerance, ref pointIndexsToKeep);
            DouglasPeuckerReduction(points, indexFarthest,
            lastPoint, tolerance, ref pointIndexsToKeep);
        }
    }

    /// <summary>
    /// The distance of a point from a line made from point1 and point2.
    /// </summary>
    /// <param name="pt1">The PT1.</param>
    /// <param name="pt2">The PT2.</param>
    /// <param name="p">The p.</param>
    /// <returns></returns>
    public static Double PerpendicularDistance
        (Point Point1, Point Point2, Point Point)
    {
        //Area = |(1/2)(x1y2 + x2y3 + x3y1 - x2y1 - x3y2 - x1y3)|   *Area of triangle
        //Base = v((x1-x2)²+(x1-x2)²)                               *Base of Triangle*
        //Area = .5*Base*H                                          *Solve for height
        //Height = Area/.5/Base

        Double area = Math.Abs(.5 * (Point1.X * Point2.Y + Point2.X *
        Point.Y + Point.X * Point1.Y - Point2.X * Point1.Y - Point.X *
        Point2.Y - Point1.X * Point.Y));
        Double bottom = Math.Sqrt(Math.Pow(Point1.X - Point2.X, 2) +
        Math.Pow(Point1.Y - Point2.Y, 2));
        Double height = area / bottom * 2;

        return height;
    }
}