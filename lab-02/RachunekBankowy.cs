using System.Text;

public class RachunekBankowy{
    private string numer;
    private decimal stanRachunku;
    private bool czyDozwolonyDebet;
    private List<PosiadaczRachunku> posiadaczeRachunku;

    private List<Transakcja> transakcje;

    public string Numer 
    {
        get { return numer; }
        set { numer = value; }
    }

    public decimal StanRachunku 
    {
        get { return stanRachunku; }
        set { stanRachunku = value;}
    }

    public bool CzyDozwolonyDebet 
    {
        get { return czyDozwolonyDebet; }
        set { czyDozwolonyDebet = value; }
    }

    public List<PosiadaczRachunku> PosiadaczeRachunku {
        get { return posiadaczeRachunku; }
        set { posiadaczeRachunku = value; }
    }

    public List<Transakcja> Transakcje
    {
        get { return transakcje; }
        set { transakcje = value; }
    }

    public RachunekBankowy(string numer, decimal stanRachunku, bool czyDozwolonyDebet, List<PosiadaczRachunku> posiadaczeRachunku){
        this.numer = numer;
        this.stanRachunku = stanRachunku;
        this.czyDozwolonyDebet = czyDozwolonyDebet;
        if(posiadaczeRachunku != null && posiadaczeRachunku.Count > 0){
            this.posiadaczeRachunku = posiadaczeRachunku;
        }
        else{
            throw new Exception("Lista posiadaczy rachunku musi mieć co najmniej jedną pozycję.");
        }
        transakcje = new List<Transakcja>();
    }

    public static void DokonajTransakcji(RachunekBankowy rachunekZrodlowy, RachunekBankowy rachunekDocelowy, decimal kwota, string opis){
        if(kwota < 0){
            throw new Exception("Kwota nie może być ujemna");
        }
        else if(rachunekZrodlowy == null && rachunekDocelowy == null){
            throw new Exception("Oba rachunki nie mogą być jednocześnie null");
        }
        else if(rachunekZrodlowy != null && rachunekZrodlowy.StanRachunku < kwota && rachunekZrodlowy.czyDozwolonyDebet == false){
            throw new Exception("Rachunek źródłowy nie może przeprowadzić transakcji");
        }
        
        Transakcja transakcja = new Transakcja(rachunekZrodlowy, rachunekDocelowy, kwota, opis);
        
        if(rachunekZrodlowy == null){
            rachunekDocelowy.StanRachunku += kwota;
            rachunekDocelowy.Transakcje.Add(transakcja);
        }
        else if(rachunekDocelowy == null){
            rachunekZrodlowy.StanRachunku -= kwota;
            rachunekZrodlowy.Transakcje.Add(transakcja);
        }
        else {
            rachunekZrodlowy.StanRachunku -= kwota;
            rachunekDocelowy.StanRachunku += kwota;
            rachunekDocelowy.Transakcje.Add(transakcja);
            rachunekZrodlowy.Transakcje.Add(transakcja);
        }
    }

    public static RachunekBankowy operator +(RachunekBankowy rb, PosiadaczRachunku p){
        if(rb.PosiadaczeRachunku.Contains(p)) 
            throw new Exception("Posiadacz rachunku już istnieje w liście.");

        var l1 = new List<PosiadaczRachunku>(rb.PosiadaczeRachunku);
        l1.Add(p);
        
        RachunekBankowy rb1 = new RachunekBankowy(rb.Numer, rb.StanRachunku, rb.CzyDozwolonyDebet, l1);
        rb1.Transakcje = new List<Transakcja>(rb.Transakcje);
        
        return rb1;
    }

    public static RachunekBankowy operator -(RachunekBankowy rb, PosiadaczRachunku p){
        if(rb.PosiadaczeRachunku.Count == 1) 
            throw new Exception("Nie można usunąć ostatniego posiadacza rachunku.");
        
        if(!rb.PosiadaczeRachunku.Contains(p)) 
            throw new Exception("Posiadacza rachunku nie ma na liście.");
        
        var l1 = new List<PosiadaczRachunku>(rb.PosiadaczeRachunku);
        l1.Remove(p);
        RachunekBankowy rb1 = new RachunekBankowy(rb.Numer, rb.StanRachunku, rb.CzyDozwolonyDebet, l1);
        rb1.Transakcje = new List<Transakcja>(rb.Transakcje);
        return rb1;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Rachunek {numer}, stan: {stanRachunku}");
        sb.Append("Posiadacze rachunku: ");
        foreach(PosiadaczRachunku p in posiadaczeRachunku){
            sb.Append($"{p.ToString()}, ");
        }

        sb.AppendLine("\nTransakcje: ");
        foreach(Transakcja tr in transakcje){
            sb.AppendLine($"{tr.ToString()},");
        }

        return sb.ToString(); 
    }
}