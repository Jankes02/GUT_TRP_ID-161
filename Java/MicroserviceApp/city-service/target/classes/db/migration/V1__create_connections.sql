CREATE TABLE IF NOT EXISTS connections (
                                           id VARCHAR PRIMARY KEY,
                                           from_city_id VARCHAR NOT NULL REFERENCES cities(id) ON DELETE CASCADE,
    to_city_id   VARCHAR NOT NULL REFERENCES cities(id) ON DELETE CASCADE,
    CONSTRAINT ck_connections_distinct CHECK (from_city_id <> to_city_id)
    );

CREATE UNIQUE INDEX IF NOT EXISTS ux_connections_from_to ON connections(from_city_id, to_city_id);
CREATE INDEX IF NOT EXISTS ix_connections_to ON connections(to_city_id);
