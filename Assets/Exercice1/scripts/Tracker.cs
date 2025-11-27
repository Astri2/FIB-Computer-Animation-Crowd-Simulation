using UnityEngine;

public class Tracker : MonoBehaviour
{
    public Vector3 m_prevWorldPos;
    public Vector3 m_prevLocalPos;

    public Vector3 m_worldPos;
    public Vector3 m_localPos;

    public Vector3 m_worldDisplacement;
    public Vector3 m_localDisplacement;

    public Vector3 m_worldVelocity;
    public Vector3 m_relativeVelocity;
    public float m_relativeVelocityMagnitude;

    public Vector3 m_forward;
    public Quaternion m_rotation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_worldPos = transform.position;
        m_localPos = transform.localPosition;
        m_forward = transform.forward;
    }

    // Update is called once per frame
    void Update()
    {
        // store more info for future Gizmos. Most of won't ever be used but whatever
        m_prevWorldPos = m_worldPos;
        m_prevLocalPos = m_localPos;
        m_worldPos = transform.transform.position;
        m_localPos = transform.transform.localPosition;
        m_worldDisplacement = m_worldPos - m_prevWorldPos;
        m_localDisplacement = m_localPos - m_prevLocalPos;

        // compute speed in the frame of the 
        m_worldVelocity = (m_localDisplacement) / Time.deltaTime;
        m_relativeVelocity = transform.InverseTransformDirection(m_worldVelocity);
        m_relativeVelocityMagnitude = m_relativeVelocity.magnitude;

        m_forward = transform.forward;
        m_rotation = transform.rotation;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        // speed direction
        Gizmos.color = new Color(0.0f, 0.75f, 0.0f);
        Gizmos.DrawLine(m_worldPos, m_worldPos + m_worldVelocity.normalized);

        // speed magnitude (assuming max speed is 5, this should max at magnitude=1)
        Gizmos.color = new Color(0.75f, 0.0f, 0.0f);
        Gizmos.DrawLine(m_worldPos, m_worldPos + m_worldVelocity * 0.2f);

        // character orientation
        Gizmos.color = new Color(0.0f, 0.0f, 0.75f);
        Gizmos.DrawLine(m_worldPos, m_worldPos + m_forward);
    }
}
