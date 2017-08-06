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
    public void Start ()
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
        if (mySpriteRend != null)
            mySpriteRend.color = Color.white;
    }

    public void Bad()
    {
        if(mySpriteRend != null)
            mySpriteRend.color = Color.red - new Color(0, 0, 0, .7f);
    }

    public void Good()
    {
        if (mySpriteRend != null)
            mySpriteRend.color = Color.green - new Color(0, 0, 0, .7f);
    }

    public void SetPos(Vector3 pos, Quaternion rot)
    {
        transform.position = pos;
        transform.rotation = rot;
    }

    #region MouseIF
    //////////////////////// MouseIF ///////////////////////////////

    public void OnMouseOver()
    {
        // Ignore UI click
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        // Left Click
        if (Input.GetMouseButtonUp(0))
        {
            LeftClick();
        }

        // Right Click
        if (Input.GetMouseButtonUp(1))
        {
            RightClick();
        }

        // Middle Click
        if (Input.GetMouseButtonUp(2))
        {
            Debug.Log("Pressed middle click.");
        }
    }

    public void LeftClick()
    {
        ;
    }

    public void RightClick()
    {
        ;
    }
    /////////////////////////////////////////////////////////////////////
    #endregion
}
