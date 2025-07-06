using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;

public class AsteroidMinigameManager : MonoBehaviour
{
    public GameObject asteroidPrefab; // Assign the UI asteroid prefab
    public RectTransform playArea;    // Assign the panel's RectTransform
    public RectTransform scope;       // Assign the scope RectTransform
    public RectTransform crosshair;   // Assign the crosshair RectTransform in Inspector
    public RectTransform asteroidHolder; // Assign in Inspector
    public int minAsteroids = 15;
    public int maxAsteroids = 30;
    public AsteroidHazardManager hazardManager; // Assign in inspector
    public float minigameDuration = 10f; // seconds
    [Header("UI References")]
    public GameObject uiRoot; // Assign your main UI GameObject in the Inspector

    private List<GameObject> spawnedAsteroids = new List<GameObject>();
    private bool minigameActive = false;
    private float minigameTimer = 0f;
    private Driller driller;
    private RocketCapsule rocketCapsule;
    private int totalAsteroidsToSpawn = 0;
    private int destroyedAsteroids = 0;

    void OnEnable()
    {
        Cursor.visible = false;
        if (uiRoot != null) uiRoot.SetActive(false);
        StartMinigame();
    }

    void OnDisable()
    {
        ClearMinigame();
    }

    // Update is called once per frame
    void Update()
    {
        if (!minigameActive) return;

        // Timer countdown
        minigameTimer -= Time.deltaTime;
        if (minigameTimer <= 0f)
        {
            EndMinigame();
            return;
        }

        // Only destroy asteroids if mouse is clicked AND crosshair overlaps asteroid
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            foreach (var asteroid in new List<GameObject>(spawnedAsteroids))
            {
                RectTransform asteroidRect = asteroid.GetComponent<RectTransform>();
                if (RectOverlaps(crosshair, asteroidRect))
                {
                    Destroy(asteroid);
                    OnAsteroidDestroyed(asteroid);
                }
            }
        }
    }

    void StartMinigame()
    {
        minigameActive = true;
        minigameTimer = minigameDuration;
        driller = FindFirstObjectByType<Driller>();
        rocketCapsule = FindFirstObjectByType<RocketCapsule>();
        if (hazardManager != null) hazardManager.PauseHazard();
        int asteroidCount = Random.Range(minAsteroids, maxAsteroids + 1);
        totalAsteroidsToSpawn = asteroidCount;
        destroyedAsteroids = 0;
        StartCoroutine(SpawnAsteroidsInWaves(asteroidCount));
    }

    System.Collections.IEnumerator SpawnAsteroidsInWaves(int totalToSpawn)
    {
        int spawned = 0;
        while (spawned < totalToSpawn)
        {
            int batch = Random.Range(2, 4); // 2 or 3 asteroids per batch
            batch = Mathf.Min(batch, totalToSpawn - spawned);
            for (int i = 0; i < batch; i++)
            {
                GameObject asteroid = Instantiate(asteroidPrefab, asteroidHolder); // parent is asteroidHolder
                RectTransform rect = asteroid.GetComponent<RectTransform>();
                // Use scope (background image) for bounds, not crosshair
                float minY = scope.anchoredPosition.y - (scope.rect.height / 2f) + 24;
                float maxY = scope.anchoredPosition.y + (scope.rect.height / 2f) - 24;
                float y = Random.Range(minY, maxY);
                float x = scope.anchoredPosition.x - (scope.rect.width / 2f) + 24; // left edge of scope with padding
                rect.anchoredPosition = new Vector2(x, y);
                // Initialize movement
                var mover = asteroid.GetComponent<MinigameAsteroidMover>();
                if (mover != null) mover.Initialize(playArea, this);
                spawnedAsteroids.Add(asteroid);
                spawned++;
            }
            yield return new WaitForSeconds(0.7f); // Adjust interval as needed
        }
    }

    void ClearMinigame()
    {
        foreach (var asteroid in spawnedAsteroids)
        {
            if (asteroid != null)
                Destroy(asteroid);
        }
        spawnedAsteroids.Clear();
        minigameActive = false;
    }

    void EndMinigame()
    {
        if (!minigameActive) return; // Only run once
        minigameActive = false;
        Cursor.visible = true;
        // Hide the minigame panel
        gameObject.SetActive(false);
        if (hazardManager != null) hazardManager.ResumeHazard();
        if (uiRoot != null) uiRoot.SetActive(true);
        // Optionally, notify the hazard manager or resume the main game here
        Debug.Log("Minigame complete!");
    }

    public void OnAsteroidDestroyed(GameObject asteroid, bool missed = false)
    {
        spawnedAsteroids.Remove(asteroid);
        destroyedAsteroids++;
        if (missed)
        {
            if (driller != null) driller.BreakDown();
            if (rocketCapsule != null) rocketCapsule.BreakDown();
        }
        // End minigame if all asteroids are destroyed
        if (minigameActive && destroyedAsteroids >= totalAsteroidsToSpawn)
        {
            EndMinigame();
        }
    }

    // Helper to check if two RectTransforms overlap in world space
    private bool RectOverlaps(RectTransform a, RectTransform b)
    {
        Vector3[] aCorners = new Vector3[4];
        Vector3[] bCorners = new Vector3[4];
        a.GetWorldCorners(aCorners);
        b.GetWorldCorners(bCorners);
        Rect aRect = new Rect(aCorners[0], aCorners[2] - aCorners[0]);
        Rect bRect = new Rect(bCorners[0], bCorners[2] - bCorners[0]);
        return aRect.Overlaps(bRect);
    }
}
