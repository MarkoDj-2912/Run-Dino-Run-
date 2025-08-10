using UnityEngine;
using UnityEngine.SceneManagement;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private GroundMovement groundSource;
    [SerializeField] private float fallbackSpeed = 6f;
    [SerializeField] private float killMarginLeft = 20f;

    public int startRequired = 3;
    public static int currentRequired;

    Camera cam;

    void Start()
    {
        currentRequired = startRequired;
        if (!groundSource) groundSource = UnityEngine.Object.FindFirstObjectByType<GroundMovement>();
        cam = Camera.main;
    }

    void Update()
    {
        float speed = groundSource ? groundSource.speed : fallbackSpeed;
        transform.position += Vector3.left * speed * Time.deltaTime;
        if (cam && transform.position.x < cam.transform.position.x - killMarginLeft)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        var player = other.GetComponent<Player>();
        if (!player) return;

        if (player.coins < currentRequired)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            player.coins = 0;
            currentRequired = Mathf.Min(currentRequired + 1, 10);
        }
    }
}
