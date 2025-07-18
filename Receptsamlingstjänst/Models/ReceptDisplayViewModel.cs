using MongoDB.Bson;

namespace Receptsamlingstjänst.Models
{
    public class ReceptDisplayViewModel
    {
        public ObjectId Id { get; set; }
        public string Title { get; set; }
        // This list will hold the actual Ingrediense objects to display their names and tastes
        public List<Ingrediense> Ingredients { get; set; } = new List<Ingrediense>();
    }
}
