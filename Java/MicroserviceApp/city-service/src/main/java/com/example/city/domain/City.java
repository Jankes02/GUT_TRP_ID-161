package com.example.city.domain;

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

    public City(String id, String name, String state, Integer population) {
        this.id = id; this.name = name; this.state = state; this.population = population;
    }

    public String getId() { return id; }
    public void setId(String id) { this.id = id; }
    public String getName() { return name; }
    public void setName(String name) { this.name = name; }
    public String getState() { return state; }
    public void setState(String state) { this.state = state; }
    public Integer getPopulation() { return population; }
    public void setPopulation(Integer population) { this.population = population; }
}
