using MongoDB.Bson;

namespace Receptsamlingstjänst.Models
{
    public class Recept
    {
        public ObjectId Id { get; set; } 
        public string Title { get; set; }
        public string Ingredienses { get; set; } //= string.Empty;

        public List<ObjectId> IngredientIds { get; set; } = new List<ObjectId>();

    }
}
