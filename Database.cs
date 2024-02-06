﻿using Npgsql;
namespace real_time_horror_group3;

public class Database(NpgsqlDataSource db)
{
    public async Task Create()
    {
        await using var cmd = db.CreateCommand(@"
DROP TABLE IF EXISTS public.room, public.entry_point, public.danger, public.player, public.session;
CREATE TABLE IF NOT EXISTS public.room
(
    id serial,
    name text NOT NULL,
    PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS public.entry_point
(
    id serial,
    name text NOT NULL,
    type text NOT NULL,
    is_locked boolean NOT NULL DEFAULT false,
    room_id integer NOT NULL,
    ""time"" timestamp without time zone,
    PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS public.danger
(
    id serial,
    type text NOT NULL,
    room_id integer NOT NULL,
    PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS public.player
(
    id serial,
    name text NOT NULL,
    location integer NOT NULL,
    is_dead boolean NOT NULL DEFAULT false,
    PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS public.session
(
    id serial,
    ""time"" timestamp without time zone NOT NULL,
    PRIMARY KEY (id)
);

ALTER TABLE IF EXISTS public.entry_point
    ADD CONSTRAINT room_id FOREIGN KEY (room_id)
    REFERENCES public.room (id);


ALTER TABLE IF EXISTS public.danger
    ADD CONSTRAINT room_id FOREIGN KEY (room_id)
    REFERENCES public.room (id);


ALTER TABLE IF EXISTS public.player
    ADD CONSTRAINT location FOREIGN KEY (location)
    REFERENCES public.room (id);

");

        await cmd.ExecuteNonQueryAsync();
        Console.WriteLine("Created and populated database");
    }
}