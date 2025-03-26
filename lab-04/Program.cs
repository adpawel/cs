using System.Globalization;

public class MainClass{
    public static void Main(string[] args){
        Reader<Territory> terrReader = new Reader<Territory>();
        List<Territory> territories = terrReader.readList("data\\territories.csv", x => new Territory(x[0], x[1], x[2]));

        Reader<EmployeeTerritory> emplTerrReader = new Reader<EmployeeTerritory>();
        List<EmployeeTerritory> employeeTerritories = emplTerrReader.readList("data\\employee_territories.csv", x => new EmployeeTerritory(x[0], x[1]));

        Reader<Region> regionReader = new Reader<Region>();
        List<Region> regions = regionReader.readList("data\\regions.csv", x => new Region(x[0], x[1]));

        Reader<Employee> emplReader = new Reader<Employee>();
        List<Employee> employees = emplReader.readList("data\\employees.csv", x => new Employee(x[0], x[1], x[2], x[3],
        x[4], x[5], x[6], x[7], x[8], x[9], x[10], x[11], x[12], x[13], x[14], x[15], x[16], x[17]));


        Console.WriteLine("-- Zadanie 2 --");
        var resultsEx2 = from e in employees 
            select new {nazwisko = e.lastname};
        foreach (var e in resultsEx2.ToList()){
            Console.WriteLine(e.nazwisko);
        }

        Console.WriteLine("\n-- Zadanie 3 --");
        var resultsEx3 = from e in employees
            join et in employeeTerritories on e.employeeid equals et.employeeid
            join t in territories on et.territoryid equals t.territoryid
            select new {nazwisko = e.lastname, reg = e.region, terytorium = t.territorydescription};
        foreach (var e in resultsEx3.ToList()){
            Console.WriteLine(e.nazwisko + ", " + e.reg + ", " + e.terytorium);
        }

        // // region: pracownicy
        Console.WriteLine("\n-- Zadanie 4 --");
        var resultsEx4 = from e in employees
                join et in employeeTerritories on e.employeeid equals et.employeeid
                join t in territories on et.territoryid equals t.territoryid
                join r in regions on t.regionid equals r.regionid
                group e.lastname by r.regiondescription into grouped
                select new { Region = grouped.Key, Employees = grouped.Distinct() };

        foreach (var region in resultsEx4)
        {
            Console.WriteLine("{0}:", region.Region);
            foreach (var employee in region.Employees)
            {
                Console.WriteLine("  {0}", employee);
            }
        }

        Console.WriteLine("\n-- Zadanie 5 --");
        foreach (var region in resultsEx4)
        {
            Console.WriteLine("{0}: {1}", region.Region, region.Employees.Count());
        }


        Reader<Order> orderReader = new Reader<Order>();
        List<Order> orders = orderReader.readList("data\\orders.csv", x => new Order(x[0], x[1], x[2], x[3], x[4], x[5], x[6], x[7], x[8],
            x[9], x[10], x[11], x[12], x[13]));

        Reader<OrderDetails> odReader = new Reader<OrderDetails>();
        List<OrderDetails> orderDetails = odReader.readList("data\\orders_details.csv", x => new OrderDetails(x[0], x[1], x[2], x[3], x[4]));
        
        Console.WriteLine("\n-- Zadanie 6 --");
        var resultsEx6 = from o in orders
                join e in employees on o.employeeid equals e.employeeid
                group o.orderid by e.employeeid into grouped
                select new { Employee = grouped.Key, Orders = grouped };
        
        foreach (var employee in resultsEx6)
        {
            double maxValue = 0;
            var employeeName = from e in employees
                                where e.employeeid == employee.Employee
                                select new {Name = e.lastname};

            Console.WriteLine("{0}:", employeeName.First().Name);
            double sum = 0;
            double mean = 0;

            var resultsOrderDetails = from od in orderDetails
                                    where employee.Orders.Contains(od.orderid)
                                    group od by od.orderid into grouped
                                    select new { Order = grouped.Key, Products = grouped };

            foreach (var order in resultsOrderDetails)
            {
                double value = 0;

                foreach (var product in order.Products)
                {
                    if(product.unitprice != null && product.quantity != null && product.discount != null){
                        double unitPrice = double.Parse(product.unitprice, CultureInfo.InvariantCulture);
                        double quantity = double.Parse(product.quantity, CultureInfo.InvariantCulture);
                        double discount = double.Parse(product.discount, CultureInfo.InvariantCulture);
                        value += unitPrice * quantity * (1 - discount);
                    }
                }
                sum += value;
                maxValue = Math.Max(maxValue, value);
            }

            int orderCount = employee.Orders.Count();
            mean = orderCount > 0 ? sum / orderCount : 0;

            Console.WriteLine("    Liczba zamówień: {0}, średnia: {1}, max: {2}", orderCount, mean, maxValue);
            }

    }
}


class Reader<T>{
    public List<T> readList(string path, Func<string[], T> generate){
        List<T> items = new List<T>();
        int i = 1;

        using (StreamReader sr = new StreamReader(path))
        {
            string? line;
            while ((line = sr.ReadLine()) != null)
            {
                if(i != 1){
                    if(!string.IsNullOrEmpty(line)){
                        string[] values = line.Split(','); 
                        try
                        {
                            items.Add(generate(values));
                        }
                        catch (IndexOutOfRangeException)
                        {
                            Console.WriteLine($"Błąd: Nieprawidłowa liczba wartości w linii({i}): {line}");
                        }
                    }
                }
                ++i;

            }
        }
        return items;
    }
}