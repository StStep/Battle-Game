using UnityEngine;
using System.Collections;

public abstract class GuiSegObject : ClickObject
{
    // Static Objects
    protected ICommandRef mCmdRef;

    public virtual void Init(GameObject par, Selector sel, GuiRender ren,
        ICommandRef cmdRef)
    {
        base.Init(par, sel, ren);
        mCmdRef = cmdRef;
    }

    public new void Awake()
    {
        base.Awake();
    }

    public new void Start()
    {
        base.Start();
    }

    public new void Update()
    {
        base.Update();
    }
}
