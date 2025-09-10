package com.example.city.controller;

import com.example.city.domain.City;
import com.example.city.dto.CityInfoDto;
import com.example.city.service.CityService;
import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/city")
public class CityController {

    private final CityService service;

    public CityController(CityService service) { this.service = service; }

    @GetMapping("/{name}")
    public City get(@PathVariable("name") String name) {
        return service.findByName(name); // ← returns Optional<City>?
    }

    @PostMapping
    @ResponseStatus(HttpStatus.CREATED)
    public City add(@RequestBody City city) { return service.addCity(city); }

    @GetMapping("/info/{name}")
    public CityInfoDto info(@PathVariable("name") String name) { return service.computeInfo(name); }
}
