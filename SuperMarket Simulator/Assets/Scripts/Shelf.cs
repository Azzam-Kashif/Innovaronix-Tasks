using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shelf : MonoBehaviour
{
    [SerializeField] Transform[] shelfSlots;  // Array of predefined shelf slots
    private bool[] slotOccupied;


    void Start()
    {
        slotOccupied = new bool[shelfSlots.Length];
    }

    // Check if there is an available slot
    public bool HasAvailableSlot(out Transform availableSlot)
    {
        for (int i = 0; i < shelfSlots.Length; i++)
        {
            if (!slotOccupied[i])
            {
                availableSlot = shelfSlots[i];
                return true;
            }
        }

        availableSlot = null;
        return false;
    }

    // Mark a slot as occupied
    public void OccupySlot(Transform slot)
    {
        int index = System.Array.IndexOf(shelfSlots, slot);
        if (index >= 0)
        {
            slotOccupied[index] = true;
        }
    }
    public PickableItem GetItemFromSlot(Transform slot)
    {
        // Ensure the slot and item exist
        if (slot == null)
        {
            Debug.LogWarning("Slot is null.");
            return null;
        }

        PickableItem item = slot.GetComponentInChildren<PickableItem>();
        if (item == null)
        {
            Debug.LogWarning("No PickableItem found in slot: " + slot.name);
        }
        else
        {
            Debug.Log("PickableItem found in slot: " + slot.name);
        }
        return item;
    }
}