using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Robot : MonoBehaviour
{
    public CharacterController controller;
    public bool isEnergized;
    public float speed = 5f;
    Vector3 moveDirection = new();

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
        StartCoroutine(EnergyChange());
        if(other.lastCameraLook != null)
            other.cineCamera.ForceCameraPosition(other.lastCameraLook.position, other.lastCameraLook.rotation);
    }

    IEnumerator EnergyChange()
    {
        yield return new WaitForSeconds(Camera.main.GetComponent<CinemachineBrain>().DefaultBlend.Time);
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

            Vector3 right = camera.right;
            right.y = 0;
            right.Normalize();

            Vector3 moveVector = (forward * moveDirection.z + right * moveDirection.x) * Time.deltaTime * speed;
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


    protected abstract void TakeAction();
    protected abstract void CancelAction();
}
