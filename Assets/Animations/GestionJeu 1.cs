using TMPro;                       // Textes TextMeshPro.
using UnityEngine;                 // Composants et fonctionnalités Unity.
using UnityEngine.SceneManagement; // Chargement et rechargement des scènes.
using UnityEngine.UI;              // Composant Image de la barre de progression.

// Centralise les règles et l'état de la partie.
public class GestionJeu : MonoBehaviour
{
    // Instance unique accessible depuis les autres scripts :
    // GestionJeu.Instance.AjouterBatterie();
    //
    // static : la propriété appartient à la classe.
    // get : les autres scripts peuvent lire Instance.
    // private set : seule cette classe peut modifier Instance.
    public static GestionJeu Instance { get; private set; }

    [Header("Progression")]

    // Nombre de batteries nécessaires pour activer la sortie.
    // SerializeField rend ce champ privé configurable dans l'Inspector.
    [SerializeField] private int objectifBatteries = 3;

    // Nombre de vies au début de la partie.
    [SerializeField] private int viesInitiales = 3;

    [Header("Interface")]

    // Textes affichant la progression et les vies restantes.
    [SerializeField] private TMP_Text texteBatteries;
    [SerializeField] private TMP_Text texteVies;

    // Image dont le remplissage représente la progression.
    [SerializeField] private Image barreProgression;

    // Panneaux affichés à la fin de la partie.
    [SerializeField] private GameObject panneauVictoire;
    [SerializeField] private GameObject panneauDefaite;

    [Header("Niveau")]

    // Porte activée lorsque l'objectif de batteries est atteint.
    [SerializeField] private GameObject porteSortie;

    // Script du joueur permettant de désactiver ses commandes.
    [SerializeField] private MouvementRobot joueur;

    // Script responsable des effets sonores.
    [SerializeField] private AudioJeu audioJeu;

    // Nombre de batteries actuellement collectées.
    private int batteriesCollectees;

    // Nombre de vies restantes.
    private int vies;

    // Empêche de modifier la partie après une victoire ou une défaite.
    private bool partieTerminee;

    // Propriété publique en lecture seule.
    // => signifie ici : retourner la valeur de partieTerminee.
    public bool PartieTerminee => partieTerminee;

    // Retourne true si le nombre de batteries est suffisant.
    // Cette condition est recalculée à chaque lecture de la propriété.
    public bool ObjectifAtteint => batteriesCollectees >= objectifBatteries;

    // Script déclenchant l'effet visuel lorsque le joueur perd une vie.
    [SerializeField] private EffetDegatsJoueur effetDegatsJoueur;

    // Initialise l'instance avant Start.
    private void Awake()
    {
        // Si un autre gestionnaire existe déjà, détruit cet objet.
        // Cela évite d'avoir deux gestionnaires actifs.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Enregistre ce composant comme instance du gestionnaire.
        Instance = this;
    }

    // Initialise la partie.
    private void Start()
    {
        // Rétablit les valeurs de départ.
        vies = viesInitiales;
        batteriesCollectees = 0;
        partieTerminee = false;

        // Cache les panneaux de fin de partie.
        panneauVictoire.SetActive(false);
        panneauDefaite.SetActive(false);

        // Cache et désactive la porte jusqu'à l'atteinte de l'objectif.
        porteSortie.SetActive(false);

        // Affiche les valeurs initiales dans l'interface.
        ActualiserInterface();

        // Joue le son de lancement si audioJeu n'est pas null en C#.
        // ?. est l'opérateur d'accès conditionnel.
        audioJeu?.JouerLancement();
    }

