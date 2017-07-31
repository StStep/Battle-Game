using System;

public interface ISelectable
{
    bool Select();

    bool Deselect();
}

public class Selector
{
    public delegate bool Sel();

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

    public bool ParentSelect()
    {
        bool ret = true;
        if (mPar != null)
        {
            ret =  mPar.SelectChild(this);
        }
        return ret;
    }

    public bool SelectChild(Selector obj)
    {
        if(mPar != null)
        {
            if (!mPar.ParentSelect())
                return false;
        }

        bool ret = true;
        if (mChild == null)
        {
            if (obj.Select())
                mChild = obj;
            else
                ret = false;
        }
        else if (obj != mChild)
        {
            if (mChild.Deselect())
            {
                mChild = null;
                if (obj.Select())
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

    public bool Select()
    {
        if (mItem == null)
            return true;

        return mItem.Select();
    }

    public bool Deselect()
    {
        // Deslect self
        if ((mItem != null) && !mItem.Deselect())
            return false;

        //Deselect Children
        if (mChild == null)
            return true;

        if (!mChild.Deselect())
            return false;

        mChild = null;
        return true;
    }
}
