using UnityEngine;
using System.Collections;

public delegate bool SelectDel();

public class SelectableComponent : MonoBehaviour
{
    public SelectDel OnSelect = null;
    public SelectDel OnDeselect = null;

    private SelectableComponent mPar = null;
    private SelectableComponent mChild = null;

    bool _selected = false;
    public bool Selected
    {
        get { return _selected; }
        protected set { _selected = value; }
    }

    // Constructor
    public void Init(SelectableComponent parent)
    {
        mPar = parent;
    }

    /// <summary>
    /// Select self from the highest parent downwards.
    /// </summary>
    /// <returns>True if sucessfully selected form highest parent</returns>
    public bool ChainSelect()
    {
        Debug.Log("Select " + gameObject.name);
        bool ret = true;
        if (mPar != null)
        {
            ret = mPar.SelectChild(this);
        }
        return ret;
    }

    /// <summary>
    /// Deselect from self to lowest child.
    /// </summary>
    /// <returns>Returns true if successfully deselect from self to lowest child</returns>
    public bool ChainDeselect()
    {
        Debug.Log("Deselect " + gameObject.name);

        if (mChild != null && !mChild.ChainDeselect())
            return false;
        mChild = null;

        // Deslect self
        if (!DeselectSelf())
            return false;

        return true;
    }

    private bool SelectChild(SelectableComponent obj)
    {
        if (!ChainSelect())
        {
            return false;
        }

        bool ret = true;
        if (mChild == null)
        {
            if (obj.SelectSelf())
                mChild = obj;
            else
                ret = false;
        }
        else if (obj != mChild)
        {
            if (mChild.ChainDeselect())
            {
                mChild = null;
                if (obj.SelectSelf())
                    mChild = obj;
                else
                    ret = false;
            }
            else
            {
                ret = false;
            }
        }
        else
        {
            // Currently selected
        }
        return ret;
    }

    private bool SelectSelf()
    {
        bool ret = true;
        if (OnSelect != null)
            ret = OnSelect();
        if(ret)
            Selected = true;
        return ret;
    }

    private bool DeselectSelf()
    {
        bool ret = true;
        if (OnDeselect != null)
            ret = OnDeselect();
        if (ret)
            Selected = false;
        return ret;
    }
}
