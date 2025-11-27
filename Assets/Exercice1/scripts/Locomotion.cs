using UnityEngine;


[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Tracker))]
public class Locomotion : MonoBehaviour
{
    private Animator m_animator;
    private Tracker m_tracker;

    [Header("Animation Settings")]
    public float dampTime = 0.1f;
    public bool rotateOverMovement = true;
    public float rotationSpeed = 90.0f;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_tracker = GetComponent<Tracker>();
    }

    void Start()
    {
        // disable movement by animation
        m_animator.applyRootMotion = false;
    }

    void LateUpdate()
    {
        m_animator.SetFloat("velX", m_tracker.m_relativeVelocity.x, dampTime, Time.deltaTime);
        m_animator.SetFloat("velZ", m_tracker.m_relativeVelocity.z, dampTime, Time.deltaTime);

        // handle smooth rotation of the player
        if (rotateOverMovement && m_tracker.m_relativeVelocityMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(m_tracker.m_worldVelocity, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(
                m_tracker.m_rotation,
                targetRot,
                rotationSpeed * Time.deltaTime
            );
        }
    }
}
