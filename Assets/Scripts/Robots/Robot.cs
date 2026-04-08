using Unity.Cinemachine;
using UnityEngine;
using static Unity.Cinemachine.CinemachineImpulseManager.ImpulseEvent;

public abstract class Robot : MonoBehaviour
{
    public CharacterController controller;
    public bool isEnergized;
    public float speed = 5f;
    public float gravity = -8f;
    Vector3 moveDirection = new();

    public float fall;
    public Robot other;
    public CinemachineCamera cineCamera;
    Transform lastCameraLook = null;

    public void Change()
    {
        lastCameraLook = cineCamera.transform;

        isEnergized = false;
        moveDirection = Vector3.zero;
        cineCamera.enabled = false;

        other.cineCamera.enabled = true;
        Invoke("EnergyOther", Camera.main.GetComponent<CinemachineBrain>().DefaultBlend.Time);
        if(other.lastCameraLook != null)
            other.cineCamera.ForceCameraPosition(other.lastCameraLook.position, other.lastCameraLook.rotation);
    }

    void EnergyOther()
    {
        other.isEnergized = true;
    }

    protected void Update()
    {
        if (isEnergized)
        {
            Transform camera = Camera.main.transform;

            Vector3 forward = camera.forward;
            forward.y = 0;
            forward.Normalize();

            Vector3 moveVector = (forward * moveDirection.z + camera.right * moveDirection.x) * Time.deltaTime * speed;

            if (controller.isGrounded)
                fall = 0;
            else
                fall += gravity * Time.deltaTime;
            moveVector.y = fall;

            controller.Move(moveVector);

            transform.rotation = Quaternion.LookRotation(forward);
        }
    }

    public void Move(Vector2 input)
    {
        if (isEnergized)
        {
            moveDirection = new Vector3(input.x, 0, input.y);
        }
    }


    public abstract void TakeAction();
    public abstract void CancelAction();
}
