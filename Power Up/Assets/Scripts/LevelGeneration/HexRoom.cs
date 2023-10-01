using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexRoom : MonoBehaviour
{
    public int weight = 1;
    [SerializeField] private bool[] passages = new bool [6];
    public bool[] Passages { get { return passages; } }
}
