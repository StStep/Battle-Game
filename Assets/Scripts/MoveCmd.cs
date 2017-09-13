﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCmd : MonoBehaviour, ICommandSegment
{
    private Path mPath;
    private PathComponent mPathComp;

    public void Awake()
    {
        // Render Comonents
        mPathComp = gameObject.AddComponent<PathComponent>().Init(Color.red);
        Sprite sp = Resources.Load<Sprite>("Sprites/Member");
        if (sp == null)
            throw new Exception("Failed to import sprite");
        SpriteRenderer s = gameObject.AddComponent<SpriteRenderer>();
        s.sprite = sp;
        s.color = Color.red;

        // Click Components
        CircleCollider2D c = gameObject.AddComponent<CircleCollider2D>();
        c.offset = Vector2.zero;
        c.radius = 0.13f;
        gameObject.AddComponent<ClickComponent>().Init();
    }

    public MoveCmd Init(Path p, ClickDel click)
    {
        mPath = p;
        mPathComp.RenderPoints = mPath.RenderPoints();
        transform.position = mPath.End;
        transform.Rotate(Vector3.forward, Vector2.SignedAngle(transform.up, mPath.EndDir));
        GetComponent<ClickComponent>().OnLeftClick = click;
        return this;
    }

    #region ICommandSegment
    //////////////////////// ICommandSegment ///////////////////////////////

    // TODO Assumes same speed across whole thing
    public float TimeDiff
    {
        get { return mPath.Length / GameManager.DEFAULT_SPEED; }

        protected set { }
    }

    public Vector2 PosDiff
    {
        get { return mPath.End - mPath.Start; }

        protected set { }
    }

    public float DirDiff
    {
        get { return Vector2.SignedAngle(mPath.StartDir, mPath.EndDir); }
        protected set { }
    }

    public Ray2D Translate(Ray2D init)
    {
        return mPath.Translate(init);
    }

    public void Remove()
    {
        Destroy(gameObject);
    }

    /////////////////////////////////////////////////////////////////////
    #endregion
}