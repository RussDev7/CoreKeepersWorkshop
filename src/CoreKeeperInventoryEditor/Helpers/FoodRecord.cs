namespace CoreKeepersWorkshop
{
    /// <summary>
    /// Represents one entry in the cookbook JSON.
    /// </summary>
    public class FoodRecord
    {
        public string Name      { get; set; } = "";
        public string Stats     { get; set; } = "";
        public int    Id        { get; set; }
        public int    Variation { get; set; }
        public int?   Skillset  { get; set; }
    }
}
