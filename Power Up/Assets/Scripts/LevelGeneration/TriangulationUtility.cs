using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Triangulation
{
    public static class Kruskai
    {
        public static List<Edge> FindMinimumSpanningTree (List<Triangle> triangles)
        {
            List<Edge> ans = new();

            List<Edge> edges = new();
            List<HalfEdge> halfEdges = DelauneyAlgorithm.TransformFromTriangleToHalfEdge(triangles);

            foreach (HalfEdge halfEdge in halfEdges)
                edges.Add(new Edge(halfEdge.vertex, halfEdge.nextEdge.vertex));

            edges.Sort(Edge.LengthComparison);

            HashSet<Vertex> vertices = new();
            foreach (Edge edge in edges)
            {
                vertices.Add(edge.v1);
                vertices.Add(edge.v2);
            }

            Dictionary<Vertex, Vertex> parents = new();
            foreach (var vertex in vertices)
                parents[vertex] = vertex;

            Vertex UnionFind (Vertex x)
            {
                if (parents[x] != x)
                    parents[x] = UnionFind(parents[x]);
                return parents[x];
            }

            foreach (var edge in edges)
            {
                var x = UnionFind(edge.v1);
                var y = UnionFind(edge.v2);
                if (x != y)
                {
                    ans.Add(edge);
                    parents[x] = y;
                }
            }
            return ans;
        }
    }
    public static class DelauneyAlgorithm {
        public static List<Triangle> TriangulateByFlippingEdges(List<Vector3> sites)
        {
            List<Vertex> vertices = new();

            for (int i = 0; i < sites.Count; i++)
                vertices.Add(new Vertex(sites[i]));

            List<Triangle> triangles = IncrementalTriangulationAlgorithm.TrianglulatePoints(vertices);
            List<HalfEdge> halfEdges = TransformFromTriangleToHalfEdge(triangles);

            int safety = 0;

            int flippedEdges = 0;

            while (true)
            {
                safety += 1;

                if (safety > 100000)
                {
                    Debug.Log("Stuck in an endless loop!");

                    break;
                }

                bool hasFlippedEdge = false;

                for (int i = 0; i < halfEdges.Count; i++)
                {
                    HalfEdge thisEdge = halfEdges[i];

                    if (thisEdge.oppositeEdge == null) continue;

                    Vertex a = thisEdge.vertex;
                    Vertex b = thisEdge.nextEdge.vertex;
                    Vertex c = thisEdge.previousEdge.vertex;
                    Vertex d = thisEdge.oppositeEdge.nextEdge.vertex;

                    Vector2 aPos = a.GetPos2D();
                    Vector2 bPos = b.GetPos2D();
                    Vector2 cPos = c.GetPos2D();
                    Vector2 dPos = d.GetPos2D();

                    if(Geometry.IsPointInsideOutsideOrOnCircle(aPos, bPos, cPos, dPos) < 0f)
                    {
                        if(Geometry.IsQuadrilateralConvex(aPos, bPos, cPos, dPos))
                        {
                            if(Geometry.IsPointInsideOutsideOrOnCircle(bPos, cPos, dPos, aPos) < 0f)
                            {
                                continue;
                            }

                            flippedEdges += 1;
                            hasFlippedEdge = true;

                            FlipEdge(thisEdge);
                        }
                    }
                }
                if (!hasFlippedEdge)
                {
                    //Debug.Log("Has found delauney triangulation");
                    break;
                }
            }
            return triangles;
        }
        public static List<HalfEdge> TransformFromTriangleToHalfEdge(List<Triangle> triangles)
        {
            Geometry.OrientTrianglesClockwise(triangles);

            List<HalfEdge> halfEdges = new List<HalfEdge>(triangles.Count * 3);

            for (int i = 0; i < triangles.Count; i++)
            {
                Triangle tri = triangles[i];

                HalfEdge he1 = new HalfEdge(tri.v1);
                HalfEdge he2 = new HalfEdge(tri.v2);
                HalfEdge he3 = new HalfEdge(tri.v3);

                he1.nextEdge = he2;
                he2.nextEdge = he3;
                he3.nextEdge = he1;

                he1.previousEdge = he3;
                he2.previousEdge = he1;
                he3.previousEdge = he2;

                he1.vertex.halfEdge = he2;
                he2.vertex.halfEdge = he3;
                he3.vertex.halfEdge = he1;

                tri.halfEdge = he1;

                he1.triangle = tri;
                he2.triangle = tri;
                he3.triangle = tri;

                halfEdges.Add(he1);
                halfEdges.Add(he2);
                halfEdges.Add(he3);
            }
            for (int i = 0; i < halfEdges.Count; i++)
            {
                HalfEdge halfEdge = halfEdges[i];

                Vertex goingToVertex = halfEdge.vertex;
                Vertex goingFromVertex = halfEdge.previousEdge.vertex;

                for (int j = 0; j < halfEdges.Count; j++)
                {
                    if (i == j) continue;

                    HalfEdge heOpposite = halfEdges[j];

                    if (goingFromVertex.Position == heOpposite.vertex.Position && goingToVertex.Position == heOpposite.previousEdge.vertex.Position)
                    {
                        halfEdge.oppositeEdge = heOpposite;

                        break;
                    }
                }
            }
            return halfEdges;
        }
        private static void FlipEdge(HalfEdge one)
        {
            HalfEdge two = one.nextEdge;
            HalfEdge three = one.previousEdge;

            HalfEdge four = one.oppositeEdge;
            HalfEdge five = one.oppositeEdge.nextEdge;
            HalfEdge six = one.oppositeEdge.previousEdge;

            Vertex a = one.vertex;
            Vertex b = one.nextEdge.vertex;
            Vertex c = one.previousEdge.vertex;
            Vertex d = one.oppositeEdge.nextEdge.vertex;

            a.halfEdge = one.nextEdge;
            c.halfEdge = one.oppositeEdge.nextEdge;

            one.nextEdge = three;
            one.previousEdge = five;

            two.nextEdge = four;
            two.previousEdge = six;

            three.nextEdge = five;
            three.previousEdge = one;

            four.nextEdge = six;
            four.previousEdge = two;

            five.nextEdge = one;
            five.previousEdge = three;

            six.nextEdge = two;
            six.previousEdge = four;

            one.vertex = b;
            two.vertex = b;
            three.vertex = c;
            four.vertex = d;
            five.vertex = d;
            six.vertex = a;

            Triangle t1 = one.triangle;
            Triangle t2 = four.triangle;

            one.triangle = t1;
            three.triangle = t1;
            five.triangle = t1;

            two.triangle = t2;
            four.triangle = t2;
            six.triangle = t2;

            t1.v1 = b;
            t1.v2 = c;
            t1.v3 = d;

            t2.v1 = b;
            t2.v2 = d;
            t2.v3 = a;

            t1.halfEdge = three;
            t2.halfEdge = four;
        }
    }

    public static class IncrementalTriangulationAlgorithm
    {
        /// <summary>
        /// Triangulates the Points Using the Incremental Method
        /// </summary>
        /// <param name="points"> List of vertices to pass.</param>
        /// <returns>Returns a list of triangles generated from the passed vertices</returns>
        public static List<Triangle> TrianglulatePoints(List<Vertex> points)
        {
            List<Triangle> triangles = new List<Triangle>();
            points = points.OrderBy(n => n.Position.x).ToList();

            Triangle newTriangle = new Triangle(points[0].Position, points[1].Position, points[2].Position);

            triangles.Add(newTriangle);

            List<Edge> edges = new List<Edge>();

            edges.Add(new Edge(newTriangle.v1, newTriangle.v2));
            edges.Add(new Edge(newTriangle.v2, newTriangle.v3));
            edges.Add(new Edge(newTriangle.v3, newTriangle.v1));

            for (int i = 3; i < points.Count; i++)
            {
                Vector3 currentPoint = points[i].Position;
                List<Edge> newEdges = new List<Edge>();

                for (int j = 0; j < edges.Count; j++)
                {
                    Edge currentEdge = edges[j];
                    Vector3 midPoint = (currentEdge.v1.Position + currentEdge.v2.Position) / 2f;
                    Edge edgeToMidpoint = new Edge(currentPoint, midPoint);

                    bool canSeeEdge = true;

                    for (int k = 0; k < edges.Count; k++)
                    {
                        if (k == j)
                        {
                            continue;
                        }

                        if (AreEdgesIntersecting(edgeToMidpoint, edges[k]))
                        {
                            canSeeEdge = false;

                            break;
                        }
                    }

                    if (canSeeEdge)
                    {
                        Edge edgeToPoint1 = new Edge(currentEdge.v1, new Vertex(currentPoint));
                        Edge edgeToPoint2 = new Edge(currentEdge.v2, new Vertex(currentPoint));

                        newEdges.Add(edgeToPoint1);
                        newEdges.Add(edgeToPoint2);

                        Triangle newTri = new Triangle(edgeToPoint1.v1, edgeToPoint1.v2, edgeToPoint2.v1);
                        triangles.Add(newTri);
                    }
                }
                for (int j = 0; j < newEdges.Count; j++) {
                    edges.Add(newEdges[j]);
                }
            }
            
            return triangles;
        }
        public static bool AreEdgesIntersecting (Edge edge1, Edge edge2)
        {
            Vector2 edge1p1 = new Vector2(edge1.v1.Position.x, edge1.v1.Position.z);
            Vector2 edge1p2 = new Vector2(edge1.v2.Position.x, edge1.v2.Position.z);
            Vector2 edge2p1 = new Vector2(edge2.v1.Position.x, edge2.v1.Position.z);
            Vector2 edge2p2 = new Vector2(edge2.v2.Position.x, edge2.v2.Position.z);

            bool isIntersecting = Intersections.AreLineSegmentsIntersecting(edge1p1, edge1p2, edge2p1, edge2p2, true);

            return isIntersecting;
        }
    }

    public static class Geometry
    {
        //Orients every triangle in a list clockwise
        public static void OrientTrianglesClockwise(List<Triangle> triangles)
        {
            for (int i = 0; i < triangles.Count; i++)
            {
                Triangle triangle = triangles[i];
                Vector2 v1 = triangle.v1.GetPos2D();
                Vector2 v2 = triangle.v2.GetPos2D();
                Vector2 v3 = triangle.v3.GetPos2D();

                if (!IsTriangleClockwise(v1,v2,v3))
                    triangle.ChangeOrientation();
            }
        }

        //Is a triangle oriented clockwise
        public static bool IsTriangleClockwise(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            bool isClockwise = true;
            float determinant = p1.x * p2.y + p3.x * p1.y + p2.x * p3.y - p1.x * p3.y - p3.x * p2.y - p2.x * p1.y;
            if (determinant > 0f)
                isClockwise = false;
            return isClockwise;
        }
        public static bool IsQuadrilateralConvex(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        {
            bool isConvex = false;

            bool abc = IsTriangleClockwise(a, b, c);
            bool abd = IsTriangleClockwise(a, b, d);
            bool bcd = IsTriangleClockwise(b, c, d);
            bool cad = IsTriangleClockwise(c, a, d);

            if (abc && abd && bcd & !cad) isConvex = true;
            else if (abc && abd && !bcd & cad) isConvex = true;
            else if (abc && !abd && bcd & cad) isConvex = true;
            else if (!abc && !abd && !bcd & cad) isConvex = true;
            else if (!abc && !abd && bcd & !cad) isConvex = true;
            else if (!abc && abd && !bcd & !cad) isConvex = true;

            return isConvex;
        }
        //Is a point d inside, outside, or on the same circle as a, b, c
        public static float IsPointInsideOutsideOrOnCircle(Vector2 aVec, Vector2 bVec, Vector2 cVec, Vector2 dVec)
        {
            float a = aVec.x - dVec.x;
            float d = bVec.x - dVec.x;
            float g = cVec.x - dVec.x;

            float b = aVec.y - dVec.y;
            float e = bVec.y - dVec.y;
            float h = cVec.y - dVec.y;

            float c = a * a + b * b;
            float f = d * d + e * e;
            float i = g * g + h * h;

            float determinant = (a * e * i) + (b * f * g) + (c * d * h) - (g * e * c) - (h * f * a) - (i * d * b);

            return determinant;
        }
    }

    public static class Intersections {
        public static bool AreLineSegmentsIntersectingDotProduct(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4) {
            bool isIntersecting = false;

            isIntersecting = (ArePointsOnDifferentSides(p1, p2, p3, p4) && ArePointsOnDifferentSides(p3, p4, p1, p2));
            
            return isIntersecting;
        }

        public static bool AreLineSegmentsIntersecting (Vector2 l1_p1, Vector2 l1_p2, Vector2 l2_p1, Vector2 l2_p2, bool shouldIncludeEndPoints)
        {
            bool isIntersecting = false;
            float denominator = (l2_p2.y - l2_p1.y) * (l1_p2.x - l1_p1.x) - (l2_p2.x - l2_p1.x) * (l1_p2.y - l1_p1.y);
            if (denominator != 0f)
            {
                float u_a = ((l2_p2.x - l2_p1.x) * (l1_p1.y - l2_p1.y) - (l2_p2.y - l2_p1.y) * (l1_p1.x - l2_p1.x)) / denominator;
                float u_b = ((l1_p2.x - l1_p1.x) * (l1_p1.y - l2_p1.y) - (l1_p2.y - l1_p1.y) * (l1_p1.x - l2_p1.x)) / denominator;

                if (shouldIncludeEndPoints)
                {
                    if (u_a >= 0f && u_a <= 1f && u_b >= 0f && u_b <= 1f)
                        isIntersecting = true;
                } 
                else {
                    if (u_a > 0f && u_a < 1f && u_b > 0f && u_b < 1f)
                        isIntersecting = true;
                }
            }
            return isIntersecting;
        }

        public static bool ArePointsOnDifferentSides (Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
        {
            bool isOnDifferentSides = false;

            Vector3 lineDir = p2 - p1;

            Vector3 lineNormal = new Vector3(-lineDir.z, lineDir.y, lineDir.x);

            float dot1 = Vector3.Dot(lineNormal, p3 - p1);
            float dot2 = Vector3.Dot(lineNormal, p4 - p1);

            isOnDifferentSides = (dot1 * dot2 < 0f);

            return isOnDifferentSides;
        }
    }

    [System.Serializable]
    public class Triangle
    {
        public Vertex v1;
        public Vertex v2;
        public Vertex v3;

        public HalfEdge halfEdge;

        public Triangle(Vertex v1, Vertex v2, Vertex v3)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
        }
        public Triangle(Vector3 position1, Vector3 position2, Vector3 position3)
        {
            this.v1 = new Vertex(position1);
            this.v2 = new Vertex(position2);
            this.v3 = new Vertex(position3);
        }
        public Triangle(HalfEdge halfEdge)
        {
            this.halfEdge = halfEdge;
        }

        public void ChangeOrientation()
        {
            Vertex temp = this.v1;
            this.v1 = this.v2;
            this.v2 = temp;
        }
    }

    [System.Serializable]
    public class Vertex
    {
        [SerializeField] private Vector3 position;
        public HalfEdge halfEdge;
        public Vertex previousVertex;
        public Vertex nextVertex;
        [SerializeField] private bool isReflex;
        [SerializeField] private bool isConvex;
        [SerializeField] private bool isEar;

        public Vector3 Position { get { return position; } }

        public Vertex(Vector3 position)
        {
            this.position = position;
        }

        public Vector2 GetPos2D()
        {
            Vector2 pos2D = new Vector2(position.x, position.z);
            return pos2D;
        }
        public override int GetHashCode()
        {
            var hashCode = 1502939027;
            hashCode = hashCode * -1521134295 + position.GetHashCode();
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            return obj is Vertex other && position == other.position;
        }
    }

    [System.Serializable]
    public class HalfEdge
    {
        public Vertex vertex;
        public Triangle triangle;
        public HalfEdge nextEdge;
        public HalfEdge previousEdge;
        public HalfEdge oppositeEdge;

        public HalfEdge(Vertex vertex)
        {
            this.vertex = vertex;
        }
    }

    [System.Serializable]
    public class Edge
    {
        public Vertex v1;
        public Vertex v2;

        public bool isIntersecting = false;
        private float length = -1;
        public float Length
        {
            get
            {
                if (length < 0f)
                {
                    Vector2 p1 = GetVertex2D(v1);
                    Vector2 p2 = GetVertex2D(v2);
                    float dx = p1.x - p2.x; 
                    float dy = p1.y - p2.y;
                    length = Mathf.Sqrt(dx * dx + dy * dy);
                }
                return length;
            }
        }

        public Edge(Vertex v1, Vertex v2)
        {
            this.v1 = v1;
            this.v2 = v2;
        }
        public Edge(Vector3 v1, Vector3 v2)
        {
            this.v1 = new Vertex(v1);
            this.v2 = new Vertex(v2);
        }

        public Vector2 GetVertex2D(Vertex v)
        {
            return new Vector2(v.Position.x, v.Position.z);
        }

        public void FlipEdge()
        {
            Vertex temp = this.v1;
            this.v1 = this.v2;
            this.v2 = temp;
        }

        public static int LengthComparison(Edge x, Edge y)
        {
            float lx = x.Length;
            float ly = y.Length;
            if (Mathf.Approximately(lx, ly))
                return 0;
            else if (lx > ly)
                return 1;
            else
                return -1;
        }
    }
}