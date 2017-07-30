using System;

/// <summary>
/// An interface for object's selectable by the GUI
/// </summary>
public interface ISelectable
{
    /// <summary>
    /// Select object, changing GUI. If returns false, nothing is done and object cannot be selected currently.
    /// </summary>
    /// <returns>True if selected, otherwise false</returns>
    bool Select();

    /// <summary>
    /// Deselect object, changing GUI. If returns false, nothing is done and object cannot be deselected currently.
    /// </summary>
    /// <returns>True if deselected, otherwise false</returns>
    bool Deselect();

    /// <summary>
    /// Name of object
    /// </summary>
    /// <returns>name</returns>
    String Name();
}