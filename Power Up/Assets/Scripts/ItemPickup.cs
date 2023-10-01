using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : Interactable
{
    public Item item;
    public override void Interact()
    {
        item.Interact();
        Destroy(gameObject);
    }
    public virtual void Initialize(Item newItem)
    {
        item = newItem;
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        meshFilter.sharedMesh = item.mesh;
        meshRenderer.material = item.material;
        boxCollider.size = meshRenderer.localBounds.size;
        boxCollider.center = meshRenderer.localBounds.center;
    }
}
