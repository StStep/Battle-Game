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
        ;
    }

    public override void LockRender()
    {
        mySpriteRend.color = Color.green;
    }

    public override void NeutralRender()
    {
        mySpriteRend.color = Color.white;
    }
}
