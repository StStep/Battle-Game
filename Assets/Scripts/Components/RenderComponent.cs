using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RenderComponent : MonoBehaviour
{
    // Status Members
    private GuiRender mGuiRender = null;

    public GuiRender Renderer
    {
        get { return mGuiRender; }
        protected set { }
    }

    public void Init(GuiRender ren)
    {
        mGuiRender = ren;
    }

    public void SetPos(Vector3 pos, Quaternion rot)
    {
        transform.position = pos;
        transform.rotation = rot;
    }

    public void Show(bool en)
    {
        gameObject.SetActive(en);
    }
}
