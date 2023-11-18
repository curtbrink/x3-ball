using System;
using Godot;

public partial class Main : Node
{
  private Rect2 _viewport;

  public override void _Ready()
  {
    _viewport = GetViewport().GetVisibleRect();

    var paddle = GetNode<Paddle>("Paddle");
    paddle.UpdateSize(1);
    paddle.Position = new Vector2(_viewport.Size.X / 2, _viewport.Size.Y - 50);

    Input.MouseMode = Input.MouseModeEnum.Captured;
  }

  public override void _Process(double delta)
  {
    if (Input.IsActionPressed("quit_game"))
    {
      GetTree().Quit();
    }
  }

  public override void _Input(InputEvent @event)
  {
    if (@event is InputEventMouseMotion eventMouseMotion)
    {
      var halfPaddleWidth = GetNode<Paddle>("Paddle").GetXBuffer();
      var minX = halfPaddleWidth;
      var maxX = _viewport.Size.X - halfPaddleWidth;

      var paddle = GetNode<Paddle>("Paddle");

      var newX = Mathf.Clamp(paddle.Position.X + eventMouseMotion.Relative.X, minX, maxX);

      paddle.Position = new Vector2(newX, paddle.Position.Y);
    }
  }

  private void OnSizeTimerTimeout()
  {
    GetNode<Paddle>("Paddle").IncreaseSize();
  }
}
