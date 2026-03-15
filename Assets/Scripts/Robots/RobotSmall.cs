using UnityEngine;
using UnityEngine.InputSystem;

public class RobotSmall:Robot
{
    protected override void TakeAction()
    {
        Debug.Log("Gancho!");
    }

    protected override void CancelAction()
    {
        throw new System.NotImplementedException();
    }
}
