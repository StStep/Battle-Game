using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D), typeof(Sprite))]
public class GuiUnit : MonoBehaviour, ISelectable
{
    private bool selected = false;
    private bool tracking = false;
    private SpriteRenderer mySpriteRend;
    private GuiGhost myGhost;
    LineRenderer myLr;

    // Use this for initialization
    public void Start()
    {
        mySpriteRend = GetComponent<SpriteRenderer>();
        this.myGhost = GetComponentInChildren(typeof(GuiGhost)) as GuiGhost;
        if(this.myGhost == null)
        {
            Debug.LogError("No ghost object found.");
        }
        this.myGhost.Hide();
        Deselect();

        // Make line
        myLr = Create.LineRender(gameObject, "MovementLine", Color.red);
    }

    // Update is called once per frame
    public void Update()
    {
        if(tracking)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Ray2D dir = new Ray2D(transform.position, transform.up);
            dir.ToString();
            Trig.LinePos lp = Trig.GetLine(dir, mousePosition, .25f);
            if(lp.Valid)
            {
                Trig.DrawLine(myLr, lp.Line);
            }
            else
            {
                Trig.ArcPos ap = Trig.GetArc(dir, mousePosition);

                if(ap.Valid)
                {
                    Trig.DrawArc(myLr, ap.Arc, 20);
                }
                else
                {
                    myLr.positionCount = 0;
                }

            }
        }
    }

    //////////////////////// ISelectable ///////////////////////////////

    public string Name()
    {
        return this.gameObject.name;
    }

    public void Select()
    {
        selected = true;
        mySpriteRend.color = Color.yellow;
    }

    public void Deselect()
    {
        selected = false;
        mySpriteRend.color = Color.blue;
    }
    /////////////////////////////////////////////////////////////////////

    //////////////////////// IClickObject ///////////////////////////////

    public void OnMouseOver()
    {
        // Ignore UI click
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        // Left Click
        if (Input.GetMouseButtonDown(0))
        {
            LeftClick();
            if (!tracking)
                tracking = true;        }

        // Right Click
        if (Input.GetMouseButtonDown(1))
            RightClick();

        // Middle Click
        if (Input.GetMouseButtonDown(2))
            Debug.Log("Pressed middle click.");
    }

    public void LeftClick()
    {
        GameManager.instance.SelectItem(this);
    }

    public void RightClick()
    {
        GameManager.instance.SelectItem(this);

        Vector3 p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        myGhost.Rotate(new Vector2(p.x, p.y));
        myGhost.Show(this.gameObject.transform.position, this.gameObject.transform.rotation);
    }
    /////////////////////////////////////////////////////////////////////
}
