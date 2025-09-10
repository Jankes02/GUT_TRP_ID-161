INSERT INTO connections (id, from_city_id, to_city_id) VALUES
                                                           ('cn1','c1','c3'),  -- New York -> Chicago
                                                           ('cn2','c3','c5'),  -- Chicago -> Miami
                                                           ('cn3','c1','c2'),  -- New York -> Los Angeles
                                                           ('cn4','c2','c4'),  -- Los Angeles -> Houston
                                                           ('cn5','c4','c5')   -- Houston -> Miami
    ON CONFLICT (id) DO NOTHING;
