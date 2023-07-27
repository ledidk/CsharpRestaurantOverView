namespace CsharpRestaurantOverView.Models
{
    public class Strings
    {
        public int id { get; set; }
        public string summary { get; set; }
        public string province { get; set; }
        public enum AllProvinces
        {
            AB,
            BC,
            MB,
            NB,
            NL,
            NS,
            ON,
            PE,
            QC,
            SK,
            NT,
            NU,
            YT
        }

        public AllProvinces GetProvinceByIndex(int index)
        {
            if (index < 0 || index >= Enum.GetValues(typeof(AllProvinces)).Length)
            {

            }
            return (AllProvinces)index;
        } 
    }
}
