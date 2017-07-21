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
    public float maxSpeed = .5F;

    public float gain = .01F;
    public float maxForce = .5F;

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
            Member m = new Member(i, x, y, this.space);
            members.Add(m);
            child.position = m.dest;
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

    public void FixedUpdate()
    {
        foreach (Member m in members)
        {
            //Transform child = this.transform.GetChild(m.pInd);
            ////child.position += speed * (new Vector3(m.dest.x, m.dest.y, 0) - child.position);
            //Rigidbody2D rb = child.GetComponent<Rigidbody2D>() as Rigidbody2D;
            //rb.AddForce(speed * (new Vector3(m.dest.x, m.dest.y, 0) - child.position));
            ////child.position = new Vector3(m.dest.x, m.dest.y, 0);

            Transform child = this.transform.GetChild(m.pInd);
            Rigidbody2D rb = child.GetComponent<Rigidbody2D>() as Rigidbody2D;

            Vector2 dist = new Vector3(m.dest.x, m.dest.y, 0) - child.position;
            // calc a target vel proportional to distance (clamped to maxVel)
            Vector2 tgtVel = Vector2.ClampMagnitude(speed * dist, maxSpeed);
            // calculate the velocity error
            Vector2 error = tgtVel - rb.velocity;
            // calc a force proportional to the error (clamped to maxForce)
            Vector2 force = Vector2.ClampMagnitude(gain * error, maxForce);
            rb.AddForce(force);
        }
    }

    // Update is called once per frame
    void Update () {

    }
}
