using System;
using UnityEngine;

// A command segemnet, as a difference from some not-given init point
public interface ICommandSegment
{
    float TimeDiff { get; }

    Vector2 PosDiff { get; }

    float DirDiff { get; }

    Ray2D Translate(Ray2D init);

    void Remove();
}

public class ZeroCmdSeg : ICommandSegment
{
    public float TimeDiff
    {
        get { return 0;  }
        protected set { }
    }

    public Vector2 PosDiff
    {
        get { return Vector2.zero; }
        protected set { }
    }

    public float DirDiff
    {
        get { return 0; }
        protected set { }
    }

    public Ray2D Translate(Ray2D init)
    {
        return init;
    }

    public void Remove() { }
}