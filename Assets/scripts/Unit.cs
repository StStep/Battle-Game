using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Unit : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        ;
    }

    // Update is called once per frame
    void Update()
    {
        ;
    }

    void LeftClick()
    {
        GameManager.instance.SelectItem(this.gameObject);
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            LeftClick();
        }
    }
}
