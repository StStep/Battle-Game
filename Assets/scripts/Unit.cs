using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Sprite))]
public class Unit : MonoBehaviour, ISelectable
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

    private void LeftClick()
    {
        GameManager.instance.SelectItem(this);
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            LeftClick();
        }
    }
}
