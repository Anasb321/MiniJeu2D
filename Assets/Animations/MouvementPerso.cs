using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class MouvementRobot : MonoBehaviour
{
    [SerializeField] private float vitesse = 5f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float limiteChuteY = -8f;

    private Rigidbody2D corps;
    private Animator animator;
    private bool commandesActives = true;
    private bool isGrounded = false;
    private float deplacementX;

    private void Awake()
    {
        corps = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!commandesActives) return;

        deplacementX = Input.GetAxisRaw("Horizontal");

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && isGrounded)
        {
            Sauter();
        }

        if (transform.position.y < limiteChuteY)
        {
            Mourir();
        }

        MettreAJourAnimator();
    }

    private void FixedUpdate()
    {
        if (!commandesActives) return;

        corps.linearVelocity = new Vector2(deplacementX * vitesse, corps.linearVelocity.y);
    }

    private void Sauter()
    {
        corps.linearVelocity = new Vector2(corps.linearVelocity.x, jumpForce);
        isGrounded = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            Mourir();
        }
    }

    private void MettreAJourAnimator()
    {
        if (animator == null) return;

        bool enMouvement = Mathf.Abs(deplacementX) > 0.1f || !isGrounded;
        animator.SetBool("EnMouvement", enMouvement);
    }

    private void Mourir()
    {
        DesactiverCommandes();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
    }

    public void DesactiverCommandes()
    {
        commandesActives = false;
        corps.linearVelocity = Vector2.zero;

        if (animator != null)
        {
            animator.SetBool("EnMouvement", false);
        }
    }
}