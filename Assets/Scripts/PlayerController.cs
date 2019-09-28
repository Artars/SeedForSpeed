using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float maxSpeed = 100f;
    // public float maxForce = 1000f;
    public float acceleration = 2f;
    public float frictionAcceleration = 0.5f;
    public float turnAngle = 60f;
    public float driftBuff = 20f;
    public AnimationCurve turnCurve;
    int accelerator;
    int brake;
    float turn;
    public bool isDrifting = false;
    float speed = 0f;
    Vector3 steering;
    Transform bodyPosition;
    Rigidbody body;

    void Start()
    {
        bodyPosition = transform;
        body = GetComponent<Rigidbody>();
    }

    void Update()
    {
        accelerator = 0;
        brake = 0;
        turn = 0;
        if (Input.GetButton("Fire1"))
        {
            accelerator = 1;
        }
        if (Input.GetButton("Fire2"))
        {
            brake = 1;
        }
        turn = Input.GetAxis("Horizontal");

        isDrifting = (accelerator == 1 && brake == 1);
        if (!isDrifting){
            speed += accelerator*acceleration;
            if (speed > 0f){
                if (accelerator == 0) speed -= frictionAcceleration;
                else if (brake == 1) speed -= acceleration;
            }
            else if (speed < 0f && (accelerator == 0 || brake == 1)) speed = 0;
            if (speed > maxSpeed) speed = maxSpeed;
            if (speed < -maxSpeed/2) speed = -maxSpeed/2;
        }
    }

    void FixedUpdate()
    {

        int isDrift = (isDrifting)?1:0;
        steering = new Vector3(0f,turnAngle*turnCurve.Evaluate(speed/maxSpeed)+driftBuff*isDrift,0f);
        Vector3 newPosition = transform.position + speed * bodyPosition.forward * Time.fixedDeltaTime;
        Quaternion deltaRotation = Quaternion.Euler(turn * steering * Time.deltaTime);
        body.MoveRotation(body.rotation * deltaRotation);
        body.MovePosition(newPosition);
    }
}
