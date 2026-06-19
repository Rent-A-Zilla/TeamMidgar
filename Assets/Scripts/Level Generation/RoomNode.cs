using UnityEngine;
public class RoomNode : Node

{
    public RoomNode(Vector2Int bottomLeftAreaCorner, Vector2Int topRightAreaCorner, Node parentNode, int index) : base(parentNode)
    {
        this.BottomRightAreaCorner = bottomLeftAreaCorner; 
        this.TopRightAreaCorner = topRightAreaCorner;
        this.BottomRightAreaCorner = new Vector2Int(topRightAreaCorner.x, topRightAreaCorner.y);
        this.TopLeftAreaCorner = new Vector2Int(bottomLeftAreaCorner.x, bottomLeftAreaCorner.y);
        this.TreeLayerIndex = index;
    }

    public int Width { get => (int)(TopLeftAreaCorner.x - BottemLeftAreaCorner.x); }
    public int Length { get => (int)(TopRightAreaCorner.y - BottomRightAreaCorner.y); }
}