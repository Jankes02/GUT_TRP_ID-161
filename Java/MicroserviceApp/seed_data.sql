-- ==========================================
-- CREATE TABLES
-- ==========================================

DROP TABLE IF EXISTS connections;
DROP TABLE IF EXISTS cities;

CREATE TABLE cities (
    population INT NOT NULL,
    id VARCHAR(10) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    state VARCHAR(10) NOT NULL
);

CREATE TABLE connections (
     from_city_id VARCHAR(10) NOT NULL,
     id VARCHAR(10) PRIMARY KEY,
     to_city_id VARCHAR(10) NOT NULL,
     valid_from DATE NOT NULL,
     valid_to DATE NOT NULL,
     CONSTRAINT fk_from_city FOREIGN KEY (from_city_id) REFERENCES cities(id),
     CONSTRAINT fk_to_city FOREIGN KEY (to_city_id) REFERENCES cities(id)
);

-- ==========================================
-- BASE CITIES
-- ==========================================
INSERT INTO cities (population, id, name, state) VALUES
     (8419600, 'c1', 'New York',      'NY'),
     (3980400, 'c2', 'Los Angeles',   'CA'),
     (2716000, 'c3', 'Chicago',       'IL'),
     (2328000, 'c4', 'Houston',       'TX'),
     (478251,  'c5', 'Miami',         'FL'),
     (675647,  'c6', 'Boston',        'MA');

-- Additional cities
INSERT INTO cities (population, id, name, state) VALUES
     (883305,  'c7',  'San Francisco', 'CA'),
     (744955,  'c8',  'Seattle',       'WA'),
     (727211,  'c9',  'Denver',        'CO'),
     (4987150, 'c10', 'Atlanta',       'GA'),
     (1660272, 'c11', 'Phoenix',       'AZ'),
     (675000,  'c12', 'Las Vegas',     'NV');

-- ==========================================
-- CONNECTIONS
-- ==========================================

-- Base connections (all year)
INSERT INTO connections (from_city_id, id, to_city_id, valid_from, valid_to) VALUES
     ('c1','r1','c3','2025-01-01','2025-12-31'),  -- New York -> Chicago
     ('c2','r2','c5','2025-01-01','2025-12-31'),  -- Los Angeles -> Miami
     ('c1','r3','c5','2025-01-01','2025-12-31'),  -- New York -> Miami
     ('c3','r4','c4','2025-01-01','2025-12-31');  -- Chicago -> Houston

-- More year-round
INSERT INTO connections (from_city_id, id, to_city_id, valid_from, valid_to) VALUES
    ('c6','r5','c1','2025-01-01','2025-12-31'),
    ('c1','r6','c6','2025-01-01','2025-12-31'),
    ('c2','r7','c11','2025-01-01','2025-12-31'),
    ('c11','r8','c2','2025-01-01','2025-12-31'),
    ('c2','r9','c12','2025-01-01','2025-12-31'),
    ('c12','r10','c2','2025-01-01','2025-12-31'),
    ('c7','r11','c2','2025-01-01','2025-12-31'),
    ('c2','r12','c7','2025-01-01','2025-12-31'),
    ('c8','r13','c7','2025-01-01','2025-12-31'),
    ('c7','r14','c8','2025-01-01','2025-12-31'),
    ('c9','r15','c3','2025-01-01','2025-12-31'),
    ('c3','r16','c9','2025-01-01','2025-12-31'),
    ('c10','r17','c5','2025-01-01','2025-12-31'),
    ('c5','r18','c10','2025-01-01','2025-12-31');

-- Seasonal: SUMMER (2025-06-01 → 2025-08-31)
INSERT INTO connections (from_city_id, id, to_city_id, valid_from, valid_to) VALUES
    ('c1','r19','c2','2025-06-01','2025-08-31'),
    ('c2','r20','c5','2025-06-01','2025-08-31'),
    ('c3','r21','c5','2025-06-01','2025-08-31'),
    ('c4','r22','c5','2025-06-01','2025-08-31'),
    ('c7','r23','c2','2025-06-01','2025-08-31'),
    ('c2','r24','c7','2025-06-01','2025-08-31');

-- Seasonal: SPRING BREAK (2025-03-10 → 2025-04-05)
INSERT INTO connections (from_city_id, id, to_city_id, valid_from, valid_to) VALUES
    ('c6','r25','c5','2025-03-10','2025-04-05'),
    ('c1','r26','c5','2025-03-10','2025-04-05'),
    ('c10','r27','c5','2025-03-10','2025-04-05');

-- Seasonal: THANKSGIVING (2025-11-24 → 2025-11-30)
INSERT INTO connections (from_city_id, id, to_city_id, valid_from, valid_to) VALUES
    ('c1','r28','c10','2025-11-24','2025-11-30'),
    ('c3','r29','c10','2025-11-24','2025-11-30'),
    ('c5','r30','c10','2025-11-24','2025-11-30');

-- Seasonal: CHRISTMAS & NEW YEAR (2025-12-15 → 2026-01-06)
INSERT INTO connections (from_city_id, id, to_city_id, valid_from, valid_to) VALUES
    ('c1','r31','c2','2025-12-15','2026-01-06'),
    ('c2','r32','c1','2025-12-15','2026-01-06'),
    ('c1','r33','c7','2025-12-15','2026-01-06'),
    ('c7','r34','c1','2025-12-15','2026-01-06'),
    ('c3','r35','c6','2025-12-15','2026-01-06'),
    ('c6','r36','c3','2025-12-15','2026-01-06'),
    ('c5','r37','c1','2025-12-15','2026-01-06'),
    ('c1','r38','c5','2025-12-15','2026-01-06'),
    ('c12','r39','c7','2025-12-15','2026-01-06'),
    ('c7','r40','c12','2025-12-15','2026-01-06');
