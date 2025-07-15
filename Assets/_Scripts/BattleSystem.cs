using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class BattleSystem : MonoBehaviour
{
    [SerializeField] private enum BattleState { Start, Selection, Battle, Won, Lost, Run }
    
    [Header("Battle State")]
    [SerializeField] private BattleState state;
    
    [Header("Spawn Points")]
    [SerializeField] private Transform[] partySpawnPoints;
    [SerializeField] private Transform[] enemySpawnPoints;
    
    [Header("Battlers")]
    [SerializeField] private List<BattleEntities> allBattlers = new List<BattleEntities>();
    [SerializeField] private List<BattleEntities> enemyBattlers = new List<BattleEntities>();
    [SerializeField] private List<BattleEntities> playerBattlers = new List<BattleEntities>();
    
    [Header("UI")]
    [SerializeField] private GameObject[] enemySelectionButtons;
    [SerializeField] private GameObject battleMenu;
    [SerializeField] private GameObject enemySelectionMenu;
    [SerializeField] private TextMeshProUGUI actionText;
    [SerializeField] private GameObject bottomTextPopUp;
    [SerializeField] private TextMeshProUGUI bottomText;
    
    private PartyManager partyManager;
    private EnemyManager enemyManager;
    private int currentPlayer;
    
    private const string ACTION_MESSAGE = "'s Action:";
    private const string WIN_MESSAGE = "You Won The Battle!";
    private const string LOSE_MESSAGE = "You Lost The Battle!";
    private const string SUCCESFULY_RUN_MESSAGE = "Your Party Run Away!";
    private const string UNSUCCESSFULY_RUN_MESSAGE = "Your Party Failed Run!";
    private const int TURN_DURATION = 2;
    private const int RUN_CHANCE = 50;
    private const string OVERWORLD_SCENE = "OverworldScene";
    
    void Start()
    {
        partyManager = GameObject.FindFirstObjectByType<PartyManager>();
        enemyManager = GameObject.FindFirstObjectByType<EnemyManager>();

        CreatePartyEntities();
        CreateEnemyEntities();
        ShowBattleMenu();
        DetermineBattleOrder();
    }

    private IEnumerator BattleRoutine()
    {
        enemySelectionMenu.SetActive(false);    // enemy selection menu disabled
        state = BattleState.Battle;     // change state to battle state
        bottomTextPopUp.SetActive(true);    // enable bottom text

        // loop all battlers
        for (int i = 0; i < allBattlers.Count; i++)
        {
            if (state == BattleState.Battle && allBattlers[i].CurrHealth > 0)
            {
                switch (allBattlers[i].BattleAction)
                {
                    case BattleEntities.Action.Attack:
                        // do attack
                        yield return StartCoroutine(AttackRoutine(i));
                        break;
                    case BattleEntities.Action.Run:
                        // run
                        yield return StartCoroutine(RunRoutine());
                        break;
                    default:
                        Debug.Log("Error - incorrect battle action");
                        break;
                }
            }

        }
        
        RemoveDeadBattlers();
        
        // if haven't won or lost, loop => open battle menu
        if (state == BattleState.Battle)
        {
            bottomTextPopUp.SetActive(false);
            currentPlayer = 0;
            ShowBattleMenu();
        }
        
        yield return null;
    }

    private IEnumerator AttackRoutine(int i)
    {
        // Player turn
        if (allBattlers[i].IsPlayer == true)
        {
            BattleEntities currArtacker = allBattlers[i];
            if (allBattlers[currArtacker.Target].CurrHealth <= 0)
            {
                currArtacker.SetTarget(GetRandomEnemy());
            }
            BattleEntities currTarget = allBattlers[currArtacker.Target];
            
            AttackAction(currArtacker, currTarget);     // attack selected enemy (attack action)
            yield return new WaitForSeconds(TURN_DURATION);      // wait
            
            // kill enemy
            if (currTarget.CurrHealth <= 0)
            {
                bottomText.text = string.Format("{0} defeated {1}", currArtacker.Name, currTarget.Name);
                yield return new WaitForSeconds(TURN_DURATION);      // wait
                enemyBattlers.Remove(currTarget);

                if (enemyBattlers.Count <= 0)   // if no enemy remain
                {
                    // -> we won
                    state = BattleState.Won;
                    bottomText.text = WIN_MESSAGE;
                    yield return new WaitForSeconds(TURN_DURATION);      // wait
                    SceneManager.LoadScene(OVERWORLD_SCENE);
                }
            }
        }
        
        
        // Enemy turn
        if (i< allBattlers.Count && allBattlers[i].IsPlayer == false)
        {
            BattleEntities currArtacker = allBattlers[i];
            currArtacker.SetTarget(GetRandomPartyMember());     // get random party member
            BattleEntities currTarget = allBattlers[currArtacker.Target];
            
            AttackAction(currArtacker, currTarget);     // attack selected party member (attack action)
            yield return new WaitForSeconds(TURN_DURATION);      // wait
            
            // kill party member
            if (currTarget.CurrHealth <= 0)
            {
                bottomText.text = string.Format("{0} defeated {1}", currArtacker.Name, currTarget.Name);
                yield return new WaitForSeconds(TURN_DURATION);      // wait
                playerBattlers.Remove(currTarget);

                if (playerBattlers.Count <= 0)   // if no member remain
                {
                    // -> we lost
                    state = BattleState.Won;
                    bottomText.text = LOSE_MESSAGE;
                    yield return new WaitForSeconds(TURN_DURATION);      // wait
                    Debug.Log("Game Over");
                }
            }
            
            
        }
        
    }

    private IEnumerator RunRoutine()
    {
        if (state == BattleState.Battle)
        {
            if (Random.Range(0, 101) >= RUN_CHANCE)
            {
                // we have run away

                bottomText.text = SUCCESFULY_RUN_MESSAGE;   // set bottom text
                state = BattleState.Run;    // set state to run
                allBattlers.Clear();    // clear all battlers list
                yield return new WaitForSeconds(TURN_DURATION);     // wait
                SceneManager.LoadScene(OVERWORLD_SCENE);
                yield break;
            }
            else
            {
                // we failed to run away
                
                bottomText.text = UNSUCCESSFULY_RUN_MESSAGE;   // set bottom text
                yield return new WaitForSeconds(TURN_DURATION);     // wait
            }
        }
    }

    private void RemoveDeadBattlers()
    {
        for (int i = 0; i < allBattlers.Count; i++)
        {
            if (allBattlers[i].CurrHealth <= 0)
            {
                allBattlers.RemoveAt(i);
            }
        }
    }
    
    
    private void CreatePartyEntities()
    {
        // get current party
        List<PartyMember> currentParty = new List<PartyMember>();
        currentParty = partyManager.GetCurrentParty();
    
        // create battle entities for our member
        for (int i = 0; i < currentParty.Count; i++)
        {
            BattleEntities tempEntity = new BattleEntities();
            // assign the values
            tempEntity.SetEntityValues(currentParty[i].MemberName, currentParty[i].CurrHealth, currentParty[i].MaxHealth, currentParty[i].Initiative, currentParty[i].Strength, currentParty[i].Level, true);
            
            // -- updating UI --
            // spawning the visuals
            BattleVisuals tempBattleVisuals = Instantiate(currentParty[i].MemberBattleVisualPrefab, partySpawnPoints[i].position, Quaternion.identity).GetComponent<BattleVisuals>();
            
            // set visuals starting values
            tempBattleVisuals.SetStartingValues(currentParty[i].CurrHealth, currentParty[i].MaxHealth, currentParty[i].Level);
            
            // assign it to the battle entity
            tempEntity.BattleVisuals = tempBattleVisuals;
            
            allBattlers.Add(tempEntity);
            playerBattlers.Add(tempEntity);
        }
        
    }

    private void CreateEnemyEntities()
    {
        List<Enemy> currentEnemies = new List<Enemy>();
        currentEnemies = enemyManager.GetCurrEnemies();
    
        for (int i = 0; i < currentEnemies.Count; i++)
        {
            BattleEntities tempEntity = new BattleEntities();
            
            tempEntity.SetEntityValues(currentEnemies[i].EnemyName, currentEnemies[i].CurrHealth, currentEnemies[i].MaxHealth, currentEnemies[i].Initiative, currentEnemies[i].Strength, currentEnemies[i].Level, false);
    
            BattleVisuals tempBattleVisuals = Instantiate(currentEnemies[i].EnemyBattleVisualPrefab, enemySpawnPoints[i].position, Quaternion.identity).GetComponent<BattleVisuals>();
            
            tempBattleVisuals.SetStartingValues(currentEnemies[i].MaxHealth, currentEnemies[i].MaxHealth, currentEnemies[i].Level);
            tempEntity.BattleVisuals = tempBattleVisuals;
            
            allBattlers.Add(tempEntity);
            enemyBattlers.Add(tempEntity);
        }
    }

    public void ShowBattleMenu()
    {
        // who action
        actionText.text = playerBattlers[currentPlayer].Name + ACTION_MESSAGE;
        battleMenu.SetActive(true);
    }

    public void ShowEnemySelectionMenu()
    {
        // disable battle menu
        battleMenu.SetActive(false);
        
        SetEnemySelectionButtons();
        enemySelectionMenu.SetActive(true);
    }

    private void SetEnemySelectionButtons()
    {
        // disable all buttons
        for (int i = 0; i < enemySelectionButtons.Length; i++)
        {
            enemySelectionButtons[i].SetActive(false);
        }
        
        // enable buttons for each enemy
        for (int j = 0; j < enemyBattlers.Count; j++)
        {
            enemySelectionButtons[j].SetActive(true);
            enemySelectionButtons[j].GetComponentInChildren<TextMeshProUGUI>().text = enemyBattlers[j].Name; // change buttons text
        }
    }

    public void SelectEnemy(int currentEnemy)
    {
        // set current member target
        BattleEntities currentPlayerEntity = playerBattlers[currentPlayer];
        currentPlayerEntity.SetTarget(allBattlers.IndexOf(enemyBattlers[currentEnemy]));

        currentPlayerEntity.BattleAction = BattleEntities.Action.Attack;

        currentPlayer++;

        if (currentPlayer >= playerBattlers.Count)     // if all player have selected an action
        {
            // start the battle
            StartCoroutine(BattleRoutine());
        }
        else
        {
            enemySelectionMenu.SetActive(false);    // show the battle menu for the next player
            ShowBattleMenu();
        }

    }

    // a template for every Attack
    private void AttackAction(BattleEntities currAttacker, BattleEntities currTarget)
    {
        int damage = currAttacker.Strength;     // get damage
        currAttacker.BattleVisuals.PlayAttackAnimation();   // play attack anim
        currTarget.CurrHealth -= damage;    // dealing the damage
        currTarget.BattleVisuals.PlayHitAnimation();    // play their hit anim
        currTarget.UpdateUI();  // update UI
        bottomText.text = string.Format("{0} attacks {1} for {2} damage", currAttacker.Name,currTarget.Name,damage);
        SaveHealth();
    }

    private int GetRandomPartyMember()
    {
        List<int> partyMembers = new List<int>(); // create a temporary list
        
        // find all party member -> add to list
        for (int i = 0; i < allBattlers.Count; i++)
        {
            if (allBattlers[i].IsPlayer == true)
            {
                partyMembers.Add(i);
            }
        }
        return partyMembers[Random.Range(0, partyMembers.Count)];   // return a random party member
    }
    
    private int GetRandomEnemy()
    {
        List<int> enemies = new List<int>();
        for (int i = 0; i < allBattlers.Count; i++)
        {
            if (allBattlers[i].IsPlayer == false)
            {
                enemies.Add(i);
            }
        }
        return enemies[Random.Range(0, enemies.Count)];
    }

    private void SaveHealth()
    {
        for (int i = 0; i < playerBattlers.Count; i++)
        {
            partyManager.SaveHealth(i, playerBattlers[i].CurrHealth);
        }
    }

    private void DetermineBattleOrder()
    {
        allBattlers.Sort((bi1, bi2) => -bi1.Initiative.CompareTo(bi2.Initiative));  // sorts list by Initiative in ascending order
    }

    public void SelectRunAction()
    {
        state = BattleState.Selection;
        // set current member target
        BattleEntities currentPlayerEntity = playerBattlers[currentPlayer];

        currentPlayerEntity.BattleAction = BattleEntities.Action.Run;

        battleMenu.SetActive(false);
        currentPlayer++;

        if (currentPlayer >= playerBattlers.Count)     // if all player have selected an action
        {
            // start the battle
            StartCoroutine(BattleRoutine());
        }
        else
        {
            enemySelectionMenu.SetActive(false);    // show the battle menu for the next player
            ShowBattleMenu();
        }
    }
}

[System.Serializable]
public class BattleEntities
{
    public enum Action { Attack, Run}
    public Action BattleAction;
    
    public string Name;
    public int CurrHealth;
    public int MaxHealth;
    public int Initiative;
    public int Strength;
    public int Level;
    public bool IsPlayer;
    public BattleVisuals BattleVisuals;
    public int Target;
    public void SetEntityValues(string name, int currHealth, int maxHealth, int initiative, int strength, int level, bool isPlayer)
    {
        Name = name;
        CurrHealth = currHealth;
        MaxHealth = maxHealth;
        Initiative = initiative;
        Strength = strength;
        Level = level;
        IsPlayer = isPlayer;
    }

    public void SetTarget(int target)
    {
        Target = target;
    }

    public void UpdateUI()
    {
        BattleVisuals.ChangeHealth(CurrHealth);
    }
    
}