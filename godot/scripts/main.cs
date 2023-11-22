using Godot;
using System;
using static System.Net.Mime.MediaTypeNames;

public partial class main : Node2D
{

    const float DROP_HEIGHT = 100f;

    RigidBody2D RigidBodyPreview = null;
    CollisionShape2D CollisionShapePreview = null;
    Sprite2D Sprite2DPreview = null;
    Sprite2D Sprite2DLine = null;
    public static RandomNumberGenerator RNG = new RandomNumberGenerator();
    static Dog CurrentType = null;
    public static Node2D thisObject = null;
    public static Node newBall;

    public CollisionShape2D CollisionShapeBottom;
    public CollisionShape2D CollisionShapeLeft;
    public CollisionShape2D CollisionShapeRight;

    static Sprite2D Sprite2DGameOver;
    static Sprite2D Sprite2DRestart;
    static bool GameOverFlag = false;
    public static float dropPosition = 0;
    public static int score = 0;
    public static Sprite2D TEST;

    //redeem 50 points to drop fruit
    //Redeem 500 points unstick/rotate/bump/tilt
    //draw scale 1 to 100 on game OR fruit animates left to right and shows number 1-100 of potision it's at
    //cooldown 10s
    //physics reaction when you move the window / resize the window

    //11 sizes
    //5 sizes droppable

    //TODO:
    //Loss state
    //scoring
    //Show whats coming next
    //Draw straight line downwards
    //Show evolution chart
    //allow user to switch between current and next
    //Different weight for each type
    //sound effects
    //music 
    //Some sort of visual representation of DEATHZONE

    //CHAT
    //Seperate scores for everyone
    //Peoples name on balls they dropped / scored from
    //In chat say what ball is up next as soon as this one is dropped
    //Lynch: !drop 77
    //ChatGuess: Lynch has dropped, next ball is an orange
    //preview scrolls on it's own and has number from 1-100
    //on screen show the command "use !suika 1-100" to drop a ball
    //ideally tho it'd be a redeem, then I can set timeouts per person
    //balls are user's profile pic, but also need some way other than size to match
    //frames to tell between the different fruits like Discord/Steam frames
    //https://store.steampowered.com/points/shop/c/avatar/cluster/1


    //Hook up to chat
    //Then person name is on the ball if they drop it OR trigger a combo
    //Was going to have a textbox anyway for showing 1-100 on where to drop.

    public override void _Ready()
    {
        SocketAsync();

        thisObject = this;
        TEST = (Sprite2D)GetNode(new NodePath("TEST"));
        RigidBodyPreview = (RigidBody2D)GetNode(new NodePath("Preview"));
        Sprite2DLine = (Sprite2D)GetNode(new NodePath("SpriteLine"));
        RigidBodyPreview.SetMeta("preview", true);
        RigidBodyPreview.BodyEntered += (Node body) => {
            if (body is RigidBody2D)
            {
                GD.Print("COLLISION A with " + body);
            }
            //body.pass();
        };
        CollisionShapePreview = (CollisionShape2D)RigidBodyPreview.FindChild("CollisionShapeDog1");
        Sprite2DPreview = (Sprite2D)RigidBodyPreview.FindChild("SpriteDog1");
        Dog.CreateDogs();
        setNextSize();

        CollisionShapeBottom = (CollisionShape2D)GetNode(new NodePath("Container/CollisionShapeBottom"));
        CollisionShapeLeft = (CollisionShape2D)GetNode(new NodePath("Container/CollisionShapeLeft"));
        CollisionShapeRight = (CollisionShape2D)GetNode(new NodePath("Container/CollisionShapeRight"));

        Sprite2DGameOver = (Sprite2D)GetNode(new NodePath("Sprite2DGameOver"));
        Sprite2DRestart = (Sprite2D)GetNode(new NodePath("Sprite2DRestart"));

    }

    public async void SocketAsync()
    {
        await WebSocketController.Setup("lynchmakesgames"/*auth.broadcaster*/);
    }

