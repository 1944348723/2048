using UnityEngine;

public enum InputDirection {
    None,
    Up,
    Down,
    Left,
    Right
}

public class KeyboardInputReader : MonoBehaviour
{
    private InputDirection currentDirection = InputDirection.None;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentDirection = InputDirection.Up;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentDirection = InputDirection.Down;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentDirection = InputDirection.Left;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentDirection = InputDirection.Right;
        } else
        {
            currentDirection = InputDirection.None;
        }
    }

    public InputDirection GetCurrentDirection()
    {
        return currentDirection;
    }

    public void SetActive(bool active)
    {
        enabled = active;
        if (!active)
        {
            currentDirection = InputDirection.None;
        }
    }
}
