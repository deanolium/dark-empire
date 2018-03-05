using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public abstract class MapManager : MonoBehaviour
{
    [HideInInspector]
    public int[,] map;

    abstract public void SetupScene();
}
