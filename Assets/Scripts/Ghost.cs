using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour, IClickObject
{
    private bool floating;
    private bool rotating;
    public List<ICommandSegment> mCmdSeg = new List<ICommandSegment>();

    // Use this for initialization
    public void Start ()
    {
        floating = false;
        rotating = false;
    }

    // Update is called once per frame
    public void Update ()
    {
        // Stop dragging when mouse is released
        if(Input.GetMouseButtonUp(0))
        {
            floating = false;
        }

        if (Input.GetMouseButtonUp(1))
        {
            rotating = false;
        }

        // Drag if floating
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if(floating)
        {
            transform.position = new Vector3(mousePosition.x, mousePosition.y, transform.position.z);
        }
        // Rotate if rotating
        else if (rotating)
        {
            // TODO Makesmoother, difference of mouse movement not absoute position
            float x = mousePosition.x - transform.position.x;
            float y = mousePosition.y - transform.position.y;
            float angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            ;
        }
    }

    public void Rotate()
    {
        rotating = true;
    }

    public void Float()
    {
        floating = true;
    }

    public void Hide()
    {
        enabled = false;
        floating = false;
    }

    public void Show(Vector3 pos)
    {
        transform.position = pos;
        enabled = true;
    }

    //////////////////////// IClickObject ///////////////////////////////

    public void LeftClick()
    {
        floating = true;
    }

    public void RightClick()
    {
        rotating = true;
    }
    /////////////////////////////////////////////////////////////////////

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
}
