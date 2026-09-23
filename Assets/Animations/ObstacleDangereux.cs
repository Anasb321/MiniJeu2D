// using UnityEngine;

// public class ObstacleDangereux : MonoBehaviour
// {
//     [SerializeField, Min(0.1f)] private float delaiEntreDegats = 1f;
//     private float prochainDegat;
//     [SerializeField] private Transform pointDepart;
//     [SerializeField] private GameObject player;

    

//     private void OnCollisionStay2D(Collision2D collision)
//     {
//         InfligerDegat(collision.gameObject);

//         //
//         player.transform.position = pointDepart.position;
//         Debug.Log("Le robot retourne au point de départ.");
//     }

//     private void OnTriggerStay2D(Collider2D collision)
//     {
//         InfligerDegat(collision.gameObject);
//     }

//     private void InfligerDegat(GameObject objet)
//     {
//         if (!objet.CompareTag("Player") || Time.time < prochainDegat) return;
//         prochainDegat = Time.time + delaiEntreDegats;
//         GestionJeu.Instance.PerdreVie();
//     }
// }


using UnityEngine;

public class ObstacleDangereux : MonoBehaviour
{
    [SerializeField, Min(0.1f)]
    private float delaiEntreDegats = 1f;

    [SerializeField]
    private Transform pointDepart;

    private float prochainDegat;

    private void OnCollisionStay2D(Collision2D collision)
    {
        InfligerDegat(collision.gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        InfligerDegat(collision.gameObject);
    }

    private void InfligerDegat(GameObject objet)
    {
        // Vérifier qu'il s'agit du joueur
        if (!objet.CompareTag("Player"))
            return;

        // Respecter le délai entre deux dégâts
        if (Time.time < prochainDegat)
            return;

        prochainDegat = Time.time + delaiEntreDegats;

        // Retirer une vie
        if (GestionJeu.Instance != null)
        {
            GestionJeu.Instance.PerdreVie();
        }

        // Déclencher l'effet visuel
        EffetDegatsJoueur effet = objet.GetComponent<EffetDegatsJoueur>();

        if (effet != null)
        {
            effet.DeclencherEffet();
        }
        else
        {
            Debug.LogWarning(
                "Le composant EffetDegatsJoueur est absent du joueur."
            );
        }

        // Replacer le joueur au point de départ
        if (pointDepart != null)
        {
            objet.transform.position = pointDepart.position;
            Debug.Log("Le robot retourne au point de départ.");
        }
        else
        {
            Debug.LogWarning(
                "Le PointDepart n'est pas assigné à l'obstacle."
            );
        }
    }
}