using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float maxSpeed = 400f;
    public float reversedMaxSpeed = 100f;
    public float defaultAcceleration = 4f;
    public float brakeAcceleration = 6f;
    public float frictionAcceleration = 2f;
    public float collisionAcceleration = 800f;
    public float turnAngle = 80f;
    public float driftBuff = 30f;
    public AnimationCurve turnCurve;
    public bool shouldMove = true;
    public bool forDebug = false;
    private bool haveCollided = false;
    public bool isReversed = false;
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

    public void acceleratorOn(){
        accelerator = 1;
    }

    public void brakeOn(){
        brake = 1;
    }

    public void reverseOn(){
        isReversed = true;
        brake = 1;
    }

    public void stop(){
        accelerator = 0;
        if (speed > 0) speed = 0;
    }

    public void turnLeft(float input){
        turn = -Mathf.Abs(input);
    }
    public void turnRight(float input){
        turn = Mathf.Abs(input);
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
            if (Input.GetButton("Fire1")) acceleratorOn();
            if (Input.GetButton("Fire2")) brakeOn();
            if (Input.GetButtonDown("Fire2") && accelerator == 0 && speed < 10) reverseOn();
            float aux = Input.GetAxis("Horizontal");
            if (aux>0) turnRight(aux);
            else if (aux<0) turnLeft(aux);
            if (brake == 0 && speed > -10) isReversed = false;
        }

        isDrifting = (accelerator == 1 && brake == 1 && speed > 10);
        if (!isDrifting && !isReversed && !haveCollided){
            if (accelerator == 1) speed += defaultAcceleration;
            if (speed > 0f){
                if (brake == 1) speed -= brakeAcceleration;
                else if (accelerator == 0) speed -= frictionAcceleration;
            }
            if (speed < 0f && (accelerator == 0 || brake == 1)) speed = 0;
            if (speed > maxSpeed) speed = maxSpeed;
        }
        else if(isReversed && !haveCollided){
            if (brake == 1) speed -= defaultAcceleration;
            if (speed < 0f){
                if (accelerator == 1) speed += brakeAcceleration;
                else if (brake == 0) speed += frictionAcceleration;
            }
            if (speed > 0f && (brake == 0 || accelerator == 1)) speed = 0;
            if (speed < -reversedMaxSpeed) speed = -reversedMaxSpeed;
        }
    }

    void FixedUpdate()
    {
        if (!shouldMove) return;
        int isDrift = (isDrifting)?1:0;
        steering = new Vector3(0f,turnAngle*turnCurve.Evaluate(Mathf.Abs(speed)/maxSpeed)+driftBuff*isDrift,0f);
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
                isReversed = false;
                speed = 0;
            }
            yield return null;
        }
    }
}
