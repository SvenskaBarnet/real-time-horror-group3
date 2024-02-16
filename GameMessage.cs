using System.Net;
using Npgsql;
namespace real_time_horror_group3;

public class GameMessage()
{
    public static string Help(HttpListenerResponse response)
    {
        string message = @"
List of possible commands and paths:

Create player: 
curl -d ""<player name>"" localhost:3000/player

Ready:
curl -X PATCH localhost:3000/<player name>/ready

Start game: 
curl localhost:3000/<player name>/start

Move to room: 
curl -X PATCH -d ""<room name>"" localhost:3000/<player name>/move

Check status of windows in current room: 
curl localhost:3000/<player name>/windows

Check status of doors in current room: 
curl localhost:3000/<player name>/doors

Check room for danger:
curl localhost3000/<player name>/room

Lock window: 
curl -X PATCH -d ""<window name>"" localhost:3000/<player name>/windows

Lock door: 
curl -X PATCH -d ""<door name>"" localhost:3000/<player name>/doors

Remove danger:
curl -X PATCH localhost:3000/<player name>/room

See time elapsed since start
curl localhost:3000/<player name>/time
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
    public static string PrintGameOverScreen(NpgsqlDataSource db, HttpListenerRequest request, HttpListenerResponse response)
    {

        var selectHighscore = db.CreateCommand(@"
      SELECT * FROM public.highscore
           ORDER BY ""time"" DESC 
           LIMIT 10;        
");


        string message = "GAMEOVER\n\nhighscore: \n";

        using var reader = selectHighscore.ExecuteReader();
        while (reader.Read())
        {
            message += $"{reader.GetString(1)} - {reader.GetString(2)}\n";
        }


        return message;

    }
    public static string Story(HttpListenerResponse respons)
    {
        string message = @"
You wake up in your house by the lake, tired and confused about a sound you hear outside. 
You are sure that you are home alone and when you go to the window you see two dark figures,
could this be the burglars that have been all over the news lately? 
You estimate you have 5 minutes until they reach your house.
while talking to the police they said they will arrive in 30 minutes to save you.
Hurry up and secure the home by locking all windows and doors.
But be aware of your surroundings so you don't kill yourself.
Check every room in the house carefully.
Events could happen anytime.
You need to continue to check the status in every room during your remaining time period.";
        return message;
    }
}