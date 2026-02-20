using Bokningssystem_max_lonnqvist;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;


namespace Bokningssystem_max_lonnqvist
{
    internal class Program
    {
        public static void UtvecklareJSON()
        {
            string utvecklare = "Max";
            string jsonString = JsonSerializer.Serialize(utvecklare, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText("utvecklare.json", jsonString);
        }

        public static string userName = "";
        static void Main(string[] args)
        {
            UtvecklareJSON();
            
            SkapaLokal.LaddaFrånJson();

            

            Console.WriteLine("Välkommen till bokningssystemet!");
            Console.Write("Login: ");
            userName = Console.ReadLine() ?? "";



            bool runProgram = true;
            while (runProgram == true)
            {
                Console.Clear();
                Console.WriteLine("[1] Boka sal");
                Console.WriteLine("[2] Bokningar");
                Console.WriteLine("[3] Ändra bokningar");
                Console.WriteLine("[4] Lokaler");
                Console.WriteLine("[5] Skapa Lokal");

                Console.WriteLine();
                Console.WriteLine("[0] Avsluta programmet");

                string input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input))
                {
                    switch (input)
                    {
                        case "1":
                            Lokal lokal = new Lokal();
                            lokal.BokaTid();
                            break;
                        case "2":
                            Lokal visaBokningar = new Lokal();
                            visaBokningar.VisaBokningar();
                            break;
                        case "3":
                            Lokal taBortBokning = new Lokal();
                            taBortBokning.TaBortBokning();
                            break;
                        case "4":
                            SkapaLokal.SkrivUtAllaGrupprum();
                            SkapaLokal.SkrivUtAllaSalar();
                            Console.WriteLine();
                            Console.WriteLine("Tryck valfri tangent...");
                            Console.ReadKey();
                            break;
                        case "5":
                            SkapaLokal.NyLokal();
                            break;


                        case "0":
                            runProgram = false;
                            break;
                        default:
                            Console.WriteLine("Ogiltigt val, försök igen.");
                            break;
                    }
                }
            }
        }
    }
}

interface IBookable
{
    void BokaTid();
}
public class Lokal : IBookable
{
    public static readonly Bokningshantering _bokningshantering = new Bokningshantering();

    //Open times
    static int openTime = 8;
    static int closingTime = 17;



