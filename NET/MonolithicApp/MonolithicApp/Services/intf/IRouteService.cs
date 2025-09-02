namespace MonolithicApp.Services.intf
{
    public interface IRouteService
    {
        List<string> FindAnyRoute(string from, string to, List<string>? route = null);
    }
}
