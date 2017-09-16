using UnityEngine;
using System.Collections;

public delegate void SelectDel();

public class SelectComponent : MonoBehaviour
{
    public SelectDel OnSelect = null;
    public SelectDel OnDeselect = null;

    bool _selected = false;
    public bool Selected
    {
        get { return _selected; }
        protected set { _selected = value; }
    }

    public void Select()
    {
        if (OnSelect != null)
            OnSelect();
        Selected = true;
    }

    public void Deselect()
    {
        if (OnDeselect != null)
            OnDeselect();
        Selected = false;
    }
}
