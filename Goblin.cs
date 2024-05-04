using System;
using System.Linq.Expressions;
using Godot;

public partial class Goblin : CharacterBody2D
{
	private float Gspeed = 140;
	private float Gravity = 20;
	AnimationPlayer Gap;
	private Knight knight;
	private bool Gactive;
	[Export] private bool Gabletoattack;
	private float GattackTimer = 1f;
	private float GattackTimerReset = 1f;
	private float Ghealth = 3;
	private Sprite2D Gsprite;
	private RayCast2D rayleft;
	private RayCast2D rayright;
	public override void _Process(double delta)
	{
		Vector2 velocity = Velocity;
		if (Gactive)
		{
			var angle = GlobalPosition.AngleToPoint(knight.GlobalPosition);
			Vector2 knightPosition = knight.GlobalPosition;
			Vector2 direction = (knightPosition - GlobalPosition).Normalized();
			velocity.X = direction.X * Gspeed;
			if (Mathf.Abs(angle) > Mathf.Pi / 2)
			{
				Gsprite.FlipH = true;
			}
			else Gsprite.FlipH = false;

			if (Gabletoattack)
			{
				GD.Print("attack");
				Gabletoattack = false;
				GattackTimer = GattackTimerReset;
				if (GattackTimer <= 0)
				{
					Gabletoattack = true;
				}
				else GattackTimer -= (float)delta;
			}
		}
		Velocity = velocity;
	}
	private void AnimationUpdate()
	{
		if (Gactive)
		{
			Gap.Play("GAttack");
		}
		else if (!Gactive)
		{
			Gap.Play("GRun");
			MoveAndSlide();
		}
	}
	private void OnPlayerDetectionBodyEntered(Node2D body)
	{
		GD.Print("enter" + body);
		if (body is Knight)
		{
			knight = body as Knight;
			Gactive = true;
		}
	}
	private void OnPlayerDetectionBodyExited(Node2D body)
	{
		GD.Print("exit" + body);
		if (body is Knight)
		{
			knight = body as Knight;
			Gactive = false;
		}
	}
	public override void _Ready()
	{
		Gap = GetNode<AnimationPlayer>("AnimationPlayer");
		Gsprite = GetNode<Sprite2D>("Sprite2D");
		rayleft = GetNode<RayCast2D>("LeftRayCast");
		rayright = GetNode<RayCast2D>("RightRayCast");
	}
	private void OnHurtBoxAreaEntered(Area2D area)
	{
		Ghealth -= 1;
		GD.Print(Ghealth);
	}
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		if (!IsOnFloor())
		{
			velocity.Y += Gravity;
		}
		if (Ghealth <= 0)
		{
			QueueFree();
		}
		AnimationUpdate();
		Velocity = velocity;
	}
}