using UnityEngine;
using System.Collections;

public class AsteroidHazard : MonoBehaviour
{
    private Vector3 moveDirection;
    private float speed;
    public FadeUI fadeUI; // Assign in Inspector or will be found at runtime

    public void Initialize(Vector3 direction, float moveSpeed)
    {
        moveDirection = direction.normalized;
        speed = moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        // If the asteroid hits the ground
        if (other.CompareTag("Ground"))
        {
            DamageAllInGameObjects();
            if (fadeUI == null)
                fadeUI = FindFirstObjectByType<FadeUI>();
            if (fadeUI != null)
            {
                fadeUI.FadeOut(() => {
                    fadeUI.StartCoroutine(FadeBackInAfterDelay(fadeUI, 1f)); // 1 second delay
                });
            }
            Destroy(gameObject);
            return;
        }

        // Check for vulnerable objects by tag or component
        var driller = other.GetComponent<Driller>();
        if (driller != null)
        {
            driller.BreakDown();
            Destroy(gameObject);
            return;
        }

        var rocketCapsule = other.GetComponent<RocketCapsule>();
        if (rocketCapsule != null)
        {
            rocketCapsule.BreakDown();
            Destroy(gameObject);
            return;
        }

        var rocketLaunchPlace = other.GetComponent<RocketLaunchPlace>();
        if (rocketLaunchPlace != null)
        {
            rocketLaunchPlace.BreakDown();
            Destroy(gameObject);
            return;
        }

        var defenseTurret = other.GetComponent<DefenseTurret>();
        if (defenseTurret != null)
        {
            defenseTurret.BreakDown();
            Destroy(gameObject);
            return;
        }

        var sleepPlace = other.GetComponent<SleepPlace>();
        if (sleepPlace != null)
        {
            sleepPlace.BreakDown();
            Destroy(gameObject);
            return;
        }

        var turretShooter = other.GetComponent<TurretShooter>();
        if (turretShooter != null)
        {
            turretShooter.BreakDown();
            Destroy(gameObject);
            return;
        }

        // Add similar checks for RocketCapsule, RocketLaunchPlace, DefenseTurret, SleepPlace, etc.
    }

    void DamageAllInGameObjects()
    {
        var driller = FindFirstObjectByType<Driller>();
        if (driller != null) driller.BreakDown();

        var rocketCapsule = FindFirstObjectByType<RocketCapsule>();
        if (rocketCapsule != null) rocketCapsule.BreakDown();

        var rocketLaunchPlace = FindFirstObjectByType<RocketLaunchPlace>();
        if (rocketLaunchPlace != null) rocketLaunchPlace.BreakDown();

        var defenseTurret = FindFirstObjectByType<DefenseTurret>();
        if (defenseTurret != null) defenseTurret.BreakDown();

        var turretShooter = FindFirstObjectByType<TurretShooter>();
        if (turretShooter != null) turretShooter.BreakDown();

        var sleepPlace = FindFirstObjectByType<SleepPlace>();
        if (sleepPlace != null) sleepPlace.BreakDown();
        // Add more as needed
    }

    private IEnumerator FadeBackInAfterDelay(FadeUI fadeUI, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        // Hide warning icon and text by calling ClearAsteroid on the manager
        var manager = FindFirstObjectByType<AsteroidHazardManager>();
        if (manager != null)
            manager.ClearAsteroid();
        fadeUI.FadeIn();
    }
}
