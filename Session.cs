using Npgsql;

namespace real_time_horror_group3;

public class Session()
{
    public static void Start(NpgsqlDataSource db)
    {
        string message = string.Empty;
        var select = db.CreateCommand(@"
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
            var insert = db.CreateCommand(@"
                            INSERT INTO public.session(
	                        time)
	                        VALUES (
                            CURRENT_TIMESTAMP);
                            UPDATE entry_point
                            SET time = CURRENT_TIMESTAMP;
                            ");
            insert.ExecuteNonQuery();
            insert.Dispose();
        }
    }

    public static bool EntryPointTimer(NpgsqlDataSource db)
    {
        bool gameOver = false;
        var cmd = db.CreateCommand(@"
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
    public static TimeSpan ElapsedTime(NpgsqlDataSource db)
    {
        var sessionStart = db.CreateCommand(@"
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
    public static string FormattedTime(NpgsqlDataSource db)
    {
        TimeSpan elapsedTime = ElapsedTime(db);
        return elapsedTime.ToString(@"hh\:mm\:ss");
    }
}