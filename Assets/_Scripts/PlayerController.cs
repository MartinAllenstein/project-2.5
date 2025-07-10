using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int speed;
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private LayerMask grassLayer;
    [SerializeField] private int stepsInGrass;

    private Rigidbody rb;
    private Vector3 movement;
    private bool movingInGrass;
    private float stepTimer;
    
    // Animator
    private const string IS_WALK_PARAM = "IsWalk";
    private const string IS_JUMP_PARAM = "IsJump";
    
    private const float timePerStep = 0.5f;
    
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
        
        Collider[] colliders = Physics.OverlapSphere(transform.position, 1, grassLayer);
        movingInGrass = colliders.Length != 0 && movement != Vector3.zero;

        if (movingInGrass == true)
        {
            stepTimer += Time.fixedDeltaTime;
            if (stepTimer >= timePerStep)
            {
                stepsInGrass++;
                stepTimer = 0;
                
                // check to see if we have reached an encounter ->change scene
                
            }
        }
    }
}