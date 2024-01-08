using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour
{
    public float launchForce = 10f; // Adjust the launch force as needed

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Assuming the player has a Rigidbody2D component
            Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();

            if (playerRb != null)
            {
                // Apply the launch force to the player in the upward direction
                playerRb.velocity = new Vector2(playerRb.velocity.x, launchForce);
            }
        }
    }
}
