using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator
{
    private int maxIterations;
    private int lengthMin;
    private int widthMin;

    public RoomGenerator(int maxIterations, int lengthMin, int widthMin)
    {
        this.maxIterations = maxIterations;
        this.lengthMin = lengthMin;
        this.widthMin = widthMin;
    }

    public List<RoomNode> GenrateRoomsInGivenSpaces(List<Node> roomSpaces, float roomBottomCornerModifer, float roomTopCornerModifier, int roomOffset)
    {
        List<RoomNode> listToReturn = new List<RoomNode>();
        foreach(var space in roomSpaces)
        {
            Vector2Int newBottomLeftPoint = StructureHelper.GenerateBottomLeftCornerBetween(space.BottemLeftAreaCorner, space.TopRightAreaCorner, roomBottomCornerModifer, roomOffset);
            Vector2Int newTopRightPoint = StructureHelper.GenerateTopRightCornerBetween(space.BottemLeftAreaCorner, space.TopRightAreaCorner, roomTopCornerModifier, roomOffset);
    

            space.BottemLeftAreaCorner = newBottomLeftPoint;
            space.TopRightAreaCorner = newTopRightPoint;
            space.BottomRightAreaCorner = new Vector2Int(newTopRightPoint.x, newBottomLeftPoint.y);
            space.TopLeftAreaCorner = new Vector2Int(newBottomLeftPoint.x, newTopRightPoint.y);
            listToReturn.Add((RoomNode)space);
        }
        return listToReturn;

    }
}