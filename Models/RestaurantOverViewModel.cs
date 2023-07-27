using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
namespace CsharpRestaurantOverView.Models
{
    public class RestaurantOverViewModel
    {
        [XmlElement("id")]
        public int Id { get; set; }

        [XmlElement("name")]
        [Display(Name = "Rataurant")]
        public string Name { get; set; }

        [XmlElement("food")]
        [Display(Name = "Food Type")]
        public string FoodType { get; set; }
        [Display(Name = "Rating (best=5)")]
        [XmlElement("rating")]
        public decimal Rating { get; set; }

        [XmlElement("price")]
        [Display(Name = "Cost (most expensive=5)")]
        public decimal Cost { get; set; }

        [XmlElement("city")]
        [Display(Name = "City")]
        public string City { get; set; }

        [XmlElement("province")]
        [Display(Name = "Province")]
        public string ProvinceState { get; set; }

    }
}
