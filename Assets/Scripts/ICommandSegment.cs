using System;
using UnityEngine;

public interface ICommandSegment
{
    float Time { get; }

    Vector2 Start { get; }

    Vector2 End { get; }

    Vector2 EndDir { get; }
}