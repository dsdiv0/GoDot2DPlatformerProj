using Godot;
public partial class Knight : CharacterBody2D
{
    private float Speed = 150;
    private float JumpForce = 500;
    private float Gravity = 20;
    private AnimationPlayer ap;
    private Sprite2D sprite;
    private Area2D hitbox;
    private bool attacking;
    private bool rolling;
    public int Khealth = 10;
    private ProgressBar healthbar;
    private Knight knight;
    private AnimationTree at;
    private enum State
    {
        MOVE,
        ROLL,
        ATTACK,
        JUMP,
        FALL
    }
    State state;
    public override void _Ready()
    {
        base._Ready();
        ap = GetNode<AnimationPlayer>("AnimationPlayer");
        sprite = GetNode<Sprite2D>("Sprite2D");
        hitbox = GetNode<Area2D>("HitboxPivot/SwordHitbox");
        healthbar = GetNode<ProgressBar>("Healthbar");
        at = GetNode<AnimationTree>("AnimationTree");
        at.Active = true;
    }

    private void Movement()
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
        Velocity = velocity;
        at.Set("parameters/Run/blend_position", input);
        at.Set("parameters/Attack/blend_position", 0);
        at.Set("parameters/Roll/blend_position", 0);
        at.Set("parameters/Jump/blend_position", 0);
        at.Set("parameters/Fall/blend_position", 0);
    }
    public override void _Process(double delta)
    {
        Vector2 velocity = Velocity;
        if (Input.IsActionJustPressed("attack"))
        {
            state = State.ATTACK;
        }
        if (Input.IsActionJustPressed("roll"))
        {
            state = State.ROLL;
        }
        if (velocity.Y < 0)
        {
            state = State.JUMP;
        }
        if (velocity.Y > 0)
        {
            state = State.FALL;
        }
        Velocity = velocity;
    }
    public void AttackState()
    {
        var stateMachine = GetNode<AnimationTree>("AnimationTree").Get("parameters/playback").As<AnimationNodeStateMachinePlayback>();
        stateMachine.Travel("Attack");
        MoveAndSlide();
    }
    public void RollState()
    {
        var stateMachine = GetNode<AnimationTree>("AnimationTree").Get("parameters/playback").As<AnimationNodeStateMachinePlayback>();
        stateMachine.Travel("Roll");
        MoveAndSlide();
    }
    public void JumpState()
    {
        var stateMachine = GetNode<AnimationTree>("AnimationTree").Get("parameters/playback").As<AnimationNodeStateMachinePlayback>();
        stateMachine.Travel("Jump");
        MoveAndSlide();
    }
    public void FallState()
    {
        var stateMachine = GetNode<AnimationTree>("AnimationTree").Get("parameters/playback").As<AnimationNodeStateMachinePlayback>();
        stateMachine.Travel("Fall");
        MoveAndSlide();
    }

    public override void _PhysicsProcess(double delta)
    {
        Movement();
        GD.Print("playa" + Velocity);
        switch (state)
        {
            case State.MOVE:
                MoveAndSlide();
                break;
            case State.ROLL:
                RollState();
                break;
            case State.ATTACK:
                AttackState();
                break;
            case State.JUMP:
                JumpState();
                break;
            case State.FALL:
                FallState();
                break;
        }
        if (Khealth <= 0)
        {
            QueueFree();
        }
    }
    public void OnHurtboxAreaEntered(Area2D area)
    {
        Khealth -= 1;
        healthbar.Value = Khealth;
        GD.Print(Khealth);
    }
    private void OnDeathLineBodyEntered(Node2D body)
    {
        QueueFree();
    }
    private void AttackAnimFinished()
    {
        state = State.MOVE;
    }
    private void RollingAnimFinished()
    {
        state = State.MOVE;
    }
}


