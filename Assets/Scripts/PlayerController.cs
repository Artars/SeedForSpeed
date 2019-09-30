using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int id;
    public float maxSpeed = 30f;
    public float reversedMaxSpeed = 15f;
    public float defaultAcceleration = 0.2f;
    public float brakeAcceleration = 0.4f;
    public float frictionAcceleration = 0.05f;
    public float collisionAcceleration = 40f;
    public float turnAngle = 120f;
    public float driftBuff = 60f;
    public float wallDebuff = 0.5f;
    public AnimationCurve turnCurve;
    public bool shouldMove = true;
    public bool forDebug = false;
    public bool gamepadInput = true;
    private bool haveCollided = false;
    public bool isReversed = false;
    public bool loser = false;
    public int seedCounter = 100;
    public int seedDrain = 1;
    public int stealCounter = 20;
    int accelerator;
    int brake;
    float turn;
    bool isDrifting = false;
    bool isOnWall = false;
    public float speed = 0f;

    [Header("Sounds")]
    public AudioSource slideSource;

    [Header("References")]
    public Animator animator;
    public MeshRenderer carRenderer;
    public int materialIndex = 0;
    public Transform[] cuckatielPositions;

    Vector3 steering;
    Transform bodyPosition;
    Rigidbody body;

    float toRight = 0;
    float toLeft = 0;

    public Color currentColor = Color.blue;

    public void acceleratorOn(){
        accelerator = 1;
    }

    void brakeOn(){
        brake = 1;
    }

    void reverseOn(){
        isReversed = true;
        brake = 1;
    }

    void turnLeft(float input){
        turn += -Mathf.Abs(input);
    }
    void turnRight(float input){
        turn += Mathf.Abs(input);
    }

    public void inputReceiveAccel(bool accel){
        accelerator = 0;
        if (accel) acceleratorOn();
    }

    public void inputReceiveBrak(bool brak){
        bool accel = accelerator == 1;
        brake = 0;
        if (brak) brakeOn();
        if (brak && !accel && speed < 0.1) reverseOn();
        if (!brak && speed > -0.1) isReversed = false;
    }

    public void inputReceiveRight(bool right){
        turn = 0;
        toRight = right ? 1:0;
        
        float direction = toRight - toLeft;
        if(direction > 0)
            turnRight(1f);
        else if(direction < 0)
            turnLeft(1f);
    }

    public void inputReceiveLeft(bool left){
        turn = 0;
        toLeft = left ? 1:0;
        
        float direction = toRight - toLeft;
        if(direction > 0)
            turnRight(1f);
        else if(direction < 0)
            turnLeft(1f);

    }

    public void disableMovement(){
        shouldMove = false;
    }

    public void enableMovement(){
        haveCollided = true;
        StartCoroutine(collisionStop());
    }

    public void stop(){
        accelerator = 0;
        if (Mathf.Abs(speed) > 0) speed = 0;
        Debug.Log("Teste");
    }

    public void wallTouching(){
        isOnWall = true;
        speed = speed*wallDebuff;
    }

    public void wallFree(){
        isOnWall = false;
    }

    public void seedGain(int x){
        seedCounter += x;
    }

    public void seedDrainSet(int x){
        seedDrain = x;
    }

    public void seedSteal (){
        seedCounter -= stealCounter;
    }

    void seedDepletion(){
        seedCounter -= seedDrain;
    }

    void Start()
    {
        bodyPosition = transform;
        body = GetComponent<Rigidbody>();
        InvokeRepeating("seedDepletion",1f,1f);
    }

    void Update()
    {
        if(loser) return;
        if (!forDebug && gamepadInput){
            accelerator = 0;
            brake = 0;
            turn = 0;
            if (Input.GetButton("Fire1")) acceleratorOn();
            if (Input.GetButton("Fire2")) brakeOn();
            if (Input.GetButton("Fire2") && accelerator == 0 && speed < 0.1) reverseOn();
            // if (Input.GetKey(KeyCode.W)) acceleratorOn();
            // if (Input.GetKey(KeyCode.S)) brakeOn();
            // if (Input.GetKey(KeyCode.S) && accelerator == 0 && speed < 0.1) reverseOn();
            float aux = Input.GetAxis("Horizontal");
            if (aux>0) turnRight(aux);
            else if (aux<0) turnLeft(aux);
            if (brake == 0 && speed > -0.1) isReversed = false;
        }

        isDrifting = (accelerator == 1 && brake == 1 && speed > 0.1);
        if (!isDrifting && !isReversed && !haveCollided){
            if (accelerator == 1) speed += defaultAcceleration;
            if (speed > 0f){
                if (brake == 1) speed -= brakeAcceleration;
                else if (accelerator == 0) speed -= frictionAcceleration;
            }
            if (speed < 0f && (accelerator == 0 || brake == 1)) speed = 0;
            if (!isOnWall && speed > maxSpeed) speed = maxSpeed;
            else if (isOnWall && speed > maxSpeed*wallDebuff) speed = maxSpeed*wallDebuff;
        }
        else if(isReversed && !haveCollided){
            if (brake == 1) speed -= defaultAcceleration;
            if (speed < 0f){
                if (accelerator == 1) speed += brakeAcceleration;
                else if (brake == 0) speed += frictionAcceleration;
            }
            if (speed > 0f && (brake == 0 || accelerator == 1)) speed = 0;
            if (!isOnWall && speed < -reversedMaxSpeed) speed = -reversedMaxSpeed;
            else if (isOnWall && speed < -reversedMaxSpeed*wallDebuff) speed = -reversedMaxSpeed*wallDebuff;
        }
        if (seedCounter <= 0){
            loser = true;
            SeedManager.instance.RemoveCar(id);
        }

        if(isDrifting && !haveCollided && !isReversed)
        {
            if(!slideSource.isPlaying)
            {
                slideSource.Play();
            }
        }
        else
        {
            if(slideSource.isPlaying)
            {
                slideSource.Stop();
            }
        }

        if(animator != null)
        {
            animator.SetBool("Drifting", isDrifting && !isReversed);
            animator.SetFloat("Steer", steering.y);
        }
    }

    void FixedUpdate()
    {
        if(loser) return;
        if (!shouldMove) return;
        int isDrift = (isDrifting)?1:0;
        steering = new Vector3(0f,turnAngle*turnCurve.Evaluate(Mathf.Abs(speed)/maxSpeed)+driftBuff*isDrift,0f);
        Vector3 newPosition = transform.position + speed * bodyPosition.forward * Time.fixedDeltaTime;
        Quaternion deltaRotation = Quaternion.Euler(turn * steering * Time.deltaTime);
        body.MoveRotation(body.rotation * deltaRotation);
        body.MovePosition(newPosition);
    }

    void OnBecameInvisible() {
        loser = true;
        SeedManager.instance.RemoveCar(id);
    }
    IEnumerator collisionStop(){
        while (haveCollided){
            body.AddForce(-body.velocity.normalized*collisionAcceleration,ForceMode.Acceleration);
            if(body.velocity.magnitude <= 0.1) {
                haveCollided = false;
                shouldMove = true;
                isReversed = false;
                speed = 0;
            }
            yield return null;
        }
    }

    public void SetCarColor(Color newColor)
    {
        currentColor = newColor;
        if(carRenderer != null)
        {
            carRenderer.materials[materialIndex].color = currentColor;
        }
    }

    public void ReactBump()
    {
        if(animator != null)
        {
            animator.SetTrigger("Bump");
        }
    }
}
