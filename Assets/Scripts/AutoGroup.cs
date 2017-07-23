using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMember
{
    public int pInd;
    public Vector2 rfInd;
    public Vector2 dest;

    public AutoMember(int i, int x, int y, float space, bool rect)
    {
        pInd = i;
        rfInd = new Vector2(x, y);
        if(rect)
        {
            dest = new Vector2(2* x * space, 2 * -y * space);
        }
        else
        {
            float height = space * 2;
            float vert = height * 3 / 4;
            float width = (float)System.Math.Sqrt((double)3) / 2 * height;
            float xd = width * x;
            if (y % 2 == 1)
                xd += width/2;
            dest = new Vector2(xd, -y * vert);
        }
    }
}

[RequireComponent(typeof(BoxCollider2D))]
public class AutoGroup : MonoBehaviour {

    private List<AutoMember> members = new List<AutoMember>();
    private BoxCollider2D coll;

    public bool rectOrder = true;
    public float inSpace = .33F;
    public int frontage = 7;
    public float bodyRadius = .12F;
    public float fightRadius = .17F;
    public Vector2 weaponOffset = new Vector2(0 ,0);
    public Vector2 weaponRads = new Vector2(0, 0);
    public float weaponLength = .5F;

    public int MemberCount = 30;

    private void OrderMembers()
    {
        // Order Children
        int x = 0, y = 0;
        float innerSpace = (this.inSpace > fightRadius) ? this.inSpace : fightRadius;
        for (int i = 0; i < this.transform.childCount; i++)
        {
            Transform child = this.transform.GetChild(i);
            AutoMember m = new AutoMember(i, x, y, innerSpace, rectOrder);
            members.Add(m);
            child.localPosition = m.dest;
            x++;
            if (x >= frontage)
            {
                x = 0;
                y++;
            }
        }
        if (x == 0)
            y--;
        Vector2 size;
        if(rectOrder)
        {
            size = new Vector2((frontage - 1) * 2 * innerSpace + 2 * bodyRadius, y * 2 * innerSpace + 2 * bodyRadius);
        }
        else
        {
            // TODO Fix this
            float height = innerSpace * 2;
            float vert = height * 3 / 4;
            float width = (float)System.Math.Sqrt((double)3) / 2 * height;
            size = new Vector2((frontage - 1) * width + 2 * bodyRadius, y * height + 2 * bodyRadius);
        }
        coll.size = size;
        coll.offset = new Vector2 (size.x /2 - bodyRadius, -size.y/2 + bodyRadius);
    }

    private void InstantiateMembers()
    {
        for (int i = 0; i < MemberCount; i++)
        {
            GameObject go = Instantiate(Resources.Load("Prefabs/AutoMember")) as GameObject;
            go.transform.parent = this.transform;
        }
    }

    // Use this for initialization
    void Start () {
        coll = GetComponentInChildren(typeof(BoxCollider2D)) as BoxCollider2D;
        InstantiateMembers();
    }
	
	// Update is called once per frame
	void Update () {
         OrderMembers();
    }
}
