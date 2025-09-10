package com.example.route.repository;

import com.example.city.domain.City;
import com.example.route.domain.Connection;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;

import java.time.LocalDate;
import java.util.List;

public interface ConnectionRepository extends JpaRepository<Connection, String> {
    List<Connection> findByFrom(City from);
    List<Connection> findByTo(City to);

    @Query("SELECT c FROM Connection c WHERE c.from = :from AND c.to = :to")
    Connection findByFromAndTo(@Param("from") City from, @Param("to") City to);

    @Query("SELECT c FROM Connection c WHERE :date BETWEEN c.validFrom AND c.validTo")
    List<Connection> findAllValidOn(@Param("date") LocalDate date);
}
