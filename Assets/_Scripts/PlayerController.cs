using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int speed;

    private Rigidbody rb;
    private Vector3 movement;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
    }

    private void FixedUpdate()
    {
        // where we are, where we want to move, and the speed
        rb.MovePosition(transform.position + movement * speed * Time.fixedDeltaTime);
    }
}