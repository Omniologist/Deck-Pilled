using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Splines;

public class BSPDungeon : MonoBehaviour
{
    [SerializeField] int dungeonWidth = 100;
    [SerializeField] int dungeonHeight = 100;
    [SerializeField] int minRoomArea = 500;

    private BSPNode root;
    private bool hasGenerated = false;

    class BSPNode
    {
        public RectInt bounds;
        public BSPNode left;
        public BSPNode right;

        public BSPNode(RectInt bounds)
        {
            this.bounds = bounds;
        }
    }

    void Start()
    {
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        if (hasGenerated)
        {
            root = null;
        }

        root = new BSPNode(new RectInt(0, 0, dungeonWidth, dungeonHeight));
        SplitNode(root);

        if (!hasGenerated)
        {
            CreateRooms(root);
            hasGenerated = true;
        }
    }


    int SqrtAndRound(int number)
    {
        return Mathf.RoundToInt(Mathf.Sqrt(number));
    }

    void SplitNode(BSPNode node)
    {
        if ((node.bounds.width * node.bounds.height) < minRoomArea)
            return;

        bool splitHorizontally = Random.Range(0f, 1f) > 0.5f;

        if (splitHorizontally)
        {
            int splitY = Random.Range(node.bounds.y + 1, node.bounds.y + node.bounds.height);
            node.left = new BSPNode(new RectInt(
                node.bounds.x,
                node.bounds.y,
                node.bounds.width,
                splitY - node.bounds.y
            ));
            node.right = new BSPNode(new RectInt(
                node.bounds.x,
                splitY,
                node.bounds.width,
                (node.bounds.y + node.bounds.height) - splitY
            ));
        }
        else
        {
            int splitX = Random.Range(node.bounds.x + 1, node.bounds.x + node.bounds.width);
            node.left = new BSPNode(new RectInt(
                node.bounds.x,
                node.bounds.y,
                splitX - node.bounds.x,
                node.bounds.height
            ));
            node.right = new BSPNode(new RectInt(
                splitX,
                node.bounds.y,
                (node.bounds.x + node.bounds.width) - splitX,
                node.bounds.height
            ));
        }

        if (node.left != null) SplitNode(node.left);
        if (node.right != null) SplitNode(node.right);
    }

    void CreateRooms(BSPNode node)
    {
        if (node == null) return;

        if (node.left == null && node.right == null)
        {
            Vector3 bottomLeft = new Vector3(node.bounds.x, 0, node.bounds.y);
            Vector3 bottomRight = new Vector3(node.bounds.xMax, 0, node.bounds.y);
            Vector3 topRight = new Vector3(node.bounds.xMax, 0, node.bounds.yMax);
            Vector3 topLeft = new Vector3(node.bounds.x, 0, node.bounds.yMax);

            Debug.DrawLine(bottomLeft, bottomRight, Color.white, 100f);
            Debug.DrawLine(bottomRight, topRight, Color.white, 100f);
            Debug.DrawLine(topRight, topLeft, Color.white, 100f);
            Debug.DrawLine(topLeft, bottomLeft, Color.white, 100f);
            return;
        }

        CreateRooms(node.left);
        CreateRooms(node.right);
    }
}