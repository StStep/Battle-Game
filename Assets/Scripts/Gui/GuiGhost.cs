using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GuiGhost : MonoBehaviour
{
    // Use this for initialization
    public void Start ()
    {
        ;
    }

    // Update is called once per frame
    public void Update ()
    {
        ;
    }

    public void Show(bool en)
    {
        gameObject.SetActive(en);
    }

    public void SetPos(Vector3 pos, Quaternion rot)
    {
        transform.position = pos;
        transform.rotation = rot;
    }

    #region MouseIF
    //////////////////////// MouseIF ///////////////////////////////

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
        ;
    }

    public void RightClick()
    {
        ;
    }
    /////////////////////////////////////////////////////////////////////
    #endregion
}
