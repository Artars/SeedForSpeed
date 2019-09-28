using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCollision : MonoBehaviour
{
    public float repulsion = 5f;
    void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.GetContact(0);
        if (contact.otherCollider.gameObject.tag == "Car"){
            Rigidbody body1 = contact.thisCollider.GetComponent<Rigidbody>();
            Rigidbody body2 = contact.otherCollider.GetComponent<Rigidbody>();
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
    }

    IEnumerator enableBackMovement(float time,PlayerController disabled){
        yield return new WaitForSeconds(time);
        disabled.enableMovement();
    }
}
