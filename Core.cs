using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;

public class Core : MonoBehaviour
{
    public GameBoard gameBoard;
    List<Tile> tiles = new List<Tile>();
    List<Nation> nations = new List<Nation>();
    public World world;

    // Start is called before the first frame update
    void Start()
    {
        startGame();
    }

    void startGame()
    {
        world.Construct();
        generateTiles(100, 10, 10);
        generateNations(3);
    }

    void generateNations(int num)
    {
        for (int i = 0; i < num; i++)
        {
            nations.Add(new Nation(i.ToString(), (Nation.coloursENUM)i));
        }
    }

    void generateTiles(int num, int w, int h)
    {
        for (int i = 0; i < num; i++)
        {
            tiles.Add(new Tile());
        }
        populateTiles();
        makeBoard(w, h);
        logTiles();
        updateBoard();
    }

    void populateTiles()
    {
        // defaults
        foreach (Tile t in tiles)
        {
            t.population.Add(new Citizen(100, true, 1, 0, 100));
            t.population.Add(new Citizen(100, false, 1, 0, 100));

            switch (Random.Range(0, 3))
            {
                case 0:
                    t.resources.Add(new Food());
                    break;
                case 1:
                    t.resources.Add(new Iron());
                    break;
                case 2:
                    t.resources.Add(new Materials());
                    break;
                default:
                    break;
            }
        }
    }

    void logTiles()
    {
        foreach (Tile t in tiles)
        {
            string msg = "Owner: " + t.owner + "\n";
            Debug.Log(msg);

            foreach (Pop p in t.population)
            {
                p.logDetails();
            }
            foreach (Building b in t.buildings)
            {
                b.logDetails();
            }
            foreach (Resource r in t.resources)
            {
                r.logDetails();
            }

            Debug.Log("------------");
        }
    }

    void makeBoard(int width, int height)
    {
        gameBoard.GenerateGameBoard(ref tiles, width, height);
    }
    void updateBoard()
    {
        gameBoard.UpdateBoard(ref tiles);
    }

    #region Button Events
    public void randomizeBoard()
    {
        foreach (Tile t in tiles)
        {
            switch (Random.Range(0, 5))
            {
                case 0:
                    t.owner = "Red";
                    break;
                case 1:
                    t.owner = "Green";
                    break;
                case 2:
                    t.owner = "Blue";
                    break;
                default:
                    t.owner = "Uncontrolled";
                    break;
            }
        }
        updateBoard();
    }
    public void advanceWorldWeek()
    {
        world.AdvanceWeek();
        for (int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i] != null)
            {
                Tile t = tiles[i];
                Nation nation = null;
                foreach (Nation n in nations)
                {
                    if (n.ReturnColour() == t.owner)
                    {
                        nation = n;
                    }
                }

                foreach (Pop p in t.population)
                {
                    populationTriggers(p, t);
                }
                foreach (Building b in t.buildings)
                {
                    buildingTriggers(b, nation);
                }
            }
        }
    }

    void populationTriggers(Pop p, Tile t)
    {
        Debug.Log(p.logDetails());
        // trigger each pops weekly actions
        bool[] resp = p.AdvanceWeek(world);
        // death_trigger
        if (resp[0])
        {
            // destroy pop
            tiles.Remove(t);
        }
    }
    void buildingTriggers(Building b, Nation n)
    {
        Debug.Log(b.logDetails());
        // trigger each buildings weekly actions
        bool[] resp = b.AdvanceWeek(world, n);
    }
    #endregion
}
#region Pops
public class Pop
{
    #region Pop Variables
    public enum AgeENUM
    {
        Child,
        Adult,
        Elderly
    }
    public enum ProfessionENUM
    {
        Citizen,
        Farmer,
        Labourer,
        Levy
    }
    public int amount;
    public bool gender; //Male/Female
    public AgeENUM age; //Child/Adult/Elderly
    public ProfessionENUM profession; //Citizen/Farmer/Labourer/Levy
    public string[] needs;
    public int quality; // Out of 10
    public int wealth; //Income from profession, spent on needs
    public int avgYearsToAge;
    #endregion

    public Pop(int num, bool g, int a, int q, int w)
    {
        amount = num;
        gender = g;
        age = (AgeENUM)a;
        quality = q;
        wealth = w;
        calculateAvgYearsToAge();
    }
    void calculateAvgYearsToAge()
    {
        // Each pop will live a total of 70 years (Child-16-->Adult-55-->Elderly-70-->death)
        switch (age)
        {
            case AgeENUM.Child:
                avgYearsToAge = 832;
                break;
            case AgeENUM.Adult:
                avgYearsToAge = 2028;
                break;
            case AgeENUM.Elderly:
                avgYearsToAge = 780;
                break;
            default:
                avgYearsToAge = 0;
                break;
        }
    }

    void ageUp(ref bool[] resp)
    {
        if (age == (AgeENUM)2)
        {
            // pop dies
            resp[0] = true;
        }
        else
        {
            age += 1;
        }
    }

    public string logDetails()
    {
        string logMsg = "";

        logMsg += "There are " + amount + " pops\n";
        logMsg += "Their gender is " + (gender == true ? "Male" : "Female") + "\n";
        logMsg += "They are " + age + " age\n";
        logMsg += "Their profession is " + profession + "\n";
        foreach (var need in needs)
        {
            logMsg += "Have need: " + need + "\n";
        }
        logMsg += "Their quality is " + quality + "\n";
        logMsg += "Their wealth is " + wealth + "\n";

        return logMsg;
    }

