using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimCmd {

    // Dynamic Objects
    List<MoveCmd> mCmds;

    public float Time()
    {
        float t = 0;
        foreach(MoveCmd s in mCmds)
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

        foreach (MoveCmd s in mCmds)
        {
            p += s.PosDiff;
            d += s.DirDiff;
        }

        return new Ray2D(init.origin + p, Quaternion.AngleAxis(d, Vector3.forward) * init.direction);
    }

    public SimCmd()
    {
        mCmds = new List<MoveCmd>();
    }

    public void Reset()
    {
        foreach (MoveCmd o in mCmds)
        {
            o.Remove();
        }
        mCmds.Clear();
    }

    public void Add(MoveCmd seg)
    {
        mCmds.Add(seg);
    }

    public MoveCmd Last()
    {
        return (mCmds.Count != 0) ? mCmds[mCmds.Count - 1] : null;
    }

    public void ShowAllButLast(bool b)
    {
        for(int i = 0; i < mCmds.Count - 1; i++)
        {
            mCmds[i].Hide(!b);
        }

        if(Last())
            Last().Hide(true);
    }

}
