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
    private List<Tools.Point> drawPoints = new List<Tools.Point>();
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
                myLr.SetPosition(1, new Vector3(lp.Line.End.x, lp.Line.End.y));
            }
            else
            {
                Tools.ArcPos ap = Tools.GetArc(dir, mousePosition);

                if(ap.Valid)
                {
                    myLr.positionCount = 2;
                    myLr.SetPosition(1, new Vector3(ap.Arc.Center.x, ap.Arc.Center.y));
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
                tracking = true;
                //StartCoroutine("TrackMouse");
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

    void DrawPath(List<Tools.Point> path)
    {
        Debug.Log(String.Format("Drawing {0} points", path.Count));
        for (int i = 0; i < path.Count - 1; i++)
        {
            Vector3 start = Camera.main.ScreenToWorldPoint(new Vector3(path[i].X, path[i].Y, 0));
            start.z = 0;
            Vector3 end = Camera.main.ScreenToWorldPoint(new Vector3(path[i + 1].X, path[i + 1].Y, 0));
            end.z = 0;

            DrawLine(start, end, Color.red, 2f);
        }
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

    IEnumerator TrackMouse()
    {
        tracking = true;
        drawPoints.Clear();

        while(Input.GetMouseButton(0))
        {
            Tools.Point mousePosition = new Tools.Point((Int32)Mathf.Round(Input.mousePosition.x), (Int32)Mathf.Round(Input.mousePosition.y));
            drawPoints.Add(mousePosition);
            yield return new WaitForSeconds(.05f);
        }

        Debug.Log(String.Format("Collected {0} points", drawPoints.Count));
        DrawPath(Tools.DouglasPeuckerReduction(drawPoints, 2f));
        tracking = false;
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
