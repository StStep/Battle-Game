using System;

public interface ICommandSegment
{
    bool IsFixedLen{ get; }

    int Length { get; }

    int AdjustLength(int diff);

    void Remove();
}