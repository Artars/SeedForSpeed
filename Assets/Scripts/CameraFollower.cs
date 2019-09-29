using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isDebug = false;
    private int debugNumCars = 3;

    public Transform toFollow;
    public Vector3 offsetOnX = new Vector3(0,200,-75);
    public Vector3 offsetOnZ = new Vector3(0,200,-120);
    public float speedOffset = -90f;

    PlayerController followedPlayer;
    Vector3 usedOffset;
    float speedDrag;
    float highDistance = 50f;
    bool isGoingX = true;
    bool lastDirectionX = false;
    void Start()
    {
        // SeedManager.instance.debugStartGame(debugNumCars);
        followedPlayer = toFollow.GetComponent<PlayerController>();
        usedOffset = offsetOnX;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 nextPosition;
        isGoingX = followedPlayer.speed > followedPlayer.reversedMaxSpeed;
        isGoingX = isGoingX && Mathf.Abs(followedPlayer.transform.forward.x) > Mathf.Abs(followedPlayer.transform.forward.z);
        if (isGoingX){
            speedDrag = speedOffset*followedPlayer.speed/followedPlayer.maxSpeed;
            nextPosition = toFollow.position+(speedDrag*toFollow.forward);
            usedOffset = Vector3.Lerp(usedOffset,offsetOnX,0.1f);
        }
        else{
            speedDrag = speedOffset*followedPlayer.speed/followedPlayer.maxSpeed;
            nextPosition = toFollow.position+(speedDrag*toFollow.forward);
            usedOffset = Vector3.Lerp(usedOffset,offsetOnZ,0.1f);
        }
        nextPosition += usedOffset;
        transform.position = nextPosition;
    }
}
