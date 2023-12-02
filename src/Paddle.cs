using System;
using Godot;

public partial class Paddle : Node2D
{
  // _size refers to the number of paddle "segments"
  private int _size = 2;

  const int MaxSize = 3;

  private int _paddleTextureWidth;
  private Texture2D _paddleTexture;

  [Signal]
  public delegate void CollisionTopEventHandler();

  [Signal]
  public delegate void CollisionLeftEventHandler();

  [Signal]
  public delegate void CollisionRightEventHandler();

  public void UpdateNodePositions()
  {
    RemoveAllPaddleSegmentNodes();

    var totalWidth = ComputeTotalWidth();
    var halfWidth = totalWidth / 2;

    var totalHeight = GetCapHeight();
    var halfHeight = totalHeight / 2;

    var leftWidth = GetLeftCapWidth();

    // set left and right cap positions
    GetNode<Sprite2D>("LeftCap").Position = new Vector2((-halfWidth) + (leftWidth / 2), 0);
    GetNode<Sprite2D>("RightCap").Position = new Vector2(halfWidth - (GetRightCapWidth() / 2), 0);

    // iterate paddle segments
    var segmentX = (-halfWidth) + leftWidth + (_paddleTextureWidth / 2);
    for (int i = 0; i < GetNumberOfSegmentNodes(); i++)
    {
      AddPaddleSegmentAtXCoordinate(segmentX);
      segmentX += _paddleTextureWidth;
    }

    // lastly resize collision bounds
    var collisionTop = GetNode<CollisionShape2D>("CollisionTop/CollisionTopArea");
    var collisionTopShape = (RectangleShape2D)collisionTop.Shape;
    collisionTopShape.Size = new Vector2(totalWidth, 1);
    collisionTop.Position = new Vector2(0, (-halfHeight) + 1);

    var collisionLeft = GetNode<CollisionShape2D>("CollisionLeft/CollisionLeftArea");
    var collisionLeftShape = (RectangleShape2D)collisionLeft.Shape;
    collisionLeftShape.Size = new Vector2(2, GetCapHeight() - 4);
    collisionLeft.Position = new Vector2(-halfWidth + 1, 0);

    var collisionRight = GetNode<CollisionShape2D>("CollisionRight/CollisionRightArea");
    var collisionRightShape = (RectangleShape2D)collisionRight.Shape;
    collisionRightShape.Size = new Vector2(2, GetCapHeight() - 4);
    collisionRight.Position = new Vector2(halfWidth - 1, 0);
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

  public int GetXBuffer()
  {
    return ComputeTotalWidth() / 2;
  }

  private int ComputeTotalWidth()
  {
    // get total width
    return GetLeftCapWidth()
      + GetRightCapWidth()
      + (GetNumberOfSegmentNodes() * _paddleTextureWidth);
  }

  private int GetLeftCapWidth()
  {
    return GetNode<Sprite2D>("LeftCap").Texture.GetWidth();
  }

  private int GetRightCapWidth()
  {
    return GetNode<Sprite2D>("RightCap").Texture.GetWidth();
  }

  private int GetNumberOfSegmentNodes()
  {
    return (2 * _size) - 1;
  }

  private int GetCapHeight()
  {
    return Math.Max(
      GetNode<Sprite2D>("LeftCap").Texture.GetHeight(),
      GetNode<Sprite2D>("RightCap").Texture.GetHeight()
    );
  }

  public override void _Ready()
  {
    var paddleTextureNode = GetNode<Sprite2D>("PaddleTexture");
    _paddleTexture = paddleTextureNode.Texture;
    _paddleTextureWidth = _paddleTexture.GetWidth();
    paddleTextureNode.Hide();

    UpdateNodePositions();
  }

  private void OnCollisionTopAreaEntered(Area2D area)
  {
    EmitSignal(SignalName.CollisionTop, area);
  }

  private void OnCollisionLeftAreaEntered(Area2D area)
  {
    EmitSignal(SignalName.CollisionLeft, area);
  }

  private void OnCollisionRightAreaEntered(Area2D area)
  {
    EmitSignal(SignalName.CollisionRight, area);
  }
}
