using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ClickType
{
    None,
    RightClick, LeftClick, MiddleClick,
    DoubleRightClick, DoubleLeftClick, DoubleMiddleClick,
    RightClickHold, LeftClickHold, MiddleClickHold
};

public delegate void ClickDel(ClickType t);

public interface IClickable
{
    void Click(ClickType t);

}
