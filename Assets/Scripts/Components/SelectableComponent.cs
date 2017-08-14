using UnityEngine;
using System.Collections;

public delegate bool SelectDel();

public class SelectableComponent : MonoBehaviour, ISelectorItem
{
    public SelectDel OnSelect = null;
    public SelectDel OnDeselect = null;

    // Status Members
    private Selector mSelector = null;

    bool _selected = false;
    public bool Selected
    {
        get { return _selected; }
        protected set { _selected = value; }
    }

    public void Init(Selector sel)
    {
        mSelector = new Selector(gameObject.name, sel, this);
    }

    public bool ChainSelect()
    {
        return mSelector.ChainSelect();
    }

    #region ISelectorItem
    //////////////////////// ISelectorItem ///////////////////////////////

    public bool SelectSelf()
    {
        Selected = true;
        if (OnSelect != null)
            return OnSelect();
        return true;
    }

    public bool DeselectSelf()
    {
        Selected = false;
        if (OnDeselect != null)
            return OnDeselect();
        return true;
    }
    /////////////////////////////////////////////////////////////////////
    #endregion
}
