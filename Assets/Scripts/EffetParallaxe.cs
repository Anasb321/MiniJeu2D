using UnityEngine;

public class EffetParallaxe : MonoBehaviour
{
    [Header("Caméra")]
    [SerializeField] private Transform cameraCible;

    [Header("Suivi de la caméra (0 = Fixe au sol, 1 = Suit à 100% la caméra)")]
    [SerializeField, Range(0f, 1f)] private float suiviHorizontal = 0.85f;
    [SerializeField, Range(0f, 1f)] private float suiviVertical = 0.90f;

    [Header("Déplacement automatique")]
    [SerializeField] private Vector2 vitesseAutomatique = new Vector2(0.01f, 0f);

    [Header("Répétition Infinie du Décor")]
    [SerializeField] private bool repetitionInfinie = true;

    private Vector3 positionInitiale;
    private Vector3 positionCameraInitiale;
    private Vector2 decalageAutomatique;
    private float longueurSprite;

    private void Start()
    {
        positionInitiale = transform.position;

        if (cameraCible == null && Camera.main != null)
        {
            cameraCible = Camera.main.transform;
        }

        if (cameraCible != null)
        {
            positionCameraInitiale = cameraCible.position;
        }

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            longueurSprite = sr.bounds.size.x;
        }
    }

    private void LateUpdate()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameOver) return;

        if (cameraCible == null)
        {
            if (Camera.main != null)
            {
                cameraCible = Camera.main.transform;
                positionCameraInitiale = cameraCible.position;
            }
            else
            {
                return;
            }
        }

        Vector3 mouvementCamera = cameraCible.position - positionCameraInitiale;

        decalageAutomatique += vitesseAutomatique * Time.deltaTime;

        float posX = positionInitiale.x + mouvementCamera.x * suiviHorizontal + decalageAutomatique.x;
        float posY = positionInitiale.y + mouvementCamera.y * suiviVertical + decalageAutomatique.y;

        transform.position = new Vector3(posX, posY, positionInitiale.z);

        if (repetitionInfinie && longueurSprite > 0f)
        {
            float temp = cameraCible.position.x * (1f - suiviHorizontal);
            if (temp > positionInitiale.x + longueurSprite)
            {
                positionInitiale.x += longueurSprite;
            }
            else if (temp < positionInitiale.x - longueurSprite)
            {
                positionInitiale.x -= longueurSprite;
            }
        }
    }
}