using MonolithicApp.Database.Model;
using MonolithicApp.DTOs;
using System.Collections.Generic;

namespace MonolithicApp.Services.intf
{
    public interface ICityService
    {
        City FindByName(string name);
        CityInfoDto ComputeInfo(string name);
        City AddCity(City city);
    }
}
