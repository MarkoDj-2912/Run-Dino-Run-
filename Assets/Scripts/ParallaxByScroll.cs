using UnityEngine;

public class ParallaxLoop : MonoBehaviour
{
    public Transform[] tiles;
    public float factor = 0.5f;
    public GroundMovement groundSource;
    public float fallbackSpeed = 2f;

    private float spriteWidth;

    private void Start()
    {
        if (tiles.Length < 2)
        {
            Debug.LogError("ParallaxLoop needs at least 2 tiles!");
            enabled = false;
            return;
        }

        spriteWidth = tiles[0].GetComponent<SpriteRenderer>().bounds.size.x;
    }

    private void Update()
    {
        float baseSpeed = groundSource ? groundSource.speed : fallbackSpeed;
        transform.position += Vector3.left * (baseSpeed * factor) * Time.deltaTime;

        if (Camera.main.transform.position.x - tiles[0].position.x > spriteWidth)
        {
            Transform firstTile = tiles[0];
            Transform lastTile = tiles[tiles.Length - 1];

            firstTile.position = new Vector3(lastTile.position.x + spriteWidth, firstTile.position.y, firstTile.position.z);

            // Pomeri element u nizu na kraj
            for (int i = 0; i < tiles.Length - 1; i++)
                tiles[i] = tiles[i + 1];
            tiles[tiles.Length - 1] = firstTile;
        }
    }
}
