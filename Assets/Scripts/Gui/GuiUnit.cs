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
        GameObject myLine = new GameObject();
        myLine.transform.parent = this.gameObject.transform;
        myLine.transform.position = this.transform.position;
        myLine.AddComponent<LineRenderer>();
        myLr = myLine.GetComponent<LineRenderer>();
        myLr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        myLr.startColor = Color.red;
        myLr.endColor = Color.red;
        myLr.startWidth = 0.05f;
        myLr.endWidth = 0.05f;
        myLr.positionCount = 1;
        myLr.SetPosition(0, this.transform.position);
    }

    // Update is called once per frame
    public void Update()
    {
        if(tracking)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Ray2D dir = new Ray2D(transform.position, transform.up);
            dir.ToString();
            Tools.LinePos lp = Tools.GetLine(dir, mousePosition, .25f);
            if(lp.Valid)
            {
                myLr.positionCount = 2;
                myLr.SetPosition(0, new Vector3(lp.Line.Start.x, lp.Line.Start.y));
                myLr.SetPosition(1, new Vector3(lp.Line.End.x, lp.Line.End.y));
            }
            else
            {
                Tools.ArcPos ap = Tools.GetArc(dir, mousePosition);

                if(ap.Valid)
                {
                    int segments = 20;
                    myLr.positionCount = segments + 1;
                    float rawAngle = Vector2.SignedAngle(ap.Arc.Start - ap.Arc.Center, ap.Arc.End - ap.Arc.Center);
                    Debug.Log(rawAngle);
                    bool clockwise = (rawAngle < 0);
                    float angle = (clockwise) ? -90 : 90;
                    float arcLength = Mathf.Abs(rawAngle);
                    float radius = Mathf.Abs(Vector2.Distance(ap.Arc.Start, ap.Arc.Center));
                    for (int i = 0; i <= segments; i++)
                    {
                        float x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
                        float y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

                        myLr.SetPosition(i, new Vector3(x + ap.Arc.Center.x, y + ap.Arc.Center.y));

                        angle += (clockwise) ? (arcLength / segments) : -(arcLength / segments);
                    }
                    myLr.SetPosition(segments, ap.Arc.End);

                }
                else
                {
                    myLr.positionCount = 1;
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


    void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        GameObject.Destroy(myLine, duration);
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
