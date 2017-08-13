using System;

public interface ISelectable
{
    bool Select();

    bool Deselect();
}

/// <summary>
/// A composite class that acts like a select/deselect chain.
/// </summary>
public class Selector
{
    private String mName;
    private Selector mPar;
    private ISelectable mItem;
    private Selector mChild;

    public Selector(String name) : this(name, null, null)
    { }

    public Selector(String name, Selector parent, ISelectable item)
    {
        mName = name;
        mChild = null;
        mPar = parent;
        mItem = item;
    }

    /// <summary>
    /// Select self from the highest parent downwards.
    /// </summary>
    /// <returns>True if sucessfully selected form highest parent</returns>
    public bool ChainSelect()
    {
        bool ret = true;
        if (mPar != null)
        {
            ret =  mPar.SelectChild(this);
        }
        return ret;
    }

    /// <summary>
    /// Deselect from self to lowest child.
    /// </summary>
    /// <returns>Returns true if successfully deselect from self to lowest child</returns>
    public bool ChainDeselect()
    {
        // Deslect self
        if ((mItem != null) && !mItem.Deselect())
            return false;

        //Deselect Children
        if (mChild == null)
            return true;

        if (!mChild.ChainDeselect())
            return false;

        mChild = null;
        return true;
    }

    /// <summary>
    /// Called to set given selector as selector item.
    /// </summary>
    /// <param name="obj">Selector to select</param>
    /// <returns>True if successfully selected.</returns>
    private bool SelectChild(Selector obj)
    {
        if(!ChainSelect())
        {
            return false;
        }

        bool ret = true;
        if (mChild == null)
        {
            if (obj.SelectItem())
                mChild = obj;
            else
                ret = false;
        }
        else if (obj != mChild)
        {
            if (mChild.ChainDeselect())
            {
                mChild = null;
                if (obj.SelectItem())
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

    /// <summary>
    /// Call item select function if it exists
    /// </summary>
    /// <returns>True if sucessfully selected</returns>
    private bool SelectItem()
    {
        if (mItem == null)
            return true;

        return mItem.Select();
    }
}
