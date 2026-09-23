using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioJeu : MonoBehaviour
{
    [Header("Volumes")]
    [SerializeField, Range(0f, 1f)] private float volumeEffets = 0.75f;
    [SerializeField, Range(0f, 1f)] private float volumeAmbiance = 0.16f;

    private AudioSource source;
    private AudioSource ambiance;
    private AudioClip lancement;
    private AudioClip collecte;
    private AudioClip impact;
    private AudioClip objectif;
    private AudioClip victoire;
    private AudioClip defaite;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f;
        source.volume = 1f;

        ambiance = gameObject.AddComponent<AudioSource>();
        ambiance.playOnAwake = false;
        ambiance.loop = true;
        ambiance.spatialBlend = 0f;
        ambiance.volume = volumeAmbiance;

        lancement = CreerSon("Lancement", new[] { 220f, 330f, 440f, 660f }, 0.11f);
        collecte = CreerSon("Collecte", new[] { 660f, 880f }, 0.09f);
        impact = CreerSon("Impact", new[] { 180f, 120f }, 0.12f);
        objectif = CreerSon("Objectif", new[] { 440f, 660f, 880f }, 0.11f);
        victoire = CreerSon("Victoire", new[] { 523f, 659f, 784f }, 0.15f);
        defaite = CreerSon("Defaite", new[] { 330f, 247f, 196f }, 0.18f);
        ambiance.clip = CreerAmbiance();
    }

    private void Start()
    {
        ambiance.Play();
    }

    public void JouerLancement() => Jouer(lancement);
    public void JouerCollecte() => Jouer(collecte);
    public void JouerImpact() => Jouer(impact);
    public void JouerObjectif() => Jouer(objectif);
    public void JouerVictoire() => Jouer(victoire);
    public void JouerDefaite() => Jouer(defaite);

    private void Jouer(AudioClip clip)
    {
        if (clip != null) source.PlayOneShot(clip, volumeEffets);
    }

    private static AudioClip CreerSon(string nom, float[] frequences, float dureeNote)
    {
        const int taux = 44100;
        int longueurNote = Mathf.RoundToInt(taux * dureeNote);
        float[] donnees = new float[longueurNote * frequences.Length];
        for (int n = 0; n < frequences.Length; n++)
        for (int i = 0; i < longueurNote; i++)
        {
            float t = i / (float)taux;
            float enveloppe = Mathf.Sin(Mathf.PI * i / longueurNote);
            donnees[n * longueurNote + i] = Mathf.Sin(2f * Mathf.PI * frequences[n] * t)
                * enveloppe * 0.3f;
        }
        AudioClip clip = AudioClip.Create(nom, donnees.Length, 1, taux, false);
        clip.SetData(donnees, 0);
        return clip;
    }

    private static AudioClip CreerAmbiance()
    {
        const int taux = 44100;
        const int duree = 4;
        float[] donnees = new float[taux * duree];
        for (int i = 0; i < donnees.Length; i++)
        {
            float t = i / (float)taux;
            float pulsation = 0.68f + 0.32f * Mathf.Sin(2f * Mathf.PI * 0.5f * t);
            donnees[i] = (Mathf.Sin(2f * Mathf.PI * 55f * t) * 0.11f +
                          Mathf.Sin(2f * Mathf.PI * 82.5f * t) * 0.05f) * pulsation;
        }
        AudioClip clip = AudioClip.Create("AmbianceStation", donnees.Length, 1, taux, false);
        clip.SetData(donnees, 0);
        return clip;
    }
}
