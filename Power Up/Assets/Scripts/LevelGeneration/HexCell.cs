using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MapUtility;
public class HexCell : MonoBehaviour
{
    public Vector2Int arrayCoords;
    public HexCoordinates coordinates;
    public HexGrid hexGrid;
    [SerializeField] private HexCell[] neighbors;
    [SerializeField] private GameObject[] sides;
    [SerializeField] private Transform spawnPos;
    public bool isPath = false;
    public bool isBlocked = false;
    public bool isDecentivized = false;
    public bool isDebugActive = true;
    public int distance;
    public HexCell PathFrom { get; set; }
    public int SearchHueristic { get; set; }
    public Vector3 SpawnPosition { get { return spawnPos.position; } }
    public int SearchPriority { get { return distance + SearchHueristic; } }

    private void Awake()
    {
        neighbors = new HexCell[6];
        sides = new GameObject[6];
    }

    [ContextMenu("Check Neighbors")]
    public void CheckNeighbors()
    {
        for (int i = 0; i < neighbors.Length; i++)
        {
            if (isPath)
            {
                if (neighbors[i] != null && neighbors[i].isPath) SetSide((HexDirection)i, hexGrid.Tileset.GetCorridor());
            }
        }
        if (!isPath) return;
        HexDirection direction = HexDirection.NE;
        bool[] pathMap = RotateToFirstPath(ref direction);
        HexRoom room = Instantiate(hexGrid.Tileset.GetRoom(pathMap), transform);
        room.transform.localRotation = Quaternion.Euler(0, 60 * (int)direction, 0);
    }

    public HexCell GetNeighbor(HexDirection direction)
    {
        return neighbors[(int)direction];
    }

    public void SetNeighbor(HexCell cell, HexDirection direction)
    {
        if (cell == null) return;

        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }

    private bool[] GetNeighboringPaths ()
    {
        bool[] boolList = new bool[6];
        for (int i = 0; i < 6; i++)
        {
            if (neighbors[i] != null)
                boolList[i] = neighbors[i].isPath;
            else
                boolList[i] = false;
        }
        return boolList;
    }

    private bool[] RotateToFirstPath (ref HexDirection dir)
    {
        bool[] neighborMap = GetNeighboringPaths();
        List<bool> rotatedBool = new List<bool>();
        for (int i = 0; i < 6; i++)
            rotatedBool.Add(neighborMap[i]);
        for (int i = 0; i < 6; i++)
        {
            if (!rotatedBool[0]) {
                rotatedBool.RemoveAt(0);
                rotatedBool.Add(false);
            }
            else {
                dir = (HexDirection)i;
                break;
            }
        }
        return rotatedBool.ToArray();
    }

    public void SetSide(HexDirection direction, GameObject prefab)
    {
        if (sides[(int)direction] != null)
        {
            return;
        }
        GameObject sideObject = Instantiate(prefab, transform);
        sideObject.transform.localRotation = Quaternion.Euler(0, 30 + 60 * (int)direction, 0);
        sides[(int)direction] = sideObject;
        if (neighbors[(int)direction] != null) neighbors[(int)direction].sides[(int)direction.Opposite()] = sideObject;
    }
}