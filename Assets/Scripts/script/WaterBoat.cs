using Lance;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WaterFloat))]
public class WaterBoat : MonoBehaviour
{
    // public visible Properties
    public Transform Motor;
    public float SteerPower = 500f;
    public float Power = 5f;
    public float MaxSpeed = 10f;
    public float Drag = 0.1f;

    // new public variables for camera configuration
    public float CameraHeight = 2f;
    public float CameraDistance = 8f;
    public float CameraLookAheadDistance = 6f;

    // used Components
    protected Rigidbody Rigidbody;
    protected Quaternion StartRotation;
    protected ParticleSystem ParticleSystem;
    protected Camera Camera;

    // internal Properties
    protected Vector3 CamVel;


    public void Awake()
    {
        ParticleSystem = GetComponentInChildren<ParticleSystem>();
        Rigidbody = GetComponent<Rigidbody>();
        StartRotation = Motor.localRotation;
        Camera = Camera.main;
    }

    public void FixedUpdate()
    {
        // default direction
        var steer = 0;

        // steer direction [-1,0,1]
        if (Input.GetKey(KeyCode.A))
            steer = 1;
        if (Input.GetKey(KeyCode.D))
            steer = -1;

        // Rotational Force
        Rigidbody.AddForceAtPosition(steer * transform.right * SteerPower / 100f, Motor.position);

        // compute vectors
        var forward = Vector3.Scale(new Vector3(1, 0, 1), transform.forward);

        // forward/backward power
        if (Input.GetKey(KeyCode.W))
            PhysicsHelper.ApplyForceToReachVelocity(Rigidbody, forward * MaxSpeed, Power);
        if (Input.GetKey(KeyCode.S))
            PhysicsHelper.ApplyForceToReachVelocity(Rigidbody, forward * -MaxSpeed, Power);

        // Motor Animation // Particle system
        Motor.SetPositionAndRotation(Motor.position, transform.rotation * StartRotation * Quaternion.Euler(0, 30f * steer, 0));
        if (ParticleSystem != null)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
                ParticleSystem.Play();
            else
                ParticleSystem.Stop();
        }

        // Determine if the boat is moving forward using a dot product
        bool movingForward = Vector3.Dot(transform.forward, Rigidbody.velocity) >= 0;
        // Set target direction based on current movement (forward or backward)
        Vector3 targetDirection = movingForward ? transform.forward : -transform.forward;
        // Calculate the angle between the current velocity and the target direction
        float angle = Vector3.SignedAngle(Rigidbody.velocity, targetDirection, Vector3.up);
        // Rotate the velocity vector gradually toward the target direction based on the Drag factor
        Rigidbody.velocity = Quaternion.AngleAxis(angle * Drag, Vector3.up) * Rigidbody.velocity;

        // --- Angular Velocity Damping ---
        // When no steering input is given, gradually reduce the angular velocity
        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            float angularDampFactor = 5f; // Adjust this factor to control damping strength
            Rigidbody.angularVelocity = Vector3.Lerp(Rigidbody.angularVelocity, Vector3.zero, Time.fixedDeltaTime * angularDampFactor);
        }

        //camera position with adjustable height and distance
        Camera.transform.LookAt(transform.position + transform.forward * CameraLookAheadDistance + transform.up * CameraHeight);
        Camera.transform.position = Vector3.SmoothDamp(Camera.transform.position, transform.position + transform.forward * -CameraDistance + transform.up * CameraHeight, ref CamVel, 0.05f);
    }
}
