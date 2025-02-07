using System.Linq;
using UnityEngine;

public class Grid : MonoBehaviour
{
    private void drawGrid(int[,] grid, float size, int gridSize)
    {
        Vector3[] points = new Vector3[gridSize * gridSize * 4];
        int pointIndex = 0;

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                // Horizontal line
                points[pointIndex++] = new Vector3(x * size, 0, y * size);
                points[pointIndex++] = new Vector3((x + 1) * size, 0, y * size);

                // Vertical line
                points[pointIndex++] = new Vector3(x * size, 0, y * size);
                points[pointIndex++] = new Vector3(x * size, 0, (y + 1) * size);
            }
        }

        Gizmos.color = Color.white;
        Gizmos.DrawLineList(points);
    }

    private void OnDrawGizmos()
    {
        int gridSize = 100;
        int[,] grid = new int[gridSize, gridSize];
        drawGrid(grid, 5f, gridSize);
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
