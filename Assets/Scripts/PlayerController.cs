using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float maxSpeed = 400f;
    public float defaultAcceleration = 8f;
    public float brakeAcceleration = 16f;
    public float frictionAcceleration = 4f;
    public float collisionAcceleration = 800f;
    public float turnAngle = 80f;
    public float driftBuff = 30f;
    public AnimationCurve turnCurve;
    public bool shouldMove = true;
    public bool forDebug = false;
    private bool haveCollided = false;
    int accelerator;
    int brake;
    float turn;
    bool isDrifting = false;
    float speed = 0f;
    Vector3 steering;
    Transform bodyPosition;
    Rigidbody body;

    public void disableMovement(){
        shouldMove = false;
    }

    public void enableMovement(){
        haveCollided = true;
        StartCoroutine(collisionStop());
    }

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
        if (!forDebug){
            if (Input.GetButton("Fire1"))
            {
                accelerator = 1;
            }
            if (Input.GetButton("Fire2"))
            {
                brake = 1;
            }
            turn = Input.GetAxis("Horizontal");
        }

        isDrifting = (accelerator == 1 && brake == 1);
        if (!isDrifting){
            if (!haveCollided){
                if (accelerator == 1) speed += defaultAcceleration;
                if (speed > 0f){
                    if (accelerator == 0) speed -= frictionAcceleration;
                    else if (brake == 1) speed -= brakeAcceleration;
                }
                else if (speed < 0f && (accelerator == 0 || brake == 1)) speed = 0;
                if (speed > maxSpeed) speed = maxSpeed;
                if (speed < -maxSpeed/2) speed = -maxSpeed/2;
            }
            else if (body.velocity.magnitude == 0) {
                haveCollided = false;
                shouldMove = true;
                speed = 0;
            }
        }
    }

    void FixedUpdate()
    {
        if (!shouldMove) return;
        int isDrift = (isDrifting)?1:0;
        steering = new Vector3(0f,turnAngle*turnCurve.Evaluate(speed/maxSpeed)+driftBuff*isDrift,0f);
        Vector3 newPosition = transform.position + speed * bodyPosition.forward * Time.fixedDeltaTime;
        Quaternion deltaRotation = Quaternion.Euler(turn * steering * Time.deltaTime);
        body.MoveRotation(body.rotation * deltaRotation);
        body.MovePosition(newPosition);
    }

    IEnumerator collisionStop(){
        while (haveCollided){
            body.AddForce(-body.velocity.normalized*collisionAcceleration,ForceMode.Acceleration);
            if(body.velocity.magnitude <= 10) {
                haveCollided = false;
                shouldMove = true;
            }
            yield return null;
        }
    }
}
