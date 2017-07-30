using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GuiGhost : MonoBehaviour
{
    private Vector2 mouseStart;

    // Use this for initialization
    public void Start ()
    {
        ;
    }

    // Update is called once per frame
    public void Update ()
    {
        //// Stop dragging when mouse is released
        //if(Input.GetMouseButtonUp(0))
        //{
        //    floating = false;
        //}

        //if (Input.GetMouseButtonUp(1))
        //{
        //    rotating = false;
        //}

        //// Drag if floating
        //Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //if(floating)
        //{
        //    transform.position = new Vector3(mousePosition.x, mousePosition.y, transform.position.z);
        //}
        //// Rotate if rotating
        //else if (rotating)
        //{
        //    // TODO Makesmoother, difference of mouse movement not absoute position
        //    float sx = mouseStart.x - transform.position.x;
        //    float sy = mouseStart.y - transform.position.y;
        //    float x = mousePosition.x - transform.position.x;
        //    float y = mousePosition.y - transform.position.y;
        //    float sangle = Mathf.Atan2(sy, sx) * Mathf.Rad2Deg;
        //    float angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
        //    transform.rotation = Quaternion.Euler(0, 0, angle - sangle);
        //}
        //else
        //{
        //    ;
        //}
    }

    //public void Rotate(Vector2 start)
    //{
    //    mouseStart = start;
    //    rotating = true;
    //}

    //public void Float()
    //{
    //    mVisible = true;
    //}

    public void Show(bool en)
    {
        gameObject.SetActive(en);
    }

    public void SetPos(Vector3 pos, Quaternion rot)
    {
        transform.position = pos;
        transform.rotation = rot;
    }

    #region IClickObject
    //////////////////////// IClickObject ///////////////////////////////

    public void OnMouseOver()
    {
        //// Ignore UI click
        //if (EventSystem.current.IsPointerOverGameObject())
        //{
        //    return;
        //}

        //// Left Click
        //if (Input.GetMouseButtonDown(0))
        //    LeftClick();

        //// Right Click
        //if (Input.GetMouseButtonDown(1))
        //    RightClick();

        //// Middle Click
        //if (Input.GetMouseButtonDown(2))
        //    Debug.Log("Pressed middle click.");
    }

    public void LeftClick()
    {
        //mVisible = true;
    }

    public void RightClick()
    {
        //Vector3 p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //mouseStart = new Vector2(p.x, p.y);
        //rotating = true;
    }
    /////////////////////////////////////////////////////////////////////
    #endregion

    #region ICommandSegment
    //////////////////////// ICommandSegment ///////////////////////////////

    public bool IsFixedLen
    {
        get { return false; }
    }

    public int Length
    {
        get { return 0; }
    }

    public int AdjustLength(int diff)
    {
        return 0;
    }

    public void Remove()
    {
        Destroy(gameObject);
    }
    /////////////////////////////////////////////////////////////////////
    #endregion
}
