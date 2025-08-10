using UnityEngine;

public class Cactus : MonoBehaviour
{
    [SerializeField] private GroundMovement groundSource;
    [SerializeField] private float fallbackSpeed = 6f;
    [SerializeField] private float killMarginLeft = 20f;

    Camera cam;

    void Awake()
    {
        if (!groundSource) groundSource = FindFirstObjectByType<GroundMovement>();
        cam = Camera.main;
    }

    void Update()
    {
        float speed = groundSource ? groundSource.speed : fallbackSpeed;
        transform.position += Vector3.left * speed * Time.deltaTime;

        if (cam && transform.position.x < cam.transform.position.x - killMarginLeft)
            Destroy(gameObject);
    }
}
