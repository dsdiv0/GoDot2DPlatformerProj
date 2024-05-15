using Godot;
public partial class Knight : CharacterBody2D
{
    private float Speed = 150;
    private float JumpForce = 500;
    private float Gravity = 20;
    private Sprite2D sprite;
    private Area2D hitbox;
    private bool attacking;
    private bool rolling;
    private int Khealth = 10;
    private ProgressBar healthbar;
    private AnimationTree at;
    private CollisionShape2D swordHit;
    private CollisionShape2D playerHitbox;
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
        sprite = GetNode<Sprite2D>("Sprite2D");
        hitbox = GetNode<Area2D>("HitboxPivot/SwordHitbox");
        healthbar = GetNode<ProgressBar>("Healthbar");
        at = GetNode<AnimationTree>("AnimationTree");
        at.Active = true;
        swordHit = GetNode<CollisionShape2D>("HitboxPivot/SwordHitbox/CollisionShape2D2");
        playerHitbox = GetNode<CollisionShape2D>("Hurtbox/CollisionShape2D");
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
        playerHitbox.Disabled = false;
    }
    public void RollState()
    {
        var stateMachine = GetNode<AnimationTree>("AnimationTree").Get("parameters/playback").As<AnimationNodeStateMachinePlayback>();
        stateMachine.Travel("Roll");
        MoveAndSlide();
        swordHit.Disabled = true;
    }
    public void JumpState()
    {
        var stateMachine = GetNode<AnimationTree>("AnimationTree").Get("parameters/playback").As<AnimationNodeStateMachinePlayback>();
        stateMachine.Travel("Jump");
        MoveAndSlide();
        swordHit.Disabled = true;
        playerHitbox.Disabled = false;
    }
    public void FallState()
    {
        var stateMachine = GetNode<AnimationTree>("AnimationTree").Get("parameters/playback").As<AnimationNodeStateMachinePlayback>();
        stateMachine.Travel("Fall");
        MoveAndSlide();
        swordHit.Disabled = true;
        playerHitbox.Disabled = false;
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
    private void FallAnimFinished()
    {
        Vector2 velocity = Velocity;
        if (velocity.Y == 0)
        {
            state = State.MOVE;
        }
        Velocity = velocity;
    }
}


