using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Receptsamlingstjänst.Models;
using System;
using System.Xml.Linq;


namespace Receptsamlingstjänst.Controllers
{
    public class IngrediensController : Controller
    {
        public IActionResult Index()
        {
            MongoClient dbClient = new MongoClient();
            var database = dbClient.GetDatabase("Recpie_application");
            var collection = database.GetCollection<Ingrediense>("ingredienses");

            List<Ingrediense> ingrediense = collection.Find(i => true).ToList();

            return View(ingrediense);
        }

        public IActionResult Make()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Make(Ingrediense ingrediense)
        {
            MongoClient dbClient = new MongoClient();
            var database = dbClient.GetDatabase("Recpie_application");
            var collection = database.GetCollection<Ingrediense>("ingredienses");

            collection.InsertOne(ingrediense);

            return Redirect("/ingrediens");
        }

        public IActionResult ShowIngrediense(string Id)
        {
            ObjectId ingredienseId = new ObjectId(Id);

            MongoClient dbClient = new MongoClient();
            var database = dbClient.GetDatabase("Recpie_application");
            var collection = database.GetCollection<Ingrediense>("ingredienses");

            Ingrediense ingrediense = collection.Find(i => i.Id == ingredienseId).FirstOrDefault();

            return View(ingrediense);
        }

        public IActionResult EditIngrediense(string Id)
        {
            ObjectId ingredienseId = new ObjectId(Id);
            MongoClient dbClient = new MongoClient();
            var database = dbClient.GetDatabase("Recpie_application");
            var collection = database.GetCollection<Ingrediense>("ingredienses");

            Ingrediense ingrediense = collection.Find(i => i.Id == ingredienseId).FirstOrDefault();

            return View(ingrediense);
        }

        [HttpPost]
        public IActionResult EditIngrediense(string Id, Ingrediense ingrediense)
        {
            ObjectId ingredienseId = new ObjectId(Id);
            MongoClient dbClient = new MongoClient();
            var database = dbClient.GetDatabase("Recpie_application");
            var collection = database.GetCollection<Ingrediense>("ingredienses");

            ingrediense.Id = ingredienseId;
            collection.ReplaceOne(i => i.Id == ingredienseId, ingrediense);

            return Redirect("/ingrediens");
        }

        [HttpPost]
        public IActionResult DeleteIngrediense(string Id)
        {
            ObjectId ingredienseId = new ObjectId(Id);

            MongoClient dbClient = new MongoClient();
            var database = dbClient.GetDatabase("Recpie_application");
            var collection = database.GetCollection<Ingrediense>("ingredienses");

            collection.DeleteOne(i => i.Id ==ingredienseId);
            return Redirect("/ingrediens");
        }

        [HttpPost]
        public IActionResult BackIngrediens()
        {
            return Redirect("/Index");
        }

        [HttpPost]
        public IActionResult BackShowIngrediense ()
        {
            return Redirect("/ShowIngrediense");
        }
    }
}


