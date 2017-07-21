using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Member
{
    public int pInd;
    public Vector2 rfInd;
    public Vector2 dest;

    public Member(int i, int x, int y, float space)
    {
        pInd = i;
        rfInd = new Vector2(x, y);
        dest = new Vector2(x*space, -y*space);
    }
}

public class Group : MonoBehaviour {

    private List<Member> members = new List<Member>();

    public float speed = .01F;

    public float space = .33F;

    public int width = 4;

	// Use this for initialization
	void Start () {
        // =  this.gameObject.GetComponentsInChildren(typeof(Transform)) as Transform[];

        // Order Children
        int x = 0, y = 0;
        for (int i = 0; i < this.transform.childCount; i++)
        {
            Transform child = this.transform.GetChild(i);
            members.Add(new Member(i, x, y, this.space));
            x++;
            if(x >= width)
            {
                x = 0;
                y++;
            }
        }


    }

    //// Sort by relative position within this.transform
    //public void SortChildren()
    //{
    //    // Set destination positions
    //    foreach(Member m in members)
    //    {
    //        // Line up front row
    //        if(m.rfInd.y == 0)
    //        {
    //            m.dest = new Vector2(this.space * m.rfInd.x, 0);
    //        }
    //        // Vetical Spacing from front
    //        else
    //        {

    //        }

    //        // Line Up left file
    //        if (m.rfInd.x == 0)
    //        {

    //        }
    //        //Horizantal spacing from left
    //        else
    //        {

    //        }


    //    }

    //}
	
	// Update is called once per frame
	void Update () {
        foreach (Member m in members)
        {
            Transform child = this.transform.GetChild(m.pInd);
            child.position += speed * (new Vector3(m.dest.x, m.dest.y, 0) - child.position);
            //child.position = new Vector3(m.dest.x, m.dest.y, 0);
        }
    }
}
