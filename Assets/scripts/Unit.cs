using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Sprite))]
public class Unit : MonoBehaviour, ISelectable, IClickObject
{
    private bool selected = false;
    private SpriteRenderer mySpriteRend;
    private Ghost myGhost;

    // Use this for initialization
    public void Start()
    {
        mySpriteRend = GetComponent<SpriteRenderer>();
        this.myGhost = GetComponentInChildren(typeof(Ghost)) as Ghost;
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

    public void LeftClick()
    {
        GameManager.instance.SelectItem(this);

        myGhost.Show(this.gameObject.transform.position);
    }

    public void RightClick()
    {
        ;
    }
    /////////////////////////////////////////////////////////////////////
}
