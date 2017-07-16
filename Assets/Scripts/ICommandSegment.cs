using System;
using UnityEngine;

public interface ICommandSegment
{
    bool IsFixedLen{ get; }

    int Length { get; }

    int AdjustLength(int diff);

    void Remove();

    Vector2 Start { get; set; }
}