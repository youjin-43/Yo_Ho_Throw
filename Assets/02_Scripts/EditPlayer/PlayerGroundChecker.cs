using System.Collections;
using UnityEngine;

public class PlayerGroundChecker : MonoBehaviour
{
    public LayerMask groundLayerMask;
    public LayerMask otherLayerMask;
    public bool IsGrounded => isGrounded;
    public bool IsColliderBelow => isColliderBelow;

    [SerializeField] float groundRayRange;
    [SerializeField] float colliderBelowRange;

    [SerializeField] float minGlideTime = 0.5f;

    float glideTime = 0f;

    bool isTriggered = false;
    bool isGrounded = true;
    bool wasGrounded = true;
    bool isColliderBelow = false;

    Ray ray = new Ray();

    RaycastHit hit;

    private void Start()
    {
        ray.direction = Vector3.down;

        StartCoroutine(UpdateGlideTimeCoroutine());
    }
    private void Update()
    {
        ray.origin = transform.position;

        isGrounded = isTriggered ? false : Physics.Raycast(ray, groundRayRange, groundLayerMask);

        isColliderBelow = Physics.Raycast(ray, out hit, colliderBelowRange, otherLayerMask);

        if (!isGrounded)
        {
            ray.origin = transform.position + (Vector3.up * 0.5f);

            if (Physics.Raycast(ray, out hit, 0.5f + groundRayRange, groundLayerMask))
            {
                if (transform.position.y - hit.point.y <= groundRayRange)
                {
                    isGrounded = true;
                }
            }
        }

        if (isGrounded != wasGrounded)
        {
            wasGrounded = isGrounded;

            if (wasGrounded)
            {
                if (glideTime > minGlideTime)
                {
                    //PlayerAudioController.PlayerAudioPlay(AudioName.PlayerLand);

                    glideTime = 0f;
                }
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        Debug.Log("Trigger OBJ : " + other.gameObject.name);

        isTriggered = true;
    }
    private void OnTriggerExit(Collider other)
    {
        isTriggered = false;
    }
    IEnumerator UpdateGlideTimeCoroutine()
    {
        while (true)
        {
            if (!wasGrounded)
            {
                glideTime += Time.deltaTime;
            }
            else
            {
                glideTime = 0f;
            }

            yield return null;
        }
    }
}
