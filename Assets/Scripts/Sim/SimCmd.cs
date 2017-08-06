using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimCmd {

    // Dynamic Objects
    List<ICommandSegment> mPaths;

    float _timeSpent;
    public float TimeSpent
    {
        get { return _timeSpent; }
        protected set { _timeSpent = value; }
    }

    public float TimeLeft
    {
        get { return GameManager.TIME_PER_TURN - TimeSpent; }
        protected set { }
    }

    public Ray2D FinalDir
    {
        get { return new Ray2D(Last().End, Last().EndDir); }
        protected set { }
    }

    public SimCmd()
    {
        mPaths = new List<ICommandSegment>();
        TimeSpent = 0;
    }

    public void Reset(Ray2D dir)
    {
        mPaths.Clear();
        mPaths.Add(new PointPath(dir));
        TimeSpent = 0;
    }

    public void Add(ICommandSegment seg)
    {
        mPaths.Add(seg);
        TimeSpent += seg.Time;
    }

    public ICommandSegment Last()
    {
        return mPaths[mPaths.Count - 1];
    }

}
