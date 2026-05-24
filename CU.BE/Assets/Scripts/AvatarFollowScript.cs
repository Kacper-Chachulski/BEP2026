using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is used for the Avatar Camera on the right bottom side of the scene, in order for it to follow the player
/// The code ensures the head of the avatar is always in view
/// </summary>
public class AvatarFollowScript : MonoBehaviour
{
    public GameObject head;
    PlayerController playerController;
    Vector3 offset;

    private void Start()
    {
        //determine offset between camera and head of player
        offset = gameObject.transform.position - head.transform.position;
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }
    void Update()
    {
        //follow players head position, except when rolling, in that case disregard the y position, so the camera does not roll with the avatar
        if (playerController.rolling)
        {
            transform.position = new Vector3(head.transform.position.x + offset.x, transform.position.y, head.transform.position.z + offset.z);
        }
        else
        {
            transform.position = head.transform.position + offset;
        }
    }
}