    public void BokaTid()
    {
        bool pickStartTime = true;
        bool pickEndTime = false;
        int IntStartTime;
        TimeOnly startTime;
        TimeOnly endTime;
        TimeSpan bokadTid;


        int valdLokalId = 0;
        string valdLokalNamn = "";
        string lokalTyp = "";
        string idInput;
        int valtId;
        Console.Clear();
        Console.WriteLine("Vill du boka ett grupprum eller en sal?");
        Console.WriteLine("[1] Grupprum");
        Console.WriteLine("[2] Sal");

        string input = Console.ReadLine();
        switch (input)
        {
            case "1":
                lokalTyp = "Grupprum";  
                Console.WriteLine("Grupprum");
                SkapaLokal.SkrivUtAllaGrupprum();

                Console.WriteLine("Ange ID på lokalen du vill boka:");
                idInput = Console.ReadLine();

                if (!int.TryParse(idInput, out valtId)) //Validering av input
                {
                    Console.WriteLine("Ogiltigt ID");
                    return;
                }

                //Hämtar det valda grupprummet baserat på ID
                var valtRum = SkapaLokal.HämtaAllaGrupprum().FirstOrDefault(r => r._lokalId == valtId); 

                if (valtRum == null) //Om inget rum hittas med det angivna ID
                {
                    Console.WriteLine("Lokalen hittades inte.");
                    return;
                }

                //Spara ID och namn på den valda lokalen
                valdLokalId = valtRum._lokalId;
                valdLokalNamn = valtRum.GrupprumNamn;

                break;

            case "2":
                lokalTyp = "Sal";
                Console.WriteLine("Salar");
                SkapaLokal.SkrivUtAllaSalar();

                Console.WriteLine("Ange ID på lokalen du vill boka:");
                idInput = Console.ReadLine();

                if (!int.TryParse(idInput, out valtId)) //Validering av input
                {
                    Console.WriteLine("Ogiltigt ID");
                    return;
                }

                //Hämtar den valda salen baserat på ID
                var valdSal = SkapaLokal.HämtaAllaSalar().FirstOrDefault(r => r._lokalId == valtId);
                
                if (valdSal == null) //Om ingen sal hittas med det angivna ID
                {
                    Console.WriteLine("Lokalen hittades inte.");
                    return;
                }

                //Spara ID och namn på den valda lokalen
                valdLokalId = valdSal._lokalId;
                valdLokalNamn = valdSal.SalNamn;
                break;
            default:
                Console.WriteLine("Ogiltigt val, du kommer att bokas i ett grupprum");
                break;
        }

        while (pickStartTime)
        {
            Console.WriteLine("[0] Tillbaka");
            Console.WriteLine($"Vilken tid vill du boka ({openTime}-{closingTime - 1})");
            string inputStartTime = Console.ReadLine();

            //Go back to main menu
            if (inputStartTime == "0")
            {
                pickStartTime = false;
                break;
            }

            bool isInt = int.TryParse(inputStartTime, out int value); //Returns true is input is an integer
                                                                      
            if (string.IsNullOrEmpty(inputStartTime) || isInt == false)
            {
                Console.WriteLine("Vänligen välj en siffra");
            }
            else
            {
                int startTimeInt = int.Parse(inputStartTime); //Makes input string into an int
                                                              //Checks if the chosen time is within open times
                if (startTimeInt >= openTime && startTimeInt < closingTime)
                {
                    startTime = new TimeOnly(startTimeInt, 0); //Displays HH:00

                    pickStartTime = false;
                    pickEndTime = true;


                    //Pick when your booked time ends
                    while (pickEndTime == true)
                    {
                        Console.WriteLine($"Välj när du vill avsulta bokningen");
                        string inputEndTime = Console.ReadLine();

                        isInt = int.TryParse(inputEndTime, out value); //Returns true is input is an integer
                                                                       
                        if (string.IsNullOrEmpty(inputEndTime) || isInt == false)
                        {
                            Console.WriteLine("Vänligen välj en siffra");
                        }
                        else
                        {
                            int endTimeInt = int.Parse(inputEndTime); //Makes input string into an int
                            if (endTimeInt > startTimeInt && endTimeInt <= closingTime)
                            {

                                endTime = new TimeOnly(endTimeInt, 0); //Displays HH:00
                                bokadTid = endTime - startTime; //Displays HH

                                _bokningshantering.LäggTillBokning(new Bokning(Program.userName,
                                    valdLokalId,
                                    valdLokalNamn,
                                    startTime,endTime));

                                pickEndTime = false;                                   
                                break;
                            }
                            else //endTime is before startTime or after closingTime
                            {
                                Console.WriteLine("Du kan inte välja den tiden");
                            }
                        }
                    }
                    break;
                }
                else //startTime is before openTime or at closingTime
                {
                    Console.WriteLine("Välj en giltig tid");
                }
            }
        }
    }
    public void VisaBokningar()
    {
        _bokningshantering.VisaAllaBokningar();
        Console.ReadKey();
    }
    public void TaBortBokning()
    {
        var bokningar = _bokningshantering.Bokningar;

        if (bokningar == null || bokningar.Count == 0) 
        {
            Console.WriteLine("Det finns inga bokningar att ta bort.");
            Console.ReadKey();
            return;
        }

        Console.Clear();
        Console.WriteLine("Välj vilken bokning du vill ta bort: ");


        for (int i = 0; i < bokningar.Count; i++)
        {
            var b = bokningar[i];
            Console.WriteLine($"[{i + 1}] Bokare: {b.Bokare} | Lokal: {b.LokalNamn} | ID: {b.LokalId} | {b.StartTid}-{b.SlutTid}");
        }
        
        Console.WriteLine();
        Console.WriteLine("[0] Avbryt");

        string input = Console.ReadLine();

        if (!int.TryParse(input, out int val))
        {
            Console.WriteLine("Ogiltigt val.");
            Console.ReadKey();
            return;
        }

        if (val == 0)
            return;

        if (val < 1 || val > bokningar.Count)
        {
            Console.WriteLine("Ogiltigt nummer.");
            Console.ReadKey();
            return;
        }

        var bokningAttTaBort = bokningar[val - 1];
        _bokningshantering.TaBortBokning(bokningAttTaBort);

        Console.WriteLine("Bokningen är nu borttagen.");
        Console.ReadKey();
    }
}

