using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestController : MonoBehaviour
{
    public static TestController instance = null;
    public GameObject manSprite;
    private MapManager mapScript;
    private GameObject selectedMan;
    private List<GameObject> men;

    private bool keyDown = false;

    // Use this for initialization
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else if (instance != this)
        {
            Destroy(gameObject);
        }
        
        DontDestroyOnLoad(gameObject);

        mapScript = GetComponent<MapManager>();

        mapScript.SetupScene();

        men = new List<GameObject>();
        men.Clear();

        // A little test to make the men -- though do we want to do this?
        men.Add(Instantiate(manSprite, new Vector3(-2.5f, -2.5f, -0.3f), Quaternion.identity) as GameObject);
        //men.Add(Instantiate(manSprite, new Vector3(-1.5f, -2.5f, -0.5f), Quaternion.identity) as GameObject);

        foreach (GameObject man in men) {
            man.GetComponent<ManController>().map = mapScript;
        }

        selectedMan = men[0];
    }

    void Start()
    {
        //Army goodArmy = new global::Army("Army of the Bold", 200, 50);
        //Army evilArmy = new global::Army("Black Riders", 100, 150);

        //Battle testBattle = new Battle(goodArmy, evilArmy);
        //while (goodArmy.men + goodArmy.horses > 0 && evilArmy.men + evilArmy.horses > 0) { 
        //Debug.Log(testBattle.resolve());
        //}
    }

    // Update is called once per frame
    void Update()
    {
        int horizontal = 0;
        int vertical = 0;

        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");

        if (horizontal != 0)
            vertical = 0;

        if ((horizontal != 0 || vertical != 0) && !keyDown)
        {
            selectedMan.GetComponent<ManController>().AttemptMove(horizontal, vertical);
            keyDown = true;
        }
        else if (horizontal == 0 && vertical == 0)
            keyDown = false;
    }

    public void MoveManUp()
    {
        selectedMan.GetComponent<ManController>().AttemptMove(0, 1);
    }

    public void MoveManDown()
    {
        selectedMan.GetComponent<ManController>().AttemptMove(0, -1);
    }

    public void MoveManLeft()
    {
        selectedMan.GetComponent<ManController>().AttemptMove(-1, 0);
    }

    public void MoveManRight()
    {
        selectedMan.GetComponent<ManController>().AttemptMove(1, 0);
    }
}


public class Army
{
    public int men { get; set; }
    public int horses { get; set; }
    public string name { get; set; }

    public Army(string name, int men, int horses)
    {
        this.name = name;
        this.men = men;
        this.horses = horses;
        Debug.Log(string.Format("Created Army: {0} - {1} men and {2} horses", name, men, horses));
    }

    public int getForces()
    {
        return men + horses;
    }

    public string getDetails()
    {
        return string.Format("{0} - {1} men, {2} riders\n", name, men, horses);
    }
}


public class Battle
{
    private Army attackingArmy, defendingArmy;

    public Battle(Army attackingArmy, Army defendingArmy)
    {
        this.attackingArmy = attackingArmy;
        this.defendingArmy = defendingArmy;
    }

    private int doSkirmish(int bodiesA, int bodiesB, int attackBonus)
    {
        int attackStrength, defenceStrength;

        for (int i=0; i<(int)(Mathf.Round(bodiesA / 5)) && bodiesB != 0; ++i) {
            attackStrength = (int)(Random.Range(0, 255));
            attackStrength += attackBonus;
            defenceStrength = (int)(Random.Range(0, 255));
                
            if (attackStrength > defenceStrength)
            {
                bodiesB--;
            }
        }

        return bodiesB;
    }

    private void doFight(Army armyA, Army armyB)
    {
        // A is attacking B
        // do horses first against horses
        if (armyA.horses > 0)
        { 
            if (armyB.horses > 0)
            {
                armyB.horses = doSkirmish(armyA.horses, armyB.horses, 0);
            }
            else
            {
                // or against men if there aren't any horses to defend
                armyB.men = doSkirmish(armyA.horses, armyB.men, 100);     // horses are dope, yo
            }
        }

        if (armyA.men > 0)
        {
            if (armyB.men > 0)
            {
                armyB.men = doSkirmish(armyA.men, armyB.men, 5);
            } else if (armyB.horses > 0)
            {
                armyB.horses = doSkirmish(armyA.men, armyB.horses, -10);    // men against horses at a disadvantage
            }
        }
    }

    public string resolve()
    {
        int startAtkMen = attackingArmy.men;
        int startAtkHorses = attackingArmy.horses;
        int startDefMen = defendingArmy.men;
        int startDefHorses = defendingArmy.horses;

        // do attack fight first
        doFight(attackingArmy, defendingArmy);

        // then defender's fight
        doFight(defendingArmy, attackingArmy);

        // store results in string
        int endAtkMen = attackingArmy.men;
        int endAtkHorses = attackingArmy.horses;
        int endDefMen = defendingArmy.men;
        int endDefHorses = defendingArmy.horses;

        string output = "";
        if (attackingArmy.getForces() > defendingArmy.getForces())
        {
            output = string.Format("{0} won the day over {1} - Killing {4} men and {5} horses, before burying {2} poor souls and their brave {3} steeds\n",
                attackingArmy.name, defendingArmy.name,
                startAtkMen - endAtkMen,
                startAtkHorses - endAtkHorses,
                startDefMen- endDefMen,
                startDefHorses - endDefHorses
                );
        } else
        {
            output = string.Format("Horrible news! {1} won the day over {0} - Slaughtering {2} of our men and {3} horses. We only slayed {4} and {5} demon steeds\n",
                attackingArmy.name, defendingArmy.name,
                startAtkMen - endAtkMen,
                startAtkHorses - endAtkHorses,
                startDefMen - endDefMen,
                startDefHorses - endDefHorses
                );
        }

        output += attackingArmy.getDetails();
        output += defendingArmy.getDetails();

        return output;
    } 
}