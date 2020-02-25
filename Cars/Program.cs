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




            var query2 = cars.GroupBy(c => c.Manufacturer)
                             .Select(g =>
                                          {
                                              var results = g.Aggregate(new CarStatistics(),
                                                                       (cst, c) => cst.Accumulate(c),
                                                                       cst => cst.Compute());
                                              return new
                                              {
                                                  Name = g.Key,
                                                  results.Min,
                                                  results.Max,
                                                  Avg = results.Average
                                              };
                                          })
                             .OrderByDescending(c => c.Max);
                                     
                               

                             

            foreach (var data in query2)
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
