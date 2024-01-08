using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 3f; // Speed at which the enemy moves
    public float patrolDistance = 5f; // Distance the enemy will patrol from its starting position

    private bool movingRight = true;
    private Vector3 startPosition;

    // Added variables for player interaction
    private bool canHitPlayer = true;

    public Transform respawnPoint;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        Patrol();
    }

    void Patrol()
    {
        // Move the enemy left or right based on the current direction
        if (movingRight)
        {
            transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
        }

        // Check if the enemy reached the patrol distance, then change direction
        if (Mathf.Abs(transform.position.x - startPosition.x) >= patrolDistance)
        {
            ChangeDirection();
        }
    }

    void ChangeDirection()
    {
        movingRight = !movingRight;

        // Flip the enemy sprite to match the new direction
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && canHitPlayer)
        {
            PlayerController player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                if (player.collectedCoins > 0)
                {
                    // Player has coins, drop them all
                    player.ResetCoinCount();
                }
                else
                {
                    // Player has no coins, respawn the player
                    player.ResetCoinCount();
                    RespawnPlayer(player);
                    player.RespawnAllCoins();
                }

                // Disable hitting the player for a short time
                StartCoroutine(InvincibilityCooldown(2f));
            }
        }
    }

    IEnumerator InvincibilityCooldown(float duration)
    {
        canHitPlayer = false;
        yield return new WaitForSeconds(duration);
        canHitPlayer = true;
    }

    void RespawnPlayer(PlayerController player)
    {
        if (respawnPoint != null)
        {
            player.transform.position = respawnPoint.position;
        }
        else
        {
            player.transform.position = Vector3.zero;
        }
    }
}
