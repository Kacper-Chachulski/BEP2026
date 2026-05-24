using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Script is attached to camera to follow the player
/// </summary>
public class FollowPlayer : MonoBehaviour
{
    public GameObject player;
    Vector3 offset;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        offset = gameObject.transform.position - player.transform.position;
    }
    void FixedUpdate()
    {
        transform.position = player.transform.position + offset;
    }
}
