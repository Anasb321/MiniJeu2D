using UnityEngine;

public class DegatBombe : MonoBehaviour
{

    private bool aTouche = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !aTouche)
        {
            Exploser();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !aTouche)
        {
            Exploser();
        }
    }

    private void Exploser()
    {
        aTouche = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }

        Destroy(gameObject);
    }
}