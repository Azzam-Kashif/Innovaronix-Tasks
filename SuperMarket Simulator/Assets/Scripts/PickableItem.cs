using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableItem : MonoBehaviour
{
    public bool isPickedUp = false;  // Keeps track of whether the item is picked up

    // You can add a function to show highlight or prompt when the item can be picked up.
    public void HighlightItem(bool highlight)
    {
        // Add highlight logic (like changing material or enabling an outline shader)
    }
}
