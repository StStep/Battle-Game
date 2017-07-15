using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Background : MonoBehaviour, IClickObject
{
    ////////////////////// CLICKOBJECT I/F //////////////////////////////

    public void LeftClick()
    {
        GameManager.instance.Deselect();
    }

    public void RightClick()
    {
        GameManager.instance.Deselect();
    }

    /////////////////////////////////////////////////////////////////////
}
