using System.Collections.Generic;
using System.Xml.Linq;
using MonolithicApp.Database.Model;
using MonolithicApp.Services.intf;

namespace MonolithicApp.Services
{
    public class CityService : ICityService
    {
        private readonly Dictionary<string, City> _cities;

        public CityService()
        {
            _cities = new();
            City city1 = new() { Name = "Gdynia", State = "Pomerania", Population = 1234};
            City city2 = new() { Name = "Gdansk", State = "Pomerania", Population = 12345};
            City city3 = new() { Name = "Sopot", State = "Pomerania", Population = 123};
            _cities.Add(city1.Name, city1);
            _cities.Add(city2.Name, city2);
            _cities.Add(city3.Name, city3);
        }
        
        public City FindByName(string name)
        {
            return _cities[name];
        }
        public List<City> GetAll()
        {
            return _cities.Values.ToList();
        }

        public bool AddCity(City city)
        {
            _cities.Add(city.Name, city);
            return true;
        }
    }
}