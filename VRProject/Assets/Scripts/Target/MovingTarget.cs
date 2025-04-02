using UnityEngine;
using UnityEngine.SocialPlatforms;

public class MovingTarget : Target
{
    [Header("Movement")]
    [SerializeField] private bool shouldMove = false; // Translation activated?
    [SerializeField] private float speed = 2f; // Speed of movement
    [SerializeField] private float distance = 3f; // Distance to move
    [SerializeField] private Vector3 moveDirection = Vector3.right; // Axis of movement

    [Header("Rotation")]
    [SerializeField] private bool shouldRotate = false; // Rotation activated?
    [SerializeField] private float rotationSpeed = 30f; // Speed of rotation
    [SerializeField] private float rotationTime = 1f; // Speed of rotation
    [SerializeField] private Vector3 rotationAxis = Vector3.up; // Axis of rotation


    private Vector3 startPosition;
    private Fracture fractureComponent;
    private Rigidbody rb;

    //Rotation variables
    private Quaternion originalRotation;
    private Quaternion targetRotation;
    private float timer;
    private bool rotatingTo90 = true;

    private void Start()
    {
        fractureComponent = GetComponent<Fracture>();
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
        originalRotation = transform.rotation;    
    }

    private void Update()
    {
        if (shouldMove)
        {
            // Move back and forth between +5 and -5 using a sine wave
            float movement = Mathf.Sin(Time.time * speed) * (distance / 2);
            transform.position = startPosition + moveDirection.normalized * movement;
        }

        if (shouldRotate)
        {
            // Handle rotation switching
            timer += Time.deltaTime;
            if (timer >= rotationTime)
            {
                rotatingTo90 = !rotatingTo90;
                timer = 0f;
            }
            targetRotation = originalRotation * Quaternion.AngleAxis(90f, rotationAxis);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotatingTo90 ? targetRotation : originalRotation, Time.deltaTime * rotationSpeed);
        }
    }

    public override void TargetHit(Vector3 hitPoint)
    {
        base.TargetHit(hitPoint);
        fractureComponent.CauseFracture();
    }    

    public void Explode()
    {
        rb.constraints = RigidbodyConstraints.None;
        rb.linearVelocity = new Vector3(10, 0, 0);
    }
}
