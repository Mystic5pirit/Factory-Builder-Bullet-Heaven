using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemSO", menuName = "ScriptableObjects/ItemSO")]
public class ItemSO: ScriptableObject
{
    public string ItemName;
    public Mesh ItemMesh;
    public Material ItemMaterial;
    
}
