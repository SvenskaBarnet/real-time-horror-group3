using Npgsql;
using System;
using System.Net;

namespace real_time_horror_group3;

public class Highscore()
{
    public static bool HandleGameOver(NpgsqlDataSource db, HttpListenerRequest request, HttpListenerResponse response)
    {
      var cmd = db.CreateCommand(@"
      SELECT COUNT(*) 
      FROM public.player
      WHERE is_dead = true
        ");

        var cmd2 = db.CreateCommand(@"
        SELECT COUNT(*) 
        FROM public.player;
        ");


        using var reader = cmd.ExecuteReader();
        int deadPlayer = 0;
        int totalPlayers = 0;


        if (reader.Read())
        {
            deadPlayer = reader.GetInt32(0);
        }

        using var reader2 = cmd2.ExecuteReader();
        if (reader2.Read())
        {
            totalPlayers = reader2.GetInt32(0);
        }

        reader.Close();
        reader2.Close();

        response.StatusCode = (int)HttpStatusCode.OK;
        if (deadPlayer == totalPlayers && totalPlayers != 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static void AddScore(NpgsqlDataSource db, HttpListenerRequest request, HttpListenerResponse response)
    {

        var selectName = db.CreateCommand(@"
        SELECT name 
        FROM public.player;
        ");

        using var reader = selectName.ExecuteReader();

        string playerNames = string.Empty;

        while (reader.Read())
        {
            playerNames += $"{reader.GetString(0)}, ";
        }
        playerNames = playerNames.Substring(0, playerNames.Length - 2);
        reader.Close();

        string time = Session.FormattedTime(db);
        var highscore = db.CreateCommand(@"
        INSERT INTO public.highscore(player_names, ""time"")
        VALUES ($1, $2);
    ");
        highscore.Parameters.AddWithValue(playerNames);
        highscore.Parameters.AddWithValue(time);
        highscore.ExecuteNonQuery();
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

    public static bool Death(NpgsqlDataSource db)
    {
        var cmd = db.CreateCommand(@"
        SELECT *
        FROM PUBLIC.PLAYER
        WHERE IS_DEAD = TRUE;
        ");

        using var reader = cmd.ExecuteReader();
        int playerDeath = 0;

        while (reader.Read())
        {
            playerDeath = reader.GetInt32(3);
        }

        reader.Close();

        if (playerDeath == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}