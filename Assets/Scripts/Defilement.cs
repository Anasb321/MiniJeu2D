using UnityEngine;

public class MoveLeft : MonoBehaviour
{
    [SerializeField] private float moveSpeedMultiplier = 1f;

    private void Update()
    {
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.isGameOver) return;

            float speed = GameManager.Instance.currentSpeed * moveSpeedMultiplier;
            transform.Translate(Vector3.left * speed * Time.deltaTime, Space.World);
        }

        if (transform.position.x < -25f)
        {
            Destroy(gameObject);
        }
    }
}