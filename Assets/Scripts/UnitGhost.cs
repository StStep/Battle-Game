using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitGhost : MonoBehaviour, ICommandSegment, IClickObject
{
    private bool floating;

    // Use this for initialization
    public void Start ()
    {
        floating = true;
    }

    // Update is called once per frame
    public void Update ()
    {
        // Stop dragging when mouse is released
        if(Input.GetMouseButtonUp(0))
        {
            floating = false;
        }

        // Drag if floating
        Vector3 mousePosition;
        if(floating)
        {
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mousePosition.x, mousePosition.y, transform.position.z);
        }
        else
        {

        }
		
	}

    //////////////////////// IClickObject ///////////////////////////////

    public void LeftClick()
    {
        floating = true;
    }

    public void RightClick()
    {
        ;
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
