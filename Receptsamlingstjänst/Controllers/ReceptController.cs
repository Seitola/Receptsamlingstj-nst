using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Receptsamlingstjänst.Models;

namespace Receptsamlingstjänst.Controllers
{
    public class ReceptController : Controller
    {

        public IActionResult Index()
        {
            MongoClient dbClient = new MongoClient();
            var database = dbClient.GetDatabase("Recpie_application");
            var collection = database.GetCollection<Recept>("recepts");

            List<Recept> recept = collection.Find(r => true).ToList();

            return View(recept);
        }

        public IActionResult CreateRecept() // Inga parametrar här, bara att visa formuläret
        {
            MongoClient dbClient = new MongoClient();
            var database = dbClient.GetDatabase("Recpie_application");
            var ingredienseCollection = database.GetCollection<Ingrediense>("ingredienses");

            // Hämta alla tillgängliga ingredienser för dropdown-listan i vyn
            List<Ingrediense> allIngredients = ingredienseCollection.Find(i => true).ToList();

            // Skapa en ViewModel för att skicka till vyn
            var viewModel = new ReceptEditViewModel // Kan återanvändas från EditRecept
            {
                AllAvailableIngredients = allIngredients,
                // SelectedIngredientIds och Title är tomma/default för ett nytt recept
            };

            return View(viewModel);
        }


        [HttpPost]
        public IActionResult CreateRecept(ReceptEditViewModel model) // VIKTIGT: Ta emot din ViewModel
        {
            MongoClient dbClient = new MongoClient();
            var database = dbClient.GetDatabase("Recpie_application");
            var receptCollection = database.GetCollection<Recept>("recepts");

            // Skapa ett nytt Recept-objekt baserat på datan från din ViewModel
            var newRecept = new Recept
            {
                Title = model.Title, // Titel från ViewModel
                IngredientIds = model.SelectedIngredientIds // De valda ingrediens-ID:na från ViewModel
            };

            // Spara det nya receptet i databasen
            receptCollection.InsertOne(newRecept);

            // Omdirigera till en sida som listar recepten eller visar det nya receptet
            return RedirectToAction("Index");
        }

        public IActionResult ShowRecept(string id) // Använder 'id' för URL parameterkonsistens
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound("Inget recept ID givet.");
            }

            // Konverterar string ID till ObjectId för MongoDB lookup
            if (!ObjectId.TryParse(id, out ObjectId receptId))
            {
                return BadRequest("Ogiltigt recept ID format.");
            }

            MongoClient dbClient = new MongoClient();
            var database = dbClient.GetDatabase("Recpie_application");
            var receptCollection = database.GetCollection<Recept>("recepts");
            var ingredienseCollection = database.GetCollection<Ingrediense>("ingredienses");

            // Hämtar reveptet från databasen
            Recept recept = receptCollection.Find(r => r.Id == receptId).FirstOrDefault();

            if (recept == null)
            {
                return NotFound($"Recipe with ID {id} not found.");
            }

            // Hämtar ingrediens detaljerna baserat på IngredientIds i receptet
            List<Ingrediense> ingredientsForRecipe = new List<Ingrediense>();
            if (recept.IngredientIds != null && recept.IngredientIds.Any())
            {
                ingredientsForRecipe = ingredienseCollection.Find(i => recept.IngredientIds.Contains(i.Id)).ToList();
            }

            // Skapar en ViewModel som skickas till viewn
            var viewModel = new ReceptDisplayViewModel
            {
                Id = recept.Id,
                Title = recept.Title,
                Ingredients = ingredientsForRecipe // Skicka listan över fullständiga Ingrediense-objekt
            };

