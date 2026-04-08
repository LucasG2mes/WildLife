using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;

public class RobotSmall:Robot
{
    public enum State
    {
        Idle,
        Shooting,
        Retracting,
        PullObject,
        PullSelf,
        Inertial
    }
    public State currentState = State.Idle;


    public Transform raycastOffset;
    public MagnetHook magnet;
    

    Vector3 magnetStart;
    Vector3 target;
    Vector3 directionFall;
    float selfGravity;
    private void Start()
    {
        selfGravity = gravity;
        magnetStart = magnet.gameObject.transform.localPosition;
    }

    void SearchTarget()
    {
        Ray rayTarget = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        Vector3 vectorOffset = raycastOffset.position - rayTarget.origin;

        float offset = Vector3.Dot(vectorOffset, rayTarget.direction);
        rayTarget.origin = rayTarget.origin + (Camera.main.transform.forward * offset);

        if (Physics.Raycast(rayTarget, out RaycastHit hit, magnet.maxDistance))
        {
            target = hit.point;
            magnet.ShootMagnet();
            currentState = State.Shooting;
        }
    }

    void Shoot()
    {
        Vector3 direction = (target - magnet.gameObject.transform.position).normalized;

        magnet.gameObject.GetComponent<Rigidbody>().linearVelocity = direction * magnet.magnetSpeed;

        float distanceToPlayer = Vector3.Distance(magnet.gameObject.transform.localPosition, magnetStart);
        if (magnet.hasHooked())
        {
            currentState = State.PullObject;
        }
        else if (distanceToPlayer >= Vector3.Distance(target, magnetStart) || magnet.hit)
        {
            currentState = State.Retracting;
        }
    }

    void PullObject()
    {
        float distanceToPlayer = Vector3.Distance(magnet.gameObject.transform.localPosition, magnetStart);
        if (magnet.hasHooked()) { }
            if (magnet.pullself)
                currentState = State.PullSelf;

        if (distanceToPlayer < 1.5f)
            currentState = State.Retracting;
    }

    void PullSelf()
    {
        gravity =  0;
        float distanceToHook = Vector3.Distance(magnetStart, magnet.gameObject.transform.localPosition);
        if (distanceToHook > 0.8f)
        {
            Vector3 direction = (magnet.gameObject.transform.position - transform.position).normalized;
            Vector3 move = direction * magnet.playerPullSpeed * Time.deltaTime;
            directionFall = direction;
            controller.Move(move);
        }
        else
        {
            currentState = State.Retracting;
        }
    }

    void InertialMove()
    {
        gravity = selfGravity;
        if (!controller.isGrounded)
        {
            Vector3 move = directionFall * magnet.playerPullSpeed * Time.deltaTime;
            controller.Move(move);
        }
        else
        {
            currentState = State.Idle;
        }
    }

    void Retract()
    {
        gravity = selfGravity;
        magnet.ReleaseHooked();
        if (Vector3.Distance(magnet.gameObject.transform.localPosition, magnetStart) < 0.1f)
        {
            currentState = State.Idle;
        }
    }

    public override void TakeAction()
    {
        if(currentState == State.Idle || currentState == State.Retracting)
        {
            SearchTarget();
        }

    }

    public override void CancelAction()
    {
        if (currentState == State.PullSelf)
        {
            magnet.ReleaseHooked();
            currentState = State.Inertial;
        }
        else if(currentState != State.Inertial)
        {
            Debug.Log("é pra soltar");
            currentState = State.Retracting;
        }
    }

    new private void Update()
    {
        base.Update();

        switch (currentState)
        {
            case State.Shooting:
                Shoot();
                break;
            case State.PullObject:
                PullObject();
                break;
            case State.PullSelf:
                PullSelf();
                break;
            case State.Inertial:
                InertialMove();
                break;
            case State.Retracting:
                Retract();
                break;
            case State.Idle:
                break;
            default:
                CancelAction();
                break;
        }

    }
}
