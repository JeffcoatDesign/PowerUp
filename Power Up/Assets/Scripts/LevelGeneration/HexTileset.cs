using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Hex Tileset", menuName ="Hex/Tileset")]
public class HexTileset : ScriptableObject
{
    public Tile[] corridors;
    public HexRoom[] hexRooms;
    private int m_corridorTotalWeight = 0;

    public void CalculateWeights ()
    {
        m_corridorTotalWeight = 0;
        foreach (Tile tile in corridors)
        {
            m_corridorTotalWeight += tile.weight;
        }
    }
    public GameObject GetCorridor()
    {
        int randomWeight = Random.Range(0, m_corridorTotalWeight);
        int weightCount = 0;
        for (int i = 0; i < corridors.Length; i++)
        {
            weightCount += corridors[i].weight;
            if (randomWeight <= weightCount)
            {
                return corridors[i].Prefab;
            }
        }
        return null;
    }

    public HexRoom GetRoom(bool[] map)
    {
        List<HexRoom> rooms = new List<HexRoom>();
        int weightSum = 0;
        for (int i = 0; i < hexRooms.Length; i++) {
            bool[] passages = hexRooms[i].Passages;
            if (AreBoolArrsEqual(map, passages)) {
                rooms.Add(hexRooms[i]);
                weightSum += hexRooms[i].weight;
            }
        }
        int randomWeight = Random.Range(0, weightSum);
        int weightCount = 0;
        for (int i = 0; i < rooms.Count; i++) {
            weightCount += rooms[i].weight;
            if (randomWeight <= weightCount)
                return rooms[i];
        }
        return null;
    }

    private bool AreBoolArrsEqual (bool[] bool1, bool[] bool2)
    {
        if (bool1.Length != bool2.Length) return false;
        bool value = true;
        for (int i = 0; i < bool1.Length; i++)
        {
            if (bool1[i] != bool2[i]) {
                value = false;
                break;
            }
        }
        return value;
    }
}

[System.Serializable]
public struct Tile {
    [SerializeField] private GameObject prefab;
    public int weight;
    public GameObject Prefab { get { return prefab; } }

    public Tile (GameObject prefab, int weight)
    {
        this.prefab = prefab;
        this.weight = weight;
    }
}