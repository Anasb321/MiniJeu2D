using UnityEngine;

public class MouvementPerso : MonoBehaviour
{
    [Header("Déplacement & Saut")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpForce = 15f;

    [Header("Se Baisser")]
    [SerializeField] private float crouchSpeed = 4f;
    [SerializeField] private Vector2 crouchColliderSize = new Vector2(0.8f, 0.5f);
    [SerializeField] private Vector2 crouchColliderOffset = new Vector2(0f, -0.25f);

    [Header("Limite de Chute dans le Vide")]
    [SerializeField] private float fallThreshold = -7.5f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator anim;
    private BoxCollider2D boxCollider;

    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;

    private bool isGrounded = false;
    private bool isCrouching = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();

        if (boxCollider != null)
        {
            originalColliderSize = boxCollider.size;
            originalColliderOffset = boxCollider.offset;
        }
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameOver) return;

        if (transform.position.y < fallThreshold)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }
            return;
        }

        if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && isGrounded)
        {
            SeBaisser(true);
        }
        else
        {
            SeBaisser(false);
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float currentSpeed = isCrouching ? crouchSpeed : moveSpeed;
        rb.linearVelocity = new Vector2(horizontal * currentSpeed, rb.linearVelocity.y);

        if (horizontal > 0 && spriteRenderer != null) spriteRenderer.flipX = false;
        else if (horizontal < 0 && spriteRenderer != null) spriteRenderer.flipX = true;

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump")) && isGrounded && !isCrouching)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
        }

        MettreAJourAnimator(horizontal);
    }

    private void SeBaisser(bool baisse)
    {
        if (isCrouching == baisse) return;

        isCrouching = baisse;

        if (boxCollider != null)
        {
            if (isCrouching)
            {
                boxCollider.size = crouchColliderSize;
                boxCollider.offset = crouchColliderOffset;
            }
            else
            {
                boxCollider.size = originalColliderSize;
                boxCollider.offset = originalColliderOffset;
            }
        }
    }

    private void MettreAJourAnimator(float horizontal)
    {
        if (anim == null) return;

        anim.SetFloat("Speed", Mathf.Abs(horizontal));
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isCrouching", isCrouching);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
                return;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }
}