using MonolithicApp.Database;
using MonolithicApp.Database.Model;
using MonolithicApp.DTOs;
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
            var city = _dbContext.Cities.FirstOrDefault(c => c.Name == name);
            if (city == null)
            {
                throw new Exception("City not found");
            }
            return city;
        }

        public City AddCity(City city)
        {
            _dbContext.Cities.Add(city);
            _dbContext.SaveChanges();
            return city;
        }

        public CityInfoDto ComputeInfo(string name)
        {
            City c = FindByName(name);
            int pop = c.Population;
            string cat = pop < 50_000 ? "SMALL" : pop < 250_000 ? "MEDIUM" : "LARGE";
            int nameLen = c.Name?.Length ?? 0;
            int vowels = c.Name?.ToLower().Count(ch => "aeiouyąęó".Contains(ch)) ?? 0;
            int projection = (int)Math.Round(pop * Math.Pow(1 + 0.012, 5));
            return new CityInfoDto(c.Name, c.State, pop, cat, nameLen, vowels, projection);
        }
    }
}