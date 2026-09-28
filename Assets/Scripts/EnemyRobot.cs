using UnityEngine;

public class EnnemiRobot : MonoBehaviour
{
    public enum TypeComportement { PatrouilleurZone, ChasseurSuiveur }

    [Header("--- Type de Robot ---")]
    [SerializeField] private TypeComportement typeRobot = TypeComportement.PatrouilleurZone;

    [Header("--- Vitesse ---")]
    [SerializeField] private float vitesse = 3.0f;

    [Header("--- Configuration Patrouille ---")]
    [SerializeField] private float distancePatrouille = 3.0f; // Distance max à gauche/droite de sa zone
    private Vector3 positionDepart;
    private bool deplacementDroite = true;

    [Header("--- Configuration Chasseur ---")]
    [SerializeField] private float distanceDetection = 12.0f;  // Détecte le joueur s'il approche
    [SerializeField] private float distanceMaxChasse = 22.0f;  // Ne dépasse pas 20-25 unités de course
    private Transform joueur;
    private float distanceParcourueChasse = 0f;
    private Vector3 dernierePosition;

    private void Start()
    {
        positionDepart = transform.position;
        dernierePosition = transform.position;

        GameObject objJoueur = GameObject.FindGameObjectWithTag("Player");
        if (objJoueur != null)
        {
            joueur = objJoueur.transform;
        }
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameOver) return;

        switch (typeRobot)
        {
            case TypeComportement.PatrouilleurZone:
                GererPatrouille();
                break;

            case TypeComportement.ChasseurSuiveur:
                GererChasse();
                break;
        }
    }

    private void GererPatrouille()
    {
        if (deplacementDroite)
        {
            transform.Translate(Vector3.right * vitesse * Time.deltaTime);
            if (transform.position.x >= positionDepart.x + distancePatrouille)
                deplacementDroite = false;
        }
        else
        {
            transform.Translate(Vector3.left * vitesse * Time.deltaTime);
            if (transform.position.x <= positionDepart.x - distancePatrouille)
                deplacementDroite = true;
        }
    }

    private void GererChasse()
    {
        if (joueur == null) return;

        float distanceAuJoueur = Vector3.Distance(transform.position, joueur.position);

        if (distanceAuJoueur <= distanceDetection && distanceParcourueChasse < distanceMaxChasse)
        {
            Vector3 cible = new Vector3(joueur.position.x, transform.position.y, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, cible, vitesse * Time.deltaTime);

            distanceParcourueChasse += Vector3.Distance(transform.position, dernierePosition);
            dernierePosition = transform.position;
        }
    }
}