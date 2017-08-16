using System;

// Types
public enum State { None, Moving };

public interface ICommandRef
{
    bool SetMoving();

    void ResetPath();
}
