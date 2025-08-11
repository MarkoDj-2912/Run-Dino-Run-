using UnityEngine;

public class GroundMovement : MonoBehaviour
{
    public GameObject groundPrefab;
    public float speed = 6f;
    public float speedIncrease = 0.7f;
    public float smoothTime = 0.6f;

    private bool hasSpawnedGround = false;
    private float targetSpeed;
    private float smoothVel;

    void Start()
    {
        targetSpeed = speed;
    }

    void Update()
    {
       
        if (Vector3.Distance(new Vector3(-0.819091976f, -3.38622999f, 0), transform.position) < 0.2f && !hasSpawnedGround)
        {
            Instantiate(groundPrefab, new Vector3(-0.819091976f + 20, -3.38622999f, 0), Quaternion.identity);
            hasSpawnedGround = true;
        }
        else if (Vector3.Distance(new Vector3(-21f, -3.38622999f, 0), transform.position) < 0.2f)
        {
            Destroy(gameObject);
        }

        transform.Translate(Vector3.left * speed * Time.deltaTime);

    
        speed = Mathf.SmoothDamp(speed, targetSpeed, ref smoothVel, smoothTime);
    }

    public void BumpSpeed()
    {
        speed += speedIncrease;
        targetSpeed += speedIncrease;
    }
}
