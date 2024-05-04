using Godot;
public partial class Knight : CharacterBody2D
{
    private float Speed = 150;
    private float JumpForce = 500;
    private float Gravity = 20;
    private AnimationPlayer ap;
    private Sprite2D sprite;
    private Area2D hitbox;
    [Export] public bool attacking = false ;
    [Export] public bool rolling = false ;
    public int Khealth = 10;
    private ProgressBar healthbar;
    private Knight knight;
    public override void _Ready()
    {
        base._Ready();
        ap = GetNode<AnimationPlayer>("AnimationPlayer");
        sprite = GetNode<Sprite2D>("Sprite2D");
        hitbox = GetNode<Area2D>("HitboxPivot/SwordHitbox");
        healthbar = GetNode<ProgressBar>("Healthbar");
    }
    public void OnHurtboxAreaEntered(Area2D area)
    {
        Khealth -= 1;
        healthbar.Value = Khealth;
        GD.Print(Khealth);
    }

    public void UpdateAnimation()
    {
        Vector2 velocity = Velocity;
        Velocity = velocity;
        if (IsOnFloor())
        {
            if (!attacking && !rolling)
            {
                if (velocity.X != 0)
                {
                    ap.Play("Run");
                }
                else ap.Play("Idle");
            }
        }

        if (velocity.Y < 0)
        {
            ap.Play("Jump");
        }
        else if (velocity.Y > 0)
        {
            ap.Play("Fall");
        }

    }
    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("attack"))
        {
            Attack();
        }
        if (Input.IsActionJustPressed("roll"))
        {
            Roll();
        }
    }
    public void Attack()
    {
        attacking = true;
        ap.Play("Attack");
    }
    public void Roll()
    {
        rolling = true;
        ap.Play("Roll");
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;
        var input = Input.GetAxis("move_left", "move_right");
        velocity.X = Speed * input;
        if (!IsOnFloor())
        {
            velocity.Y += Gravity;
        }
        if (input != 0)
        {
            sprite.FlipH = input == -1;
        }
        if (Input.IsActionJustPressed("jump") && IsOnFloor())
        {
            velocity.Y = -JumpForce;
        }
        if (Khealth <= 0)
        {
            QueueFree();
        }
        Velocity = velocity;
        UpdateAnimation();
        MoveAndSlide();
    }
    private void OOnDeathLineBodyEntered(Node2D body)
    {
        if (body is Knight)
        {
            knight = body as Knight;
            QueueFree();
        }
        QueueFree();
    }
}


