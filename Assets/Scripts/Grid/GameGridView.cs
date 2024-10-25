using System;
using UnityEngine;

public class Node
{
    public int x;
    public int y;
    public int gCost;
    public int hCost;
    public int fCost => gCost + hCost;
    public Node parent;
    public bool isWalkable = true;

    public Node(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

public class GameGridView : MonoBehaviour
{
    [field: SerializeField] public int x;
    [field: SerializeField] public int y;
    
    private Node [,] _grid;

    private void Awake()
    {
        _grid = new Node[x, y];
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                var positionWithTransformPos = transform.position + new Vector3(i, j, 0);
                _grid[i, j] = new Node((int)positionWithTransformPos.x, (int)positionWithTransformPos.y);
            }
        }
    }
    
    public void SetNode(int x, int y, Node node)
    {
        _grid[x, y] = node;
    }
    
    public Node GetNode(int x, int y)
    {
        return _grid[x, y];
    }
    
    public Vector2 GetSize()
    {
        return new Vector2(_grid.GetLength(0), _grid.GetLength(1));
    }

    private void OnDrawGizmos()
    {
        if (_grid == null) return;

        float tileSize = 32f;
        float spacing = 0.05f;

        Vector3 cellSize = new Vector3(tileSize, tileSize, 0);

        foreach (var node in _grid)
        {
            Gizmos.color = node.isWalkable ? Color.grey : Color.red;
            Vector3 position = new Vector3(
                node.x * (cellSize.x + spacing), 
                node.y * (cellSize.y + spacing), 
                0);
            
            Gizmos.DrawWireCube(position, cellSize);
            
        }
    }

}
