﻿using System.Collections;
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
    public const float DEFAULTMIN_TURN_RAD = 3f;

    public const float MOVE_LINE_TOL = 0.25f;
    public const float MOVE_MIN_PNT_DIST = 0.1f;
    public const float MOVE_MIN_ARC_DIST = 0.5f;


    /// <summary>
    /// The single current instance of this class
    /// </summary>
    public static GameManager instance;

    public Selector mSelector = new Selector("Game Manager");

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
    }

    // Use this for initialization
    public void Start()
    {

    }

    // Update is called once per frame
    public void Update()
    {

    }

    public void DebugPress()
    {
        //string name = "N/A";
        //if(mSelectedObject != null)
        //{
        //    name = mSelectedObject.Name();
        //}
        //Debug.Log(string.Format("Pressed a button with {0} selected", name));
    }
}
