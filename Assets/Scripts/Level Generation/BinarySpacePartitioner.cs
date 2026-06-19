using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class BinarySpacePartitioner
{
    RoomNode rootNode;
   

    public RoomNode RootNode { get => rootNode; }

    public BinarySpacePartitioner(int levelWidth, int levelLength)
    {
        this.rootNode = new RoomNode(new Vector2Int(0, 0), new Vector2Int(levelWidth, levelLength), null, 0);
    }

    public List<RoomNode> PrepareNodeCollection(int maxIterations, int widthMin, int lengthMin)
    {
        Queue<RoomNode> graph = new Queue<RoomNode>();
        List<RoomNode> listToReturn = new List<RoomNode>();
        graph.Enqueue(this.rootNode);
        listToReturn.Add(this.rootNode);
        int interations = 0;
        while(interations < maxIterations && graph.Count > 0)
        {
            maxIterations++;
            RoomNode currentNode = graph.Dequeue();
            if(currentNode.Width >= widthMin*2 || currentNode.Length >= lengthMin*2)
            {
                SplitTheSpace(currentNode,listToReturn,lengthMin,widthMin, graph);
            }
        }
        return listToReturn;
    }

    private void SplitTheSpace(RoomNode currentNode, List<RoomNode> listToReturn, int lengthMin, int widthMin, Queue<RoomNode> graph)
    {
        Line line = GetLineDividingSpace(currentNode.BottemLeftAreaCorner, currentNode.TopRightAreaCorner, widthMin, lengthMin);

        RoomNode node1, node2;
        if (line.Orientation == Orientation.Horizontal)
        {
            node1 = new RoomNode(currentNode.BottemLeftAreaCorner,
                new Vector2Int(currentNode.TopRightAreaCorner.x, line.Coordinates.y),
                currentNode, currentNode.TreeLayerIndex + 1);

            node2 = new RoomNode(new Vector2Int(currentNode.BottemLeftAreaCorner.x, line.Coordinates.y),
                currentNode.TopRightAreaCorner,
                currentNode,
                currentNode.TreeLayerIndex + 1);
        }
        else
        {
            node1 = new RoomNode(currentNode.BottemLeftAreaCorner,
                new Vector2Int(line.Coordinates.x,currentNode.TopRightAreaCorner.y),
                currentNode, currentNode.TreeLayerIndex + 1);

            node2 = new RoomNode(new Vector2Int(line.Coordinates.x,currentNode.BottemLeftAreaCorner.y),
                currentNode.TopRightAreaCorner,
                currentNode,
                currentNode.TreeLayerIndex + 1);
        }
        AddNewNodesToCollections(listToReturn, graph, node1);
        AddNewNodesToCollections(listToReturn, graph, node2);
    }

    private void AddNewNodesToCollections(List<RoomNode> listToReturn, Queue<RoomNode> graph, RoomNode node)
    {
        listToReturn.Add(node);
        graph.Enqueue(node);
    }

    private Line GetLineDividingSpace(Vector2Int bottemLeftAreaCorner, Vector2Int topRightAreaCorner, int widthMin, int lengthMin)
    {
        Orientation orientation;
        bool lengthStatus = (topRightAreaCorner.y - bottemLeftAreaCorner.y) >= 2 * lengthMin;
        bool widthStatus = (topRightAreaCorner.x - bottemLeftAreaCorner.x) >= 2 * widthMin;
        if(lengthStatus && widthStatus)
        {
            orientation = (Orientation)(Random.Range(0, 2));
        }
        else if (widthStatus)
        {
            orientation = Orientation.Vertical;
        }
        else
        {
            orientation = Orientation.Horizontal;
        }
        return new Line(orientation, GetCoordiantesForOrientation(orientation, bottemLeftAreaCorner, topRightAreaCorner, widthMin, lengthMin));
    }

    private Vector2Int GetCoordiantesForOrientation(Orientation orientation, Vector2Int bottemLeftAreaCorner, Vector2Int topRightAreaCorner, int widthMin, int lengthMin)
    {
        Vector2Int coordinates = Vector2Int.zero;
        if (orientation == Orientation.Horizontal)
        {
            coordinates = new Vector2Int(
                0,
                Random.Range((bottemLeftAreaCorner.y + lengthMin),
                (topRightAreaCorner.y - lengthMin)));
        }
        else
        {
            coordinates = new Vector2Int(
                Random.Range(
                    (bottemLeftAreaCorner.x + widthMin),
                    (topRightAreaCorner.x - widthMin)),
                0);
        }
        return coordinates;
    }
}