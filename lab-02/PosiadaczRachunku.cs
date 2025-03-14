using System.Text;
using System.Text.RegularExpressions;
public abstract class PosiadaczRachunku{
    public abstract override string ToString();
}

public class OsobaFizyczna : PosiadaczRachunku{
    private string? imie;
    private string? drugieImie;
    private string? nazwisko;
    private string? PESEL;
    private string? numerPaszportu;

    public string? Imie
    {
        get { return imie; }
        set { imie = value; }
    }

    public string? DrugieImie
    {
        get { return drugieImie; }
        set { drugieImie = value; }
    }

    public string? Nazwisko
    {
        get { return nazwisko; }
        set { nazwisko = value; }
    }

    public string? Pesel
    {
        get { return PESEL; }
        set {
                if(value != null && value.Length != 11)
                    throw new Exception("Niepoprawny PESEL");
                 
                PESEL = value; 
            }
    }

    public string? NumerPaszportu
    {
        get { return numerPaszportu; }
        set { numerPaszportu = value; }
    }

    public OsobaFizyczna(string imie, string nazwisko, string drugieImie, string? pesel, string? numerPaszportu){
        this.imie = imie;
        this.nazwisko = nazwisko;
        this.drugieImie = drugieImie;
        
        if(pesel == null && numerPaszportu == null)
            throw new Exception("PESEL albo numer paszportu muszą być nie null");
        
        if(pesel != null && (pesel.Length != 11 || !Regex.IsMatch(pesel, @"^\d+$")))
            throw new Exception("Niepoprawny PESEL");

        PESEL = pesel;
        this.numerPaszportu = numerPaszportu;
        
    }

    public override string ToString()
    {
        return "Osoba fizyczna " + Imie + " " + Nazwisko;
    }
}


public class OsobaPrawna : PosiadaczRachunku{
    private string? nazwa;
    private string? siedziba;
    
    public string? Nazwa
    {
        get { return nazwa; }
    }

    public string? Siedziba
    {
        get { return siedziba; }
    }

    public OsobaPrawna(string nazwa, string siedziba){
        this.nazwa = nazwa;
        this.siedziba = siedziba;
    }

    public override string ToString()
    {
        return $"Osoba prawna {Nazwa} {Siedziba}";
    }

}