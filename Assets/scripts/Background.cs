using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Background : MonoBehaviour
{

    private bool selected = false;

    // Use this for initialization
    public void Start()
    {
        ;
    }

    // Update is called once per frame
    public void Update()
    {
        ;
    }

    private void LeftClick()
    {
        GameManager.instance.Deselect();
    }

    public void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            LeftClick();
        }
    }
}
