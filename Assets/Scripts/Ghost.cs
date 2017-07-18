using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour, IClickObject
{
    private bool floating;
    private bool rotating;
    private Vector2 mouseStart;
    private SpriteRenderer mySpriteRend;
    public List<ICommandSegment> mCmdSeg = new List<ICommandSegment>();

    // Use this for initialization
    public void Start ()
    {
        mySpriteRend = GetComponent<SpriteRenderer>();
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
            float sx = mouseStart.x - transform.position.x;
            float sy = mouseStart.y - transform.position.y;
            float x = mousePosition.x - transform.position.x;
            float y = mousePosition.y - transform.position.y;
            float sangle = Mathf.Atan2(sy, sx) * Mathf.Rad2Deg;
            float angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - sangle);
        }
        else
        {
            ;
        }
    }

    public void Rotate(Vector2 start)
    {
        mouseStart = start;
        rotating = true;
    }

    public void Float()
    {
        floating = true;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        floating = false;
        rotating = false;
    }

    public void Show(Vector3 pos, Quaternion rot)
    {
        transform.position = pos;
        transform.rotation = rot;
        gameObject.SetActive(true);
    }

    //////////////////////// IClickObject ///////////////////////////////

    public void LeftClick()
    {
        floating = true;
    }

    public void RightClick()
    {
        Vector3 p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseStart = new Vector2(p.x, p.y);
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
