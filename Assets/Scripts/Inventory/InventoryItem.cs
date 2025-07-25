using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InventoryItem {

    public string ItemType {get; set;}

    public virtual int GetItemScore() => 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
