using System;
using Godot;

public partial class Main : Node
{
  public override void _Ready()
  {
    var paddle = GetNode<Paddle>("Paddle");
    paddle.UpdateSize(1);
    paddle.Position = new Vector2(400, 400);
  }
}
