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

    public List<Node> CalculateRooms(int maxIterations, int widthMin, int lengthMin)
    {
        BinarySpacePartitioner bsp = new BinarySpacePartitioner(levelWidth, levelLength);
        allSpaceNodes = bsp.PrepareNodeCollection(maxIterations, widthMin, lengthMin);
        List<Node> roomSpaces = StructureHelper.TraverseGraphToExtractLowestLeafes(bsp.RootNode);

        RoomGenerator roomGenerator = new RoomGenerator(maxIterations, lengthMin, widthMin);
        List<RoomNode> roomList = roomGenerator.GenrateRoomsInGivenSpaces(roomSpaces);
        return new List<Node>(roomList);
    }
}