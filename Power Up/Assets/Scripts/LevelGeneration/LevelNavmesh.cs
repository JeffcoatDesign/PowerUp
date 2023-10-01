using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class LevelNavmesh : MonoBehaviour
{
    public delegate void NavMeshBuilt();
    public static event NavMeshBuilt OnNavMeshBuilt;
    private void OnEnable()
    {
        LevelGenerator.OnGetNavmesh += BuildNavMesh;
    }
    private void OnDisable()
    {
        LevelGenerator.OnGetNavmesh -= BuildNavMesh;
    }
    void BuildNavMesh()
    {
        NavMeshSurface navMeshSurface = GetComponent<NavMeshSurface>();
        navMeshSurface.BuildNavMesh();
        OnNavMeshBuilt?.Invoke();
    }
}