    public override void _Process(double delta)
    {
        if (GameOverFlag == true)
        {
            PhysicsServer2D.SetActive(false);
            Sprite2DGameOver.Visible = true;
            Sprite2DRestart.Visible = true;
            return;
        }

        if (dropPosition != 0)
        {
            float circleRadius = 10f;
            float wallRadius = 10f;

            float minimum = CollisionShapeLeft.Position.X + CurrentType.Scale * circleRadius + wallRadius;
            float maximum = CollisionShapeRight.Position.X - CurrentType.Scale * circleRadius - wallRadius;

            TEST.Position = new Vector2(minimum + (maximum - minimum)/2, TEST.Position.Y);
            TEST.Scale = new Vector2(maximum-minimum, TEST.Scale.Y);

            GD.Print("minimum = " + minimum);
            GD.Print("maximum = " + maximum);

            float dropX = ((maximum - minimum) / 100f) * dropPosition + minimum;

            GD.Print("dropping at = " + dropX);
            GD.Print("RigidBodyPreview.Position.Y = " + RigidBodyPreview.Position.Y);

            dropPosition = 0;

            drop(new Vector2(dropX, DROP_HEIGHT));
        }

        Vector2 MousePosition = GetViewport().GetMousePosition();
        //RigidBodyPreview.Position = new Vector2(MousePosition.X, DROP_HEIGHT);
        //if (RigidBodyPreview.Position.X < CollisionShapeLeft.Position.X)
        //{
        //    RigidBodyPreview.Position = new Vector2(CollisionShapeLeft.Position.X, RigidBodyPreview.Position.Y);
        //}
        //else if (RigidBodyPreview.Position.X > CollisionShapeRight.Position.X)
        //{
        //    RigidBodyPreview.Position = new Vector2(CollisionShapeRight.Position.X, RigidBodyPreview.Position.Y);
        //}

        RigidBodyPreview.Position = new Vector2(200, DROP_HEIGHT+50);

        Sprite2DLine.Position = new Vector2(RigidBodyPreview.Position.X, Sprite2DLine.Position.Y);

        if (newBall != null)
        {
            thisObject.AddChild(newBall);
            newBall = null;
        }
    }

    public override void _Input(InputEvent @event)
    {
        // Mouse in viewport coordinates.
        if (@event is InputEventMouseButton eventMouseButton)
        {
            GD.Print("mouse click");

            //mouseDown = true;
            if(GameOverFlag)
            {

                score = 0;
                GD.Print("mouse click gameover");
                //if (Sprite2DRestart.GetRect().HasPoint(ToLocal(eventMouseButton.Position)))
                //{
                    GD.Print("mouse click restart");
                    //Clicked on restart button
                    //Delete all the balls except the preview ball
                    //hide restart button
                    //hide the gameover image
                    //set gameover flag to false

                    foreach (Node child in GetChildren())
                    {
                        if (child is RigidBody2D && !(bool)child.GetMeta("preview", false))
                        {
                            child.Free();
                        }
                    }
                    PhysicsServer2D.SetActive(true);
                    Sprite2DGameOver.Visible = false;
                    Sprite2DRestart.Visible = false;
                    GameOverFlag = false;
                //}
                
            } else if (eventMouseButton.ButtonIndex == MouseButton.Left && eventMouseButton.Pressed)
            {
                //drop(RigidBodyPreview.Position);
            }
        }
    }
    public void drop(Vector2 position)
    {
        CreateDog(position, CurrentType.Type);
        setNextSize();
    }

    public void setNextSize()
    {
        int random = RNG.RandiRange(0, 4);

        CurrentType = Dog.Dogs[random];


        foreach (Node child in CollisionShapePreview.GetChildren())
        {
            ((Sprite2D)child).Visible = false;
        }        

        ((Sprite2D)CollisionShapePreview.GetChildren()[random]).Visible = true;

        CollisionShapePreview.Scale = new Vector2(CurrentType.Scale, CurrentType.Scale);
    }

    public static void CreateDog(Vector2 Position, int Type)
    {

        GD.Print("CreateDog: " + Position + ", " + Type);

        if (Type >= Dog.Dogs.Length)
        {
            Type = 0;
        }

        Dog TypeToUse = Dog.Dogs[Type];
        RigidBody2D RigidBodyDog = (RigidBody2D)ResourceLoader.Load<PackedScene>("res://dog1.tscn").Instantiate();
        RigidBodyDog.Mass = TypeToUse.Scale;
        //RigidBodyDog.GravityScale = 0;
        //RigidBodyDog.LinearVelocity = new Vector2(0, 100);
        //RigidBodyDog.Mass = 0;

        CollisionShape2D CollisionShapeDog = (CollisionShape2D)RigidBodyDog.FindChild("CollisionShapeDog1");
        Sprite2D Sprite2DDog = (Sprite2D)RigidBodyDog.FindChild("SpriteDog1");
        CollisionShapeDog.Scale = new Vector2(TypeToUse.Scale, TypeToUse.Scale);
        RigidBodyDog.Position = Position;
        RigidBodyDog.SetMeta("type", TypeToUse.Type);

        ((Sprite2D)CollisionShapeDog.GetChildren()[Type]).Visible = true;
        //CollisionShapePreview.Scale = new Vector2(CurrentType.Scale, CurrentType.Scale);

        //CollisionShapeDog.SetPhysicsProcess(false);
        //RigidBodyDog.SetPhysicsProcess(false);

        newBall = RigidBodyDog;

    }
    public static void GameOver()
    {
        GameOverFlag = true;
    }
}