    // Ajoute des batteries au compteur.
    // Sans argument, la valeur ajoutée est 1 :
    // AjouterBatterie() équivaut à AjouterBatterie(1).
    public void AjouterBatterie(int valeur = 1)
    {
        // Ignore la collecte si la partie est terminée.
        if (partieTerminee) return;

        // Augmente le compteur.
        batteriesCollectees += valeur;

        // Joue le son de collecte.
        audioJeu?.JouerCollecte();

        // Met à jour le compteur et la barre.
        ActualiserInterface();

        // Vérifie si le joueur a suffisamment de batteries.
        if (ObjectifAtteint)
        {
            // Rend la sortie visible et active.
            porteSortie.SetActive(true);

            // Joue le son indiquant que l'objectif est atteint.
            audioJeu?.JouerObjectif();
        }
    }

    // Retire une vie, par exemple après une collision avec un ennemi.
    public void PerdreVie()
    {
        // Ne retire plus de vies après la fin de la partie.
        if (partieTerminee)
            return;

        // Retire une vie sans descendre sous zéro.
        // Mathf.Max retourne la plus grande des deux valeurs.
        vies = Mathf.Max(vies - 1, 0);

        // Joue le son d'impact.
        audioJeu?.JouerImpact();

        // Déclenche l'effet visuel si le composant est disponible.
        if (effetDegatsJoueur != null)
            effetDegatsJoueur.DeclencherEffet();

        // Actualise notamment le texte des vies.
        ActualiserInterface();

        // Si aucune vie ne reste, termine la partie par une défaite.
        if (vies == 0)
            DeclencherDefaite();
    }

    // Peut être appelée lorsque le joueur atteint la sortie.
    public void DeclencherVictoire()
    {
        // Refuse la victoire si :
        // - la partie est déjà terminée;
        // - OU l'objectif de batteries n'est pas atteint.
        // || signifie OU et ! signifie NON.
        if (partieTerminee || !ObjectifAtteint) return;

        // Marque la partie comme terminée.
        partieTerminee = true;

        // Affiche le panneau de victoire.
        panneauVictoire.SetActive(true);

        // Empêche le joueur de continuer à utiliser ses commandes.
        joueur.DesactiverCommandes();

        // Joue le son de victoire.
        audioJeu?.JouerVictoire();
    }

    // Termine la partie par une défaite.
    public void DeclencherDefaite()
    {
        // Évite de déclencher plusieurs fois la fin de partie.
        if (partieTerminee) return;

        partieTerminee = true;

        // Affiche le panneau de défaite.
        panneauDefaite.SetActive(true);

        // Désactive les commandes du joueur.
        joueur.DesactiverCommandes();

        // Joue le son de défaite.
        audioJeu?.JouerDefaite();
    }

    // Méthode appelée par MinuterieJeu lorsque le temps atteint zéro.
    // Écriture raccourcie d'une méthode appelant DeclencherDefaite().
    public void TempsEcoule() => DeclencherDefaite();

    // Synchronise les éléments de l'interface avec l'état du jeu.
    private void ActualiserInterface()
    {
        // Exemple : "Batteries : 2/3".
        texteBatteries.text =
            $"Batteries : {batteriesCollectees}/{objectifBatteries}";

        // Exemple : "Vies : 2".
        texteVies.text = $"Vies : {vies}";

        // Met à jour la barre uniquement si elle est assignée.
        if (barreProgression != null)
            // L'opérateur ternaire suit la forme :
            // condition ? valeurSiVrai : valeurSiFaux.
            //
            // Si l'objectif est positif, calcule la proportion collectée.
            // Le cast (float) permet une division avec des décimales :
            // 2 / 3 donnerait 0 avec des entiers;
            // (float)2 / 3 donne environ 0,67.
            //
            // Sinon, utilise 0 pour éviter une division par zéro.
            barreProgression.fillAmount = objectifBatteries > 0
                ? (float)batteriesCollectees / objectifBatteries
                : 0f;
    }

    // Peut être associée au OnClick d'un bouton "Recommencer".
    public void RecommencerPartie()
    {
        // Recharge la scène active à partir de son index.
        // Les objets de la scène sont recréés et la partie réinitialisée.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}