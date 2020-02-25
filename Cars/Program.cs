using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cars
{
    class Program
    {
        static void Main(string[] args)
        {
            var cars = ProcessFile("fuel.csv");
            var manufacturers = ProcessManufacturers("manufacturers.csv");

            var query = from manufacturer in manufacturers
                        join car in cars on manufacturer.Name equals car.Manufacturer
                            into carGroup
                        select new
                        {
                            Manufacturer = manufacturer,
                            Cars = carGroup
                        };

            var query2 = manufacturers.GroupJoin(cars, m => m.Name, c => c.Manufacturer,
                    (m, g) =>
                            new
                            {
                                Manufacturer = m,
                                Cars = g
                            })
                    .OrderBy(m => m.Manufacturer.Name);

            foreach (var group in query2)
            {
                Console.WriteLine($"{group.Manufacturer.Name}:{group.Manufacturer.Headquarters}");
                foreach (var car in group.Cars.OrderByDescending(c => c.Combined).Take(2))
                {
                    Console.WriteLine($"\t{car.Name} : {car.Combined}");
                }
            }
            
            Console.ReadLine();

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

    }
}
