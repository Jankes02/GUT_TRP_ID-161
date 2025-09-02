using System.Collections.Generic;
using System.Xml.Linq;
using MonolithicApp.Database;
using MonolithicApp.Database.Model;
using MonolithicApp.Services.intf;

namespace MonolithicApp.Services
{
    public class CityService : ICityService
    {
        private readonly AppDbContext _dbContext;

        public CityService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public City FindByName(string name)
        {
            return _dbContext.Cities.First(c => c.Name == name);
        }
        public List<City> GetAll()
        {
            return _dbContext.Cities.ToList();
        }

        public void AddCity(City city)
        {
            _dbContext.Cities.Add(city);
            _dbContext.SaveChanges();
        }
    }
}