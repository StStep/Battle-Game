using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimCmd {

    // Dynamic Objects
    List<ICommandSegment> mCmds;

    public float Time()
    {
        float t = 0;
        foreach(ICommandSegment s in mCmds)
            t+= s.TimeDiff;
        return t;
    }

    public float TimeLeft
    {
        get { return GameManager.TIME_PER_TURN - Time(); }
        protected set { }
    }

    public Ray2D FinalDir(Ray2D init)
    {
        Vector2 p = Vector2.zero;
        float d = 0;

        foreach (ICommandSegment s in mCmds)
        {
            p += s.PosDiff;
            d += s.DirDiff;
        }

        return new Ray2D(init.origin + p, Quaternion.AngleAxis(d, Vector3.forward) * init.direction);
    }

    public SimCmd()
    {
        mCmds = new List<ICommandSegment>();
    }

    public void Reset()
    {
        foreach (ICommandSegment o in mCmds)
        {
            o.Remove();
        }
        mCmds.Clear();
    }

    public void Add(ICommandSegment seg)
    {
        mCmds.Add(seg);
    }

    public ICommandSegment Last()
    {
        return mCmds[mCmds.Count - 1];
    }

}
