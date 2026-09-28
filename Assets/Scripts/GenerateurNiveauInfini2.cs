using System.Collections.Generic;
using UnityEngine;

public class GenerateurNiveauInfini2 : MonoBehaviour
{
    [Header("--- Références ---")]
    [SerializeField] private Transform joueur;

    [Header("--- Prefabs ---")]
    [SerializeField] private List<GameObject> prefabsPlateformes;
    [SerializeField] private List<GameObject> prefabsCristaux;
    [SerializeField] private GameObject prefabBombe;
    [SerializeField] private List<GameObject> prefabsRobots; // <--- AJOUT : Prefabs des robots

    [Header("--- Probabilités (%) ---")]
    [Range(0f, 100f)][SerializeField] private float chanceCristal = 80f;
    [Range(0f, 100f)][SerializeField] private float chanceBombe = 40f;
    [Range(0f, 100f)][SerializeField] private float chanceRobot = 35f; // <--- AJOUT : Chance d'avoir un robot

    [Header("--- Sécurité au Départ ---")]
    [Tooltip("Nombre de plateformes créées sans bombe ni robot au début")]
    [SerializeField] private int plateformesSansDanger = 4;

    [Header("--- Réglages Espacement ---")]
    [SerializeField] private float espacementMin = 2.5f;
    [SerializeField] private float espacementMax = 4.0f;
    [SerializeField] private float variationHauteurMax = 1.0f;
    [SerializeField] private float hauteurAbsolueMin = -3.5f;
    [SerializeField] private float hauteurAbsolueMax = 2.5f;

    [Header("--- Portée de Génération ---")]
    [SerializeField] private float distanceApparition = 25f;  // Distance devant le joueur
    [SerializeField] private float distanceDestruction = 15f; // Distance derrière le joueur

    private List<GameObject> plateformesActives = new List<GameObject>();
    private int compteurPlateformes = 0;

    private void Start()
    {
        if (joueur == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) joueur = playerObj.transform;
        }

        if (joueur == null)
        {
            Debug.LogError("Générateur : Joueur introuvable ! Assigne-le dans l'Inspector.");
            return;
        }

        // Crée la toute première plateforme sous le joueur
        Vector3 posDepart = new Vector3(joueur.position.x, joueur.position.y - 1.2f, 0f);
        GenererPlateformeInitiale(posDepart);
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameOver) return;
        if (joueur == null) return;

        if (plateformesActives.Count > 0)
        {
            Transform dernierePlateforme = plateformesActives[plateformesActives.Count - 1].transform;

            if (dernierePlateforme.position.x < joueur.position.x + distanceApparition)
            {
                GenererPlateformeSuivante(dernierePlateforme.position);
            }
        }

        NettoyerAnciennesPlateformes();
    }

    private void GenererPlateformeInitiale(Vector3 position)
    {
        if (prefabsPlateformes == null || prefabsPlateformes.Count == 0) return;

        GameObject prefabDepart = prefabsPlateformes[0];
        GameObject obj = Instantiate(prefabDepart, position, Quaternion.identity, transform);

        plateformesActives.Add(obj);
        compteurPlateformes = 1;
    }

    private void GenererPlateformeSuivante(Vector3 posAncienne)
    {
        if (prefabsPlateformes == null || prefabsPlateformes.Count == 0) return;

        GameObject prefabChoisi = prefabsPlateformes[Random.Range(0, prefabsPlateformes.Count)];

        float espacement = Random.Range(espacementMin, espacementMax);
        float x = posAncienne.x + espacement;

        float deltaY = Random.Range(-variationHauteurMax, variationHauteurMax);
        float y = Mathf.Clamp(posAncienne.y + deltaY, hauteurAbsolueMin, hauteurAbsolueMax);

        Vector3 nouvellePos = new Vector3(x, y, 0f);
        GameObject nouvellePlateforme = Instantiate(prefabChoisi, nouvellePos, Quaternion.identity, transform);
        plateformesActives.Add(nouvellePlateforme);

        compteurPlateformes++;

        bool zoneSecurisee = compteurPlateformes <= (plateformesSansDanger + 1);

        // 1. Cristaux sur la plateforme
        if (prefabsCristaux != null && prefabsCristaux.Count > 0 && Random.Range(0f, 100f) <= chanceCristal)
        {
            GenererCristauxSurPlateforme(nouvellePlateforme);
        }

        // 2. Bombes dans l'arc de saut
        if (!zoneSecurisee && prefabBombe != null && Random.Range(0f, 100f) <= chanceBombe)
        {
            GenererBombeDansArc(posAncienne, nouvellePos, nouvellePlateforme.transform);
        }

        // 3. Robots sur la plateforme (AJOUT)
        if (!zoneSecurisee && prefabsRobots != null && prefabsRobots.Count > 0 && Random.Range(0f, 100f) <= chanceRobot)
        {
            GenererRobotSurPlateforme(nouvellePlateforme);
        }
    }

    private void GenererCristauxSurPlateforme(GameObject plateforme)
    {
        GameObject prefabCristal = prefabsCristaux[Random.Range(0, prefabsCristaux.Count)];
        int nombreCristaux = Random.Range(1, 3);

        for (int i = 0; i < nombreCristaux; i++)
        {
            float offsetX = (nombreCristaux == 1) ? 0f : (i == 0 ? -0.35f : 0.35f);
            float offsetY = 1.15f;

            Vector3 posLocale = new Vector3(offsetX, offsetY, 0f);
            Instantiate(prefabCristal, plateforme.transform.position + posLocale, Quaternion.identity, plateforme.transform);
        }
    }

    private void GenererBombeDansArc(Vector3 posDepart, Vector3 posArrivee, Transform parent)
    {
        float midX = (posDepart.x + posArrivee.x) / 2f;
        float hauteurMaxObstacle = Mathf.Max(posDepart.y, posArrivee.y) + Random.Range(1.0f, 1.4f);

        Vector3 posBombe = new Vector3(midX, hauteurMaxObstacle, 0f);
        Instantiate(prefabBombe, posBombe, Quaternion.identity, parent);
    }

    // AJOUT : Génération du Robot
    private void GenererRobotSurPlateforme(GameObject plateforme)
    {
        GameObject prefabRobot = prefabsRobots[Random.Range(0, prefabsRobots.Count)];
        Vector3 posLocale = new Vector3(0f, 1.0f, 0f); // Posé sur la plateforme
        Instantiate(prefabRobot, plateforme.transform.position + posLocale, Quaternion.identity, plateforme.transform);
    }

    private void NettoyerAnciennesPlateformes()
    {
        for (int i = plateformesActives.Count - 1; i >= 0; i--)
        {
            if (plateformesActives[i] != null)
            {
                if (plateformesActives[i].transform.position.x < joueur.position.x - distanceDestruction)
                {
                    Destroy(plateformesActives[i]);
                    plateformesActives.RemoveAt(i);
                }
            }
        }
    }
}