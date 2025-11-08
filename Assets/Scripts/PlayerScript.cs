using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    
    private Rigidbody2D rb;
    private Vector2 moveDirection;
    
    void Start()
    {
        // Get the Rigidbody2D component
        rb = GetComponent<Rigidbody2D>();
        
        // If no Rigidbody2D exists, add one
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        
        // Freeze rotation to prevent player from rotating
        rb.freezeRotation = true;
        
        // Set gravity scale to 0 for top-down 2D movement (or keep it if you want gravity)
        rb.gravityScale = 0f;
    }
    
    void Update()
    {
        // Get input from keyboard
        float horizontal = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right Arrow
        float vertical = Input.GetAxisRaw("Vertical");     // W/S or Up/Down Arrow
        
        // Calculate movement direction (2D)
        moveDirection = new Vector2(horizontal, vertical).normalized;
    }
    
    void FixedUpdate()
    {
        // Apply movement
        if (moveDirection.magnitude >= 0.1f)
        {
            // Move the player using velocity for smooth movement
            rb.velocity = moveDirection * moveSpeed;
        }
        else
        {
            // Stop movement when no input
            rb.velocity = Vector2.zero;
        }
    }
}