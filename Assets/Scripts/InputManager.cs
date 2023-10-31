using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one InputManager" + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public Vector2 GetMoveDirectionVector()
    {
        Vector2 inputMoveDirection = Vector2.zero;

        if (Input.GetKey(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            inputMoveDirection = Vector2.up;
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            inputMoveDirection = Vector2.down;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            inputMoveDirection = Vector2.left;
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            inputMoveDirection = Vector2.right;
        }

        return inputMoveDirection;

    }
}
