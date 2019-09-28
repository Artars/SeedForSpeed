using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityEstimator : MonoBehaviour
{
    public float maxStep = 1;
    protected Transform myTransform;
    protected Vector3 lastPosition;
    public Vector3 speed{
        get {return _speed;}
    }
    protected Vector3 _speed;

    // Start is called before the first frame update
    void Start()
    {
        _speed = Vector3.zero;
        myTransform = transform;
        lastPosition = myTransform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 currentPosition = myTransform.position;
        Vector3 difSpeed = currentPosition - lastPosition;
        _speed = difSpeed/Time.deltaTime;
        lastPosition = currentPosition;
    }

    void ResetSpeed(){
        _speed = Vector3.zero;
        lastPosition = myTransform.position;
    }
}
