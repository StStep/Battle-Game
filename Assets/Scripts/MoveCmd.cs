using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCmd : MonoBehaviour, ICommandSegment
{
    private Path mPath;
    private PathComponent mPathComp;
    private SpriteRenderer mSpRender;
    private CircleCollider2D mCollider;

    public void Awake()
    {
        // Render Comonents
        mPathComp = gameObject.AddComponent<PathComponent>().Init(Color.red);
        Sprite sp = Resources.Load<Sprite>("Sprites/Member");
        if (sp == null)
            throw new Exception("Failed to import sprite");
        mSpRender = gameObject.AddComponent<SpriteRenderer>();
        mSpRender.sprite = sp;
        mSpRender.color = Color.red;

        // Click Components
        mCollider = gameObject.AddComponent<CircleCollider2D>();
        mCollider.offset = Vector2.zero;
        mCollider.radius = 0.13f;
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

    public void Hide(bool b)
    {
        mSpRender.enabled = !b;
        mCollider.enabled = !b;
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