﻿namespace real_time_horror_group3;
using Npgsql;

public class Database
{
    public static async Task Create(NpgsqlDataSource db)
    {

        await using var cmd = db.CreateCommand(@"
CREATE TABLE IF NOT EXISTS room
(
    id serial,
    name text NOT NULL,
    PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS entry_point
(
    id serial,
    name text NOT NULL,
    room_id integer,
    is_locked boolean NOT NULL DEFAULT False,
    PRIMARY KEY (id)
);

ALTER TABLE IF EXISTS entry_point
    ADD CONSTRAINT room_id FOREIGN KEY (room_id) REFERENCES room (id); 

INSERT INTO room(
	name)
	VALUES ('Cabin');

INSERT INTO entry_point(
	name, room_id)
	VALUES 
	('Window A', 1), 
	('Window B', 1),
	('Door A', 1);
");

        await cmd.ExecuteNonQueryAsync();
        Console.WriteLine("Created and populated DB");
    }
}