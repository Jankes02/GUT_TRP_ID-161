namespace CityService.DTOs
{
    public class CityInfoDto
    {
        public string Name { get; set; }
        public string State { get; set; }
        public int Population { get; set; }
        public string Category { get; set; }
        public int NameLength { get; set; }
        public int Vowels { get; set; }
        public int Projection { get; set; }

        public CityInfoDto(string name, string state, int population, string category, int nameLength, int vowels, int projection)
        {
            Name = name;
            State = state;
            Population = population;
            Category = category;
            NameLength = nameLength;
            Vowels = vowels;
            Projection = projection;
        }
    }
}
