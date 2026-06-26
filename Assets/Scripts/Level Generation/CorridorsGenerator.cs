using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class CorridorsGenerator
{
    public List<Node> CreateCorridor(List<RoomNode> allNodesCollection, int corridorWidth)
    {
        List<Node> corridorList = new List<Node>();
        Queue<RoomNode> structuresToCheck = new Queue<RoomNode>(
            allNodesCollection.OrderByDescending(node => node.TreeLayerIndex).ToList());
        while (structuresToCheck.Count > 0)
        {
            var node = structuresToCheck.Dequeue();
            if (node.ChildrenNodeList.Count == 0)
            {
                continue;
            }
            (Node leftRoom, Node rightRoom) =
    GetClosestRooms(node.ChildrenNodeList[0],node.ChildrenNodeList[1]);

            CorridorNode corridor = new CorridorNode(leftRoom, rightRoom,corridorWidth);

            corridorList.Add(corridor);
        }
        return corridorList;
    }
    private (Node, Node) GetClosestRooms(Node leftParent, Node rightParent)
    {
        List<Node> leftRooms = StructureHelper.TraverseGraphToExtractLowestLeafes(leftParent);
        List<Node> rightRooms = StructureHelper.TraverseGraphToExtractLowestLeafes(rightParent);

        float shortestDistance = float.MaxValue;

        Node bestLeft = null;
        Node bestRight = null;

        foreach (Node left in leftRooms)
        {
            foreach (Node right in rightRooms)
            {
                Vector2 leftCenter =
                    (left.BottemLeftAreaCorner + left.TopRightAreaCorner) / 2;

                Vector2 rightCenter =
                    (right.BottemLeftAreaCorner + right.TopRightAreaCorner) / 2;

                float dist = Vector2.Distance(leftCenter, rightCenter);

                if (dist < shortestDistance)
                {
                    shortestDistance = dist;
                    bestLeft = left;
                    bestRight = right;
                }
            }
        }

        return (bestLeft, bestRight);
    }
}
