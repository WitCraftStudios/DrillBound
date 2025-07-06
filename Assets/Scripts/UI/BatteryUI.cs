using UnityEngine;
using TMPro;

public class BatteryUI : MonoBehaviour
{
    public TMP_Text batteryCountText; // Assign in Inspector
    private PlayerInventory playerInventory;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInventory = FindFirstObjectByType<PlayerInventory>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInventory != null)
        {
            batteryCountText.text = playerInventory.batteryCount.ToString();
        }
    }
}
