using UnityEngine;
using System.Collections;

public abstract class GuiRender
{
    protected GameObject mGo = null;

    public GuiRender(GameObject go)
    {
        mGo = go;
    }

    public abstract void SelectedRender(bool en);

    public abstract void NeutralRender();

    public abstract void LockRender();
}
