using UnityEngine;

public class Agent : MonoBehaviour
{
    public float minSpeed = 1.0f;
    public float maxSpeed = 3.0f;
    public float speed { get; private set; } = 0.0f;
    public float colliderRadius = 0.5f;

    public Vector3 velocity;

    public PathManager pathManager { get; private set; }

    public Rigidbody rb { get; private set; }
    private CapsuleCollider capsuleCollider;
    

    void Start()
    {
        RandomizeColorAndSpeed();
        pathManager = this.GetComponentInChildren<PathManager>();
        rb = this.GetComponent<Rigidbody>();
        capsuleCollider = this.GetComponentInChildren<CapsuleCollider>();
    }

    private void Update()
    {
        capsuleCollider.radius = colliderRadius;
    }

    private void FixedUpdate()
    {
        this.rb.position += velocity * Time.fixedDeltaTime;
    }

    public void RandomizeColorAndSpeed()
    {
        // choose a random color in the texture, then set the speed depending on the chosen color
        Renderer renderer = this.GetComponentInChildren<Renderer>();
        Material mat = renderer.material;
        int id = Random.Range(1, 16);
        mat.mainTextureOffset = new Vector2(0.0f, (float)id / 16.0f);
        speed = minSpeed + (id - 1) / 15.0f * (maxSpeed - minSpeed);
    }
}
