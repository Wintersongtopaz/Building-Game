using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voxel 
{
    public enum Type
    {
        Air, Dirt, Grass, Stone, Trunk, Leaves
    }

    public enum Face
    {
        Top, Side, Bottom
    }

    static Dictionary<Type, GridCoordinates> textureGridCoordinates = new Dictionary<Type, GridCoordinates>
    {
        {Voxel.Type.Dirt, new GridCoordinates(new Vector2Int(0,0), new Vector2Int(0,0), new Vector2Int(0,0))},
        {Voxel.Type.Grass, new GridCoordinates(new Vector2Int(1,0), new Vector2Int(0,1), new Vector2Int(0,0))},
        {Voxel.Type.Stone, new GridCoordinates(new Vector2Int(0,2), new Vector2Int(0,2), new Vector2Int(0,2))},
        {Voxel.Type.Trunk, new GridCoordinates(new Vector2Int(0,3), new Vector2Int(0,4), new Vector2Int(0,3))},
        {Voxel.Type.Leaves, new GridCoordinates(new Vector2Int(0,5), new Vector2Int(0,5), new Vector2Int(0,5))},
    };

    public static Vector2[] GetUVs(Voxel.Type type, Voxel.Face face)
    {
        Vector2Int coordinates = textureGridCoordinates[type].GetCoordinates(face);
        return MeshGenerator.GetGridUVs(coordinates.x, coordinates.y, 16);
    }

    struct GridCoordinates
    {
        Vector2Int top;
        Vector2Int sides;
        Vector2Int bottom;

        public GridCoordinates(Vector2Int top, Vector2Int sides, Vector2Int bottom)
        {
            this.top = top;
            this.sides = sides;
            this.bottom = bottom;
        }

        public Vector2Int GetCoordinates(Voxel.Face face)
        {
            switch (face)
            {
                case Face.Top: return top;
                case Face.Side: return sides;
                case Face.Bottom: return bottom;
            }
            return Vector2Int.zero;
        }
    }

    //creates voxel numbers
    // Top
    public static Vector3[] topVerts
    {
        get => new Vector3[]
        {
            new Vector3(0,1,0),
            new Vector3(0,1,1),
            new Vector3(1,1,1),
            new Vector3(1,1,0)
        };
    }
    // Bottom
    public static Vector3[] bottomVerts
    {
        get => new Vector3[]
        {
            new Vector3(0,0,0),
            new Vector3(1,0,0),
            new Vector3(1,0,1),
            new Vector3(0,0,1)
        };
    }
    // Front
    public static Vector3[] frontVerts
    {
        get => new Vector3[]
        {
            new Vector3(0,0,0),
            new Vector3(0,1,0),
            new Vector3(1,1,0),
            new Vector3(1,0,0)
        };
    }
    //Right
    public static Vector3[] rightVerts
    {
        get => new Vector3[]
        {
            new Vector3(1,0,0),
            new Vector3(1,1,0),
            new Vector3(1,1,1),
            new Vector3(1,0,1)
        };
    }
    // Back
    public static Vector3[] backVerts
    {
        get => new Vector3[]
        {
            new Vector3(1,0,1),
            new Vector3(1,1,1),
            new Vector3(0,1,1),
            new Vector3(0,0,1)
        };
    }
    // Left
    public static Vector3[] leftVerts
    {
        get => new Vector3[]
        {
            new Vector3(0,0,1),
            new Vector3(0,1,1),
            new Vector3(0,1,0),
            new Vector3(0,0,0)
        };
    }
       
       
}
