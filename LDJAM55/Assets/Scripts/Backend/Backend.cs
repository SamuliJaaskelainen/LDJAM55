using DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class Backend : MonoBehaviour
{
    [System.Serializable]
    public struct DialogueAtLevel
    {
        // Represents one dialogue as a list of strings
        [SerializeField]
        public List<DialogueManager.Dialogue> dialogue;
    }

    [System.Serializable]
    public struct DialoguesAtLevel
    {
        // 4 levels expected, index 0 corresponds to power <= 0.25, index 1 to power <= 0.5 etc
        [SerializeField]
        public List<DialogueAtLevel> dialoguesPerPowerLevel;
    }

    // Role indexes correspond to enum indexes in Developer.RoleType
    public List<Sprite> rolePortraits = new();

    // Role indexes correspond to enum indexes in Developer.RoleType
    public List<DialoguesAtLevel> roleDialogues = new();

    // Trait indexes correspond to enum indexes in Developer.Trait
    public List<DialogueAtLevel> traitDialogues = new();

    // "Desks"
    public const int ACTIVE_DEVELOPERS = 4;
    Developer[] activeDevelopers = new Developer[ACTIVE_DEVELOPERS];

    List<Task> hiddenBugs = new();
    List<Task> foundBugs = new();
    List<Task> backlog = new();

    ProductState productState = new();

    public Developer[] ActiveDevelopers { get => activeDevelopers; }
    public ProductState ProductState { get => productState; }
    public bool AllowAudioSpawn { get => allowAudioSpawn; set => allowAudioSpawn = value; }

    bool allowAudioSpawn = false;

    public float FoundBugs()
    {
        float work = 0;
        foreach (Task bug in foundBugs)
        {
            work += bug.Size;
        }
        return work * ProductState.PowerScale;
    }

    public float Backlog()
    {
        float work = 0;
        foreach (Task bug in backlog)
        {
            work += bug.Size;
        }
        return work * ProductState.PowerScale;
    }

    // The last tick's boosts are moved here at the beginning of the next tick, these will then affect that tick
    // Producer boost currently affects the designer, programmer, artist and audio roles
    float currentProducerBoost = 0f;
    float currentInfluencerBoost = 0f;

    public Developer FetchDeveloperFromPool()
    {
        Developer developer = new Developer();

        int developerRoleAsIndex = Developer.RandomDeveloperIndex(allowAudioSpawn);

        developer.Role = (Developer.RoleType)developerRoleAsIndex;

        developer.Traits = Developer.RandomTraits();

        // Power is lower-bounded by current influencer boost level
        developer.Power = Developer.RandomPower(currentInfluencerBoost);

        developer.Durability = Developer.RandomDurability();

        if (developerRoleAsIndex >= rolePortraits.Count)
        {
            Debug.LogWarning("Role " + developer.Role.ToString() + " id " + developerRoleAsIndex + " missing from backend dialogue list (size " + roleDialogues.Count + ")!");
            return developer;
        }

        developer.portrait = rolePortraits[developerRoleAsIndex];

        if (developerRoleAsIndex >= roleDialogues.Count)
        {
            Debug.LogWarning("Role " + developer.Role.ToString() + " id " + developerRoleAsIndex + " missing from backend dialogue list (size " + roleDialogues.Count + ")!");
            return developer;
        }

        const int dialogueLevelCount = 4;
        if (roleDialogues[developerRoleAsIndex].dialoguesPerPowerLevel.Count < dialogueLevelCount)
        {
            Debug.LogWarning("Role " + developer.Role.ToString() + " id " + developerRoleAsIndex + " missing dialogues from backend list, expected to find " + dialogueLevelCount + " levels but only found " + roleDialogues[developerRoleAsIndex].dialoguesPerPowerLevel.Count + "!");
            return developer;
        }

        int powerLevelAsDialogueIndex = Math.Clamp((int)(developer.Power * dialogueLevelCount), 0, dialogueLevelCount - 1);

        developer.Dialogue = roleDialogues[developerRoleAsIndex].dialoguesPerPowerLevel[powerLevelAsDialogueIndex].dialogue;
        // TODO: add trait based dialog?

        return developer;
    }

    public void AddActiveDeveloper(Developer developer)
    {
        bool developerAdded = false;
        for (int i = 0; i < activeDevelopers.Length; ++i)
        {
            if (activeDevelopers[i] == null || !activeDevelopers[i].IsAlive)
            {
                // Keith TODO: Add summon developer audio
                developerAdded = true;
                activeDevelopers[i] = developer;
                Debug.Log("Added new active developer: " + developer.Role);
                return;
            }
        }

        if (!developerAdded)
        {
            Debug.LogWarning("Cannot add more active developers! Active developers array full.");
        }
    }

    public void ProgressTick()
    {
        if (Mathf.Approximately(Time.timeScale, 0f))
            return;

        float deltaTime = Time.deltaTime;

        // Reduce time left to develop
        productState.TimeLeft -= deltaTime;

        // Reset influencer and producer boosts
        currentInfluencerBoost = 0f;
        currentProducerBoost = 0f;

        // Check for producers first so that their effects get applied correctly for this tick
        foreach (Developer developer in activeDevelopers)
        {
            if (developer == null || !developer.IsAlive) continue;
            if (developer.Role.Equals(Developer.RoleType.Producer))
            {
                currentProducerBoost = Math.Max(currentProducerBoost, developer.Power * deltaTime);
            }
        }

        // Then handle the rest of the developers
        foreach (Developer developer in activeDevelopers)
        {
            if (developer == null || !developer.IsAlive) continue;

            HandleDeveloperRoleEffects(developer.Role, developer.Power * deltaTime);
            HandleDeveloperTraitEffects(developer.Traits, developer.Power * deltaTime);

            developer.Durability -= deltaTime;
        }
    }

    // Handles actions taken by developer depending on role, producers are skipped
    private void HandleDeveloperRoleEffects(Developer.RoleType role, float power)
    {
        // Handle role-specific effect based on power
        switch (role)
        {
            case Developer.RoleType.Designer:
                {
                    // Create feature for the backlog based on power
                    backlog.Add(new Task(power + currentProducerBoost));
                    break;
                }
            case Developer.RoleType.Programmer:
                {

                    // N increments on bugs or tasks depending on developer power
                    float powerToSpend = power + currentProducerBoost;
                    while (powerToSpend > 0f)
                    {
                        if (foundBugs.Count != 0 && backlog.Count != 0)
                        {
                            // 50/50 split between bug fixing and backlog progress if both are not empty
                            float powerToSpendOnMechanics = powerToSpend / 2f;
                            float powerToSpendOnBugfixes = powerToSpend / 2f;
                            productState.AddMechanicsFeature(ProgressTasks(ref backlog, ref powerToSpendOnMechanics));
                            productState.AddPolishFeature(ProgressTasks(ref foundBugs, ref powerToSpendOnBugfixes));
                            powerToSpend = powerToSpendOnMechanics + powerToSpendOnBugfixes;
                        }
                        else if (foundBugs.Count != 0)
                        {
                            productState.AddPolishFeature(ProgressTasks(ref foundBugs, ref powerToSpend));
                        }
                        else if (backlog.Count != 0)
                        {
                            productState.AddMechanicsFeature(ProgressTasks(ref backlog, ref powerToSpend));
                        }
                        else
                        {
                            break;
                        }
                    }
                    break;

                }
            case Developer.RoleType.QA:
                {
                    // QA currently not affected by producer boost
                    float powerToSpend = power;
                    float foundBugPower = ProgressTasks(ref hiddenBugs, ref powerToSpend);
                    if (foundBugPower > 0f)
                    {
                        // Currently the found bugs get lumped together into one big "found" bug,
                        // once we decide how how the found bug size is calculated we might also want to refactor
                        // this so that individual unfound bugs become separate found bugs
                        foundBugs.Add(new Task(foundBugPower));
                    }
                    break;
                }
            case Developer.RoleType.Artist:
                {
                    productState.AddVisualsFeature(power + currentProducerBoost);
                    break;
                }
            case Developer.RoleType.Audio:
                {
                    productState.AddAudioFeature(power + currentProducerBoost);
                    break;
                }
            case Developer.RoleType.Producer:
                {
                    // Nothing to do, producers handled separately
                    return;
                }
            case Developer.RoleType.Influencer:
                {
                    // Influencer currently not affected by producer boost
                    currentInfluencerBoost = Math.Max(currentInfluencerBoost, power);
                    break;
                }
            default:
                {
                    Debug.LogError("Missing role type!");
                    break;
                }

        }

        // Probability that a bug will be created if this role is applicable
        const float bugCreationChance = 0.33f;
        var bugCreatingRoles = Array.AsReadOnly(new Developer.RoleType[] { Developer.RoleType.Programmer, Developer.RoleType.Artist, Developer.RoleType.Audio });
        if (bugCreatingRoles.Contains(role) && UnityEngine.Random.Range(0f, 1f) < bugCreationChance)
        {
            // TODO: Just using developer power to determine bug severity for now. Not affected by producer boost.
            float bugCost = power;
            hiddenBugs.Add(new Task(bugCost));
            productState.AddPolishFeature(-bugCost);
        }
    }

    // Progress the provided queue until it is empty or powerToSpend is 0. Return the sum of impacts of completed tasks.
    private float ProgressTasks(ref List<Task> taskList, ref float powerToSpend)
    {
        float completedImpact = 0f;
        while (powerToSpend > 0f && taskList.Count != 0)
        {
            float workLeft = taskList.First().WorkRemaining;
            if (workLeft > powerToSpend)
            {
                taskList.First().Progress += powerToSpend;
                powerToSpend = 0f;
            }
            else
            {
                // TODO: for now just using the tasks's size as the impact
                completedImpact += taskList.First().Size;
                taskList.RemoveAt(0);
                powerToSpend -= workLeft;
            }
        }
        return completedImpact;
    }

    private void HandleDeveloperTraitEffects(List<Developer.Trait> traits, float power)
    {
        if (traits == null)
        {
            Debug.LogError("Missing traits!");
            return;
        }
        foreach (Developer.Trait trait in traits)
        {
            switch (trait)
            {
                case Developer.Trait.Fun:
                    productState.AddFunScore(power);
                    break;
                case Developer.Trait.Innovation:
                    productState.AddInnovationScore(power);
                    break;
                case Developer.Trait.Theme:
                    productState.AddThemeScore(power);
                    break;
                case Developer.Trait.Graphics:
                    productState.AddGraphicsScore(power);
                    break;
                case Developer.Trait.Audio:
                    productState.AddAudioScore(power);
                    break;
                case Developer.Trait.Humor:
                    productState.AddHumorScore(power);
                    break;
                case Developer.Trait.Mood:
                    productState.AddMoodScore(power);
                    break;
                default:
                    Debug.LogError("Missing trait type!");
                    break;
            }
        }

    }
}