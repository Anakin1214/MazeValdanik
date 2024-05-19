using UnityEngine;

public enum WallOrientation{
    LeftWall = 0, TopWall = 1, RightWall = 2, BotWall = 3
}

public class Cell : MonoBehaviour
{
    public GameObject[] walls;
    public bool isVisited = false;
    public bool[] wallsActive = {true, true, true, true};
}
