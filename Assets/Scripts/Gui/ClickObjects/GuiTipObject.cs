using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuiTipObject : GuiSegObject
{
    public override void Init(GameObject par, Selector sel, GuiRender ren,
        ICommandRef cmdRef)
    {
        base.Init(par, sel, ren, cmdRef);
    }

    // Use this for initialization
    public new void Start()
    {
        base.Start();
    }

    #region Utility

    public void Move()
    {
        if (!mSel)
        {
            mSelector.ChainSelect();
        }

        if (mSel)
        {
            mCmdRef.SetState(State.Moving);
        }
    }

    public void Rotate()
    {
        if (!mSel)
        {
            mSelector.ChainSelect();
        }

        // TODO Rotate
    }

    #endregion

    #region IClickable
    //////////////////////// IClickable ///////////////////////////////

    public override void Click(ClickType t)
    {
        switch(t)
        {
            case ClickType.LeftClick:
                Move();
                break;
            case ClickType.RightClick:
                Rotate();
                break;
            default:
                break;
        }
    }

    /////////////////////////////////////////////////////////////////////
    #endregion
}
