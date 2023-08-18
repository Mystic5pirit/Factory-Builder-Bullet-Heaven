using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemSO", menuName = "ScriptableObjects/ItemSO")]
public class ItemSO: ScriptableObject
{
    /// <summary>
    /// Name of the item
    /// </summary>
    public string ItemName;
    /// <summary>
    /// Mesh of the item
    /// </summary>
    public Mesh ItemMesh;
    /// <summary>
    /// Material of the item
    /// </summary>
    public Material ItemMaterial;
    /// <summary>
    /// How long ResourceGenerator takes to make this item
    /// </summary>
    public float CreationTime = 1;
}
