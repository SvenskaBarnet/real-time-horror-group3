using Npgsql;
using System.Globalization;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.Serialization;

namespace real_time_horror_group3;

public class Session(NpgsqlDataSource db)
{
    public void Start()
    {
        string message = string.Empty;
        using var select = db.CreateCommand(@"
                        SELECT COUNT(id)
                        FROM public.session
                        ");

        var reader = select.ExecuteReader();
        int count = 1;
        if (reader.Read())
        {
            count = reader.GetInt32(0);
        }
        if (count is 0)
        {
            using var insert = db.CreateCommand(@"
                            INSERT INTO public.session(
	                        time)
	                        VALUES (
                            CURRENT_TIMESTAMP);
                            UPDATE entry_point
                            SET time = CURRENT_TIMESTAMP;
                            ");
            insert.ExecuteNonQuery();
        }
    }

    public bool EntryPointTimer()
    {
        bool gameOver = false;
        using var cmd = db.CreateCommand(@"
        SELECT to_char(""time"", 'HH24:MI:SS')
        FROM public.entry_point
        WHERE time is not null;
        ");

        var reader = cmd.ExecuteReader();
        TimeOnly currentTime = TimeOnly.FromDateTime(DateTime.Now);
        String sessionStart = string.Empty;

        while (reader.Read())
        {
            sessionStart = reader.GetString(0);

            var split = sessionStart.Split(":");

            TimeOnly startTime = new(int.Parse(split[0]), int.Parse(split[1]), int.Parse(split[2]));




            TimeSpan timeElapsed = currentTime - startTime;
            if ((timeElapsed.TotalSeconds > 360)) // 4 minuter tills det blir "game over"
            {
                gameOver = true;
                break;
            }
            else
            {
                gameOver = false;

            }

        }

        return gameOver;
    }
    public TimeSpan ElapsedTime()
    {
        using var sessionStart = db.CreateCommand(@"
            SELECT to_char(""time"", 'HH24:MI:SS')
            FROM public.session 
            WHERE time is not null;
            ");
        var reader = sessionStart.ExecuteReader();

        TimeOnly currentTime = TimeOnly.FromDateTime(DateTime.Now);
        TimeOnly startTime = currentTime;
        while (reader.Read())
        {
            string[] time = reader.GetString(0).Split(":");
            startTime = new(int.Parse(time[0]), int.Parse(time[1]), int.Parse(time[2]));
        }

        TimeSpan interval = currentTime - startTime;
        return interval;
    }
    public string FormattedTime()
    {
        TimeSpan elapsedTime = ElapsedTime();

        return elapsedTime.ToString(@"hh\:mm\:ss");
    }
}