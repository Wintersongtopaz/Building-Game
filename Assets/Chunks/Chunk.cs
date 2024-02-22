using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class Chunk : MonoBehaviour
{
    public const int width = 16;
    public const int height = 64;

    Vector3Int chunkCell; //the orgin cell of a chunk

    static Dictionary<Vector3Int, Dictionary<Vector3Int, Voxel.Type>> deltas = new Dictionary<Vector3Int, Dictionary<Vector3Int, Voxel.Type>>();
    //Check if block at coordinates has been changed in deltas
    public static Voxel.Type GetVoxelType(Vector3Int coordinates)
    {
        Vector3Int chunkCell = Vector3Int.zero;
        int x = coordinates.x; int y = coordinates.y; int z = coordinates.z;
        GlobalToLocal(ref chunkCell, ref x, ref y, ref z);

        if (deltas.ContainsKey(chunkCell) && deltas[chunkCell].ContainsKey(new Vector3Int(x, y, z)))
        {
            return deltas[chunkCell][new Vector3Int(x, y, z)];
        }

        return TerrainGenerator.GetVoxelType(coordinates.x, coordinates.y, coordinates.z);
        if (coordinates.y == 1) return Voxel.Type.Dirt;
        return Voxel.Type.Air;
    }
    // Add new voxel type to deltas
    public static void SetVoxelType(int x, int y, int z, Voxel.Type newVoxelType)
    {
        Vector3Int chunkCell = Vector3Int.zero;
        GlobalToLocal(ref chunkCell, ref x, ref y, ref z);

        if (!deltas.ContainsKey(chunkCell))
        {
            deltas.Add(chunkCell, new Dictionary<Vector3Int, Voxel.Type>());
        }

        if (deltas[chunkCell].ContainsKey(new Vector3Int(x, y, z))) deltas[chunkCell][new Vector3Int(x, y, z)] = newVoxelType;
        else deltas[chunkCell].Add(new Vector3Int(x, y, z), newVoxelType);

        ChunkLoader.instance.ReloadActiveChunk(chunkCell);
        if (x == 0) ChunkLoader.instance.ReloadActiveChunk(chunkCell + new Vector3Int(-1, 0, 0));
        else if (x >= width - 1) ChunkLoader.instance.ReloadActiveChunk(chunkCell + new Vector3Int(1, 0, 0));
        if (z == 0) ChunkLoader.instance.ReloadActiveChunk(chunkCell + new Vector3Int(0, 0, -1));
        else if (z >= width - 1) ChunkLoader.instance.ReloadActiveChunk(chunkCell + new Vector3Int(0, 0, 1));
    }

    // chunk cell correspondong to global coordinates
    public static Vector3Int GetChunkCell(int x, int y, int z)
    {
        int tempX = x / width;
        int tempZ = z / width;
        if (x < 0 && x % width != 0) tempX -= 1;
        if (z < 0 && z % width != 0) tempZ -= 1;

        Vector3Int targetCell = new Vector3Int();

        targetCell.x = tempX * width;
        targetCell.y = 0;
        targetCell.z = tempZ * width;

        return targetCell;
    }
    // chunck cell corresponding to a global position
    public static Vector3Int GetChunkCell(Vector3 position)
    {
        return GetChunkCell(
            Mathf.RoundToInt(position.x),
            0,
            Mathf.RoundToInt(position.z)
        );
    }
    // converts global coordinates to local coordinates
    public static void GlobalToLocal(ref Vector3Int targetCell, ref int x, ref int y, ref int z)
    {
        int tempX = x / width;
        int tempZ = z / width;
        if (x < 0 && width != 0) tempX -= 1;
        if (z < 0 && width != 0) tempZ -= 1;

        targetCell.x = tempX * width;
        targetCell.y = 0;
        targetCell.z = tempZ * width;

        x -= targetCell.x;
        z -= targetCell.z;
    }

    
    // Generate a mesh from voxels in chunk
    public void BuildMesh()
    {
        List<Vector3> verticies = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        chunkCell = new Vector3Int(
            Mathf.RoundToInt(transform.position.x),
            Mathf.RoundToInt(transform.position.y),
            Mathf.RoundToInt(transform.position.z)
        );

        for(int x = 0; x < width; x++)
        {
            for(int z =0; z < width; z++)
            {
                for(int y = 0; y < height; y++)
                {
                    Voxel.Type voxelType = GetVoxelType(chunkCell + new Vector3Int(x, y, z));
                    if (voxelType == Voxel.Type.Air) continue;

                    Vector3 blockPosition = new Vector3(x, y, z);
                    blockPosition -= Vector3.one * 0.5f;
                    int faceCount = 0;

                    // Top
                    if (y < height - 1 && GetVoxelType(chunkCell + new Vector3Int(x,y + 1,z)) == Voxel.Type.Air)
                    {
                        foreach (Vector3 v in Voxel.topVerts) verticies.Add(blockPosition + v);
                        faceCount++;
                        uvs.AddRange(Voxel.GetUVs(voxelType, Voxel.Face.Top));
                    }

                    // Bottom
                    if (y < 0 && GetVoxelType(chunkCell + new Vector3Int(x, y - 1, z)) == Voxel.Type.Air)
                    {
                        foreach (Vector3 v in Voxel.bottomVerts) verticies.Add(blockPosition + v);
                        faceCount++;
                        uvs.AddRange(Voxel.GetUVs(voxelType, Voxel.Face.Bottom));
                    }

                    //Front
                    if (GetVoxelType(chunkCell + new Vector3Int(x, y, z - 1)) == Voxel.Type.Air)
                    {
                        foreach (Vector3 v in Voxel.frontVerts) verticies.Add(blockPosition + v);
                        faceCount++;
                        uvs.AddRange(Voxel.GetUVs(voxelType, Voxel.Face.Side));
                    }

                    //Right
                    if (GetVoxelType(chunkCell + new Vector3Int(x + 1, y, z)) == Voxel.Type.Air)
                    {
                        foreach (Vector3 v in Voxel.rightVerts) verticies.Add(blockPosition + v);
                        faceCount++;
                        uvs.AddRange(Voxel.GetUVs(voxelType, Voxel.Face.Side));
                    }

                    // Back
                    if (GetVoxelType(chunkCell + new Vector3Int(x, y, z + 1)) == Voxel.Type.Air)
                    {
                        foreach (Vector3 v in Voxel.backVerts) verticies.Add(blockPosition + v);
                        faceCount++;
                        uvs.AddRange(Voxel.GetUVs(voxelType, Voxel.Face.Side));
                    }

                    // Left
                    if (GetVoxelType(chunkCell + new Vector3Int(x - 1, y, z)) == Voxel.Type.Air)
                    {
                        foreach (Vector3 v in Voxel.leftVerts) verticies.Add(blockPosition + v);
                        faceCount++;
                        uvs.AddRange(Voxel.GetUVs(voxelType, Voxel.Face.Side));
                    }

                    int h = verticies.Count - 4 * faceCount;
                    for (int i = 0; i < faceCount; i++)
                    {
                        triangles.AddRange(new int[]{
                            h + i * 4, h + i * 4 + 1, h + i * 4 + 2,
                            h + i * 4, h + i * 4 + 2, h + i * 4 + 3,
                        });
                    }
                }
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = verticies.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }
}
