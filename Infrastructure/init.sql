-- Create new database
-- CREATE DATABASE mydb;

-- Switch to new database
\c mydb;

-- Create tables

CREATE TABLE IF NOT EXISTS points (
    id VARCHAR PRIMARY KEY,
    name VARCHAR NOT NULL,
    lat DOUBLE PRECISION,
    lon DOUBLE PRECISION
);

INSERT INTO points (id, name, lat, lon) VALUES
('p1', 'Statue of Liberty', 40.6892, -74.0445),
('p2', 'Hollywood Sign', 34.1341, -118.3215),
('p3', 'Willis Tower', 41.8789, -87.6359),
('p4', 'Space Center Houston', 29.5502, -95.0970),
('p5', 'Miami Beach', 25.7907, -80.1300);

CREATE TABLE IF NOT EXISTS cities (
    id VARCHAR PRIMARY KEY,
    name VARCHAR NOT NULL,
    state VARCHAR,
    population INTEGER
);
-- Insert sample cities
INSERT INTO cities (id, name, state, population) VALUES
-- Major hubs
('c1', 'New York', 'NY', 8419600),
('c2', 'Los Angeles', 'CA', 3980400),
('c3', 'Chicago', 'IL', 2716000),
('c4', 'Houston', 'TX', 2328000),
('c5', 'Miami', 'FL', 478251),

-- Regional cities
('c6', 'Denver', 'CO', 715522),
('c7', 'Seattle', 'WA', 753675),
('c8', 'Atlanta', 'GA', 498715),
('c9', 'Boston', 'MA', 685094),
('c10', 'Phoenix', 'AZ', 1608139),

-- Smaller connector cities
('c11', 'Kansas City', 'MO', 508090),
('c12', 'Salt Lake City', 'UT', 200133),
('c13', 'Nashville', 'TN', 689447),
('c14', 'Portland', 'OR', 652503),
('c15', 'Buffalo', 'NY', 278349);

-- Create connections table
CREATE TABLE connections (
    id VARCHAR(64) PRIMARY KEY,
    from_city_id VARCHAR(10) NOT NULL,
    to_city_id VARCHAR(10) NOT NULL,
    valid_from DATE NOT NULL,
    valid_to DATE NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    FOREIGN KEY (from_city_id) REFERENCES cities(id),
    FOREIGN KEY (to_city_id) REFERENCES cities(id)
);

-- Create indexes for performance
CREATE INDEX ix_conn_from ON connections(from_city_id);
CREATE INDEX ix_conn_to ON connections(to_city_id);
CREATE INDEX ix_conn_valid_dates ON connections(valid_from, valid_to);

-- Sample connection data
INSERT INTO connections (id, from_city_id, to_city_id, valid_from, valid_to) VALUES

-- East Coast Chain (linear path)
('conn1', 'c9', 'c15', '2025-01-01', '2025-12-31'),   -- Boston -> Buffalo
('conn2', 'c15', 'c1', '2025-01-01', '2025-12-31'),   -- Buffalo -> New York
('conn3', 'c1', 'c8', '2025-01-01', '2025-12-31'),    -- New York -> Atlanta
('conn4', 'c8', 'c5', '2025-01-01', '2025-12-31'),    -- Atlanta -> Miami

-- Central Route (through heartland)
('conn5', 'c9', 'c3', '2025-01-01', '2025-12-31'),    -- Boston -> Chicago
('conn6', 'c3', 'c11', '2025-01-01', '2025-12-31'),   -- Chicago -> Kansas City
('conn7', 'c11', 'c6', '2025-01-01', '2025-12-31'),   -- Kansas City -> Denver
('conn8', 'c6', 'c12', '2025-01-01', '2025-12-31'),   -- Denver -> Salt Lake City
('conn9', 'c12', 'c2', '2025-01-01', '2025-12-31'),   -- Salt Lake City -> Los Angeles

-- Western Chain
('conn10', 'c7', 'c14', '2025-01-01', '2025-12-31'),  -- Seattle -> Portland
('conn11', 'c14', 'c2', '2025-01-01', '2025-12-31'),  -- Portland -> Los Angeles
('conn12', 'c2', 'c10', '2025-01-01', '2025-12-31'),  -- Los Angeles -> Phoenix
('conn13', 'c10', 'c4', '2025-01-01', '2025-12-31'),  -- Phoenix -> Houston

-- Southern Route
('conn14', 'c5', 'c13', '2025-01-01', '2025-12-31'),  -- Miami -> Nashville
('conn15', 'c13', 'c11', '2025-01-01', '2025-12-31'), -- Nashville -> Kansas City
('conn16', 'c4', 'c11', '2025-01-01', '2025-12-31'),  -- Houston -> Kansas City

-- Cross-country shortcuts (limited availability)
('conn17', 'c1', 'c2', '2025-06-01', '2025-08-31'),   -- NY -> LA (summer only)
('conn18', 'c3', 'c7', '2025-03-01', '2025-05-31'),   -- Chicago -> Seattle (spring only)

-- Additional regional connections
('conn19', 'c6', 'c10', '2025-01-01', '2025-12-31'),  -- Denver -> Phoenix
('conn20', 'c8', 'c13', '2025-01-01', '2025-12-31'),  -- Atlanta -> Nashville
('conn21', 'c12', 'c7', '2025-01-01', '2025-12-31'),  -- Salt Lake City -> Seattle

-- Reverse connections for bidirectional travel
('conn22', 'c15', 'c9', '2025-01-01', '2025-12-31'),  -- Buffalo -> Boston
('conn23', 'c1', 'c15', '2025-01-01', '2025-12-31'),  -- New York -> Buffalo
('conn24', 'c8', 'c1', '2025-01-01', '2025-12-31'),   -- Atlanta -> New York
('conn25', 'c5', 'c8', '2025-01-01', '2025-12-31'),   -- Miami -> Atlanta
('conn26', 'c3', 'c9', '2025-01-01', '2025-12-31'),   -- Chicago -> Boston
('conn27', 'c11', 'c3', '2025-01-01', '2025-12-31'),  -- Kansas City -> Chicago
('conn28', 'c6', 'c11', '2025-01-01', '2025-12-31'),  -- Denver -> Kansas City
('conn29', 'c12', 'c6', '2025-01-01', '2025-12-31'),  -- Salt Lake City -> Denver
('conn30', 'c2', 'c12', '2025-01-01', '2025-12-31'),  -- Los Angeles -> Salt Lake City

-- Island connections (must go through specific hubs)
('conn31', 'c7', 'c12', '2025-01-01', '2025-12-31'),  -- Seattle -> Salt Lake City
('conn32', 'c10', 'c6', '2025-01-01', '2025-12-31'),  -- Phoenix -> Denver
('conn33', 'c4', 'c13', '2025-01-01', '2025-12-31'),  -- Houston -> Nashville
('conn34', 'c13', 'c8', '2025-01-01', '2025-12-31');  -- Nashville -> Atlanta
