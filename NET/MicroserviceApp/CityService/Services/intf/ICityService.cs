using CityService.DTOs;

namespace CityService.Services.intf
{
    public interface ICityService
    {
        City FindByName(string name);
        City AddCity(City city);
        CityInfoDto ComputeInfo(string name);
    }
}
