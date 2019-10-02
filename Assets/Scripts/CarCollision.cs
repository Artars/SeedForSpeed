using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCollision : MonoBehaviour
{
    public float repulsion = 5f;
    public float wallDistance = 13f;
    public AudioSource crashSoundSource;
    bool onWall = false;
    Vector3 lastTarget;
    ContactPoint contact;
    PlayerController movementControl;
    void OnCollisionEnter(Collision collision)
    {
        bool shouldPlaySound = false;
        contact = collision.GetContact(0);
        Rigidbody body1 = contact.thisCollider.GetComponent<Rigidbody>();
        Rigidbody body2 = contact.otherCollider.GetComponent<Rigidbody>();
        if (Vector3.Angle(Vector3.down,-contact.normal) < 10f) return;
        if (contact.otherCollider.gameObject.tag == "Car"){
            shouldPlaySound = true;
            float speed1 = GetComponent<VelocityEstimator>().speed.magnitude;
            float speed2 = contact.otherCollider.GetComponent<VelocityEstimator>().speed.magnitude;

            if (speed1 > speed2){
                PlayerController player1 = GetComponent<PlayerController>();
                PlayerController player2 = contact.otherCollider.GetComponent<PlayerController>();
                if (player2 != null){
                    player2.disableMovement();
                    StartCoroutine(enableBackMovement(0.2f,player2));
                }
                body2.AddForceAtPosition(-contact.normal*(speed1-speed2)*repulsion,contact.point,ForceMode.Impulse);
                player2.seedSteal();
                player1.seedGain(player1.stealCounter);
            }
        }
        else if (contact.otherCollider.gameObject.tag != "Prop"){
            if (Physics.Raycast(transform.position+new Vector3(0f,1.5f,0f),transform.forward,wallDistance,LayerMask.GetMask("Parede"))) {
                shouldPlaySound = true;
                movementControl.GetComponent<PlayerController>().stop();
                if(SeedManager.instance != null) 
                    SeedManager.instance.ScrambleCarPlaces(GetComponent<PlayerController>().id);
            }
            else if (movementControl.isReversed){
                movementControl.GetComponent<PlayerController>().stop();
            }
            else if (!movementControl.isReversed) {
                int reverse = (GetComponent<PlayerController>().isReversed)?-1:1;
                // float angle = Vector3.Angle(reverse*transform.forward,-contact.normal);
                Vector3 target = Quaternion.AngleAxis(90f, Vector3.up)*(-contact.normal);
                target = target.normalized*Vector3.Dot(reverse*transform.forward, target);
                transform.forward = target.normalized;
                movementControl.wallTouching();
                onWall = true;
                lastTarget = target;
            }
        }

        
        if(shouldPlaySound)
        {
            PlaySound();
        }
    }

    void Start() {
        movementControl = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (onWall && Vector3.Angle(transform.forward,lastTarget) > 5f){
            onWall = false;
            movementControl.wallFree();
        }
        int reverseNumber = movementControl.isReversed?-1:1;
        if (Vector3.Angle(reverseNumber*transform.forward,-contact.normal) > 10f) return;
        if (Physics.Raycast(transform.position+new Vector3(0f,1.5f,0f),-transform.forward,wallDistance,LayerMask.GetMask("Parede"))){
            movementControl.stop();
        }
        if (Physics.Raycast(transform.position+new Vector3(0f,1.5f,0f),transform.forward,wallDistance,LayerMask.GetMask("Parede"))){
            movementControl.stop();
        }
    }

    IEnumerator enableBackMovement(float time,PlayerController disabled){
        yield return new WaitForSeconds(time);
        disabled.enableMovement();
    }

    public void PlaySound()
    {
        if(crashSoundSource != null)
        {
            crashSoundSource.Play();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position+new Vector3(0f,1.5f,0f),transform.forward*wallDistance);
        Gizmos.DrawRay(transform.position+new Vector3(0f,1.5f,0f),-transform.forward*wallDistance);
    }
}
