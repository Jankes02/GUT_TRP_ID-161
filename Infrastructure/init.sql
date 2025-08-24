-- Create new database
-- CREATE DATABASE mydb;

-- Switch to new database
\c mydb;

-- Create tables
CREATE TABLE IF NOT EXISTS cities (
    id VARCHAR PRIMARY KEY,
    name VARCHAR NOT NULL,
    state VARCHAR,
    population INTEGER
);

CREATE TABLE IF NOT EXISTS points (
    id VARCHAR PRIMARY KEY,
    name VARCHAR NOT NULL,
    lat DOUBLE PRECISION,
    lon DOUBLE PRECISION
);

-- Insert sample cities
INSERT INTO cities (id, name, state, population) VALUES
('c1', 'New York', 'NY', 8419600),
('c2', 'Los Angeles', 'CA', 3980400),
('c3', 'Chicago', 'IL', 2716000),
('c4', 'Houston', 'TX', 2328000),
('c5', 'Miami', 'FL', 478251);

-- Insert sample points
INSERT INTO points (id, name, lat, lon) VALUES
('p1', 'Statue of Liberty', 40.6892, -74.0445),
('p2', 'Hollywood Sign', 34.1341, -118.3215),
('p3', 'Willis Tower', 41.8789, -87.6359),
('p4', 'Space Center Houston', 29.5502, -95.0970),
('p5', 'Miami Beach', 25.7907, -80.1300);
