using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A singleton for managing game information such as selected units
/// </summary>
public class GameManager : MonoBehaviour
{
    // Constants
    public const float TIME_PER_TURN = 20f;
    public const float DEFAULT_SPEED = 4f / TIME_PER_TURN;
    public const float DEFAULT_MIN_TURN_RAD = 3f;

    public const float MOVE_LINE_TOL = 0.25f;
    public const float MOVE_MIN_PNT_DIST = 0.1f;
    public const float MOVE_MIN_ARC_DIST = 0.5f;


    /// <summary>
    /// The single current instance of this class
    /// </summary>
    public static GameManager instance;

    public SelectComponent mSelector = null;

    protected void Awake()
    {
        // Only have one in game
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

        mSelector = gameObject.AddComponent<SelectComponent>();

        // Make Initial gamestate
        MakeBackground();

        // Make test units
        GameObject u = new GameObject();
        u.name = "Units";
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        MakeUnit(u, "Example Unit 1", Vector2.zero, 0);
        MakeUnit(u, "Example Unit 2", new Vector2(.5f, 2), -45);
        MakeUnit(u, "Example Unit 3", new Vector2(3, 2), -90);
        MakeUnit(u, "Example Unit 4", new Vector2(-2, -3), 135);
    }

    // Use this for initialization
    public void Start()
    { }

    // Update is called once per frame
    public void Update()
    { }

    public void DebugPress()
    {
        //string name = "N/A";
        //if(mSelectedObject != null)
        //{
        //    name = mSelectedObject.Name();
        //}
        //Debug.Log(string.Format("Pressed a button with {0} selected", name));
    }

    #region GameCreation

    private GameObject MakeBackground()
    {
        GameObject g = new GameObject();
        g.name = "Background";
        g.transform.position = new Vector3(0, 0, 5);
        g.AddComponent<BoxCollider2D>().size = new Vector2(30, 30);
        ClickComponent c = g.AddComponent<ClickComponent>().Init();
        c.OnLeftClick = () => instance.mSelector.ChainDeselect();
        c.OnRightClick = () => instance.mSelector.ChainDeselect();

        return g;
    }

    private GameObject MakeUnit(GameObject par,string name, Vector2 pos, float rot)
    {
        GameObject g = new GameObject();
        g.name = name;
        g.transform.parent = par.transform;
        g.transform.localPosition = Vector3.zero;
        g.transform.localRotation = Quaternion.identity;
        g.transform.position = new Vector3(pos.x, pos.y, 0);
        g.transform.rotation = Quaternion.AngleAxis(rot, Vector3.forward);
        g.AddComponent<BoxCollider2D>().size = new Vector2(1.357859f, 0.6493425f);
        Sprite sp = Resources.Load<Sprite>("Sprites/Unit_Arr");
        if (sp == null)
            throw new Exception("Failed to import sprite");
        g.AddComponent<SpriteRenderer>().sprite = sp;
        g.AddComponent<GuiUnit>();

        return g;
    }

    #endregion
}
