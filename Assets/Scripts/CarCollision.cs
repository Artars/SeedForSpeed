using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCollision : MonoBehaviour
{
    public float repulsion = 5f;
    public float wallDistance = 13f;
    void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.GetContact(0);
        Rigidbody body1 = contact.thisCollider.GetComponent<Rigidbody>();
        Rigidbody body2 = contact.otherCollider.GetComponent<Rigidbody>();
        if (contact.otherCollider.gameObject.tag == "Car"){
            float speed1 = GetComponent<VelocityEstimator>().speed.magnitude;
            float speed2 = contact.otherCollider.GetComponent<VelocityEstimator>().speed.magnitude;

            if (speed1 > speed2){
                PlayerController player2 = contact.otherCollider.GetComponent<PlayerController>();
                if (player2 != null){
                    player2.disableMovement();
                    StartCoroutine(enableBackMovement(0.2f,player2));
                }
                body2.AddForceAtPosition(-contact.normal*(speed1-speed2)*repulsion,contact.point,ForceMode.Impulse);
            }
        }
        else if (contact.otherCollider.gameObject.tag != "Prop"){
            if (Physics.Raycast(transform.position,transform.forward,wallDistance,LayerMask.GetMask("Parede"))) {
                GetComponent<PlayerController>().stop();
            }
            else {
                int reverse = (GetComponent<PlayerController>().isReversed)?-1:1;
                float angle = Vector3.Angle(reverse*transform.forward,-contact.normal);
                Vector3 target = Quaternion.AngleAxis(90f, Vector3.up)*(-contact.normal);
                target = target.normalized*Vector3.Dot(reverse*transform.forward, target);
                transform.forward = target.normalized;
            }
        }
    }

    void Update()
    {
        if (Physics.Raycast(transform.position,transform.forward,wallDistance,LayerMask.GetMask("Parede"))){
            GetComponent<PlayerController>().stop();
        }
    }

    IEnumerator enableBackMovement(float time,PlayerController disabled){
        yield return new WaitForSeconds(time);
        disabled.enableMovement();
    }

    // void OnDrawGizmos()
    // {
    //     Gizmos.DrawRay(transform.position,transform.forward*wallDistance);
    // }
}
