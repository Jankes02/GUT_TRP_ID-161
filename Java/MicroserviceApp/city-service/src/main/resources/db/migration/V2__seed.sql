-- V2: seed cities and connections
INSERT INTO cities (id, name, state, population) VALUES
('c1','New York','NY',8419600),
('c2','Los Angeles','CA',3980400),
('c3','Chicago','IL',2716000),
('c4','Houston','TX',2328000),
('c5','Miami','FL',478251)
ON CONFLICT (id) DO NOTHING;

INSERT INTO connections (id, from_city_id, to_city_id) VALUES
('cn1','c1','c3'),
('cn2','c3','c5'),
('cn3','c1','c2'),
('cn4','c2','c4'),
('cn5','c4','c5')
ON CONFLICT (id) DO NOTHING;
