using KarshenasiModern.Models;

namespace KarshenasiModern.Models
{
    public class StatusItem
    {
        public string Title { get; set; } = string.Empty;
        public BodyPartStatus Value { get; set; }
    }
}