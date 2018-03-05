using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class MapManagerWithGO : MapManager {
    public GameObject mountainTile;
    public GameObject hillTile;
    public GameObject plainsTile;

    private Transform mapHolder;
     
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
        mapHolder = new GameObject("Map").transform;
        
        for (int y=0; y<8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                GameObject tileToInstantiate = null;

                switch (map[y,x]) {
                    case 0:
                        tileToInstantiate = plainsTile;
                        break;

                    case 1:
                        tileToInstantiate = hillTile;
                        break;

                    case 2:
                        tileToInstantiate = mountainTile;
                        break;
                }

                GameObject tileInstance = Instantiate(tileToInstantiate, new Vector3(x - 3.5f, y - 3.5f, 0f), Quaternion.identity) as GameObject;

                tileInstance.transform.SetParent(mapHolder);
            }
        }
    }

    public override void SetupScene()
    {
        InitializeMap();
        MapSetup();
    }
}
