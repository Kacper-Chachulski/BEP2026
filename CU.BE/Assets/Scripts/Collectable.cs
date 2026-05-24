using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource), typeof(ParticleSystem))]
public class Collectable : MonoBehaviour
{
    /// <summary>
    /// The score that gets added when the player
    /// collides with this game object.
    /// </summary>
    public int Score = 1;

    /// <summary>
    /// An event that is fired when the player collides with the coin.
    /// </summary>
    public UnityEvent PlayerCollided = new();

    /// <summary>
    /// Disable the mesh renderer when the player
    /// collides with this object.
    /// </summary>
    public MeshRenderer MeshRenderer;

    /// <summary>
    /// An audio source that is played when the player collides with this collectable.
    /// </summary>
    //private AudioSource _hitSound;

    /// <summary>
    /// 
    /// </summary>
    private ParticleSystem _hitParticleSystem;


    void Awake()
    {
        _hitParticleSystem = GetComponent<ParticleSystem>();

        if(MeshRenderer == null)
            MeshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    IEnumerator AddScore(int score)
    {
        GameState.Score = Math.Max(0, GameState.Score + score);
        yield return null;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {

            if (Score > 0 || GameState.Score > 0)
            {
                // Only play the particle system if the score will change.
                _hitParticleSystem.Play();
            }
            //if obstacle is penalty
            if (Score == -5)
            {
                //Log obstacle hit event
                GameState.LiveInput("Obstacle");
                GameState.BarrierHits += 1;
            }
            else
            {
                //Log coin hit event
                GameState.LiveInput("Coin");
            }
            // Start a co-routine to add the score 
            StartCoroutine(AddScore(Score));

            // Fire event
            PlayerCollided.Invoke();

            // Hide the coin
            if(MeshRenderer != null)
                MeshRenderer.enabled = false;

            // Destroy (after sound plays)
            Destroy(gameObject, 0.2f);
        }
    }
}
