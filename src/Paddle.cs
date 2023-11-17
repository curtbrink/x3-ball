using System;
using Godot;

public partial class Paddle : Area2D
{
  // _size refers to the number of paddle "segments"
  private int _size = 2;

  const int MaxSize = 3;

  private int _paddleTextureWidth;
  private Texture2D _paddleTexture;

  public void UpdateNodePositions()
  {
    RemoveAllPaddleSegmentNodes();

    var leftCap = GetNode<Sprite2D>("LeftCap");
    var leftCapWidth = leftCap.Texture.GetWidth();
    var rightCap = GetNode<Sprite2D>("RightCap");
    var rightCapWidth = rightCap.Texture.GetWidth();

    var capHeight = rightCap.Texture.GetHeight();

    var numberOfNodes = (2 * _size) - 1; // e.g. size 2 = 3 segment nodes, size 3 = 5, etc.

    // get total width
    var totalWidth = leftCapWidth + rightCapWidth + (numberOfNodes * _paddleTextureWidth);
    var halfTotal = totalWidth / 2;

    // set left and right cap positions
    leftCap.Position = new Vector2((-halfTotal) + (leftCapWidth / 2), 0);
    rightCap.Position = new Vector2(halfTotal - (rightCapWidth / 2), 0);

    // iterate paddle segments
    var segmentX = (-halfTotal) + leftCapWidth + (_paddleTextureWidth / 2);
    for (int i = 0; i < numberOfNodes; i++)
    {
      AddPaddleSegmentAtXCoordinate(segmentX);
      segmentX += _paddleTextureWidth;
    }

    // lastly resize collision bounds
    var collisionShape = (RectangleShape2D)GetNode<CollisionShape2D>("CollisionArea").Shape;
    collisionShape.Size = new Vector2(totalWidth, capHeight);
  }

  private void AddPaddleSegmentAtXCoordinate(int x)
  {
    var newSegment = new Sprite2D
    {
      Name = "Segment",
      Position = new Vector2(x, 0),
      Texture = _paddleTexture,
    };
    AddChild(newSegment);
    newSegment.AddToGroup("paddleSegments");
  }

  private void RemoveAllPaddleSegmentNodes()
  {
    GetTree().CallGroup("paddleSegments", Node.MethodName.QueueFree);
  }

  public void UpdateSize(int numberOfSegments)
  {
    _size = Math.Clamp(numberOfSegments, 1, MaxSize);
    UpdateNodePositions();
  }

  public void IncreaseSize()
  {
    UpdateSize(_size + 1);
  }

  public void DecreaseSize()
  {
    UpdateSize(_size - 1);
  }

  public override void _Ready()
  {
    var paddleTextureNode = GetNode<Sprite2D>("PaddleTexture");
    _paddleTexture = paddleTextureNode.Texture;
    _paddleTextureWidth = _paddleTexture.GetWidth();
    paddleTextureNode.Hide();

    UpdateNodePositions();
  }
}
