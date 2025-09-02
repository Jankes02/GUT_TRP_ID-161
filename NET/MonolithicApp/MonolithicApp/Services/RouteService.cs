using System;
using System.Collections.Generic;
using System.Linq;
using MonolithicApp.Database;
using MonolithicApp.Database.Model;
using MonolithicApp.Services.intf;

namespace MonolithicApp.Services
{
    public class RouteService : IRouteService
    {
        private readonly AppDbContext _dbContext;

        public RouteService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<string>? FindAnyRoute(string from, string to, List<string>? route = null)
        {
            if (route is null)
                route = new List<string>();

            if (route.Contains(from))
                return null;

            route.Add(from);

            if (from == to)
                return route;

            var nextStepList = _dbContext.RouteFragments
                .Where(rf => rf.From == from)
                .ToList();

            foreach (var nextStep in nextStepList)
            {
                var result = FindAnyRoute(nextStep.To, to, new List<string>(route));
                if (result is not null)
                    return result;
            }

            return null;
        }
    }
}