public class Employee{
    public string? employeeid {get; set;}
    public string? lastname {get; set;}
    public string? firstname {get; set;}
    public string? title {get; set;}
    public string? titleofcourtesy {get; set;}
    public string? birthdate {get; set;}
    public string? hiredate {get; set;}
    public string? address {get; set;}
    public string? city {get; set;}
    public string? region {get; set;}
    public string? postalcode {get; set;}
    public string? country {get; set;}
    public string? homephone {get; set;}
    public string? extension {get; set;}
    public string? photo {get; set;}
    public string? notes {get; set;}
    public string? reportsto {get; set;}
    public string? photopath {get; set;}

    public Employee(string employeeid, string lastname, string firstname, string title, string titleofcourtesy, string birthdate, string hiredate,
        string address, string city, string region, string postalcode, string country, string homephone, string extension, 
        string photo, string notes, string reportsto, string photopath){
            this.employeeid = employeeid;
            this.lastname = lastname;
            this.firstname = firstname;
            this.title = title;
            this.titleofcourtesy = titleofcourtesy;
            this.birthdate = birthdate;
            this.hiredate = hiredate;
            this.address = address;
            this.city = city;
            this.region = region;
            this.postalcode = postalcode;
            this.country = country;
            this.homephone = homephone;
            this.extension = extension;
            this.photo = photo;
            this.notes = notes;
            this.reportsto = reportsto;
            this.photopath = photopath;
    }
}


public class Territory{
    public string? territoryid {get; set;}
    public string? territorydescription {get; set;}
    public string? regionid {get; set;}

    public Territory(string territoryid, string territorydescription, string regionid){
        this.territoryid = territoryid;
        this.territorydescription = territorydescription;
        this.regionid = regionid;
    }

    public override string ToString(){
        return $"{territoryid}, {territorydescription}, {regionid}";
    }
}


public class Region{
    public string? regionid {get; set;}
    public string? regiondescription {get; set;}

    public Region(string regionid, string regiondescription){
        this.regionid = regionid;
        this.regiondescription = regiondescription;
    }
}

public class EmployeeTerritory{
    public string? employeeid {get; set;}
    public string? territoryid {get; set;}

    public EmployeeTerritory(string employeeid, string territoryid){
        this.employeeid = employeeid;
        this.territoryid = territoryid;
    }
}

public class Order{
    public string? orderid {get; set;}
    public string? customerid {get; set;}
    public string? employeeid {get; set;}
    public string? orderdate {get; set;}
    public string? requireddate {get; set;}
    public string? shippeddate {get; set;}
    public string? shipvia {get; set;}
    public string? freight {get; set;}
    public string? shipname {get; set;}
    public string? shipaddress {get; set;}
    public string? shipcity {get; set;}
    public string? shipregion {get; set;}
    public string? shippostalcode {get; set;}
    public string? shipcountry {get; set;}


    public Order(string orderid, string customerid, string employeeid, string orderdate, string requireddate, string shippeddate,
        string shipvia, string freight, string shipname, string shipaddress, string shipcity, string shipregion,
        string shippostalcode, string shipcountry){
        this.orderid = orderid;
        this.customerid = customerid;
        this.employeeid = employeeid;
        this.orderdate = orderdate;
        this.requireddate = requireddate;
        this.shippeddate = shippeddate;
        this.shipvia = shipvia;
        this.freight = freight;
        this.shipname = shipname;
        this.shipaddress = shipaddress;
        this.shipcity = shipcity;
        this.shipregion = shipregion;
        this.shippostalcode = shippostalcode;
        this.shipcountry = shipcountry;
    }
}

public class OrderDetails{
    public string? orderid {get; set;}
    public string? productid {get; set;}
    public string? unitprice {get; set;}
    public string? quantity {get; set;}
    public string? discount {get; set;}

    public OrderDetails(string orderid, string productid, string unitprice, string quantity, string discount){
        this.orderid = orderid;
        this.productid = productid;
        this.unitprice = unitprice;
        this.quantity = quantity;
        this.discount = discount;
    }
}