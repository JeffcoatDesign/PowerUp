using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using MapUtility;
public class LevelGenerator : MonoBehaviour
{
    public delegate void GetNavMesh();
    public delegate void LevelGenerated();
    public static event GetNavMesh OnGetNavmesh;
    public static event LevelGenerated OnLevelGenerated;
    [SerializeField] private HexGrid hexGrid;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject portalPrefab;

    void Start()
    {
        StartCoroutine(GenerateDungeon());
    }

    IEnumerator GenerateDungeon()
    {
        yield return hexGrid.GenerateMap(); 
        yield return hexGrid.GeneratePaths();
        GameObject player = Instantiate(playerPrefab);
        player.transform.position = hexGrid.StartCell.SpawnPosition;
        player.transform.localRotation = GetCorridorDirection(hexGrid.StartCell);
        GameObject portal = Instantiate(portalPrefab);
        portal.transform.position = hexGrid.EndCell.SpawnPosition;
        portal.transform.localRotation = GetCorridorDirection(hexGrid.EndCell);
        if (OnGetNavmesh != null) OnGetNavmesh();
        if (OnLevelGenerated != null) OnLevelGenerated();
    }
    private Quaternion GetCorridorDirection(HexCell cell)
    {
        HexDirection direction = HexDirection.NE;
        for (HexDirection i = HexDirection.NE; i <= HexDirection.NW; i++)
        {
            if (cell.GetNeighbor(i) != null && cell.GetNeighbor(i).isPath)
            {
                direction = i;
                break;
            }
        }
        return Quaternion.Euler(0, 30 + 60 * (int)direction, 0);
    }
}
