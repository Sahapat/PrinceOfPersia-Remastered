using UnityEngine;

public class InputManager
{
    public static bool GetKeyUp_Right()
    {
        return Input.GetKeyUp(KeyCode.RightArrow)||Input.GetKeyUp(KeyCode.D);
    }
    public static bool GetKeyUp_Left()
    {
        return Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A);
    }
    public static bool GetKeyUp_Up()
    {
        return Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.W);
    }
    public static bool GetKeyUp_Down()
    {
        return Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S);
    }
    public static bool GetKeyDown_Right()
    {
        return Input.GetKeyDown(KeyCode.RightArrow)||Input.GetKeyDown(KeyCode.D);
    }
    public static bool GetKeyDown_Left()
    {
        return Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A);
    }
    public static bool GetKeyDown_Up()
    {
        return Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W);
    }
    public static bool GetKeyDown_Down()
    {
        return Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S);
    }
    public static bool GetKey_Right()
    {
        return Input.GetKey(KeyCode.RightArrow)||Input.GetKey(KeyCode.D);
    }
    public static bool GetKey_Left()
    {
        return Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
    }
    public static bool GetKey_Up()
    {
        return Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);
    }
    public static bool GetKey_Down()
    {
        return Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
    }
    public static bool GetKey_Interact()
    {
        return Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }
    public static bool GetKeyDown_Interact()
    {
        return Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift);
    }
    public static bool GetKeyUp_Interact()
    {
        return Input.GetKeyUp(KeyCode.Z) || Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift);
    }
}