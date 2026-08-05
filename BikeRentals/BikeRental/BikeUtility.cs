

namespace BikeRental
{
    public class BikeUtility
    {
        public void AddBikeDetails(
            string model,
            string brand,
            int pricePerDay)
        {
            int key = Program.bikeDetails.Count + 1;

            Bike bike = new Bike();

            bike.Model = model;
            bike.Brand = brand;
            bike.PricePerDay = pricePerDay;

            Program.bikeDetails.Add(key, bike);
        }

        public SortedDictionary<string, List<Bike>>
            GroupBikesByBrand()
        {
            SortedDictionary<string, List<Bike>> result =
                new SortedDictionary<string, List<Bike>>();

            foreach (Bike bike in Program.bikeDetails.Values)
            {
                if (!result.ContainsKey(bike.Brand))
                {
                    result.Add(
                        bike.Brand,
                        new List<Bike>()
                    );
                }

                result[bike.Brand].Add(bike);
            }

            return result;
        }
    }
}