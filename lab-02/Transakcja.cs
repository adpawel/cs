using System.Text;

public class Transakcja{
    private RachunekBankowy? rachunekZrodlowy;
    private RachunekBankowy? rachunekDocelowy;
    private decimal kwota;
    private string? opis;

    public RachunekBankowy? RachunekZrodlowy
    {
        get { return rachunekZrodlowy; }
        set { rachunekZrodlowy = value; }
    }

    public RachunekBankowy? RachunekDocelowy
    {
        get { return rachunekDocelowy; }
        set { rachunekDocelowy = value; }
    }

    public decimal Kwota 
    {
        get { return kwota; }
        set { kwota = value; }
    }

    public string? Opis 
    {
        get { return opis; }
        set { opis = value; }
    }

    public Transakcja(RachunekBankowy? rachunekZrodlowy, RachunekBankowy? rachunekDocelowy, decimal kwota, string opis){
        if(rachunekZrodlowy != null || rachunekDocelowy != null){
            this.rachunekZrodlowy = rachunekZrodlowy;
            this.rachunekDocelowy = rachunekDocelowy;
        }
        else {
            throw new Exception("Rachunek zrodlowy i docelowy nie mogą być null");
        }

        this.kwota = kwota;
        this.opis = opis;
    }

    public override string ToString()
    {   
        StringBuilder sb = new StringBuilder();
        if(rachunekZrodlowy != null) sb.Append($"Transakcja z rachunku {rachunekZrodlowy.Numer} ");
        else sb.Append("Wpłata gotówkowa ");

        if(rachunekDocelowy != null) sb.Append($"na rachunek {rachunekDocelowy.Numer} ");
        else sb.Append("gotówkowa ");

        sb.Append($"o wartości {kwota} ");
        if(opis != null) sb.Append($"\'{opis}\'");

        return sb.ToString();
    }
}