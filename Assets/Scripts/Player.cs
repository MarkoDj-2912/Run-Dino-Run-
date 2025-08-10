using UnityEngine;


public class Player : MonoBehaviour
{
    public int coins = 0;
    public float jumpForce;
    public LayerMask groundLayer;
    public float rayLength = 0.1f;

    public AudioClip jumpClip;
    public AudioClip landClip;
    public AudioClip dieClip;


    private Rigidbody2D rb;
    private bool isGrounded;
    private Animator animator;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }


    void Update()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, rayLength, groundLayer);
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForceY(jumpForce, ForceMode2D.Impulse);
            AudioManager.instance.PlaySFX(jumpClip);
        }
        if (isGrounded)
        {
            animator.Play("Player_Run");
        }
        else
        {
            animator.Play("Player_Jump");
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Damage")
        {
            AudioManager.instance.PlaySFX(dieClip);
            Invoke("WaitForSceneLoad", dieClip.length);
        }
        else
        {
            AudioManager.instance.PlaySFX(landClip);
        }
    }
    void WaitForSceneLoad()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }
}
