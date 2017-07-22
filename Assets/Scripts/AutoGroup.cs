using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoGroup : MonoBehaviour {

    private List<Member> members = new List<Member>();

    public float inSpace = .33F;
    public float bodyRadius = .1F;
    public float fightRadius = .2F;
    public Vector2 weaponOffset = new Vector2(0 ,0);
    public Vector2 weaponRads = new Vector2(0, 0);
    public float weaponLength = .5F;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
