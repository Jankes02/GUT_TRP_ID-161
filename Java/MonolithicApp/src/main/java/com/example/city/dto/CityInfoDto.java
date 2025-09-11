package com.example.city.dto;

public class CityInfoDto {
    private String name;
    private String state;
    private int population;
    private String populationCategory;
    private int nameLength;
    private int vowelCount;
    private int projection5Years;

    public CityInfoDto(String name, String state, int population, String populationCategory,
                       int nameLength, int vowelCount, int projection5Years) {
        this.name = name; this.state = state; this.population = population;
        this.populationCategory = populationCategory; this.nameLength = nameLength;
        this.vowelCount = vowelCount; this.projection5Years = projection5Years;
    }
    public String getName() { return name; }
    public String getState() { return state; }
    public int getPopulation() { return population; }
    public String getPopulationCategory() { return populationCategory; }
    public int getNameLength() { return nameLength; }
    public int getVowelCount() { return vowelCount; }
    public int getProjection5Years() { return projection5Years; }
}
