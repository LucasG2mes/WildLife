using UnityEngine;
using UnityEngine.InputSystem;

public class RobotManager : MonoBehaviour
{
    public enum RobotType { SMALL, BIG}
    public Robot small;
    public Robot big;

    Robot controlledRobot;
    RobotType robot = RobotType.SMALL;

    private void Start()
    {
        controlledRobot = small;    
    }

    public void OnChange(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            controlledRobot.Change();

            robot = robot == RobotType.SMALL ? RobotType.BIG : RobotType.SMALL;
            controlledRobot = robot == RobotType.SMALL ? small : big;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        controlledRobot.Move(context.ReadValue<Vector2>());
    }
}
