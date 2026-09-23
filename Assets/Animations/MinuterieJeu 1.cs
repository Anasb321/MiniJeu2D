using UnityEngine; // MonoBehaviour, Time, Mathf, Color et Debug.
using TMPro;       // TMP_Text pour afficher du texte avec TextMeshPro.

// Composant Unity qui gère un compte à rebours.
public class MinuterieJeu : MonoBehaviour
{
    [Header("Configuration")]

    // Durée initiale en secondes : 120 secondes = 2 minutes.
    // SerializeField permet de modifier ce champ privé dans l'Inspector.
    [SerializeField] private float dureeDepart = 120f;

    [Header("Interface")]

    // Texte TextMeshPro à associer dans l'Inspector.
    [SerializeField] private TMP_Text texteTimer;

    // Temps restant en secondes, incluant les fractions de seconde.
    private float tempsRestant;

    // Indique si le compte à rebours doit continuer.
    private bool minuterieActive = true;

    // Appelée une fois avant la première exécution de Update.
    private void Start()
    {
        // Initialise le compte à rebours avec la durée configurée.
        tempsRestant = dureeDepart;

        // Affiche immédiatement le temps initial.
        ActualiserAffichage();
    }

    // Appelée à chaque image lorsque le composant est actif.
    private void Update()
    {
        // Si la minuterie est arrêtée, quitte la méthode.
        if (!minuterieActive)
            return;

        // Retire le temps écoulé depuis l'image précédente.
        // Le décompte suit ainsi le temps du jeu, indépendamment des FPS.
        // Time.deltaTime est influencé par Time.timeScale.
        tempsRestant -= Time.deltaTime;

        // Vérifie si le compte à rebours est terminé.
        if (tempsRestant <= 0f)
        {
            // Empêche l'affichage d'un temps négatif.
            tempsRestant = 0f;

            // Arrête la minuterie pour ne déclencher la fin qu'une fois.
            minuterieActive = false;

            // Déclenche les actions prévues à la fin du temps.
            TempsEcoule();
        }

        // Met à jour le texte et sa couleur.
        ActualiserAffichage();
    }

    // Convertit le temps restant au format minutes:secondes.
    private void ActualiserAffichage()
    {
        // Division par 60 pour obtenir les minutes complètes.
        // FloorToInt arrondit vers l'entier inférieur.
        // Exemple : 95 / 60 = 1,58... → 1 minute.
        int minutes = Mathf.FloorToInt(tempsRestant / 60f);

        // Le modulo (%) donne le reste de la division par 60.
        // Exemple : 95 % 60 = 35 → 35 secondes.
        int secondes = Mathf.FloorToInt(tempsRestant % 60f);

        // L'interpolation ($) insère les valeurs dans le texte.
        // Le format "00" affiche chaque valeur sur au moins deux chiffres.
        // Exemple : 1 minute et 5 secondes → "01:05".
        texteTimer.text = $"{minutes:00}:{secondes:00}";

        // Rouge lorsqu'il reste 10 secondes ou moins.
        // Les composantes rouge, verte et bleue vont ici de 0 à 1.
        if (tempsRestant <= 10f)
            texteTimer.color = new Color(1f, 0.25f, 0.25f);

        // Sinon, orange lorsqu'il reste 30 secondes ou moins.
        else if (tempsRestant <= 30f)
            texteTimer.color = new Color(1f, 0.75f, 0.2f);

        // Bloc désactivé : au-dessus de 30 secondes,
        // le texte conserve sa couleur actuelle.
        // Décommenter ces lignes pour imposer la couleur blanche.
        // else
        //     texteTimer.color = Color.white;
    }

    // Exécute les actions lorsque le temps atteint zéro.
    private void TempsEcoule()
    {
        // Affiche un message dans la console Unity.
        Debug.Log("Temps écoulé!");

        // Prévient le gestionnaire du jeu que le temps est terminé.
        // L'opérateur ?. appelle la méthode seulement si Instance
        // n'est pas une référence null en C#.
        GestionJeu.Instance?.TempsEcoule();
    }
}