using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class MapManagerWithMesh : MapManager {
    public Texture2D mountainTile;
    public Texture2D hillTile;
    public Texture2D plainsTile;

    public int size_x;
    public int size_y;
    public int width;
    public int height;
    public int tile_resolution;

    Mesh _mapMesh;

    MeshFilter _mesh_filter;
    MeshRenderer _mesh_renderer;
    MeshCollider _mesh_collider;

    int _v_size_x;
    int _v_size_y;
    float _mapMultiply_x;
    float _mapMultiply_y;

    void InitializeMap()
    {
        map = new int[,]
        {
            {2,2,2,2,2,2,2,2 },
            {2,0,0,1,0,0,0,2 },
            {2,0,1,2,1,1,0,2 },
            {2,0,1,1,2,1,0,2 },
            {2,0,0,1,2,1,0,2 },
            {2,0,0,0,1,1,0,2 },
            {2,0,0,0,0,0,0,2 },
            {2,2,2,2,2,2,2,2 },
        };
    }

    void MapSetup()
    {
        BuildMesh();
        DrawMap();
    }

    void BuildVerticesAndUV(Vector3[] vertices, Vector2[] uv, Vector3 worldOffset, float bump_amount)
    {
        // Builds the vertices and texture coords for a big rectangle (ie, the map)
        // Also jiggles the z coord a bit to create a wrinkled effect - should really use
        // a nicer noise function to smooth it out a little...
        for (int i = 0; i < _v_size_y; ++i)
        {
            for (int j = 0; j < _v_size_x; ++j)
            {

                vertices[(i * _v_size_x) + j] = new Vector3(j * _mapMultiply_x,
                    (_v_size_y - 1 - i) * _mapMultiply_y,
                    Random.Range(-bump_amount, bump_amount))
                    + worldOffset;

                uv[(i * _v_size_x) + j] = new Vector2((float)j / (_v_size_x - 1), (float)(_v_size_y - 1 - i) / (_v_size_y - 1));
            }
        }
    }

    void BuildTriangles(int[] triangles)
    {
        // Make the triangles for the map mesh
        // Basically assigning the vertices to each triangle (in one big array)
        for (int i = 0; i < size_y; ++i)
        {
            for (int j = 0; j < size_x; ++j)
            {
                int triangleOffset = ((i * size_x) + j) * 6;
                int vertexOffset = ((i * _v_size_x) + j);

                triangles[triangleOffset + 0] = vertexOffset + 0;
                triangles[triangleOffset + 1] = vertexOffset + 1;
                triangles[triangleOffset + 2] = vertexOffset + _v_size_x + 1;

                triangles[triangleOffset + 3] = vertexOffset + 0;
                triangles[triangleOffset + 4] = vertexOffset + _v_size_x + 1;
                triangles[triangleOffset + 5] = vertexOffset + _v_size_x;
            }
        }
    }

    void BuildMesh()
    {
        // Create the Mesh we want to hold the map in
        Vector3 world_offset = new Vector3(-4.0f, -3.80f, 0f);  // An offset to make this fit with the men
        int num_squares = size_x * size_y;
        _v_size_x = size_x + 1;
        _v_size_y = size_y + 1;

        _mapMultiply_x = (float)width / size_x;
        _mapMultiply_y = (float)height / size_y;

        // set up the data structure needed for the map mesh

        Vector3[] vertices = new Vector3[_v_size_x * _v_size_y];
        int[] triangles = new int[num_squares*2*3];
        Vector2[] uv = new Vector2[_v_size_x * _v_size_y];

        float bump_amount = 0.15f;

        BuildVerticesAndUV(vertices, uv, world_offset, bump_amount);
        BuildTriangles(triangles);

        // Now create the mesh
        _mapMesh = new Mesh();
        _mapMesh.vertices = vertices;
        _mapMesh.triangles = triangles;
        _mapMesh.uv = uv;
        _mapMesh.RecalculateNormals();
        
        // And attach it to the components on this object
        _mesh_filter = GetComponent<MeshFilter>();
        _mesh_renderer = GetComponent<MeshRenderer>();
        _mesh_collider = GetComponent<MeshCollider>();

        _mesh_filter.mesh = _mapMesh;
        _mesh_collider.sharedMesh = _mapMesh;
    }

    void DrawMap()
    {
        // create a big texture map of the right size
        Texture2D mapTexture = new Texture2D(width * tile_resolution, height * tile_resolution);

        // Go through the map

        for (var y=0; y<height; y++)
        {
            for (var x=0; x<width; x++)
            {
                Texture2D refTexture = null;

                // DIRTY - using hardset ints
                // Instead should use delicious Enums!
                switch(map[y,x])
                {
                    case 0:
                        refTexture = plainsTile;
                        break;

                    case 1:
                        refTexture = hillTile;
                        break;

                    case 2:
                        refTexture = mountainTile;
                        break;
                }

                // for each tile, copy the appropriate image onto the texture map
                // Like a BLIT!
                Color[] srcPixels = refTexture.GetPixels(0, 0, tile_resolution, tile_resolution);

                //if (map[y,x] == 2)
                //{
                //    for (int i=0; i<srcPixels.Length; ++i)
                //    {
                //        srcPixels[i].r = srcPixels[i].r + 0.75f;
                //        srcPixels[i].g = srcPixels[i].g + 0.75f;
                //        srcPixels[i].b = srcPixels[i].b + 0.75f;
                //    }
                //}

                mapTexture.SetPixels(x * tile_resolution, y * tile_resolution, tile_resolution, tile_resolution, srcPixels);            
            }
        }

        // apply the texture to the mesh
        mapTexture.Apply();

        // We want old skool pixelated style!
        mapTexture.filterMode = FilterMode.Point;

        // and set the texture on this to the new map!
        _mesh_renderer.material.mainTexture = mapTexture;
    }

    public override void SetupScene()
    {
        // Creates the map, and then displays it
        InitializeMap();
        MapSetup();
    }
}
