using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MeshGenerator))]
public class MeshGeneratorEditor : Editor
{
    
    public override void OnInspectorGUI() // allows us to customize how objects appear in the Inspector
    {
        base.OnInspectorGUI();
        MeshGenerator meshGenerator = (MeshGenerator)target;
        // creates buttons 
        if (GUILayout.Button("Generate Rectangle")) meshGenerator.GenerateRectangle();
        if (GUILayout.Button("Generate Plane")) meshGenerator.GeneratePlane();
        if (GUILayout.Button("Generate Cube")) meshGenerator.GenerateCube();
        if (GUILayout.Button("Generate Voxel")) meshGenerator.GenerateVoxel();
    }
    private void OnSceneGUI() //allows us to customize how objects appear in the scene view
    {
        MeshGenerator meshGenerator = (MeshGenerator)target;
        for (int i = 0; i < meshGenerator.vertices.Count; i++)
        {
            Handles.Label(meshGenerator.vertices[i], i.ToString());
        }
    }
}

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MeshGenerator : MonoBehaviour
{
    public List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();
    public int width = 3;
    public Voxel.Type generateVoxelType;
    // creates flat square 
    public void GenerateRectangle()
    {
        vertices = new List<Vector3>();
        triangles = new List<int>();
        vertices.Add(new Vector3(0,0,0));
        vertices.Add(new Vector3(1,0,0));
        vertices.Add(new Vector3(1,0,1));
        vertices.Add(new Vector3(0, 0, 1));
        triangles.AddRange(new int[] { 0, 2, 1, 3, 2, 0 });
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = new Vector2[]
        {
            new Vector2(0,0),
            new Vector2(1,0),
            new Vector2(1,1),
            new Vector2(0,1),
        };
        mesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = mesh;
    }
    // creates bigger flat square
    public void GeneratePlane()
    {
        vertices = new List<Vector3>();
        triangles = new List<int>();
        uvs = new List<Vector2>();

        for (int z = 0; z <= width; z++)
        {
            for(int x = 0; x <= width; x++)
            {
                vertices.Add(new Vector3(x, 0f, z));
                float uvx = (float)x / (float)width;
                float uvy = (float)z / (float)width;
            }
        }

        for(int r = 0; r < width; r++)
        {
            for(int c = 0; c < width; c++)
            {
                int s = c + (width + 1) * r;
                triangles.AddRange(new int[]
                {
                    s,
                    s + (width + 2),
                    s + 1,
                    s,
                    s + (width + 1),
                    s + (width + 2)
                });
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    public static Vector2[] GetGridUVs(int x, int y, int gridSize)
    {
        float res = 1f / gridSize;
        return new Vector2[]
        {
            new Vector2(x * res, y * res),
            new Vector2(x * res, (y + 1) * res),
            new Vector2((x + 1) * res, (y + 1) * res),
            new Vector2((x + 1) * res, y * res)
        };
    }
    // creates cube
    public void GenerateCube()
    {
        vertices = new List<Vector3>();
        triangles = new List<int>();
        uvs = new List<Vector2>();

        // Top
        vertices.AddRange(new Vector3[]{
            new Vector3(0,1,0),
            new Vector3(0,1,1),
            new Vector3(1,1,1),
            new Vector3(1,1,0)
        });
        uvs.AddRange(GetGridUVs(0, 0, 6));
        // Bottom
        vertices.AddRange(new Vector3[]{
            new Vector3(0,0,0),
            new Vector3(1,0,0),
            new Vector3(1,0,1),
            new Vector3(0,0,1)
        });
        uvs.AddRange(GetGridUVs(0, 1, 6));
        // Front
        vertices.AddRange(new Vector3[]{
            new Vector3(0,0,0),
            new Vector3(0,1,0),
            new Vector3(1,1,0),
            new Vector3(1,0,0)
        });
        uvs.AddRange(GetGridUVs(0, 2, 6));
        // Right
        vertices.AddRange(new Vector3[]{
            new Vector3(1,0,0),
            new Vector3(1,1,0),
            new Vector3(1,1,1),
            new Vector3(1,0,1)
        });
        uvs.AddRange(GetGridUVs(0, 3, 6));
        // Back
        vertices.AddRange(new Vector3[]{
            new Vector3(1,0,1),
            new Vector3(1,1,1),
            new Vector3(0,1,1),
            new Vector3(0,0,1)
        });
        uvs.AddRange(GetGridUVs(0, 4, 6));
        // Left
        vertices.AddRange(new Vector3[]{
            new Vector3(0,0,1),
            new Vector3(0,1,1),
            new Vector3(0,1,0),
            new Vector3(0,0,0)
        });
        uvs.AddRange(GetGridUVs(0, 5, 6));

        for (int i = 0; i < 6; i++)
        {
            triangles.AddRange(new int[]{
                i * 4, i * 4 + 1, i * 4 + 2,
                i * 4, i * 4 + 2, i * 4 + 3,
            });
        }
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = mesh;
    }
    // creates numbers which are voxels
    public void GenerateVoxel()
    {
        vertices = new List<Vector3>();
        triangles = new List<int>();
        uvs = new List<Vector2>();

        Voxel.Type type = generateVoxelType;

        // Top
        vertices.AddRange(Voxel.topVerts);
        uvs.AddRange(Voxel.GetUVs(type, Voxel.Face.Top));
        // Bottom
        vertices.AddRange(Voxel.bottomVerts);
        uvs.AddRange(Voxel.GetUVs(type, Voxel.Face.Bottom));
        // Front
        vertices.AddRange(Voxel.frontVerts);
        uvs.AddRange(Voxel.GetUVs(type, Voxel.Face.Side));
        // Right
        vertices.AddRange(Voxel.rightVerts);
        uvs.AddRange(Voxel.GetUVs(type, Voxel.Face.Side));
        // Back
        vertices.AddRange(Voxel.backVerts);
        uvs.AddRange(Voxel.GetUVs(type, Voxel.Face.Side));
        // Left
        vertices.AddRange(Voxel.leftVerts);
        uvs.AddRange(Voxel.GetUVs(type, Voxel.Face.Side));

        for (int i = 0; i < 6; i++)
        {
            triangles.AddRange(new int[]{
                i * 4, i * 4 + 1, i * 4 + 2,
                i * 4, i * 4 + 2, i * 4 + 3,
            });
        }
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = mesh;
    }
}
