using UnityEngine;
using System.Collections;

public class ManController : MonoBehaviour {
    [HideInInspector] public MapManager map;
    private int x, y;

	// Use this for initialization
	void Start () {
        x = 1;
        y = 1;
	}
	
	// Update is called once per frame
	void Update () {

	}

    public void AttemptMove(int xDir, int yDir)
    {
        if (map.map[y + yDir, x + xDir] != 2)
        {
            x += xDir;
            y += yDir;

            transform.position += new Vector3(xDir, yDir, 0.0f);
        }
    }
}
