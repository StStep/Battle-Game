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
    }

    // Update is called once per frame
    public void Update()
    {
        ;
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
            if(!tracking)
                StartCoroutine("TrackMouse");
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

    IEnumerator TrackMouse()
    {
        tracking = true;
        drawPoints.Clear();

        while(Input.GetMouseButton(0))
        {
            Tools.Point mousePosition = new Tools.Point((Int32)Mathf.Round(Input.mousePosition.x), (Int32)Mathf.Round(Input.mousePosition.y));
            drawPoints.Add(mousePosition);
            yield return new WaitForSeconds(.1f);
        }

        Debug.Log(String.Format("Collected {0} points", drawPoints.Count));
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