            return View(viewModel);
        }

        public IActionResult EditRecept(string id) // Tar emot ID för receptet som ska redigeras
        {
            ObjectId receptId = new ObjectId(id);

            MongoClient dbClient = new MongoClient();
            var database = dbClient.GetDatabase("Recpie_application");
            var receptCollection = database.GetCollection<Recept>("recepts");
            var ingredienseCollection = database.GetCollection<Ingrediense>("ingredienses");

            //Hämta det specifika receptet från databasen
            Recept recept = receptCollection.Find(r => r.Id == receptId).FirstOrDefault();

            if (recept == null)
            {
                return NotFound($"Recept med ID {id} hittades inte.");
            }

            //Hämta ALLA tillgängliga ingredienser för dropdown-listan
            List<Ingrediense> allIngredients = ingredienseCollection.Find(i => true).ToList();

            //Skapa en ReceptEditViewModel och fyll den med data
            var viewModel = new ReceptEditViewModel
            {
                Id = recept.Id,                           // Receptets ID
                Title = recept.Title,                     // Receptets befintliga titel
                AllAvailableIngredients = allIngredients, // Alla ingredienser för dropdown
                SelectedIngredientIds = recept.IngredientIds ?? new List<ObjectId>() // Befintliga ingrediens-ID:n
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult EditRecept(ReceptEditViewModel model) // Tar emot ViewModel från formuläret
        {
            if (!ModelState.IsValid) // Grundläggande validering om du har några regler i din ViewModel
            {
                // Om validering misslyckas, hämta om alla ingredienser och returnera vyn med felmeddelanden
                MongoClient dbClientFailed = new MongoClient();
                var databaseFailed = dbClientFailed.GetDatabase("Recpie_application");
                var ingredienseCollectionFailed = databaseFailed.GetCollection<Ingrediense>("ingredienses");
                model.AllAvailableIngredients = ingredienseCollectionFailed.Find(_ => true).ToList();
                return View(model);
            }

            if (model.Id == ObjectId.Empty)
            {
                return BadRequest("Recept-ID saknas eller är ogiltigt för uppdatering.");
            }

            MongoClient dbClient = new MongoClient();
            var database = dbClient.GetDatabase("Recpie_application");
            var receptCollection = database.GetCollection<Recept>("recepts");

            //Hämta det befintliga receptet för att uppdatera det
            var existingRecept = receptCollection.Find(r => r.Id == model.Id).FirstOrDefault();

            if (existingRecept == null)
            {
                return NotFound($"Recept med ID {model.Id} hittades inte för uppdatering.");
            }

            //Uppdatera receptets egenskaper med data från ViewModel
            existingRecept.Title = model.Title;
            existingRecept.IngredientIds = model.SelectedIngredientIds; // De valda ingrediens-ID:na från formuläret

            //Ersätt det gamla dokumentet med det uppdaterade i databasen
            var result = receptCollection.ReplaceOne(r => r.Id == model.Id, existingRecept);

            if (result.IsAcknowledged && result.ModifiedCount > 0)
            {
                // Omdirigera till ShowRecept-sidan för att visa det uppdaterade receptet
                return RedirectToAction("ShowRecept", new { id = model.Id.ToString() });
            }
            else
            {
                ModelState.AddModelError("", "Kunde inte spara ändringarna. Receptet hittades inte eller inga ändringar gjordes.");
                // Om inget ändrades (t.ex. inga nya värden), fyll på ingredienser igen och returnera vyn
                MongoClient dbClientFailed = new MongoClient();
                var databaseFailed = dbClientFailed.GetDatabase("Recpie_application");
                var ingredienseCollectionFailed = databaseFailed.GetCollection<Ingrediense>("ingredienses");
                model.AllAvailableIngredients = ingredienseCollectionFailed.Find(_ => true).ToList();
                return View(model);
            }
        }

        [HttpPost]
        public IActionResult DeleteRecept(string Id)
        {
            ObjectId receptId = new ObjectId(Id);

            MongoClient dbClient = new MongoClient();
            var database = dbClient.GetDatabase("Recpie_application");
            var collection = database.GetCollection<Recept>("recepts");

            collection.DeleteOne(r => r.Id == receptId);

            return Redirect("/Recept");
        }

        [HttpPost]
        public IActionResult BackRecept()
        {
            return Redirect("/Index");
        }

    }
}
