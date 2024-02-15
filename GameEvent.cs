using Npgsql;

namespace real_time_horror_group3;
public class GameEvent()
{
    public static void SpawnDangerousObject(NpgsqlDataSource db)
    {
        Random random = new();
        int randomRoom = random.Next(1, 4);

        var spawnObject = db.CreateCommand(@"
            UPDATE public.room
            SET has_danger = true
            WHERE id = $1;
            ");
        spawnObject.Parameters.AddWithValue(randomRoom);
        spawnObject.ExecuteNonQuery();
    }
    public static void UnlockEntry(NpgsqlDataSource db)
    {
        var entryCount = db.CreateCommand(@"
            SELECT COUNT(id)
            FROM public.entry_point;
            ");
        using var reader1 = entryCount.ExecuteReader();
        int totalEntry = 0;
        if (reader1.Read())
        {
            totalEntry = reader1.GetInt32(0);
        }

        reader1.Close();
        Random random = new Random();
        int randomEntry = random.Next(1, totalEntry);

        var lockEntry = db.CreateCommand(@"
            UPDATE public.entry_point
            SET is_locked = false, time = CURRENT_TIMESTAMP 
            WHERE id = $1 AND is_locked = true;
            ");
        lockEntry.Parameters.AddWithValue(randomEntry);
        lockEntry.ExecuteNonQuery();
    }

    public static void RandomTrigger(NpgsqlDataSource db)
    {
        TimeSpan timeElapsed = Session.ElapsedTime(db);

        double baseProbability = 0.1;
        double exponentialRate = 0.05;
        double timeInterval = timeElapsed.TotalMinutes;

        double probability = baseProbability * Math.Exp(exponentialRate * timeInterval);

        Random random = new Random();
        double randomValue = random.NextDouble();

        if (randomValue <= probability)
        {
            int randomEvent = random.Next(1, 3);
            switch (randomEvent)
            {
                case 1:
                    GameEvent.UnlockEntry(db);
                    break;
                case 2:
                    GameEvent.SpawnDangerousObject(db);
                    break;
            }
        }
    }
}