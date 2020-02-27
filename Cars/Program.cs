using System;
using System.Collections.Generic;
using System.Data.Entity;
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
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<CarDb>());
            InsertData();
            QueryData();
        }
         
        private static void QueryData()
        {
            var db = new CarDb();
            var query = from car in db.Cars
                        orderby car.Combined descending, car.Name ascending
                        select car;

            var query2 = db.Cars.OrderByDescending(c => c.Combined)
                                .ThenBy(c => c.Name);

            foreach (var car in query2.Take(10))
            {
                Console.WriteLine($"{car.Name}:{car.Combined}");
            }
            Console.ReadLine();
        }

        private static void InsertData()
        {
            var cars = ProcessFile("fuel.csv");
            var db = new CarDb();
            if (!db.Cars.Any())
            {
                foreach (var car in cars)
                {
                    db.Cars.Add(car);
                }
                db.SaveChanges();
            }
        }

        private static void QueryXml()
        {
            var document = XDocument.Load("fuel.xml");
            var ns = (XNamespace)"http://namespace.com/cars/2020";
            var ex = (XNamespace)"http://namespace.com/cars/2020/ex";

            var query = from car in document.Element(ns + "Cars")?.Elements(ex + "Car") 
                                                        ?? Enumerable.Empty<XElement>()
                        where car.Attribute("Manufacturer")?.Value == "BMW"
                        select car.Attribute("Name").Value;

            foreach (var item in query)
            {
                Console.WriteLine(item);
            }
            Console.ReadLine();
        }

        private static void CreateXml()
        {
            var data = ProcessFile("fuel.csv");
            var document = new XDocument();
            var ns = (XNamespace)"http://namespace.com/cars/2020";
            var ex = (XNamespace)"http://namespace.com/cars/2020/ex";

            var cars = new XElement(ns + "Cars",
                                from obj in data
                                select new XElement(ex + "Car",
                                              new XAttribute("Name", obj.Name),
                                              new XAttribute("Combined", obj.Combined),
                                              new XAttribute("Manufacturer",obj.Manufacturer)));
            cars.Add(new XAttribute(XNamespace.Xmlns + "ex", ex));
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
