using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    public Transform respawnPoint;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                player.ResetCoinCount();
                RespawnPlayer(player);
            }
        }
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
