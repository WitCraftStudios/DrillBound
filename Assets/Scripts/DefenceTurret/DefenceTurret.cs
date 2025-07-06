using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public class DefenseTurret : MonoBehaviour
{
    public GameObject minigameUIPanel; // Assign in inspector (the minigame panel)
    public GameObject emptyUIPanel;    // Assign in inspector (panel with just an image/message)
    public AsteroidHazardManager hazardManager; // Assign in inspector
    public bool isBroken = false;
    public float repairTime = 10f;
    public Image repairCircle; // Assign in Inspector
    private bool playerNearby = false;
    private bool isRepairing = false;
    private float repairTimer = 0f;
    public GameObject interactionPrompt;
    public GameObject smokeParticle;

    void Update()
    {
        if (isBroken && playerNearby && !isRepairing && Keyboard.current.rKey.wasPressedThisFrame)
        {
            StartCoroutine(RepairRoutine());
        }
        if (isRepairing && repairCircle != null)
        {
            repairCircle.fillAmount = repairTimer / repairTime;
        }
        if (isBroken) {
            smokeParticle.SetActive(true);
            if (playerNearby && Keyboard.current.rKey.wasPressedThisFrame)
            {
                Debug.Log("DefenseTurret is broken! Repair it first.");
            }
            return;
        }
        else if (!isBroken)
        {
            smokeParticle.SetActive(false);
        }
        if (playerNearby && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (hazardManager != null && hazardManager.IsHazardActive())
            {
                if (minigameUIPanel != null)
                    minigameUIPanel.SetActive(true);

                hazardManager.ClearAsteroid();
            }
            else
            {
                if (emptyUIPanel != null)
                    emptyUIPanel.SetActive(true);
            }
        }
    }

    private IEnumerator RepairRoutine()
    {
        isRepairing = true;
        repairTimer = 0f;
        if (repairCircle != null)
        {
            repairCircle.gameObject.SetActive(true);
            repairCircle.fillAmount = 0f;
        }
        while (repairTimer < repairTime)
        {
            repairTimer += Time.deltaTime;
            if (repairCircle != null)
                repairCircle.fillAmount = repairTimer / repairTime;
            yield return null;
        }
        isBroken = false;
        isRepairing = false;
        if (repairCircle != null)
            repairCircle.gameObject.SetActive(false);
        Debug.Log("DefenseTurret repaired!");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            interactionPrompt.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            interactionPrompt.SetActive(false);
        }
    }

    public void BreakDown()
    {
        isBroken = true;
        Debug.Log("DefenseTurret damaged by asteroid!");
        // Add additional damage logic here
    }

    public void Repair()
    {
        if (!isBroken || isRepairing) return;
        StartCoroutine(RepairRoutine());
    }
}
