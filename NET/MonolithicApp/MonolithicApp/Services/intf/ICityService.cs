using MonolithicApp.Database.Model;

namespace MonolithicApp.Services.intf
{
    public interface ICityService
    {
        City FindByName(string name);
        List<City> GetAll();
        bool AddCity(City city);
    }
}
