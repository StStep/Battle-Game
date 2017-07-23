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

public class AutoGroup : MonoBehaviour {

    private List<AutoMember> members = new List<AutoMember>();

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
        InstantiateMembers();
    }
	
	// Update is called once per frame
	void Update () {
         OrderMembers();
    }
}
