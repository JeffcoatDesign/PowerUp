using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using MapUtility;

public class HexGrid : MonoBehaviour
{
    public int numPoints = 5;
    public int width = 6;
    public int height = 6;
    public float noiseScale = 0.5f;
    public float minimumDistance = 3f;
    public float tileScale = 1f;

    [SerializeField] private HexCell cellPrefab;
    [SerializeField] private HexTileset tileset;
    [SerializeField] private HexCell _startCell;
    public HexCell StartCell { get { return _startCell; } }
    [SerializeField] private HexCell endCell;
    public HexCell EndCell { get { return endCell; } }
    public HexTileset Tileset { get{ return tileset; } }

    private HexCell[,] cells;
    public HexCell[,] Cells { get { return cells; } }
    public List<HexCell> Nodes;

    private void Awake()
    {
        tileset.CalculateWeights();
    }

    public IEnumerator GenerateMap()
    {
        cells = new HexCell[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                StartCoroutine(CreateCell(x, z));
                yield return null;
            }
        }
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                HexCell cell = cells[x, z];
                if (x > 0) cell.SetNeighbor(cells[x - 1, z], HexDirection.W);
                if (z > 0)
                {
                    if ((z & 1) == 0)
                    {
                        cell.SetNeighbor(cells[x, z - 1], HexDirection.SE);
                        if (x > 0) cell.SetNeighbor(cells[x - 1, z - 1], HexDirection.SW);
                    }
                    else
                    {
                        cell.SetNeighbor(cells[x, z - 1], HexDirection.SW);
                        if (x < width - 1) cell.SetNeighbor(cells[x + 1, z - 1], HexDirection.SE);
                    }
                }
            }
        }
        yield return null;
    }

    private IEnumerator CreateCell (int x, int z)
    {
        Vector3 scale = Vector3.one * HexMetrics.hexScale;
        Vector3 position;
        position.x = (x + z * 0.5f - z/2) * (HexMetrics.innerRadius * 2f) * HexMetrics.hexScale * tileScale;
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f) * HexMetrics.hexScale * tileScale;

        HexCell cell = cells[x,z] = Instantiate(cellPrefab);
        cell.hexGrid = this;
        cell.transform.localScale = scale;
        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.arrayCoords = new Vector2Int(x, z);
        yield return null;
    }

    public IEnumerator GeneratePaths ()
    {
        float[,] noiseMap = MapGeneration.GenerateNoisemap(width, height, noiseScale);
        for (int z = 0; z < height; z++) {
            for (int x = 0; x < width; x++) {
                HexCell cell = cells[x, z];
                if (noiseMap[x, z] > .4f) {
                    cell.isDecentivized = true;
                    yield return null;
                }
            }
        }

        List<Vector2> points = GetRandomPoints(width, height);
        List<Vector2Pair> pointPairs = MapGeneration.GeneratePointPairs(points);
        foreach (Vector2Pair pointPair in pointPairs)
        {
            cells[pointPair.Point1.x, pointPair.Point1.y].isDecentivized = false;
            cells[pointPair.Point2.x, pointPair.Point2.y].isDecentivized = false;
            yield return FindPathFromVectorPair(pointPair);
        }
        yield return CheckForPaths();
        yield return FindFurthestPoints();
    }

    public List<Vector2> GetRandomPoints (int w, int h)
    {
        List<Vector2> points = new();
        for (int i = 0; i < numPoints; i++)
        {
            points.Add(GetRandomPointNotInList(w, h, points));
        }
        return points;
    }

    public Vector2 GetRandomPointNotInList (int w, int h, List<Vector2> list)
    {
        List<Vector2> unoccupied = new();
        for (int x = 0; x < w; x++)
        {
            for (int z = 0; z < h; z++)
            {
                unoccupied.Add(new(x, z));
            }
        }
        foreach (Vector2 vector in list)
        {
            unoccupied.Remove(vector);
        }
        int i = Random.Range(0, unoccupied.Count);
        Vector2 point = unoccupied[i];
        bool foundUniquePoint = false;
        int safety = 0;
        while (!foundUniquePoint)
        {
            safety += 1;
            if (safety > 100000) break;
            bool pointIsFar = true;
            i = Random.Range(0, unoccupied.Count);
            point = unoccupied[i];
            foreach (Vector2 pt in list)
            {
                if (Vector2.Distance(point, pt) < minimumDistance)
                {
                    unoccupied.Remove(point);
                    pointIsFar = false;
                }
            }
            foundUniquePoint = pointIsFar;
        }
        return point;
    }

    public Vector2 GetRandomPoint(int w, int h)
    {
        int x = Random.Range(0, w);
        int z = Random.Range(0, h);
        return new(x, z);
    }

    public IEnumerator FindPathFromVectorPair (Vector2Pair pair)
    {
        HexCell startCell = cells[pair.Point1.x, pair.Point1.y];
        HexCell endCell = cells[pair.Point2.x, pair.Point2.y];
        startCell.isPath = true;
        endCell.isPath = true;
        yield return FindPath(startCell, endCell);
    }

    public IEnumerator FindPath (HexCell fromCell, HexCell toCell)
    {
        if (!Nodes.Contains(fromCell)) Nodes.Add(fromCell);
        if (!Nodes.Contains(toCell)) Nodes.Add(toCell);
        yield return Search(fromCell, toCell);
    }

    public IEnumerator CheckForPaths ()
    {
        foreach (HexCell cell in cells)
        {
            yield return null;
            cell.CheckNeighbors();
        }
    }

    private IEnumerator Search (HexCell fromCell, HexCell toCell)
    {
        for (int x = 0; x < width; x++) {
            for (int z = 0; z < height; z++) {
                cells[x, z].distance = int.MaxValue;
            }
        }
        List<HexCell> frontier = new List<HexCell>();
        fromCell.distance = 0;
        frontier.Add(fromCell);
        while (frontier.Count > 0)
        {
            HexCell current = frontier[0];
            frontier.RemoveAt(0);

            if (current == toCell) {
                current = current.PathFrom;
                while (current != fromCell)
                {
                    yield return null;
                    current.isPath = true;
                    current.isDecentivized = false;
                    current = current.PathFrom;
                }
                break;
            }
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                HexCell neighbor = current.GetNeighbor(d);
                if (neighbor == null) continue;
                if (neighbor.isBlocked) continue;
                yield return null;
                int distance = current.distance;
                if (current.isDecentivized)
                    distance += 10;
                else if (!current.isPath)
                    distance += 2;
                else
                    distance += 1;
                if (neighbor.distance == int.MaxValue) {
                    neighbor.distance = distance;
                    neighbor.PathFrom = current;
                    neighbor.SearchHueristic = 
                        neighbor.coordinates.DistanceTo(toCell.coordinates);
                    frontier.Add(neighbor);
                }
                else if (distance < neighbor.distance)
                {
                    neighbor.distance = distance;
                    neighbor.PathFrom = current;
                }
                frontier.Sort((x, y) => x.SearchPriority.CompareTo(y.SearchPriority));
            }
        }
        yield return null;
        ResetDebug();
    }

    private IEnumerator FindFurthestPoints ()
    {
        HexCell start = null;
        HexCell end = null;
        int greatestLength = int.MinValue;
        foreach (HexCell cell in Nodes)
        {
            foreach (HexCell otherCell in Nodes)
            {
                if (cell == otherCell) continue;
                int currentLength = FindLength(cell, otherCell);
                if (currentLength > greatestLength)
                {
                    start = cell;
                    end = otherCell;
                    greatestLength = currentLength;
                }
                yield return null;
            }
        }
        if (start != null)
            _startCell = start;
        if (end != null)
            endCell = end;
    }

    private int FindLength (HexCell fromCell, HexCell toCell)
    {
        int pathLength = 0;
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                cells[x, z].distance = int.MaxValue;
                cells[x, z].isDebugActive = false;
            }
        }
        List<HexCell> frontier = new List<HexCell>();
        fromCell.distance = 0;
        frontier.Add(fromCell);
        while (frontier.Count > 0)
        {
            HexCell current = frontier[0];
            frontier.RemoveAt(0);

            if (current == toCell)
            {
                current = current.PathFrom;
                while (current != fromCell)
                {
                    pathLength++;
                    current = current.PathFrom;
                }
                break;
            }
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                HexCell neighbor = current.GetNeighbor(d);

                if (neighbor == null) continue;
                if (neighbor.isBlocked) continue;
                if (!neighbor.isPath) continue;
                int distance = current.distance;
                if (current.isDecentivized)
                    distance += 20;
                else
                    distance += 1;
                if (neighbor.distance == int.MaxValue)
                {
                    neighbor.distance = distance;
                    neighbor.PathFrom = current;
                    neighbor.SearchHueristic =
                        neighbor.coordinates.DistanceTo(toCell.coordinates);
                    frontier.Add(neighbor);
                }
                else if (distance < neighbor.distance)
                {
                    neighbor.distance = distance;
                    neighbor.PathFrom = current;
                }
                frontier.Sort((x, y) => x.SearchPriority.CompareTo(y.SearchPriority));
            }
        }
        return pathLength;
    }

    private void ResetDebug ()
    {
        foreach (HexCell cell in cells)
        {
            cell.distance = int.MaxValue;
        }
    }
}
