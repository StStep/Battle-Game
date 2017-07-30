using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D))]
public class GuiBack : MonoBehaviour
{
    #region MouseIF
    ////////////////////// MouseIF //////////////////////////////

    public void OnMouseOver()
    {
        // Ignore UI click
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        // Left Click
        if (Input.GetMouseButtonDown(0))
            LeftClick();

        // Right Click
        if (Input.GetMouseButtonDown(1))
            RightClick();

        // Middle Click
        if (Input.GetMouseButtonDown(2))
            Debug.Log("Pressed middle click.");
    }

    public void LeftClick()
    {
        GameManager.instance.mSelector.Deselect();
    }

    public void RightClick()
    {
        GameManager.instance.mSelector.Deselect();
    }

    /////////////////////////////////////////////////////////////////////
    #endregion
}
