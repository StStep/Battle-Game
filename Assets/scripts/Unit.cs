using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Sprite))]
public class Unit : MonoBehaviour, ISelectable, IClickObject
{
    private bool selected = false;
    private SpriteRenderer mySpriteRend;

    List<ICommandSegment> mCmdSeg = new List<ICommandSegment>();

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

        foreach(ICommandSegment seg in mCmdSeg)
        {
            seg.Remove();
        }
        mCmdSeg.Clear();
        GameObject newseg = Instantiate(Resources.Load("Prefabs/UnitGhost")) as GameObject;
        newseg.transform.position = this.gameObject.transform.position;
        newseg.transform.parent = this.gameObject.transform;
        mCmdSeg.Add(newseg.GetComponent<ICommandSegment>());
    }

    public void RightClick()
    {
        ;
    }
    /////////////////////////////////////////////////////////////////////
}
