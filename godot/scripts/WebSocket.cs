using WebSocketSharp;
using Newtonsoft.Json;
using Godot;
using System.Threading.Tasks;
using System;

public class WebSocketController
{
    public static WebSocket ws;
    public static Boolean connected = false;
    public static String username;

    public async static Task Setup(string username)
    {
        WebSocketController.username = username;

        GD.Print("WebSocket setup...");

        if (ws != null && ws.IsAlive)
            return;

        GD.Print("Websocket connecting...");

        ws = new WebSocket("ws://localhost:8081");

        ws.OnError += (sender, eventArgs) =>
        {
            GD.PrintErr("Websocket Error " + sender + ", " + eventArgs);
        };

        ws.OnOpen += (sender, eventArgs) =>
        {


            GD.Print("Websocket Open");
            //public static bool showStartRoundButton = false;
            //public static bool showGiveUpButton = false;
            //public static bool showAutoButton = false;
            //public static bool hideStartRoundButton = false;
            //public static bool hideAutoButton = false;


            connected = true;
            ws.Send("{ \"type\": \"connected\", \"username\": \"" + username + "\"}");
        };

        ws.OnClose += (sender, eventArgs) =>
        {
            GD.Print("Websocket Closed");
            connected = false;
            Task.Delay(10_000).ContinueWith(t => Setup(username));
        };

        ws.OnMessage += (sender, messageEventArgs) =>
        {
            GD.Print("Websocket message received from " + ((WebSocket)sender).Url + ", Data : " + messageEventArgs.Data);
            WebSocketMessage wsm = JsonConvert.DeserializeObject<WebSocketMessage>(messageEventArgs.Data);
            if (wsm.type == "drop")
            {
                GD.Print("Message type is drop");
                WebSocketMessageDrop wsmd = JsonConvert.DeserializeObject<WebSocketMessageDrop>(messageEventArgs.Data);
                GD.Print("wsmd.postion = " + wsmd.position);
                main.dropPosition = wsmd.position;
                //game.ShowImage(wsmih.gameImage, wsmih.maxWidth);
            }
        };

        ws.Connect();
    }

    public async static void SendChatMessage(string message)
    {
        if (ws != null && ws.IsAlive)
            ws.Send(message);
    }

    //{ type: "drop", position: 100}

    class WebSocketMessage
    {
        public string type;
    }

    class WebSocketMessageDrop : WebSocketMessage
    {
        public int position;
    }
}