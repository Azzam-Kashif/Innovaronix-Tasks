using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableItem : MonoBehaviour
{
        public bool isPickedUp = false;

        public void PickUp()
        {
            isPickedUp = true;
            // Handle picking up logic here, such as changing the item's state
            // You might want to disable its collider or set it to inactive
            gameObject.SetActive(false);
        }

        public void Place()
        {
            isPickedUp = false;
            // Handle placing logic here, such as changing the item's state back
            // You may want to re-enable the item or move it to a specific location
            gameObject.SetActive(true); // or any other logic to place it
        }
    }