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

        NeutralRender();
    }

    public override void SelectedRender(bool en)
    {
        if(en)
            mySpriteRend.color = Color.yellow;
        else
            mySpriteRend.color = Color.blue;
    }

    public override void LockRender()
    {
        mySpriteRend.color = Color.green;
    }

    public override void NeutralRender()
    {
        mySpriteRend.color = Color.white;
    }

    public override void BadRender()
    {
        mySpriteRend.color = Color.red - new Color(0, 0, 0, .7f);
    }

    public override void GoodRender()
    {
        mySpriteRend.color = Color.green - new Color(0, 0, 0, .7f);
    }
}
