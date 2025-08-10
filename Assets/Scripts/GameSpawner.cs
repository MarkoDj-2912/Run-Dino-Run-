using UnityEngine;

public class GameSpawner : MonoBehaviour
{
    [Header("Speed source")]
    public GroundMovement groundSource;
    public float fallbackSpeed = 6f;

    [Header("Prefabs")]
    public GameObject cactusA, cactusB, cactusC;
    public GameObject coinPrefab;
    public GameObject checkpointPrefab;
    public GameObject meatPrefab;

    [Header("Spawn distances (world units)")]
    public float cactusDistance = 15f;
    public float coinDistance = 10f;
    public float checkpointDistance = 80f;
    public float meatDistance = 30f;

    [Header("Positions")]
    public float groundY = -3.38f;
    public float coinYMin = -3.0f, coinYMax = -2.2f;
    public float meatYOffset = -3.1f;
    public float spawnAhead = 15f;

    [Header("Coin group")]
    public int   coinGroupMin = 3, coinGroupMax = 5;
    public float coinGroupSpacing = 0.8f;

    [Header("Min spacings (world units)")]
    public float minCactusSpacing = 6.5f;
    public float minCheckpointToCactus = 9f;
    public float minMeatSpacing = 12f;
    public float minMeatToCactus = 10f;
    public float minMeatToCheckpoint = 8f;

    [Header("Limits")]
    public float maxExtraAhead = 25f;

    [Header("Spawn safety")]
    public LayerMask obstacleMask;
    public Vector2 meatCheckSize = new Vector2(1.4f, 1.4f);
    public Vector2 cactusCheckSize = new Vector2(1.2f, 1.2f);
    public int     maxAdjustTries = 16;
    public float   adjustStep = 1.2f;
    public float   meatLiftNearCactus = 0.3f;

    // Accumulators and last-spawn world Xs
    float cactusAcc, coinAcc, checkpointAcc, meatAcc;
    float lastCactusX = -9999f, lastCheckpointX = -9999f, lastMeatX = -9999f;

    Camera cam;

    void Awake()
    {
        cam = Camera.main;
        if (!groundSource) groundSource = FindFirstObjectByType<GroundMovement>();
    }

    float CamX() => cam ? cam.transform.position.x : 0f;

    void Update()
    {
        float speed = groundSource ? groundSource.speed : fallbackSpeed;
        float moved = Mathf.Max(0f, speed) * Time.deltaTime;

        // advance distance accumulators
        cactusAcc     += moved;
        coinAcc       += moved;
        checkpointAcc += moved;
        meatAcc       += moved;

        // ★ keep lastX in world-space synced with scrolling world (they move left)
        lastCactusX     -= moved;
        lastCheckpointX -= moved;
        lastMeatX       -= moved;

        // spawn loops – only consume accumulator when spawn actually succeeds
        while (cactusAcc >= cactusDistance)
        {
            if (SpawnCactus()) cactusAcc -= cactusDistance;
            else break;
        }

        while (coinAcc >= coinDistance)
        {
            if (SpawnCoinsGroup()) coinAcc -= coinDistance;
            else break;
        }

        while (checkpointAcc >= checkpointDistance)
        {
            if (SpawnCheckpoint()) checkpointAcc -= checkpointDistance;
            else break;
        }

        while (meatAcc >= meatDistance)
        {
            if (SpawnMeat()) meatAcc -= meatDistance;
            else break;
        }
    }

