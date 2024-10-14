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
}
