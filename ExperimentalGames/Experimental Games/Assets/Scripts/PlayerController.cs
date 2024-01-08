using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Parameters")]
    public float baseMoveSpeed = 0.1f;
    public float acceleration = 0.1f;
    public float baseJumpForce = 10f; // Initial jump force

    [Header("Coin Parameters")]
    public int maxCoins = 100;
    public TextMeshProUGUI coinCountText;

    [Header("Collect/Drop Coin Parameters")]
    public float maxMoveSpeed = 13f; // Initial maximum speed
    public float speedChangePerCoin = 1f; // Change in max speed per collected/dropped coin
    public float jumpChangePerCoin = 1f; // Change in jump force per collected/dropped coin

    [HideInInspector]
    public float displayJumpForce = 6f; // This variable is for display in the inspector only

    [HideInInspector]
    [SerializeField]
    private float currentJumpForce; // Serialized field for debugging

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool canJump = true; // Flag to check if the player can jump
    public Transform groundCheck;
    public LayerMask groundLayer;

    private float inputDirection;
    public float currentMoveSpeed;

    public int collectedCoins = 0;

    private GameObject[] coins;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentMoveSpeed = baseMoveSpeed;
        UpdateCoinCountUI();
        coins = GameObject.FindGameObjectsWithTag("Coin");
    }

    void Update()
    {
        // Check if the player is grounded
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

        // Get input from the horizontal axis
        float horizontalInput = Input.GetAxis("Horizontal");

        // If changing direction, change instantly
        if (Mathf.Sign(horizontalInput) != Mathf.Sign(rb.velocity.x))
        {
            currentMoveSpeed = baseMoveSpeed * horizontalInput;
        }
        else
        {
            // Accelerate or decelerate the player's movement based on input
            currentMoveSpeed = Mathf.MoveTowards(rb.velocity.x, maxMoveSpeed * horizontalInput, acceleration * Time.deltaTime);
        }

        // Move the player
        rb.velocity = new Vector2(currentMoveSpeed, rb.velocity.y);

        // Check for jump input
        if (isGrounded && Input.GetButtonDown("Jump") && canJump)
        {
            // Apply a vertical force to make the player jump
            rb.velocity = new Vector2(rb.velocity.x, CalculateJumpForce());
            canJump = false; // Disable jumping until the player lands
        }

        // Check for coin collection input
        if (Input.GetKeyDown(KeyCode.E) && collectedCoins > 0)
        {
            DropCoins();

        }

    }

    float CalculateJumpForce()
    {
        // Calculate the jump force based on the base jump force and the change per collected coin
        float modifiedJumpForce = Mathf.Max(baseJumpForce - collectedCoins * jumpChangePerCoin, displayJumpForce);

        // Cap the jump force to a reasonable maximum value
        float finalJumpForce = Mathf.Min(modifiedJumpForce, 10f);

        // Update the serialized variable for debugging
        currentJumpForce = finalJumpForce;

        return finalJumpForce;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coin"))
        {
            CollectCoin(other.gameObject);
        }

        // Check for death zone collision
        if (other.CompareTag("DeathZone"))
        {
            ResetCoinCount();
            RespawnAllCoins();
        }

        if (other.CompareTag("Enemy"))
        {
            // Destroy the enemy when the player collides with the enemy's attack point
            Destroy(other.transform.parent.gameObject);
        }
    }

    public void DropCoins()
    {
        // Drop one coin at a time
        collectedCoins--;

        // Increase maximum speed and jump force based on the number of coins dropped
        maxMoveSpeed += speedChangePerCoin;

        // Adjust jump force when a coin is dropped
        displayJumpForce += jumpChangePerCoin;

        // Cap the jump force to a reasonable maximum value
        displayJumpForce = Mathf.Min(displayJumpForce, 20f);

        // Cap the speed to maxMoveSpeed
        currentMoveSpeed = Mathf.Clamp(currentMoveSpeed, baseMoveSpeed, maxMoveSpeed);

        // Update UI
        UpdateCoinCountUI();
    }

    public void RespawnAllCoins()
    {
        // Delay the respawn to the next frame to ensure proper initialization
        StartCoroutine(RespawnCoroutine());
    }

    System.Collections.IEnumerator RespawnCoroutine()
    {
        yield return null; // Wait for the next frame

        foreach (GameObject coin in coins)
        {
            // Set the coin back to active
            coin.SetActive(true);
        }
    }

    public void CollectCoin(GameObject coin)
    {
        // Logic to collect coins
        if (collectedCoins < maxCoins)
        {
            collectedCoins++;

            // Decrease maximum speed and jump force based on the number of coins collected
            maxMoveSpeed -= speedChangePerCoin;

            // Adjust jump force when a coin is collected
            displayJumpForce -= jumpChangePerCoin;

            // Cap the speed to not go below baseMoveSpeed
            maxMoveSpeed = Mathf.Max(maxMoveSpeed, baseMoveSpeed);

            // Cap the jump force to a reasonable minimum value
            displayJumpForce = Mathf.Max(displayJumpForce, 6f);

            UpdateCoinCountUI();

            // Destroy the collected coin GameObject
            coin.SetActive(false);
        }
    }

    public void ResetCoinCount()
    {
        // Reset collected coins to 0
        collectedCoins = 0;

        // Reset movement speed, max speed, and jump force back to base values
        baseMoveSpeed = 0.1f;
        maxMoveSpeed = 13f;
        currentMoveSpeed = baseMoveSpeed;
        baseJumpForce = 10f;

        // Update UI
        UpdateCoinCountUI();
    }


    public void UpdateCoinCountUI()
    {
        // Update the UI text to display the current number of collected coins
        coinCountText.text = "Coins: " + collectedCoins;
    }


    void LateUpdate()
    {
        // Reset the player's rotation to prevent any rotation around the Z-axis
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    void FixedUpdate()
    {
        // Debugging: Draw a ray to visualize the ground check position
        Debug.DrawRay(groundCheck.position, Vector2.down * 0.1f, isGrounded ? Color.green : Color.red);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the player has landed on the ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            canJump = true; // Allow jumping again
        }
    }
}