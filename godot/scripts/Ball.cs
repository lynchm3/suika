using Godot;
using GodotPlugins.Game;
using System;

public partial class Ball : RigidBody2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        this.BodyEntered += (body) => {

            //GD.Print("otherBody = " + body);

            if (body is RigidBody2D)
            {
                //RigidBody2D thisBody = (RigidBody2D)this;
                RigidBody2D otherBody = (RigidBody2D)body;
                //GD.Print("COLLISION B with = " + otherBody);
                //GD.Print("COLLISION B scale = " + otherBody.Scale);
                //GD.Print("RigidBodyDog = " + RigidBodyDog);

                int otherType = (int)otherBody.GetMeta("type");
                int thisType = (int)this.GetMeta("type");

                if (otherType == thisType)
                {
                    main.Score(thisType);
                    main.CreateDog(CalculateMidpoint(otherBody.Position, this.Position), thisType+1);
                    otherBody.Free();
                    this.QueueFree();
                }
            } else if(body is Area2D)
            {
                GD.Print("GAME OVER");
            }
            //body.pass();
        };

    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public static Vector2 CalculateMidpoint(Vector2 point1, Vector2 point2)
    {
        return new Vector2((point1.X + point2.X) / 2, (point1.Y + point2.Y) / 2);
    }
}
