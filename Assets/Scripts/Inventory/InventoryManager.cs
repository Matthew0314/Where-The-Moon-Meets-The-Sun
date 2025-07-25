using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{

    private static List<InventoryItem>[] Inventory = new List<InventoryItem>[6];

    void Awake() {

        for (int i = 0; i < Inventory.Length; i++)
        {
            Inventory[i] = new List<InventoryItem>();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SortFullInventory() {
        foreach (var itemList in Inventory)
        {
            itemList.Sort((a, b) => a.GetItemScore().CompareTo(b.GetItemScore()));
        }
    }

    void SortInventorySection(int index) {
        if (index < Inventory.Length && index >= 0) {
            Inventory[index].Sort((a, b) => a.GetItemScore().CompareTo(b.GetItemScore()));
        }
    }
}
