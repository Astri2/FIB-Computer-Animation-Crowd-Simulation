using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class KeyboardController : MonoBehaviour
{

    private CharacterController m_controller;

    [Header("Movement Settings")]
    public float maxSpeed = 5f;        // Maximum movement speed
    public float acceleration = 5f;    // How fast speed ramps up
    public float deceleration = 5f;    // How fast speed ramps down
    public float counterVelocityFactor = 2.0f;
    public bool damp = true;

    private Vector3 velocity = Vector3.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        m_controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        // WASD locomotion
        float inputX = Input.GetKey(KeyCode.D) ? 1f : Input.GetKey(KeyCode.A) ? -1f : 0f;
        float inputZ = Input.GetKey(KeyCode.W) ? 1f : Input.GetKey(KeyCode.S) ? -1f : 0f;

        Vector3 inputDir = new Vector3(inputX, 0, inputZ);
        if (inputDir.magnitude > 1f)
            inputDir.Normalize();

        Vector3 targetVelocity = inputDir * maxSpeed;
        if (damp)
        {
            // if input is in the opposite direction, use 2x acceleration to counter current velocity
            float accelFactor = Vector3.Dot(velocity, inputDir) < 0 ? counterVelocityFactor : 1.0f;
            velocity = Vector3.MoveTowards(velocity, targetVelocity, (inputDir.magnitude > 0 ? accelFactor * acceleration : deceleration) * Time.deltaTime);
        }
        else velocity = targetVelocity;

        m_controller.Move(velocity * Time.deltaTime);
    }
}
