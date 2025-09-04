package com.example.city.service;

import com.example.city.domain.City;
import com.example.city.dto.CityInfoDto;

public interface CityService {
    City findByName(String name);
    City addCity(City city);
    CityInfoDto computeInfo(String name);
}
