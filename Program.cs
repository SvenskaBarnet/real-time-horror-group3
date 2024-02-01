using real_time_horror_group3;
using Npgsql;

string dbUri = "Host=localhost;Port=5455;Username=postgres;Password=postgres;Database=NotSoHomeAlone";
await using var db = NpgsqlDataSource.Create(dbUri);

await Database.Create(db);


