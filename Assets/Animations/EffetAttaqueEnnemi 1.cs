using System.Collections; // Permet d'utiliser IEnumerator pour les coroutines.
using UnityEngine;

// Exige un SpriteRenderer sur le même GameObject.
// Unity l'ajoute automatiquement si nécessaire lors de l'ajout du script.
[RequireComponent(typeof(SpriteRenderer))]
public class EffetAttaqueEnnemi : MonoBehaviour
{
    // Couleur appliquée au début de l'attaque : orange opaque.
    // Les valeurs correspondent à rouge, vert, bleu et alpha.
    // new(...) est une écriture abrégée de new Color(...).
    [SerializeField]
    private Color couleurAttaque = new(1f, 0.45f, 0.12f, 1f);

    // Multiplicateur de taille pendant l'attaque.
    // 1.22 signifie 122 % de la taille initiale.
    // Min impose une valeur minimale dans l'Inspector.
    [SerializeField, Min(1f)]
    private float agrandissement = 1.22f;

    // Durée de référence, en secondes, utilisée pour les pauses.
    [SerializeField, Min(0.05f)]
    private float duree = 0.18f;

    // Composant qui affiche le sprite de l'ennemi.
    private SpriteRenderer rendu;

    // Apparence mémorisée pour pouvoir la restaurer après l'effet.
    private Color couleurInitiale;
    private Vector3 echelleInitiale;

    // Référence à la coroutine active.
    // Permet de l'interrompre si une nouvelle attaque survient.
    private Coroutine effetEnCours;

    private void Awake()
    {
        // Récupère le SpriteRenderer du même GameObject.
        rendu = GetComponent<SpriteRenderer>();

        // Mémorise la couleur du sprite.
        couleurInitiale = rendu.color;

        // Mémorise l'échelle locale, relative à l'objet parent.
        echelleInitiale = transform.localScale;
    }

    // Méthode publique à appeler depuis le script d'attaque de l'ennemi.
    public void Declencher()
    {
        // Interrompt l'effet précédent s'il est encore en cours.
        // Cela évite que plusieurs effets modifient simultanément le sprite.
        if (effetEnCours != null)
            StopCoroutine(effetEnCours);

        // Lance une nouvelle séquence et conserve sa référence.
        effetEnCours = StartCoroutine(JouerEffet());
    }

    // Une coroutine permet d'exécuter une séquence avec des pauses
    // sans bloquer le reste du jeu.
    private IEnumerator JouerEffet()
    {
        // ÉTAPE 1 : coloration orange et agrandissement immédiat.
        rendu.color = couleurAttaque;
        transform.localScale = echelleInitiale * agrandissement;

        // Suspend cette coroutine pendant 45 % de la durée de référence.
        // WaitForSeconds utilise le temps du jeu : Time.timeScale l'affecte.
        yield return new WaitForSeconds(duree * 0.45f);

        // ÉTAPE 2 : teinte blanche et légère contraction.
        // Color.white affiche les couleurs du sprite sans teinte ajoutée :
        // cela ne transforme pas forcément tout le dessin en blanc.
        rendu.color = Color.white;

        // Réduit l'objet à 92 % de son échelle initiale.
        transform.localScale = echelleInitiale * 0.92f;

        // Attend pendant 25 % de la durée de référence.
        yield return new WaitForSeconds(duree * 0.25f);

        // ÉTAPE 3 : restauration de l'apparence d'origine.
        rendu.color = couleurInitiale;
        transform.localScale = echelleInitiale;

        // Signale qu'aucun effet n'est désormais en cours.
        effetEnCours = null;
    }

    // Appelée notamment lorsque le composant ou son GameObject
    // est désactivé.
    private void OnDisable()
    {
        // Restaure la couleur si le SpriteRenderer est disponible.
        if (rendu != null)
            rendu.color = couleurInitiale;

        // Restaure la taille pour éviter de conserver un agrandissement.
        transform.localScale = echelleInitiale;
    }
}