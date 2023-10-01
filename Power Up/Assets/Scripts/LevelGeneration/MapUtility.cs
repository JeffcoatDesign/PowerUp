using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapUtility
{
    public static class HexMetrics
    {
        public const float outerRadius = 1f;
        public const float innerRadius = 0.866f;
        public const float hexScale = 1f;
    }

    public enum HexDirection
    {
        NE, E, SE, SW, W, NW
    }

    public static class HexDirectionExtensions
    {
        public static HexDirection Opposite(this HexDirection direction)
        {
            return (int)direction < 3 ? (direction + 3) : (direction - 3);
        }
    }

    public static class MapGeneration
    {
        public static List<Vector2Pair> GeneratePointPairs (List<Vector2> points)
        {
            List<Vector3> points3D = new();
            for (int i = 0; i < points.Count; i++)
            {
                Vector3 point = new(points[i].x, 0f, points[i].y);
                points3D.Add(point);
            }
            List<Triangulation.Triangle> triangles = Triangulation.DelauneyAlgorithm.TriangulateByFlippingEdges(points3D);
            List<Triangulation.Edge> edges = Triangulation.Kruskai.FindMinimumSpanningTree(triangles);
            List<Vector2Pair> pointPairs = new List<Vector2Pair>(edges.Count);
            for (int i = 0; i < edges.Count; i++)
            {
                Vector2Pair vectorPair = new(edges[i].v1.GetPos2D(), edges[i].v2.GetPos2D());
                pointPairs.Add(vectorPair);
            }
            return pointPairs;
        }
        public static float[,] GenerateFalloff(int width, int height)
        {
            float[,] falloffMap = new float[width, height];
            for (int z = 0; z < height; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    float xv = x / (float)width * 2 - 1;
                    float zv = z / (float)height * 2 - 1;
                    float v = Mathf.Max(Mathf.Abs(xv), Mathf.Abs(zv));
                    falloffMap[x, z] = Mathf.Pow(v, 3f) / (Mathf.Pow(v, 3f) + Mathf.Pow(2.2f - 2.2f * v, 3f));
                }
            }
            return falloffMap;
        }
        public static float[,] GenerateNoisemap(int width, int height, float noiseScale)
        {
            float[,] noiseMap = new float[width, height];
            float[,] falloffMap = GenerateFalloff(width, height);
            (float xOffset, float yOffset) = (Random.Range(-10000f, 10000f), Random.Range(-10000f, 10000f));
            for (int z = 0; z < height; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    float noiseValue = Mathf.PerlinNoise(x * noiseScale + xOffset, z * noiseScale + yOffset);
                    //noiseValue += falloffMap[x, z];
                    noiseMap[x, z] = noiseValue;
                }
            }
            return noiseMap;
        }

        public static Vector2Int GetRandomPoint(int width, int height)
        {
            return new Vector2Int(Random.Range(0, width), Random.Range(0, height));
        }
    }

    [System.Serializable]
    public struct HexCoordinates
    {
        [SerializeField] private int x, z;
        public int X { get { return x; } private set { } }
        public int Y { get { return -X - Z; } }
        public int Z { get { return z; } private set { } }

        public HexCoordinates(int x, int z)
        {
            this.x = x;
            this.z = z;
        }

        public int DistanceTo(HexCoordinates other)
        {
            return
                ((x < other.x ? other.x - x : x - other.x) +
                (Y < other.Y ? other.Y - Y : Y - other.Y) +
                (z < other.z ? other.z - z : z - other.z)) / 2;
        }

        public static HexCoordinates FromOffsetCoordinates(int x, int z)
        {
            return new HexCoordinates(x - z / 2, z);
        }

        public override string ToString()
        {
            return "(" + X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + ")";
        }

        public string ToStringOnSeperateLines()
        {
            return X.ToString() + "\n" + Z.ToString();
        }
    }

    [System.Serializable]
    public struct Vector2Pair
    {
        [SerializeField] private Vector2Int point1;
        [SerializeField] private Vector2Int point2;
        public Vector2Int Point1 { get { return point1; } }
        public Vector2Int Point2 { get { return point2; } }

        public Vector2Pair(Vector2 point1, Vector2 point2)
        {
            this.point1 = new Vector2Int(Mathf.RoundToInt(point1.x), Mathf.RoundToInt(point1.y));
            this.point2 = new Vector2Int(Mathf.RoundToInt(point2.x), Mathf.RoundToInt(point2.y));
        }

        public override string ToString()
        {
            return "(" + point1.ToString() + ", " + point2.ToString() + ")";
        }
    }
}