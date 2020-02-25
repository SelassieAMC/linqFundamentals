using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cars
{
    public class Car
    {
        public int Year { get; set; }
        public string Manufacturer { get; set; }
        public string Name { get; set; }
        public double Displacement { get; set; }
        public int Cylinders { get; set; }
        public int City { get; set; }
        public int Highway { get; set; }
        public int Combined { get; set; }

        internal static Car ParceFromCsv(string line)
        {
            var carInfo = line.Split(',');
            return new Car()
            {
                Year = int.Parse(carInfo[0]),
                Manufacturer = carInfo[1],
                Name = carInfo[2],
                Displacement = double.Parse(carInfo[3]),
                Cylinders = int.Parse(carInfo[4]),
                City = int.Parse(carInfo[5]),
                Highway = int.Parse(carInfo[6]),
                Combined = int.Parse(carInfo[7])
            };
        }
    }
}
