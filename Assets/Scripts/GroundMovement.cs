using UnityEngine;

public class GroundMovement : MonoBehaviour
{
    public GameObject groundPrefab;

    public float speed;

    private bool hasSpawnedGround = false;

    private void Update()
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
    }

}
