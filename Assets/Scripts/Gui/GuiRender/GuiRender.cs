using UnityEngine;
using System.Collections;

public abstract class GuiRender
{
    protected GameObject mGo = null;

    public GuiRender(GameObject go)
    {
        mGo = go;
    }

    public abstract void Render();

    public abstract void Neutral();

    public abstract void Bad();

    public abstract void Good();

    public abstract void Final();

    public abstract void Show(bool en);
}
