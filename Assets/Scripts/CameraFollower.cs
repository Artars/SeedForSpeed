using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform toFollow;
    public Vector3 offsetOnX = new Vector3(0,30,-1);
    public Vector3 offsetOnZ = new Vector3(0,30,-8);
    public float speedZoomOut = 10f;
    public float speedOffsetX = -20f;
    public float speedOffsetZ = -8f;

    PlayerController followedPlayer;
    Vector3 usedOffset;
    float usedZoomOut = 0f;
    float usedSpeedDrag = 0f;
    float speedDrag;
    float zoomOut;
    bool isGoingX = true;

    public void setTarget(Transform newTarget){
        if (toFollow != newTarget) toFollow = newTarget;
        followedPlayer = toFollow.GetComponent<PlayerController>();
    }

    void Start()
    {
        // SeedManager.instance.debugStartGame(debugNumCars);
        if (toFollow != null) followedPlayer = toFollow.GetComponent<PlayerController>();
        usedOffset = offsetOnX;
    }

    // Update is called once per frame
    void Update()
    {
        if (followedPlayer != null){
            Vector3 nextPosition;
            isGoingX = followedPlayer.speed > followedPlayer.reversedMaxSpeed;
            isGoingX = isGoingX && Mathf.Abs(followedPlayer.transform.forward.x) > Mathf.Abs(followedPlayer.transform.forward.z);
            if (isGoingX){
                speedDrag = speedOffsetX*followedPlayer.speed/followedPlayer.maxSpeed;
                usedSpeedDrag = Mathf.Lerp(usedSpeedDrag,speedDrag,0.02f);
                zoomOut = speedZoomOut*followedPlayer.speed/followedPlayer.maxSpeed;
                nextPosition = toFollow.position+(speedDrag*toFollow.forward);
                usedZoomOut = Mathf.Lerp(usedZoomOut,zoomOut,0.1f);
                usedOffset = Vector3.Lerp(usedOffset,offsetOnX,0.01f);
            }
            else{
                speedDrag = speedOffsetZ*followedPlayer.speed/followedPlayer.maxSpeed;
                usedSpeedDrag = Mathf.Lerp(usedSpeedDrag,speedDrag,0.02f);
                zoomOut = speedZoomOut*followedPlayer.speed/followedPlayer.maxSpeed;
                usedZoomOut = Mathf.Lerp(usedZoomOut,zoomOut,0.1f);
                usedOffset = Vector3.Lerp(usedOffset,offsetOnZ,0.01f);
            }
            nextPosition = toFollow.position+(usedSpeedDrag*toFollow.forward);
            nextPosition += usedOffset;
            nextPosition -= usedZoomOut*transform.forward;
            transform.position = nextPosition;
        }
        else if (toFollow != null){
            transform.position = offsetOnZ+toFollow.position;
        }
    }
}
