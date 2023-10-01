using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : ScriptableObject
{
    public GameObject itemPrefab;
    public Mesh mesh;
    public Material material;
    public Sprite inventorySprite;

    public virtual void Interact()
    {
        Debug.Log("Interacted with: " + name);
    }
    public virtual void SpawnItem(Vector3 position)
    {
        ItemPickup itemObject = Instantiate(itemPrefab).GetComponent<ItemPickup>();
        itemObject.transform.position = position;
        itemObject.Initialize(this);
    }
}
