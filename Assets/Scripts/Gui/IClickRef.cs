using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void Click();

public interface IClickRef
{
    void SetLeft(Click del);

    void SetRight(Click del);
}
