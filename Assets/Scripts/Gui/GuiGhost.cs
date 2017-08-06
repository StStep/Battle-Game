using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class GuiGhost : MonoBehaviour, IClickRef
{
    // Components
    private SpriteRenderer mySpriteRend;

    // Delegates
    private Click mLeftDel;
    private Click mRightDel;

    // Use this for initialization
    public void Awake ()
    {
        mySpriteRend = GetComponent<SpriteRenderer>();
        Neutral();

        mLeftDel = null;
        mRightDel = null;
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

    #region IClickRef
    //////////////////////// IClickRef ///////////////////////////////

    public void SetLeft(Click del)
    {
        mLeftDel = del;
    }

    public void SetRight(Click del)
    {
        mRightDel = del;
    }

    /////////////////////////////////////////////////////////////////
    #endregion

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
        if (mLeftDel != null && Input.GetMouseButtonUp(0))
        {
            mLeftDel();
        }

        // Right Click
        if (mRightDel != null && Input.GetMouseButtonUp(1))
        {
            mRightDel();
        }

        // Middle Click
        if (Input.GetMouseButtonUp(2))
        {
            Debug.Log("Pressed middle click.");
        }
    }

    /////////////////////////////////////////////////////////////////////
    #endregion
}
