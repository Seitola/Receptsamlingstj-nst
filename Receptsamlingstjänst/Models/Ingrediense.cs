using MongoDB.Bson;

namespace Receptsamlingstjänst.Models
{
    public class Ingrediense
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public string Taste { get; set; }
    }
}
