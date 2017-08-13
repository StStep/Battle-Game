using System;

// Types
public enum State { None, Moving };

public interface ICommandRef
{
    bool SetState(State st);

    void ResetPath();
}
