using MongoDB.Bson;

namespace Receptsamlingstjänst.Models
{
    public class ReceptEditViewModel
    {
        public ObjectId Id { get; set; }
        public string Title { get; set; }
        public List<Ingrediense> AllAvailableIngredients { get; set; } = new List<Ingrediense>(); // För dropdownen i GET-metoden
        public List<ObjectId> SelectedIngredientIds { get; set; } = new List<ObjectId>(); // Dessa kommer från formulärets dolda inputs
        public List<Ingrediense> Ingredients { get; set; } = new List<Ingrediense>();
        
    }
}
