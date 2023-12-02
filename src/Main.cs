using System;
using Godot;

public partial class Main : Node
{
  private Rect2 _viewport;

  private int _ballVelocity;
  private Vector2 _ballHeading;

  private RandomNumberGenerator rng = new();

  public override void _Ready()
  {
    _viewport = GetViewport().GetVisibleRect();

    var paddle = GetNode<Paddle>("Paddle");
    paddle.UpdateSize(1);
    paddle.Position = new Vector2(_viewport.Size.X / 2, _viewport.Size.Y - 50);

    var ball = GetNode<Ball>("Ball");
    ball.Position = new Vector2(100, 100);
    _ballHeading = Vector2.Down;
    _ballVelocity = 200;

    Input.MouseMode = Input.MouseModeEnum.Captured;
  }

  public override void _Process(double delta)
  {
    if (Input.IsActionPressed("quit_game"))
    {
      GetTree().Quit();
    }

    GetNode<Ball>("Ball").Position += _ballHeading * (_ballVelocity * (float)delta);
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

  private void OnBallAreaEntered(Area2D area)
  {
    switch (area.Name)
    {
      case "LeftWall":
        handleWallBounce(Vector2.Right);
        break;
      case "RightWall":
        handleWallBounce(Vector2.Left);
        break;
      case "TopWall":
        handleWallBounce(Vector2.Down);
        break;
      case "BottomWall":
        resetBall();
        break;
    }
  }

  private void handlePaddleBounce()
  {
    _ballHeading = (Vector2.Up + Vector2.Left).Normalized();
  }

  private void handleWallBounce(Vector2 normal)
  {
    _ballHeading = _ballHeading.Bounce(normal).Normalized();
  }

  private void resetBall()
  {
    GetNode<Ball>("Ball").Position = new Vector2(100, 100);
    _ballHeading = new Vector2(
      rng.RandfRange(-1.0f, 1.0f),
      rng.RandfRange(-1.0f, 1.0f)
    ).Normalized();
  }

  private void OnPaddleCollisionLeft(Area2D area)
  {
    handleWallBounce(Vector2.Left);
  }

  private void OnPaddleCollisionRight(Area2D area)
  {
    handleWallBounce(Vector2.Right);
  }

  private void OnPaddleCollisionTop(Area2D area)
  {
    handleWallBounce(Vector2.Up);
  }
}
