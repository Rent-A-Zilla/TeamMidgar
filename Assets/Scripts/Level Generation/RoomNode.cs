using UnityEngine;
public class RoomNode : Node
{
    public RoomNode(
        Vector2Int bottomLeftAreaCorner,
        Vector2Int topRightAreaCorner,
        Node parentNode,
        int index) : base(parentNode)
    {
        BottemLeftAreaCorner = bottomLeftAreaCorner;
        TopRightAreaCorner = topRightAreaCorner;

        BottomRightAreaCorner = new Vector2Int(topRightAreaCorner.x, bottomLeftAreaCorner.y);

        TopLeftAreaCorner = new Vector2Int(bottomLeftAreaCorner.x, topRightAreaCorner.y);

        TreeLayerIndex = index;
    }

    public int Width
    {
        get => TopRightAreaCorner.x - BottemLeftAreaCorner.x;
    }

    public int Length
    {
        get => TopRightAreaCorner.y - BottemLeftAreaCorner.y;
    }
}