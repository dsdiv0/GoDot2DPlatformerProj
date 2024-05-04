using Godot;
using System;

public partial class Shroom : CharacterBody2D
{
	public int Shspeed = 100;
	private float Gravity = 50;
	AnimationPlayer Shap;
	private Knight knight;
	public bool Shactive;
	[Export]public bool Shabletoattack;
	public float ShattackTimer = 1f;
	private float ShattackTimerReset = 1f;
	public float Shhealth = 5;
	private Sprite2D Shsprite;
	public override void _Process(double delta)
	{
		Vector2 velocity = Velocity;
		if (Shactive)
		{
			var angle = GlobalPosition.AngleToPoint(knight.GlobalPosition);
			Vector2 knightPosition = knight.GlobalPosition;
			Vector2 direction = (knightPosition - GlobalPosition).Normalized();
			velocity.X = direction.X * Shspeed;
			if (Mathf.Abs(angle) > Mathf.Pi / 2)
			{
				Shsprite.FlipH = true;
			}
			else Shsprite.FlipH = false;

			if (Shabletoattack)
			{
				GD.Print("attack");
				Shabletoattack = false;
				ShattackTimer = ShattackTimerReset;
				if (ShattackTimer <= 0)
				{
					Shabletoattack = true;
				}
				else ShattackTimer -= (float)delta;
			}
		}
		Velocity = velocity;
	}
	private void AnimationUpdate()
	{
		if (Shactive)
		{
			Shap.Play("ShAttack");
		}
		else
		{
			Shap.Play("ShRun");
		}
	}
	private void OnPlayerDetectionBodyEntered(Node2D body)
	{
		GD.Print("enter" + body);
		if (body is Knight)
		{
			knight = body as Knight;
			Shactive = true;
		}
	}
	private void OnPlayerDetectionBodyExited(Node2D body)
	{
		GD.Print("exit" + body);
		if (body is Knight)
		{
			knight = body as Knight;
			Shactive = false;
		}
	}
	public override void _Ready()
	{
		Shap = GetNode<AnimationPlayer>("AnimationPlayer");
		Shsprite = GetNode<Sprite2D>("Sprite2D");
	}
	private void OnHurtBoxAreaEntered(Area2D area)
	{
		Shhealth -= 1;
		GD.Print(Shhealth);
	}
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		if (!IsOnFloor())
		{
			velocity.Y = Gravity;
		}
		if (Shhealth <= 0)
		{
			QueueFree();
		}
		AnimationUpdate();
		Velocity = velocity;
		MoveAndSlide();
	}
}
