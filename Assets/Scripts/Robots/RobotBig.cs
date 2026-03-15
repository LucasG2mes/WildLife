using UnityEngine;
using UnityEngine.InputSystem;

public class RobotBig:Robot
{
    private void Awake()
    {
        if (isEnergized == false && other.isEnergized == false)
        {
            Change();
        }
    }
    protected override void TakeAction()
    {
        Debug.Log("Carregar!");
    }

    protected override void CancelAction()
    {
        throw new System.NotImplementedException();
    }
}
