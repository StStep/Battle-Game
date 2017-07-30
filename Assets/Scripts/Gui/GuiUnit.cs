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
    private LineRenderer mLrMove;
    private LineRenderer mLrLGuide;
    private LineRenderer mLrRGuide;
    private LineRenderer mLrCGuide;


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

        // Make lines
        mLrMove = Create.LineRender(gameObject, "MovementLine", Color.red);

        mLrLGuide = Create.LineRender(gameObject, "LeftGuide", Color.yellow);
        mLrLGuide.useWorldSpace = false;
        Trig.DrawLine(mLrLGuide, new Vector2(0, 0), new Vector2(100, 100));

        mLrRGuide = Create.LineRender(gameObject, "RightGuide", Color.yellow);
        mLrRGuide.useWorldSpace = false;
        Trig.DrawLine(mLrRGuide, new Vector2(0, 0), new Vector2(-100, 100));

        mLrCGuide = Create.LineRender(gameObject, "CenterGuide", Color.green);
        mLrCGuide.useWorldSpace = false;
        Trig.DrawLine(mLrCGuide, new Vector2(0, 0), new Vector2(0, 100));

        EnableGuides(false);

    }

    private void EnableGuides(bool en)
    {
        mLrLGuide.enabled = en;
        mLrRGuide.enabled = en;
        mLrCGuide.enabled = en;

    }

    // Update is called once per frame
    public void Update()
    {
        if(tracking)
        {
            EnableGuides(true);
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Ray2D dir = new Ray2D(transform.position, transform.up);
            dir.ToString();
            Trig.LinePos lp = Trig.GetLine(dir, mousePosition, .25f);
            if(lp.Valid)
            {
                Trig.DrawLine(mLrMove, lp.Line);
            }
            else
            {
                Trig.ArcPos ap = Trig.GetArc(dir, mousePosition);

                if(ap.Valid)
                {
                    Trig.DrawArc(mLrMove, ap.Arc, 20);
                }
                else
                {
                    mLrMove.positionCount = 0;
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
                tracking = true;
        }

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
