using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Sprite))]
public class Unit : MonoBehaviour, ISelectable, IClickObject
{
    private bool selected = false;
    private SpriteRenderer mySpriteRend;

    // Use this for initialization
    public void Start()
    {
        mySpriteRend = GetComponent<SpriteRenderer>();
        Deselect();
    }

    // Update is called once per frame
    public void Update()
    {
        ;
    }

    ////////////////////// SELECTABLE I/F //////////////////////////////

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

    ////////////////////// CLICKOBJECT I/F //////////////////////////////

    public void LeftClick()
    {
        GameManager.instance.SelectItem(this);
    }

    public void RightClick()
    {
        ;
    }
    /////////////////////////////////////////////////////////////////////
}
