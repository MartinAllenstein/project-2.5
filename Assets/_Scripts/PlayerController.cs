using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int speed;
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer playerSprite;

    private Rigidbody rb;
    private Vector3 movement;

    // Animator
    private const string IS_WALK_PARAM = "IsWalk";
    private const string IS_JUMP_PARAM = "IsJump";
    
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
        
        anim.SetBool(IS_WALK_PARAM, movement!=Vector3.zero);

        // Flip sprite
        if (movement.x != 0 && movement.x < 0)
        {
            playerSprite.flipX = true;
        }

        if (movement.x != 0 && movement.x > 0)
        {
            playerSprite.flipX = false;
        }
    }

    private void FixedUpdate()
    {
        // where we are, where we want to move, and the speed
        rb.MovePosition(transform.position + movement * speed * Time.fixedDeltaTime);
    }
}