using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
    
class Dog
    {

        public static Dog[] Dogs =  new Dog[11];
        static float increaseSize = 0f;

        public static void CreateDogs()
        {
            Dogs[0] = new Dog(Type: 0, Scale: 3f+increaseSize, Color: Color.FromHtml("#FF0000"));
            Dogs[1] = new Dog(Type: 1, Scale: 4f + increaseSize, Color: Color.FromHtml("#F0F000"));
            Dogs[2] = new Dog(Type: 2, Scale: 5f + increaseSize, Color: Color.FromHtml("#00F000"));
            Dogs[3] = new Dog(Type: 3, Scale: 6f + increaseSize, Color: Color.FromHtml("#00F0F0"));
            Dogs[4] = new Dog(Type: 4, Scale: 7f + increaseSize, Color: Color.FromHtml("#0000FF"));
        
        //This one is good size
            Dogs[5] = new Dog(Type: 5, Scale: 8f + increaseSize, Color: Color.FromHtml("#F0F000"));        
            Dogs[6] = new Dog(Type: 6, Scale: 9f + increaseSize, Color: Color.FromHtml("#0F0F00"));
            Dogs[7] = new Dog(Type: 7, Scale: 10f + increaseSize, Color: Color.FromHtml("#00F0F0"));
            Dogs[8] = new Dog(Type: 8, Scale: 11f + increaseSize, Color: Color.FromHtml("#000F0F"));
            Dogs[9] = new Dog(Type: 9, Scale: 12f + increaseSize, Color: Color.FromHtml("#F00F00"));
            Dogs[10] = new Dog(Type: 10, Scale: 13f + increaseSize, Color: Color.FromHtml("#00F00F"));
        }

    public int Type;
    public float Scale;
    public Color Color;

        public Dog(int Type, float Scale, Color Color)
    {
        this.Type = Type;
        this.Scale = Scale;
        this.Color = Color; 
        }
    }
