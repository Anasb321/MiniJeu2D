using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] private int points = 100;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AjouterScore(points);
            }

            Destroy(gameObject);
        }
    }
}