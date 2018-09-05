using UnityEngine;

public class InputManager
{
    public static bool getInputKeyDown_Right()
    {
        return Input.GetKeyDown(KeyCode.RightArrow)||Input.GetKeyDown(KeyCode.D);
    }
    public static bool getInputKeyDown_Left()
    {
        return Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A);
    }
    public static bool getInputKeyDown_Up()
    {
        return Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W);
    }
    public static bool getInputKeyDown_Down()
    {
        return Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.W);
    }
    public static bool getInputKey_Right()
    {
        return Input.GetKey(KeyCode.RightArrow)||Input.GetKey(KeyCode.D);
    }
    public static bool getInputKey_Left()
    {
        return Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
    }
    public static bool getInputKey_Up()
    {
        return Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);
    }
    public static bool getInputKey_Down()
    {
        return Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.W);
    }
    public static bool getInputKey_Interact()
    {
        return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }
}