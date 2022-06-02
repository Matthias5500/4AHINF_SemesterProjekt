
using UnityEngine;

public class FollowPlayer : MonoBehaviour{

    public Transform target;
    
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    private void FixedUpdate(){
        //the position that the camera is trying to reach
        Vector3 desiredPosition = target.position + offset;
        //gives a value between the camera's current position and the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        //Camera follows the player smoothly
        transform.position = smoothedPosition;
    }
}