public class Bokning
{
    public string Bokare { get; set; }
    public int LokalId { get; set; }
    public string LokalNamn { get; set; }
    public TimeOnly StartTid { get; set; }
    public TimeOnly SlutTid { get; set; }

    public Bokning() { }

    public Bokning(string bokare, int lokalId, string lokalNamn,
                   TimeOnly startTid, TimeOnly slutTid)
    {
        Bokare = bokare;
        LokalId = lokalId;
        LokalNamn = lokalNamn;
        StartTid = startTid;
        SlutTid = slutTid;
    }
}
public class Bokningshantering
{
    public List<Bokning> Bokningar { get; set; }
    public Bokningshantering()
    {
        Bokningar = new List<Bokning>();
    }
    public void LäggTillBokning(Bokning bokning)
    {
        if (bokning == null) return;
        Bokningar.Add(bokning);
    }
    public void VisaAllaBokningar()
    {
        if (Bokningar == null || Bokningar.Count == 0)
        {
            Console.WriteLine("Det finns inga bokningar.");
            return;
        }
        foreach (var bokning in Bokningar)
        {
            Console.WriteLine($"Bokare: {bokning.Bokare} | Lokal: {bokning.LokalNamn} | Starttid: {bokning.StartTid} | Sluttid: {bokning.SlutTid}");
        }
    }
    public void TaBortBokning(Bokning bokning)
    {
        if (bokning == null) return;
        Bokningar.Remove(bokning);
    }
}

public class Grupprum : Lokal
{
    private static int LokalID = 1; //ID för lokalen som automatiskt tilldelas när man skapar ett nytt rum
    public static void SättNästaId(int nästaId)
    {
        LokalID = nästaId;
    }
    public int _lokalId { get; set; }
    public int Kapacitet { get; set; }
    public string GrupprumNamn { get; set; }
    public string Typ { get; set; }
    public static List<Grupprum> AllaGrupprum { get; set; }

    public Grupprum() { } //Tom konstruktor för json serialisering
    public Grupprum(string typ, int kapacitet, string grupprumsNamn)
    {
        _lokalId = LokalID;
        LokalID++;
        Typ = typ;
        Kapacitet = kapacitet;
        GrupprumNamn = grupprumsNamn;
    }
}
public class Sal : Lokal
{
    private static int LokalID = 1; //Namnet för lokalen som automatiskt tilldelas när man skapar ett nytt rum
    public static void SättNästaId(int nästaId)
    {
        LokalID = nästaId;
    }
    public int _lokalId { get; set; }
    public int Kapacitet { get; set; }
    public string SalNamn { get; set; }
    public string Typ { get; set; }

    public Sal() { } //Tom konstruktor för json serialisering
    public Sal(string typ, int kapacitet, string salNamn)
    {
        _lokalId = LokalID;
        LokalID++;
        Typ = typ;
        Kapacitet = kapacitet;
        SalNamn = salNamn;
    }
}

