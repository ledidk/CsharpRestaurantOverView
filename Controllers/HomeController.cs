using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using CsharpRestaurantOverView.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CsharpRestaurantOverView.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Calling the LoadRestaurants method to get the populated lists
            (List<RestaurantOverViewModel> loadedRestaurants, List<RestaurantEditViewModel> loadedRestaurantsEdit) = LoadRestaurants();

            // Initializing the restaurantEdit 
            List<RestaurantOverViewModel> restaurants = new List<RestaurantOverViewModel>();

            // Find the restaurantEdit and restaurant based on the provided id
            if (loadedRestaurants != null && loadedRestaurantsEdit != null)
            {
                restaurants = loadedRestaurants.ToList();
            }

            foreach(var item in restaurants)
            {
                if (int.TryParse(item.ProvinceState, out int number))
                {
                    string provinceName = Enum.GetName(typeof(StateProvinceType.ProvinceCode), number);
                    item.ProvinceState = provinceName;
                }
            }
            return View(restaurants);
        }

        // Getting  Restaurant ID from index view to edit in this Get Method
        public IActionResult Edit(int? id)
        {
            // Call the LoadRestaurants method to get the populated lists
            (List<RestaurantOverViewModel> loadedRestaurants, List<RestaurantEditViewModel> loadedRestaurantsEdit) = LoadRestaurants();

            // Initialize the restaurantEdit and restaurant lists
            List<RestaurantEditViewModel> restaurantEdit = new List<RestaurantEditViewModel>();
            List<RestaurantOverViewModel> restaurant = new List<RestaurantOverViewModel>();

            // Find the restaurantEdit and restaurant based on the provided id
            if (loadedRestaurants != null && loadedRestaurantsEdit != null)
            {
                restaurant = loadedRestaurants.Where(p => p.Id == id).ToList();
                restaurantEdit = loadedRestaurantsEdit.Where(r => r.Id == id).ToList();
            }

            RestaurantEditViewModel EditRestaurant = new RestaurantEditViewModel();

            foreach (var item in restaurantEdit)
            {
                EditRestaurant.Id = item.Id;
                EditRestaurant.Name = item.Name;
                EditRestaurant.Rating = item.Rating;
                EditRestaurant.Summary = item.Summary;
                EditRestaurant.PostalZipCode = item.PostalZipCode;
                EditRestaurant.StreetAddress = item.StreetAddress;
                EditRestaurant.City = item.City;
                EditRestaurant.ProvinceState = item.ProvinceState;
                EditRestaurant.StreetAddress = item.StreetAddress;
            }


            var wew = EditRestaurant.PostalZipCode;

            return View("Edit", EditRestaurant); // 
        }

        // Post edited data to save changes
        [HttpPost]
        public IActionResult Edit(RestaurantEditViewModel editedRestaurant)
        {
            // Call the LoadRestaurants method to get the populated lists
            (List<RestaurantOverViewModel> loadedRestaurants, List<RestaurantEditViewModel> loadedRestaurantsEdit) = LoadRestaurants();

            // Find the corresponding restaurantEdit based on the provided id
            RestaurantEditViewModel existingRestaurant = loadedRestaurantsEdit.FirstOrDefault(r => r.Id == editedRestaurant.Id);
            if (existingRestaurant != null)
            {
                // Update the existingRestaurant with the edited data
                existingRestaurant.Name = editedRestaurant.Name;
                existingRestaurant.Rating = editedRestaurant.Rating;
                existingRestaurant.Summary = editedRestaurant.Summary;
                existingRestaurant.StreetAddress = editedRestaurant.StreetAddress;
                existingRestaurant.City = editedRestaurant.City;
                existingRestaurant.ProvinceState = editedRestaurant.ProvinceState;
                existingRestaurant.PostalZipCode = editedRestaurant.PostalZipCode;

                // Save the changes back to the XML file
                SaveRestaurants(loadedRestaurants, loadedRestaurantsEdit);

                return RedirectToAction("Index");
            }

            return View(editedRestaurant);
        }


        private (List<RestaurantOverViewModel>, List<RestaurantEditViewModel>) LoadRestaurants()
        {
            string xmlFilePath = "XMLFile/LabXMLFile.xml";

            List<RestaurantOverViewModel> restaurants = new List<RestaurantOverViewModel>();
            List<RestaurantEditViewModel> restaurantsEdit = new List<RestaurantEditViewModel>();

            XElement root = XElement.Load(xmlFilePath);
            IEnumerable<XElement> restaurantElements = root.Elements("restaurant");

            int customId = 5813;
            foreach (XElement restaurantElement in restaurantElements)
            {
                RestaurantOverViewModel restaurant = new RestaurantOverViewModel();
                RestaurantEditViewModel restaurantEdit = new RestaurantEditViewModel();

                restaurant.Name = (string)restaurantElement.Element("name");
                restaurant.FoodType = (string)restaurantElement.Element("food");
                restaurant.Cost = (decimal)restaurantElement.Element("price");
                restaurant.Rating = (decimal)restaurantElement.Element("rating");
                restaurant.City = (string)restaurantElement.Element("address").Element("city");
                restaurant.ProvinceState = (string)restaurantElement.Element("address").Element("province");
                restaurant.Id = customId;

                restaurantEdit.Id = customId;
                restaurantEdit.Name = restaurant.Name;
                restaurantEdit.Rating = restaurant.Rating;
                restaurantEdit.Summary = (string)restaurantElement.Element("summary");
                restaurantEdit.StreetAddress = (string)restaurantElement.Element("address").Element("street");
                restaurantEdit.City = restaurant.City;
                restaurantEdit.ProvinceState = restaurant.ProvinceState;
                restaurantEdit.PostalZipCode = (string)restaurantElement.Element("address").Element("postalCode");

                restaurants.Add(restaurant);
                restaurantsEdit.Add(restaurantEdit);

                customId += 2749;
            }

            // method returning both restaurants 
            return (restaurants, restaurantsEdit);
        }

        private void SaveRestaurants(List<RestaurantOverViewModel> restaurants, List<RestaurantEditViewModel> restaurantsEdit)
        {
            string xmlFilePath = "XMLFile/labXMLFile.xml";

            XElement root = new XElement("restaurants");

            for (int i = 0; i < restaurantsEdit.Count; i++)
            {
                XElement restaurantElement = new XElement("restaurant",
                    new XElement("name", restaurantsEdit[i].Name),
                    new XElement("food", restaurants[i].FoodType),
                    new XElement("price", restaurants[i].Cost),
                    new XElement("rating", restaurantsEdit[i].Rating),
                    new XElement("summary", restaurantsEdit[i].Summary),
                    new XElement("address",
                        new XElement("street", restaurantsEdit[i].StreetAddress),
                        new XElement("postalCode", restaurantsEdit[i].PostalZipCode),
                        new XElement("city", restaurantsEdit[i].City),
                        new XElement("province", restaurantsEdit[i].ProvinceState)
                    )
                );

                root.Add(restaurantElement);
            }

            XDocument document = new XDocument(root);
            document.Save(xmlFilePath);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
