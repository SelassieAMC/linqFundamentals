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

            var query = from car in cars
                        group car by car.Manufacturer into carGroup
                        select new
                        {
                            Name = carGroup.Key,
                            Max = carGroup.Max(c => c.Combined),
                            Min = carGroup.Min(c => c.Combined),
                            Avg = carGroup.Average(c => c.Combined)
                        } into result
                        orderby result.Max descending
                        select result;
                        



            var query2 = manufacturers.GroupJoin(cars, m => m.Name, c => c.Manufacturer,
                    (m, g) =>
                            new
                            {
                                Manufacturer = m,
                                Cars = g
                            })
                    .GroupBy(m => m.Manufacturer.Headquarters);

            foreach (var data in query)
            {
                Console.WriteLine($"{data.Name}:");
                Console.WriteLine($"\t Max : {data.Max}");
                Console.WriteLine($"\t Min : {data.Min}");
                Console.WriteLine($"\t Avg : {data.Avg}");
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
