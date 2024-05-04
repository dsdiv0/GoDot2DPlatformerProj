using Godot;
using System;

public partial class Boss : CharacterBody2D
{
	public int Bspeed = 100;
	private float BGravity = 20;
	AnimationPlayer Bap;
	private Knight knight;
	public bool Bactive;
	[Export] public bool Babletoattack;
	public float BattackTimer = 1f;
	private float BattackTimerReset = 1f;
	public float Bhealth = 15;
	private Sprite2D Bsprite;
	public override void _Process(double delta)
	{
		Vector2 velocity = Velocity;
		if (Bactive)
		{
			var angle = GlobalPosition.AngleToPoint(knight.GlobalPosition);
			Vector2 knightPosition = knight.GlobalPosition;
			Vector2 direction = (knightPosition - GlobalPosition).Normalized();
			velocity.X = direction.X * Bspeed;
			if (Mathf.Abs(angle) > Mathf.Pi / 2)
			{
				Bsprite.FlipH = true;
			}
			else Bsprite.FlipH = false;

			if (Babletoattack)
			{
				GD.Print("attack");
				Babletoattack = false;
				BattackTimer = BattackTimerReset;
				if (BattackTimer <= 0)
				{
					Babletoattack = true;
				}
				else BattackTimer -= (float)delta;
			}
		}
		Velocity = velocity;
	}
	private void AnimationUpdate()
	{
		if (Bactive)
		{
			Bap.Play("BAttack");
		}
		else
		{
			Bap.Play("BIdle");
			MoveAndSlide();
		}
	}
	private void OnPlayerDetectionBodyEntered(Node2D body)
	{
		GD.Print("enter" + body);
		if (body is Knight)
		{
			knight = body as Knight;
			Bactive = true;
		}
	}
	private void OnPlayerDetectionBodyExited(Node2D body)
	{
		GD.Print("exit" + body);
		if (body is Knight)
		{
			knight = body as Knight;
			Bactive = false;
		}
	}
	public override void _Ready()
	{
		Bap = GetNode<AnimationPlayer>("AnimationPlayer");
		Bsprite = GetNode<Sprite2D>("Sprite2D");
	}
	private void OnHurtBoxAreaEntered(Area2D area)
	{
		Bhealth -= 1;
		GD.Print(Bhealth);
	}
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		if (IsOnFloor())
		{
			velocity.Y += BGravity;
		}
		if (Bhealth <= 0)
		{
			QueueFree();
		}
		AnimationUpdate();
		Velocity = velocity;
		GD.Print("boss", velocity);
	}
}