    public bool[] AdvanceWeek(World w)
    {
        bool[] resp = new bool[] { false };
        avgYearsToAge -= 1;
        if (avgYearsToAge < 1)
        {
            ageUp(ref resp);
            calculateAvgYearsToAge();
        }
        return resp;
    }
}

public class Citizen : Pop
{
    public Citizen(int num, bool g, int a, int q, int w) : base(num, g, a, q, w)
    {
        needs = new string[] { "food" };
        profession = (ProfessionENUM)0;
    }
}

public class Farmer : Pop
{
    public Farmer(int num, bool g, int a, int q, int w) : base(num, g, a, q, w)
    {
        needs = new string[] { "food" };
        profession = (ProfessionENUM)1;
    }
}

public class Labourer : Pop
{
    public Labourer(int num, bool g, int a, int q, int w) : base(num, g, a, q, w)
    {
        needs = new string[] { "food" };
        profession = (ProfessionENUM)2;
    }
}

public class Levy : Pop
{
    public Levy(int num, bool g, int a, int q, int w) : base(num, g, a, q, w)
    {
        needs = new string[] { "food", "iron" };
        profession = (ProfessionENUM)3;
    }
}
#endregion
#region Resources
public class Resource
{
    public enum typeENUM
    {
        Food,
        Iron,
        Materials
    }
    public typeENUM type;

    public string logDetails()
    {
        string logMsg = "Type of resource: " + type + "\n";
        return logMsg;
    }
}

public class Food : Resource
{
    public Food()
    {
        type = (typeENUM)0;
    }
}
public class Iron : Resource
{
    public Iron()
    {
        type = (typeENUM)1;
    }
}
public class Materials : Resource
{
    public Materials()
    {
        type = (typeENUM)2;
    }
}
#endregion
#region Tiles
public class Tile
{
    public string owner;
    public List<Pop> population = new List<Pop>();
    public List<Building> buildings = new List<Building>();
    public List<Resource> resources = new List<Resource>();

    public Tile()
    {
        owner = "Uncontrolled";
    }

    public void updateOwner(string newOwner)
    {
        owner = newOwner;
    }

    public void addPop(Pop p)
    {
        population.Add(p);
    }
    public void removePop(Pop p)
    {
        population.Remove(p);
    }

    public void addBuilding(Building b)
    {
        buildings.Add(b);
    }
    public void removeBuilding(Building b)
    {
        buildings.Remove(b);
    }

    public void addResource(Resource r)
    {
        resources.Add(r);
    }
    public void removeResource(Resource r)
    {
        resources.Remove(r);
    }
}
#endregion
#region Buildings
public class Building
{
    public enum typeENUM
    {
        Farm,
        Labourer_Quarters,
        Mine,
        Barracks,
        Housing
    }
    public typeENUM type;
    public int output;

    public string logDetails()
    {
        string logMsg = "Type of building: " + type + "\n";
        return logMsg;
    }

    public bool[] AdvanceWeek(World w, Nation n)
    {
        bool[] resp = new bool[] { };
        n.addResource(((int)type), output);
        return resp;
    }

    public int collectFromBuilding()
    {
        return output;
    }
}
public class Farm : Building
{
    public Farm()
    {
        type = (typeENUM)0;
        output = 1;
    }
}
public class Labourer_Quarters : Building
{
    public Labourer_Quarters()
    {
        type = (typeENUM)1;
        output = 1;
    }
}
public class Mine : Building
{
    public Mine()
    {
        type = (typeENUM)2;
        output = 1;
    }
}
public class Barracks : Building
{
    public Barracks()
    {
        type = (typeENUM)3;
        output = 1;
    }
}
public class Housing : Building
{
    public Housing()
    {
        type = (typeENUM)4;
        output = 1;
    }
}
#endregion
#region Nations
public class Nation
{
    public enum coloursENUM
    {
        Red,
        Green,
        Blue
    }
    public string name;
    public coloursENUM colour;
    public int[] resources = new int[3];

    public Nation(string n, coloursENUM c)
    {
        name = n;
        colour = c;
        resources = new int[3] { 0, 0, 0 };
    }

    public void addResource(int r, int amount)
    {
        resources[r] += amount;
    }
    public void removeResource(int r, int amount)
    {
        resources[r] -= amount;
    }

    public string ReturnColour()
    {
        return colour.ToString();
    }
}
#endregion
#region Events
public class Event
{
    public enum typeENUM
    {
        Migration,
        Crop_Harvest
    }
    public typeENUM type;
    public string description;
    public string effect;
}

public class MigrationEvent : Event
{
    public MigrationEvent()
    {
        type = (typeENUM)0;
        description = "Migrants have arrived!";
        effect = "You gain 100 citizens";
    }

    public void triggerEffect(Pop p)
    {
        p.amount += 100;
    }
}
public class Crop_HarvestEvent : Event
{
    public Crop_HarvestEvent()
    {
        type = (typeENUM)1;
        description = "Bad crop harvest!";
        effect = "You lose 100 food";
    }

    public void triggerEffect(Nation n)
    {
        n.removeResource(0, 100);
    }
}
#endregion
