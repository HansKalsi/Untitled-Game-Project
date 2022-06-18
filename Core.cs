using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using TMPro;

public class Core : MonoBehaviour
{
    public GameBoard gameBoard;
    List<Tile> tiles = new List<Tile>();
    List<Nation> nations = new List<Nation>();
    public World world;
    public GameObject wealthTextREF;
    public GameObject foodTextREF;
    public GameObject matsTextREF;
    public GameObject ironTextREF;

    // Start is called before the first frame update
    void Start()
    {
        startGame();
        InvokeRepeating("advanceWorldWeek", 0f, 0.25f);
    }

    void startGame()
    {
        world.Construct();
        generateTiles(100, 10, 10);
        logTiles();
        generateNations(3);
    }

    void generateNations(int num)
    {
        for (int i = 0; i < num; i++)
        {
            int[] startingResources = new int[3] { 1000, 200, 100 };
            nations.Add(new Nation(i.ToString(), (Nation.coloursENUM)i, startingResources));
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
                Debug.Log(p.logDetails());
            }
            foreach (Building b in t.buildings)
            {
                Debug.Log(b.logDetails());
            }
            foreach (Resource r in t.resources)
            {
                Debug.Log(r.logDetails());
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
    #endregion
    void advanceWorldWeek()
    {
        if (world.AdvanceWeek())
        {
            for (int i = 0; i < tiles.Count; i++)
            {
                if (tiles[i] != null)
                {
                    Tile t = tiles[i];
                    Nation nation = null;
                    for (int n = 0; n < nations.Count; n++)
                    {
                        if (nations[n].ReturnColour() == t.owner)
                        {
                            nation = nations[n];
                            break;
                        }
                    }

                    if (nation != null)
                    {
                        foreach (Pop p in t.population)
                        {
                            populationTriggers(p, t, nation);
                        }
                        foreach (Building b in t.buildings)
                        {
                            buildingTriggers(b, nation);
                        }
                    }
                }
            }
            updateTopNavUI((nations[0].wealth).ToString(), (nations[0].resources[0]).ToString(), (nations[0].resources[1]).ToString(), (nations[0].resources[2]).ToString());
        }
    }

    #region Triggers
    void populationTriggers(Pop p, Tile t, Nation n)
    {
        // trigger each pops weekly actions
        bool[] resp = p.AdvanceWeek(world, n);
        // death_trigger
        if (resp[0])
        {
            // destroy pop
            tiles.Remove(t);
        }

        Debug.Log(p.logDetails());
    }
    void buildingTriggers(Building b, Nation n)
    {
        // trigger each buildings weekly actions
        bool[] resp = b.AdvanceWeek(world, n);

        Debug.Log(b.logDetails());
    }
    #endregion

    void updateTopNavUI(string wText, string fText, string mText, string iText)
    {
        wealthTextREF.GetComponent<TMPro.TextMeshProUGUI>().text = wText;
        foodTextREF.GetComponent<TMPro.TextMeshProUGUI>().text = fText;
        matsTextREF.GetComponent<TMPro.TextMeshProUGUI>().text = mText;
        ironTextREF.GetComponent<TMPro.TextMeshProUGUI>().text = iText;
    }
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
    public Dictionary<string, float> needs = new Dictionary<string, float>();
    // require KeyValuePair to be read ^
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

        logMsg += "Population: " + amount + "\n";
        logMsg += "Gender: " + (gender == true ? "Male" : "Female") + "\n";
        logMsg += "Age: " + age + "\n";
        logMsg += "Profession: " + profession + "\n";
        foreach (KeyValuePair<string, float> n in needs)
        {
            logMsg += "Need: " + n.Key + " Desire: " + n.Value + "\n";
        }
        logMsg += "Quality: " + quality + "\n";
        logMsg += "Wealth: " + wealth + "\n";

        return logMsg;
    }

    public bool[] AdvanceWeek(World w, Nation nation)
    {
        bool[] resp = new bool[] { false };
        tickYearsToAge(ref resp);
        needsEffects(nation);
        return resp;
    }

    void tickYearsToAge(ref bool[] resp)
    {
        avgYearsToAge -= 1;
        if (avgYearsToAge < 1)
        {
            ageUp(ref resp);
            calculateAvgYearsToAge();
        }
    }

    #region needs Effects
    void needsEffects(Nation nation)
    {
        foreach (KeyValuePair<string, float> n in needs.ToList())
        {
            // check desire trigger
            if (n.Value >= 1)
            {
                if (attemptConsumption(n.Key, nation))
                {
                    needs[n.Key] -= 1f;
                }
            }
            // if pop hasn't consumed within 3 months, take attrition
            if (n.Value >= 3)
            {
                takeAttrition(n.Key);
            }
            // add desire for need (4 weeks for trigger)
            needs[n.Key] += 0.25f;
        }
    }

    void takeAttrition(string need)
    {
        switch (need)
        {
            case "food":
                // 20% attrition for entire pop
                amount = ((int)(amount * 0.8));
                break;
            case "iron":
                quality -= 1;
                break;
            default:
                Debug.Log("no attrition occurred");
                break;
        }
    }

    bool attemptConsumption(string need, Nation nation)
    {
        switch (need)
        {
            case "food":
                if (wealth >= 1)
                {
                    if (nation.attemptFoodPurchase())
                    {
                        // successfully purchased food
                        wealth -= 1;
                        return true;
                    }
                    else
                    {
                        Debug.Log("nation has no food stockpiled");
                        return false;
                    }
                }
                else
                {
                    Debug.Log("pop doesn't have enough wealth to attempt to buy food");
                    return false;
                }
            case "iron":
                if (wealth >= 1)
                {
                    if (nation.attemptIronPurchase())
                    {
                        // successfully purchased iron
                        wealth -= 1;
                        return true;
                    }
                    else
                    {
                        Debug.Log("nation has no iron stockpiled");
                        return false;
                    }
                }
                else
                {
                    Debug.Log("pop doesn't have enough wealth to attempt to buy iron");
                    return false;
                }
            default:
                Debug.Log("no consumption occurred for: " + need);
                return false;
        }
    }
    #endregion
}

public class Citizen : Pop
{
    public Citizen(int num, bool g, int a, int q, int w) : base(num, g, a, q, w)
    {
        needs.Add("food", 0);
        profession = (ProfessionENUM)0;
    }
}

public class Farmer : Pop
{
    public Farmer(int num, bool g, int a, int q, int w) : base(num, g, a, q, w)
    {
        needs.Add("food", 0);
        profession = (ProfessionENUM)1;
    }
}

public class Labourer : Pop
{
    public Labourer(int num, bool g, int a, int q, int w) : base(num, g, a, q, w)
    {
        needs.Add("food", 0);
        profession = (ProfessionENUM)2;
    }
}

public class Levy : Pop
{
    public Levy(int num, bool g, int a, int q, int w) : base(num, g, a, q, w)
    {
        needs.Add("food", 0);
        needs.Add("iron", 0);
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
        n.addResource(((int)type), collectFromBuilding());
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
    public int wealth;

    public Nation(string n, coloursENUM c, int[] startingResources)
    {
        name = n;
        colour = c;
        resources = startingResources;
        wealth = 0;
    }

    public void addResource(int r, int amount)
    {
        resources[r] += amount;
    }
    public void removeResource(int r, int amount)
    {
        resources[r] -= amount;
    }

    public void addWealth(int amount)
    {
        wealth += amount;
    }
    public void removeWealth(int amount)
    {
        wealth -= amount;
    }

    public string ReturnColour()
    {
        return colour.ToString();
    }

    public bool attemptFoodPurchase()
    {
        if (resources[0] >= 1)
        {
            resources[0] -= 1;
            addWealth(1);
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool attemptIronPurchase()
    {
        if (resources[2] >= 1)
        {
            resources[2] -= 1;
            addWealth(1);
            return true;
        }
        else
        {
            return false;
        }
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
