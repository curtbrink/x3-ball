using System;
using Godot;

public partial class Paddle : Node2D
{
  // _size refers to the number of paddle "segments"
  private int _size = 3;

  //
  private int _paddleTextureWidth;
  private Texture2D _paddleTexture;

  public void UpdateTexturePositions()
  {
    _removeAllPaddleSegments();

    var leftCap = GetNode<Sprite2D>("LeftCap");
    var leftCapWidth = leftCap.Texture.GetWidth();
    var rightCap = GetNode<Sprite2D>("RightCap");
    var rightCapWidth = rightCap.Texture.GetWidth();

    // get total width
    var totalWidth = leftCapWidth + rightCapWidth + (_size * _paddleTextureWidth);
    var halfTotal = totalWidth / 2;

    // center is at totalWidth/2
    leftCap.Position = new Vector2((-halfTotal) + (leftCapWidth / 2), 0);
    rightCap.Position = new Vector2((halfTotal) - (rightCapWidth / 2), 0);

    var segmentX = (-halfTotal) + leftCapWidth + (_paddleTextureWidth / 2);
    for (int i = 0; i < _size; i++)
    {
      _addPaddleSegmentAtPosition(segmentX);
      segmentX += _paddleTextureWidth;
    }
  }

  private void _addPaddleSegmentAtPosition(int x)
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

  private void _removeAllPaddleSegments()
  {
    GetTree().CallGroup("paddleSegments", Node.MethodName.QueueFree);
  }

  public void UpdateSize()
  {
    _size += 2;
    if (_size == 7)
    {
      _size = 1;
    }
    UpdateTexturePositions();
  }

  public override void _Ready()
  {
    var paddleTextureNode = GetNode<Sprite2D>("PaddleTexture");
    _paddleTexture = paddleTextureNode.Texture;
    _paddleTextureWidth = _paddleTexture.GetWidth();
    paddleTextureNode.Hide();

    UpdateTexturePositions();

    Position = new Vector2(200, 200);
  }
}