public class SkapaLokal 
{
    private static readonly HanteringAvGrupprum _gruppHantering = new HanteringAvGrupprum();
    private static readonly HanteringAvSalar _salHantering = new HanteringAvSalar(); 

    public static void NyLokal()
    {
        Console.Clear();
        Console.WriteLine("Vilken typ av lokal vill du lägga till? (1-2).");
        Console.WriteLine("[1] Sal");
        Console.WriteLine("[2] Grupprum");
        while (true)
        {
            Console.Write("Ditt val: ");
            string input = Console.ReadLine(); //Tar in användarens val
            if (int.TryParse(input, out int userChoice)) //Validering
            {
                if (userChoice == 1)
                {
                    SkapaNySal(); //Skickar användaren till metoden för att skapa en ny sal
                    break;
                }
                else if (userChoice == 2)
                {
                    SkapaNyGrupprum(); //Skickar användaren till metoden för att skapa ett nytt grupprum
                    break;
                }
                else
                {
                    Console.WriteLine("Vänliga välj en siffra mellan (1-2)");
                }
            }
            else
                Console.WriteLine("Något gick fel försök igen.");
        }
    }
    //Metod för att skapa ett nytt grupprum
    public static void SkapaNyGrupprum()
    {
        string grupprumsNamn = "";
        while (grupprumsNamn == "")
        {
            Console.WriteLine($"Vad heter grupprummet?: ");
            grupprumsNamn = Console.ReadLine() ?? "";
            if (grupprumsNamn == "")
            {
                Console.WriteLine("Namnet får inte vara tomt, försök igen.");
            }
        }

        int kapacitet = 0;
        Console.WriteLine($"Hur många personer får plats i grupprummet?: (1-8)");
        while (kapacitet < 1 || kapacitet > 8)
        {
            string kapacitetInput = Console.ReadLine();
            if (!string.IsNullOrEmpty(kapacitetInput))
            {
                //Validering av input
                if (!int.TryParse(kapacitetInput, out _))
                {
                    Console.WriteLine("Vänligen ange en siffra för kapaciteten.");
                }
                else
                {
                    //Om valideringen går igenom så parsas inputen till en int
                    kapacitet = int.Parse(kapacitetInput);
                }
            }
            else
            {
                Console.WriteLine("Vänligen ange en siffra för kapaciteten.");
            }

            if (kapacitet < 1 || kapacitet > 8)
            {
                Console.WriteLine("Kapaciteten måste vara mellan 1 och 8, försök igen.");
            }
        }

        _gruppHantering.LäggTillGrupprum(new Grupprum("Grupprum", kapacitet, grupprumsNamn));
        _gruppHantering.SparaTillJson();


        Console.WriteLine($"Grupprummet {grupprumsNamn} med kapacitet {kapacitet} har skapats.");
        Console.ReadKey();
    }

    //Metod för att skapa en ny sal
    public static void SkapaNySal()
    {
        string salNamn = "";
        while (salNamn == "")
        {
            Console.WriteLine($"Vad heter salen?: ");
            salNamn = Console.ReadLine() ?? "";
            if (salNamn == "")
            {
                Console.WriteLine("Namnet får inte vara tomt, försök igen.");
            }
        }

        int kapacitet = 0;
        Console.WriteLine($"Hur många personer får plats i salen?: (8-50)");
        while (kapacitet < 8 || kapacitet > 50)
        {
            string kapacitetInput = Console.ReadLine();
            if (!string.IsNullOrEmpty(kapacitetInput))
            {
                //Validering av input
                if (!int.TryParse(kapacitetInput, out _))
                {
                    Console.WriteLine("Vänligen ange en siffra för kapaciteten.");
                }
                else
                {
                    //Om valideringen går igenom så parsas inputen till en int
                    kapacitet = int.Parse(kapacitetInput);
                }
            }
            else
            {
                Console.WriteLine("Vänligen ange en siffra för kapaciteten.");
            }

            if (kapacitet < 8 || kapacitet > 50)
            {
                Console.WriteLine("Kapaciteten måste vara mellan 8 och 50, försök igen.");
            }
        }
            
        _salHantering.LäggTillSal(new Sal("Sal", kapacitet, salNamn));
        _salHantering.SparaTillJson();

        Console.WriteLine($"Salen {salNamn} med kapacitet {kapacitet} har skapats.");
        Console.ReadKey();
    }

