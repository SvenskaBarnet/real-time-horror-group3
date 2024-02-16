using System.Net;
namespace real_time_horror_group3;

public class GameMessage()
{
    public static string Help(HttpListenerResponse response)
    {
        string message = @"
List of possible commands and paths:

Create player: 
curl -d <player name> localhost:3000/player

Ready:
curl -X PATCH localhost:3000/jimmy/ready

Start game: 
curl localhost:3000/<player name>/start

Move: 
curl -X PATCH -d <room name> localhost:3000/<player name>/move

Check window status: 
curl localhost:3000/<player name>/windows

Check door status: 
curl localhost:3000/<player name>/doors

Check room for danger:
curl localhost3000/<player name>/room

Lock window: 
curl -X PATCH -d <window name> localhost:3000/<player name>/windows

Lock door: 
curl -X PATCH -d <door name> localhost:3000/<player name>/doors

Remove danger:
curl -X PATCH localhost:3000/<player name>/room
"; 


        response.StatusCode = (int)HttpStatusCode.OK;
        return message;
    }

    DateTime then = new(2020, 05, 06);
    DateTime now = DateTime.Now;
    public static string NotFound(HttpListenerResponse response)
    {
        string message = "Invalid path or command.\nFor list of available commands: curl localhost:3000/help";
        response.StatusCode = (int)HttpStatusCode.NotFound;
        return message;
    }
}