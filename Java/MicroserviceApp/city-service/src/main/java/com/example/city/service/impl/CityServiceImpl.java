package com.example.city.service.impl;

import com.example.city.domain.City;
import com.example.city.dto.CityInfoDto;
import com.example.city.repository.CityRepository;
import com.example.city.service.CityService;
import org.springframework.http.HttpStatus;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.web.server.ResponseStatusException;

@Service
@Transactional
public class CityServiceImpl implements CityService {

    private final CityRepository repo;

    public CityServiceImpl(CityRepository repo) { this.repo = repo; }

    @Override
    @Transactional(readOnly = true)
    public City findByName(String name) {
        return repo.findByName(name)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "City not found"));
    }

    @Override
    public City addCity(City city) {
        return repo.save(city);
    }

    @Override
    @Transactional(readOnly = true)
    public CityInfoDto computeInfo(String name) {
        City c = findByName(name);
        int pop = c.getPopulation() == null ? 0 : c.getPopulation();
        String cat = pop < 50_000 ? "SMALL" : pop < 250_000 ? "MEDIUM" : "LARGE";
        int nameLen = c.getName() == null ? 0 : c.getName().length();
        int vowels = (int) c.getName().toLowerCase().chars().filter(ch -> "aeiouyąęó".indexOf(ch) >= 0).count();
        int projection = (int) Math.round(pop * Math.pow(1 + 0.012, 5));
        return new CityInfoDto(c.getName(), c.getState(), pop, cat, nameLen, vowels, projection);
    }
}
