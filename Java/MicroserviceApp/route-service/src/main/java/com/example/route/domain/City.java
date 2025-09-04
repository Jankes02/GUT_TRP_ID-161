package com.example.route.domain;

import jakarta.persistence.*;

@Entity
@Table(name = "cities")
public class City {
    @Id
    @Column(name = "id", nullable = false, updatable = false, length = 64)
    private String id;
    @Column(nullable = false, unique = true, length = 120)
    private String name;
    private String state;
    private Integer population;

    public City() {}
    public String getId() { return id; }
    public String getName() { return name; }
    public String getState() { return state; }
    public Integer getPopulation() { return population; }
}
