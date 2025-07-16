using System;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;
using UnityEngine.Splines;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int speed;
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private LayerMask grassLayer;
    [SerializeField] private int stepsInGrass;
    [SerializeField] private int minStepsToEncounter;
    [SerializeField] private int maxStepsToEncounter;

    private PlayerControls playerControls;
    private Rigidbody rb;
    private Vector3 movement;
    private bool movingInGrass;
    private float stepTimer;
    private int stepsToEncounter;
    private PartyManager partyManager;
    
    // Animation
    private const string IS_WALK_PARAM = "IsWalk";
    
    private const string BATTLE_SCENE = "BattleScene";
    private const float TIME_PER_STEP = 0.5f;

    private void Awake()
    {
        playerControls = new PlayerControls();
        CalculateStepToNextEncounter();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        partyManager = GameObject.FindFirstObjectByType<PartyManager>();
        if (partyManager.GetPosition() != Vector3.zero)     // if we have a position saved
        {
            transform.position = partyManager.GetPosition();    // move the player
        }
    }

    void Update()
    {
        float x = playerControls.Player.Move.ReadValue<Vector2>().x;
        float z = playerControls.Player.Move.ReadValue<Vector2>().y;
        
        movement = new Vector3(x, 0, z);
        
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
            if (stepTimer >= TIME_PER_STEP)
            {
                stepsInGrass++;
                stepTimer = 0;

                // check to see if we have reached an encounter ->change scene
                if (stepsInGrass >= stepsToEncounter)
                {
                    partyManager.SetPosition(transform.position);
                    SceneManager.LoadScene(BATTLE_SCENE);
                }
            }
        }
    }

    private void CalculateStepToNextEncounter()
    {
        stepsToEncounter = Random.Range(minStepsToEncounter, maxStepsToEncounter);
    }

    public void SetOverworldVisuals(Animator animator, SpriteRenderer spriteRenderer)
    {
        anim = animator;
        playerSprite = spriteRenderer;
    }
}