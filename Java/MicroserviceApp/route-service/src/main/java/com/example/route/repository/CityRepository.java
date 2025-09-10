package com.example.route.repository;

import com.example.route.domain.City;
import org.springframework.data.jpa.repository.JpaRepository;
import java.util.Optional;

public interface CityRepository extends JpaRepository<City, String> {
    Optional<City> findByNameIgnoreCase(String name);
}
