using System;
using System.Collections;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Variables
    public float moveSpeed;
    public int moveRange;
    
    private bool isMoving;
    private Vector2 movement; // Stores player input
    private Animator animator;

    public event Action OnEncountered;

    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    public void RunUpdate()
    {
        // Movement
        if (!isMoving)
        {
            // Capture input from the player
            movement.x = Input.GetAxisRaw("Horizontal") * moveRange;
            movement.y = Input.GetAxisRaw("Vertical") * moveRange;
            

            if (movement != Vector2.zero)
            {
                // Set Animation
                animator.SetFloat("moveX", movement.x);
                animator.SetFloat("moveY", movement.y);

                // Get Target Position
                var targetPosition = transform.position;
                targetPosition.x += movement.x;
                targetPosition.y += movement.y;       
                
                StartCoroutine(MoveToPosition(targetPosition));

            }
        }

        // Animate
        animator.SetBool("isMoving", isMoving);

        if (Input.GetKeyDown(KeyCode.B))
        {
            OnEncountered();
        }
    }

    // Move to Position
    IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        isMoving = true;
        // Check if the player is not at the target position
        while ((targetPosition - transform.position).sqrMagnitude > Mathf.Epsilon) // Mathf.Epsilon is an infinitly small positive number
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // Default Case
        transform.position = targetPosition;
        isMoving = false;
    }
}