    GameObject RandomCactus()
    {
        int r = Random.Range(0, 3);
        return r == 0 ? cactusA : (r == 1 ? cactusB : cactusC);
    }

bool SpawnCactus()
{
    float camX = CamX();
    float minX = camX + spawnAhead;

    // poštuj razmake naspram prethodnih objekata
    minX = Mathf.Max(minX, lastCactusX     + minCactusSpacing);
    minX = Mathf.Max(minX, lastCheckpointX + minCheckpointToCactus);
    minX = Mathf.Max(minX, lastMeatX       + minMeatToCactus); // ★ novo: izbegni poslednji meat

    float maxX = camX + spawnAhead + maxExtraAhead;
    if (minX > maxX) return false;

    Vector3 pos = new Vector3(minX, groundY, 0f);

    int tries = 0;
    while (tries < maxAdjustTries)
    {
        // osnovni sudar
        bool blocked = Physics2D.OverlapBox(pos, cactusCheckSize, 0f, obstacleMask);

        // prošireni “clearance” po X da izbegne meat/cactus u blizini
        Vector2 clearance = new Vector2(cactusCheckSize.x + minMeatToCactus, cactusCheckSize.y);
        bool tooClose = Physics2D.OverlapBox(pos, clearance, 0f, obstacleMask);

        if (!blocked && !tooClose) break;

        pos.x += adjustStep;
        if (pos.x > maxX) return false;
        tries++;
    }

    Instantiate(RandomCactus(), pos, Quaternion.identity);
    lastCactusX = pos.x;
    return true;
}

    bool SpawnCheckpoint()
    {
        float camX = CamX();
        float minX = camX + spawnAhead;

        minX = Mathf.Max(minX, lastCactusX + minCheckpointToCactus);
        minX = Mathf.Max(minX, lastMeatX   + minMeatToCheckpoint);

        float maxX = camX + spawnAhead + maxExtraAhead;
        if (minX > maxX) return false;

        Vector3 pos = new Vector3(minX, groundY, 0f);
        Instantiate(checkpointPrefab, pos, Quaternion.identity);
        lastCheckpointX = pos.x;
        return true;
    }

bool SpawnMeat()
{
    float camX = CamX();
    float minX = camX + spawnAhead;

    // razmaci prema poslednjim postavljenim
    minX = Mathf.Max(minX, lastMeatX       + minMeatSpacing);
    minX = Mathf.Max(minX, lastCactusX     + minMeatToCactus);
    minX = Mathf.Max(minX, lastCheckpointX + minMeatToCheckpoint);

    float maxX = camX + spawnAhead + maxExtraAhead;
    if (minX > maxX) return false;

    Vector3 pos = new Vector3(minX, meatYOffset, 0f);

    int tries = 0;
    while (tries < maxAdjustTries)
    {
        // osnovni sudar
        bool blocked = Physics2D.OverlapBox(pos, meatCheckSize, 0f, obstacleMask);

        // prošireni “clearance” po X — NEMA blizine kaktusa
        Vector2 clearance = new Vector2(meatCheckSize.x + minMeatToCactus, meatCheckSize.y);
        bool tooClose = Physics2D.OverlapBox(pos, clearance, 0f, obstacleMask);

        if (!blocked && !tooClose) break;

        pos.x += adjustStep;
        if (pos.x > maxX) return false;
        tries++;
    }

    // ukidamo “lift near cactus” da ne bi ikad bio iznad kaktusa
    // if (pos.x < lastCactusX + (minMeatToCactus + 1f)) pos.y = meatYOffset + meatLiftNearCactus;

    Instantiate(meatPrefab, pos, Quaternion.identity);
    lastMeatX = pos.x;
    return true;
}

    bool SpawnCoinsGroup()
    {
        float camX = CamX();
        float startX = camX + spawnAhead;
        float maxX   = camX + spawnAhead + maxExtraAhead;

        if (startX > maxX) return false; // no window this frame

        int count = Random.Range(coinGroupMin, coinGroupMax + 1);
        float y = Random.Range(coinYMin, coinYMax);

        // spawn coins; if group goes past window it's fine—rest will slide in
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = new Vector3(startX + i * coinGroupSpacing, y, 0f);
            Instantiate(coinPrefab, pos, Quaternion.identity);
        }

        return true;
    }

    // (Optional) Gizmos to visualize safety boxes
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.25f);
        // just draw sizes at an example position to preview box sizes
        Gizmos.DrawWireCube(new Vector3(CamX() + spawnAhead, meatYOffset, 0f), meatCheckSize);
        Gizmos.color = new Color(0, 1, 0, 0.25f);
        Gizmos.DrawWireCube(new Vector3(CamX() + spawnAhead, groundY, 0f), cactusCheckSize);
    }
}