    public static void SkrivUtAllaGrupprum()
    {
        Console.Clear();
        Console.WriteLine("Grupprum");
        _gruppHantering.VisaAllaGrupprum();
    }
    public static List<Grupprum> HämtaAllaGrupprum()
    {
        return _gruppHantering.GrupprumLista;
    }
    public static void SkrivUtAllaSalar()
    {
        Console.WriteLine();
        Console.WriteLine("Salar");
        _salHantering.VisaAllaSalar();
    }
    public static List<Sal> HämtaAllaSalar()
    {
        return _salHantering.SalLista;
    }
    public static void LaddaFrånJson()
    {
        _gruppHantering.LäsJson();
        _salHantering.LäsJson();
}
}
public class HanteringAvGrupprum
{
    public List<Grupprum> GrupprumLista { get; set; }

    public HanteringAvGrupprum()
    {
        GrupprumLista = new List<Grupprum>();
    }

    public void LäggTillGrupprum(Grupprum grupprum)
    {
        if (grupprum == null) return;

        GrupprumLista.Add(grupprum);
    }

    public void VisaAllaGrupprum()
    {
        if (GrupprumLista == null || GrupprumLista.Count == 0)
        {
            Console.WriteLine("Det finns inga grupprum.");
            return;
        }

        foreach (var rum in GrupprumLista)
        {
            Console.WriteLine($"ID: {rum._lokalId} | Namn: {rum.GrupprumNamn} | Kapacitet: {rum.Kapacitet}");
        }
    }

    public void SparaTillJson()
    {
        string jsonString = JsonSerializer.Serialize(GrupprumLista, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText("grupprum.json", jsonString);
    }
    public void LäsJson()
    {
        if (File.Exists("grupprum.json"))
        {
            string jsonString = File.ReadAllText("grupprum.json");
            GrupprumLista = JsonSerializer.Deserialize<List<Grupprum>>(jsonString)
                            ?? new List<Grupprum>();
            if (GrupprumLista.Count > 0)
            {
                int maxId = GrupprumLista.Max(r => r._lokalId);
                Grupprum.SättNästaId(maxId + 1);
            }
        }
    }


}
public class HanteringAvSalar
{
    public List<Sal> SalLista { get; set; }

    public HanteringAvSalar()
    {
        SalLista = new List<Sal>();
    }

    public void LäggTillSal(Sal sal)
    {
        if (sal == null) return;

        SalLista.Add(sal);
    }
    public void VisaAllaSalar()
    {
        if (SalLista == null || SalLista.Count == 0)
        {
            Console.WriteLine("Det finns inga salar.");
            return;
        }

        foreach (var sal in SalLista)
        {
            Console.WriteLine($"ID: {sal._lokalId} | Namn: {sal.SalNamn} | Kapacitet: {sal.Kapacitet}");
        }
    }
    public void SparaTillJson()
    {
        string jsonString = JsonSerializer.Serialize(SalLista, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText("salar.json", jsonString);
    }

    public void LäsJson()
    {
        if (File.Exists("salar.json"))
        {
            string jsonString = File.ReadAllText("salar.json");
            SalLista = JsonSerializer.Deserialize<List<Sal>>(jsonString)
                        ?? new List<Sal>();
            if (SalLista.Count > 0)
            {
                int maxId = SalLista.Max(r => r._lokalId);
                Sal.SättNästaId(maxId + 1);
            }
        }
    }

}
