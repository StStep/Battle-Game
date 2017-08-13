using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GhostRender : GuiRender
{
    // Components
    protected SpriteRenderer mySpriteRend;

    // Use this for initialization
    public GhostRender(GameObject go, Sprite sp) : base(go)
    {
        mySpriteRend = mGo.AddComponent<SpriteRenderer>();
        mySpriteRend.sprite = sp;

        Neutral();
    }

    public override void Render()
    {
        throw new NotImplementedException();
    }

    public override void Neutral()
    {
        mySpriteRend.color = Color.white;
    }

    public override void Bad()
    {
        mySpriteRend.color = Color.red - new Color(0, 0, 0, .7f);
    }

    public override void Good()
    {
        mySpriteRend.color = Color.green - new Color(0, 0, 0, .7f);
    }

    public override void Final()
    {
        mySpriteRend.color = Color.green;
    }

    public override void Show(bool en)
    {
        mySpriteRend.enabled = en;
    }
}
