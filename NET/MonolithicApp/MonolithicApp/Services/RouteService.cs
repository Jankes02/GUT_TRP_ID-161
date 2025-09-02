using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Routing;
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

        public List<List<string>> FindAllRoutes(string from, string to, List<string>? route = null)
        {
            if (route is null)
                route = new List<string>();

            if (route.Contains(from))
                return new List<List<string>>();

            var currentRoute = new List<string>(route) { from };

            if (from == to)
                return new List<List<string>> { currentRoute };

            var allRoutes = new List<List<string>>();

            var nextStepList = _dbContext.RouteFragments
                .Where(rf => rf.From == from)
                .ToList();

            foreach (var nextStep in nextStepList)
            {
                var subRoutes = FindAllRoutes(nextStep.To, to, currentRoute);
                allRoutes.AddRange(subRoutes);
            }

            return allRoutes;
        }
    }
}