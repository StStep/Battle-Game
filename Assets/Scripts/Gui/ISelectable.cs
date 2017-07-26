using System;

public interface ISelectable
{
    void Select();

    void Deselect();

    String Name();
}