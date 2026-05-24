using UnityEngine;

[RequireComponent(typeof(Animation))]
public class BobAndSpin : MonoBehaviour
{
    [HideInInspector]
    public float bob;

    [HideInInspector]
    public float spin;

    /// <summary>
    /// The target transform to apply the animation to.
    /// </summary>
    public Transform target;

    /// <summary>
    /// The animation to play on the target.
    /// </summary>
    private new Animation animation;

    // Start is called before the first frame update
    void Start()
    {
        animation = GetComponent<Animation>();
        if (animation)
        {
            animation.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        var position = target.localPosition;
        position.y = bob;
        target.localPosition = position;

        var rotation = target.localEulerAngles;
        rotation.y = spin;
        target.localEulerAngles = rotation;
    }
}
