using Lance;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(WaterFloat))]
public class WaterBoat : MonoBehaviour
{
    public Transform Motor;
    public float SteerPower = 500f;
    public float Power = 5f;
    public float MaxSpeed = 10f;
    public float Drag = 0.1f;

    public float CameraHeight = 2f;
    public float CameraDistance = 8f;
    public float CameraLookAheadDistance = 6f;

    protected Rigidbody Rigidbody;
    protected Quaternion StartRotation;
    protected ParticleSystem ParticleSystem;
    protected Camera Camera;

    protected Vector3 CamVel;

    private bool playerIsInBoat = false;

    public void Awake()
    {
        ParticleSystem = GetComponentInChildren<ParticleSystem>();
        Rigidbody = GetComponent<Rigidbody>();
        StartRotation = Motor.localRotation;
        Camera = Camera.main;

        // Start with physics disabled
        Rigidbody.isKinematic = true;
    }

    public void EnterBoat()
    {
        playerIsInBoat = true;
        Rigidbody.isKinematic = false;
        Debug.Log("Boat physics enabled (player entered)");
    }

    public void ExitBoat()
    {
        playerIsInBoat = false;
        Rigidbody.isKinematic = true;
        Debug.Log("Boat physics disabled (player exited)");
    }

    public void FixedUpdate()
    {
        if (!playerIsInBoat) return;

        var steer = 0;
        if (Input.GetKey(KeyCode.A)) steer = 1;
        if (Input.GetKey(KeyCode.D)) steer = -1;

        Rigidbody.AddForceAtPosition(steer * transform.right * SteerPower / 100f, Motor.position);

        var forward = Vector3.Scale(new Vector3(1, 0, 1), transform.forward);

        if (Input.GetKey(KeyCode.W))
            PhysicsHelper.ApplyForceToReachVelocity(Rigidbody, forward * MaxSpeed, Power);
        if (Input.GetKey(KeyCode.S))
            PhysicsHelper.ApplyForceToReachVelocity(Rigidbody, forward * -MaxSpeed, Power);

        Motor.SetPositionAndRotation(Motor.position, transform.rotation * StartRotation * Quaternion.Euler(0, 30f * steer, 0));
        if (ParticleSystem != null)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)) ParticleSystem.Play();
            else ParticleSystem.Stop();
        }

        bool movingForward = Vector3.Dot(transform.forward, Rigidbody.velocity) >= 0;
        Vector3 targetDirection = movingForward ? transform.forward : -transform.forward;
        float angle = Vector3.SignedAngle(Rigidbody.velocity, targetDirection, Vector3.up);
        Rigidbody.velocity = Quaternion.AngleAxis(angle * Drag, Vector3.up) * Rigidbody.velocity;

        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            float angularDampFactor = 5f;
            Rigidbody.angularVelocity = Vector3.Lerp(Rigidbody.angularVelocity, Vector3.zero, Time.fixedDeltaTime * angularDampFactor);
        }

        Camera.transform.LookAt(transform.position + transform.forward * CameraLookAheadDistance + transform.up * CameraHeight);
        Camera.transform.position = Vector3.SmoothDamp(Camera.transform.position, transform.position + transform.forward * -CameraDistance + transform.up * CameraHeight, ref CamVel, 0.05f);
    }
}
