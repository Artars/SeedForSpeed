using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform toFollow;
    public Vector3 offsetOnX = new Vector3(0,30,-12);
    public Vector3 offsetOnZ = new Vector3(0,30,-15);
    public float speedOffset = -8f;

    PlayerController followedPlayer;
    Vector3 usedOffset;
    float speedDrag;
    bool isGoingX = true;
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
            usedOffset = Vector3.Lerp(usedOffset,offsetOnZ,0.05f);
        }
        nextPosition += usedOffset;
        transform.position = nextPosition;
    }
}
