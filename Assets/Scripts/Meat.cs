using UnityEngine;
using UnityEngine.SceneManagement;

public class Meat : MonoBehaviour
{
    [SerializeField] private GroundMovement groundSource;
    [SerializeField] private float fallbackSpeed = 6f;
    [SerializeField] private float killMarginLeft = 20f;

    public AudioClip biteClip;

    Transform player;
    bool collected;
    Camera cam;

    void Awake()
    {
        if (!groundSource) groundSource = FindFirstObjectByType<GroundMovement>();
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p) player = p.transform;
        cam = Camera.main;

        // failsafe: ako je mesto zauzeto obstacle-om (kaktus), pomeri malo napred
        var mask = LayerMask.GetMask("Obstacle");
        Vector3 pos = transform.position;
        int tries = 0;
        while (Physics2D.OverlapBox(pos, new Vector2(1.2f, 1.2f), 0f, mask) && tries < 12)
        {
            pos.x += 0.8f;
            tries++;
        }
        transform.position = pos;
    }

    void Update()
    {
        float speed = groundSource ? groundSource.speed : fallbackSpeed;
        transform.position += Vector3.left * speed * Time.deltaTime;

        if (!collected && player && transform.position.x < player.position.x - 0.5f)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        if (cam && transform.position.x < cam.transform.position.x - killMarginLeft)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        collected = true;
        AudioManager.instance.PlaySFX(biteClip);
        Destroy(gameObject);
    }
}
