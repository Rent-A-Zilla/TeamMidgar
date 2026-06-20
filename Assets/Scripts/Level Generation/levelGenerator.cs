using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class levelGenerator
{
    
    List<RoomNode> allSpaceNodes = new List<RoomNode>(); 
    private int levelWidth;
    private int levelLength;

    public levelGenerator(int levelWidth, int levelLength)
    {
        this.levelWidth = levelWidth;
        this.levelLength = levelLength;
    }

    public List<Node> CalculateLevel(int maxIterations, int widthMin, int lengthMin, float roomBottomCornerModifer, float roomTopCornerModifier, int roomOffset, int corridorWidth)
    {
        BinarySpacePartitioner bsp = new BinarySpacePartitioner(levelWidth, levelLength);
        allSpaceNodes = bsp.PrepareNodeCollection(maxIterations, widthMin, lengthMin);
        Debug.Log("Total BSP Nodes: " + allSpaceNodes.Count);
        Debug.Log("Root Children: " + bsp.RootNode.ChildrenNodeList.Count);
        List<Node> roomSpaces = StructureHelper.TraverseGraphToExtractLowestLeafes(bsp.RootNode);

        RoomGenerator roomGenerator = new RoomGenerator(maxIterations, lengthMin, widthMin);
        List<RoomNode> roomList = roomGenerator.GenrateRoomsInGivenSpaces(roomSpaces, roomBottomCornerModifer, roomTopCornerModifier, roomOffset);

        CorridorsGenerator corridorGenerator = new CorridorsGenerator();

        var corridorList = corridorGenerator.CreateCorridor(allNodesCollection, corridorWidth);


        return new List<Node>(roomList);
    }
}