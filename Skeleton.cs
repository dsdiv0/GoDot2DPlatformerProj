using Godot;
using System;

public partial class Skeleton : CharacterBody2D
{
	public float Sspeed = 100;
	private float SGravity = 100;
	AnimationPlayer Sap;
	private Knight knight;
	public bool Sactive;
	[Export]public bool Sabletoattack;
	public float SattackTimer = 1f;
	private float SattackTimerReset = 1f;
	public float Shealth = 4;
	private Sprite2D Ssprite;
	public override void _Process(double delta)
	{
		Vector2 velocity = Velocity;
		if (Sactive)
		{
			var angle = GlobalPosition.AngleToPoint(knight.GlobalPosition);
			Vector2 knightPosition = knight.GlobalPosition;
			Vector2 direction = (knightPosition - GlobalPosition).Normalized();
			velocity.X = direction.X * Sspeed;
			if (Mathf.Abs(angle) > Mathf.Pi / 2)
			{
				Ssprite.FlipH = true;
			}
			else Ssprite.FlipH = false;

			if (Sabletoattack)
			{
				GD.Print("attack");
				Sabletoattack = false;
				SattackTimer = SattackTimerReset;
				if (SattackTimer <= 0)
				{
					Sabletoattack = true;
				}
				else SattackTimer -= (float)delta;
			}
		}
		Velocity = velocity;
	}
	private void AnimationUpdate()
	{
		if (Sactive)
		{
			Sap.Play("SAttack");
		}
		else
		{
			Sap.Play("SRun");
		}
	}
	private void OnPlayerDetectionBodyEntered(Node2D body)
	{
		GD.Print("enter" + body);
		if (body is Knight)
		{
			knight = body as Knight;
			Sactive = true;
		}
	}
	private void OnPlayerDetectionBodyExited(Node2D body)
	{
		GD.Print("exit" + body);
		if (body is Knight)
		{
			knight = body as Knight;
			Sactive = false;
		}
	}
	public override void _Ready()
	{
		Sap = GetNode<AnimationPlayer>("AnimationPlayer");
		Ssprite = GetNode<Sprite2D>("Sprite2D");
	}
	private void OnHurtBoxAreaEntered(Area2D area)
	{
		Shealth -= 1;
		GD.Print(Shealth);
	}
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		if (!IsOnFloor())
		{
			velocity.Y += SGravity;
		}
		if (Shealth <= 1)
		{
			QueueFree();
		}
		AnimationUpdate();
		Velocity = velocity;
		MoveAndSlide();
	}
}