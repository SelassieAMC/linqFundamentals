using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Cars
{
    class Program
    {
        static void Main(string[] args)
        {
            var data = ProcessFile("fuel.csv");

            var document = new XDocument();
            var cars = new XElement("Cars");

            foreach (var obj in data)
            {
                var car = new XElement("Car");
                var name = new XAttribute("Name", obj.Name);
                var combined = new XAttribute("Combined", obj.Combined);
                car.Add(name);
                car.Add(combined);

                cars.Add(car);
            }
            document.Add(cars);
            document.Save("fuel.xml");

        }

        private static List<Manufacturer> ProcessManufacturers(string path)
        {
            return File.ReadAllLines(path)
                            .Where(l => l.Length > 1)
                            .Select(
                                l =>
                                {
                                    var columns = l.Split(',');
                                    return new Manufacturer
                                    {
                                        Name = columns[0],
                                        Headquarters = columns[1],
                                        Year = int.Parse(columns[2])
                                    };
                                }).ToList();
        }

        private static List<Car> ProcessFile(string path)
        {
            return 
                File.ReadLines(path)
                    .Skip(1)
                    .Where(line => line.Length > 1)
                    .Select(Car.ParceFromCsv)
                    .ToList();
        }

        private class CarStatistics
        {
            public CarStatistics()
            {
                Min = Int32.MaxValue;
                Max = Int32.MinValue;
            }

            public CarStatistics Accumulate(Car c)
            {
                Min = Math.Min(Min,c.Combined);
                Max = Math.Max(Max, c.Combined);
                Count += 1;
                Total += c.Combined;
                return this;
            }

            public CarStatistics Compute()
            {
                Average = Total / Count;
                return this;
            }
            public int Max { get; set; }
            public int Min { get; set; }
            public int Count { get; set; }
            public int Total { get; set; }
            public double Average { get; set; }
        }
    }
}
