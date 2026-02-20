using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;


namespace Bokningssystem_max_lonnqvist
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SkapaLokal.LaddaFrånJson();

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
                            break;
                        case "2":
                            break;
                        case "3":
                            break;
                        case "4":
                            SkapaLokal.SkrivUtAllaLokaler();
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



        while (pickStartTime)
        {
            Console.Clear();
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
                    while (pickEndTime = true)
                    {
                        Console.WriteLine($"Välj när du vill avsulta bokningen");
                        string inputEndTime = Console.ReadLine();

                        isInt = int.TryParse(inputEndTime, out value); //Returns true is input is an integer
                                                                       //Checks if the input is a valid input
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
}
public class Grupprum : Lokal
{
    private static int LokalID = 1; //ID för lokalen som automatiskt tilldelas när man skapar ett nytt rum
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
    private static int ID = 1; //Namnet för lokalen som automatiskt tilldelas när man skapar ett nytt rum
    public int _lokalId { get; set; }
    public int Kapacitet { get; set; }
    public string SalNamn { get; set; }
    public string Typ { get; set; }

    public Sal() { } //Tom konstruktor för json serialisering
    public Sal(string typ, int kapacitet, string salNamn)
    {
        _lokalId = ID;
        ID++;
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
            // coalesce null to empty string so compiler knows it's non-null
            grupprumsNamn = Console.ReadLine() ?? "";
            if (grupprumsNamn == "")
            {
                Console.WriteLine("Namnet får inte vara tomt, försök igen.");
            }
        }

        int kapacitet = 0;
        Console.WriteLine($"Hur många personer får plats i grupprummet?: ");
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
        Console.WriteLine($"Hur många personer får plats i salen?: ");
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

    public static void SkrivUtAllaLokaler()
    {
        Console.Clear();
        Console.WriteLine("Grupprum");
        _gruppHantering.VisaAllaGrupprum();

        Console.WriteLine();
        Console.WriteLine("Salar");
        _salHantering.VisaAllaSalar();

        Console.WriteLine();
        Console.WriteLine("Tryck valfri tangent...");
        Console.ReadKey();
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
        }
    }

}
