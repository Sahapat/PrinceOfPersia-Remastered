using UnityEngine;

public class InputManager
{
    public static bool getInputRight()
    {
        return Input.GetKey(KeyCode.RightArrow)||Input.GetKey(KeyCode.D);
    }
    public static bool getInputLeft()
    {
        return Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
    }
    public static bool getInputUp()
    {
        return Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);
    }
    public static bool getInputDown()
    {
        return Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.W);
    }
    public static bool getInputInteract()
    {
        return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }
}