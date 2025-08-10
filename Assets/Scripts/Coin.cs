using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private GroundMovement groundSource;
    [SerializeField] private float fallbackSpeed = 6f;
    [SerializeField] private float lifeTime = 12f;

    public AudioClip coinClip;

    private bool collected;
    private Collider2D col;

    void Awake()
    {
        if (!groundSource)
            groundSource = UnityEngine.Object.FindFirstObjectByType<GroundMovement>();
        col = GetComponent<Collider2D>();
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        float speed = groundSource ? groundSource.speed : fallbackSpeed;
        transform.position += Vector3.left * speed * Time.deltaTime;

    }

  private void OnTriggerEnter2D(Collider2D other)
{
    if (collected) return;

    var player = other.GetComponentInParent<Player>();
    if (player == null) return; 

    collected = true;
    if (col) col.enabled = false;

    player.coins += 1;
    ScoreManager.AddCoin(1);
    AudioManager.instance.PlaySFX(coinClip);

    Destroy(gameObject);
}

}
