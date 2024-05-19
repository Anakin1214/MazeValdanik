using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Matriz : MonoBehaviour
{   
    public int width;
    public int height;
    public Transform cameraTransform;
    public GameObject cellPrefab;
    private Cell[,] cellMatriz;
    private Stack<Cell> stack = new Stack<Cell>();
    private System.Random rand = new System.Random();

    void Start()
    {
        Debug.Log("iniciando matriz");
        this.cellMatriz = new Cell[this.width, this.height];
        this.cameraTransform.position = new Vector3(
            this.width / 2,
            this.height,
            this.height /2 - 1
        );
        CrearMatriz();
        StartCoroutine(GenerateMaze());
    }
    void CrearMatriz()
    {
        for (int i = 0; i < this.width; i++)
        {
            for (int j = 0; j < this.height; j++)
            {
                GameObject cellRun = Instantiate(this.cellPrefab, new Vector3(i, 0, j), Quaternion.identity);
                this.cellMatriz[i, j] = cellRun.GetComponent<Cell>(); 
            }
        }
        Debug.Log("matriz creada");
    }
    IEnumerator GenerateMaze()
    {
        Cell currentCell = cellMatriz[0, 0];
        currentCell.isVisited = true;
        stack.Push(currentCell);
        Debug.Log("inicio de la generacion del laberinto");

        while (stack.Count > 0)
        {
            List<Cell> neighbors = GetUnvisitedNeighbors(currentCell);

            if (neighbors.Count > 0)
            {
                Cell nextCell = neighbors[rand.Next(neighbors.Count)];
                RemoveWalls(currentCell, nextCell);
                stack.Push(nextCell);
                nextCell.isVisited = true;
                currentCell = nextCell;
            }
            else
            {
                currentCell = stack.Pop();
            }

            yield return new WaitForSeconds(0.01f); // Pequeño retraso para visualizar la generación paso a paso
        }
        Debug.Log("generacion del laberinto completada");
    }


    List<Cell> GetUnvisitedNeighbors(Cell cell)
    {
        List<Cell> neighbors = new List<Cell>();
        Vector2Int pos = GetCellPosition(cell);

        if (pos == -Vector2Int.one)
        {
            Debug.LogError("no se pudo encontrar la posicion de la celda");
            return neighbors;
        }

        // Vecino arriba
        if (pos.y < height - 1 && !cellMatriz[pos.x, pos.y + 1].isVisited)
        {
            neighbors.Add(cellMatriz[pos.x, pos.y + 1]);
        }

        // Vecino derecho
        if (pos.x < width - 1 && !cellMatriz[pos.x + 1, pos.y].isVisited)
        {
            neighbors.Add(cellMatriz[pos.x + 1, pos.y]);
        }

        // Vecino abajo
        if (pos.y > 0 && !cellMatriz[pos.x, pos.y - 1].isVisited)
        {
            neighbors.Add(cellMatriz[pos.x, pos.y - 1]);
        }

        // Vecino izquierdo
        if (pos.x > 0 && !cellMatriz[pos.x - 1, pos.y].isVisited)
        {
            neighbors.Add(cellMatriz[pos.x - 1, pos.y]);
        }

        return neighbors;
    }


    Vector2Int GetCellPosition(Cell cell)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (cellMatriz[i, j] == cell)
                {
                    return new Vector2Int(i, j);
                }
            }
        }
        return -Vector2Int.one;
    }
    void RemoveWalls(Cell current, Cell next)
    {
        Vector2Int currentPos = GetCellPosition(current);
        Vector2Int nextPos = GetCellPosition(next);
        int currentWall, nextWall;

        if (currentPos.x == nextPos.x)
        {
            if (currentPos.y > nextPos.y)
            {
                currentWall = (int)WallOrientation.BotWall;
                nextWall = (int)WallOrientation.TopWall;
            }
            else
            {
                currentWall = (int)WallOrientation.TopWall;
                nextWall = (int)WallOrientation.BotWall;
            }
        }
        else if (currentPos.y == nextPos.y)
        {
            if (currentPos.x > nextPos.x)
            {
                currentWall = (int)WallOrientation.LeftWall;
                nextWall = (int)WallOrientation.RightWall;
            }
            else
            {
                currentWall = (int)WallOrientation.RightWall;
                nextWall = (int)WallOrientation.LeftWall;
            }
        }
        else
        {
            return;
        }

        current.walls[currentWall].SetActive(false);
        next.walls[nextWall].SetActive(false);
    }
}
