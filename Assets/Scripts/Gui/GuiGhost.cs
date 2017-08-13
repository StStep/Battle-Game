using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class GuiGhost : MonoBehaviour
{
    // Components
    private SpriteRenderer mySpriteRend;

    // Use this for initialization
    public void Awake ()
    {
        mySpriteRend = GetComponent<SpriteRenderer>();
        Neutral();
    }

    // Update is called once per frame
    public void Update ()
    {
        ;
    }

    public void Show(bool en)
    {
        gameObject.SetActive(en);
    }

    public void Neutral()
    {
        mySpriteRend.color = Color.white;
    }

    public void Bad()
    {
        mySpriteRend.color = Color.red - new Color(0, 0, 0, .7f);
    }

    public void Good()
    {
        mySpriteRend.color = Color.green - new Color(0, 0, 0, .7f);
    }

    public void Final()
    {
        mySpriteRend.color = Color.green;
    }

    public void SetPos(Vector3 pos, Quaternion rot)
    {
        transform.position = pos;
        transform.rotation = rot;
    }
}
