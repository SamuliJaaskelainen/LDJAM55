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

    // Callback used to clear hell when an influencer is spawned
    public Action clearHell;

    // "Desks"
    public const int ACTIVE_DEVELOPERS = 4;
    Developer[] activeDevelopers = new Developer[ACTIVE_DEVELOPERS];

    float hiddenBugMass = 0f;
    List<Task> hiddenBugs = new();
    float foundBugMass = 0f;
    List<Task> foundBugs = new();
    float backlogMass = 0f;
    List<Task> backlog = new();

    ProductState productState = new();

    bool allowAudioSpawn = false;

    // The last tick's boosts are moved here at the beginning of the next tick, these will then affect that tick
    // Producer boost currently affects the designer, programmer, artist and audio roles
    float currentProducerBoost = 1f;
    float currentInfluencerBoost = 1f;

    public void Reset()
    {
        for (int i = 0; i < activeDevelopers.Length; ++i)
        {
            activeDevelopers[i] = null;
        }

        hiddenBugMass = 0f;
        hiddenBugs = new();
        foundBugMass = 0f;
        foundBugs = new();
        backlogMass = 0f;
        backlog = new();

        productState.Reset();

        allowAudioSpawn = false;
        currentProducerBoost = 1f;
        currentInfluencerBoost = 1f;

        // Start with some hidden bugs initially
        const int ticketCount = 10;
        const float initialHiddenBugPower = 0.05f;
        const float initialBacklogPower = 0.1f;
        for (int i = 0; i < ticketCount; ++i)
        {
            float bugPower = (initialHiddenBugPower / ticketCount) / productState.PowerScale;
            hiddenBugs.Add(new Task(bugPower));
            hiddenBugMass += bugPower;
            productState.AddPolishFeature(-bugPower);

            float backlogPower = (initialBacklogPower / ticketCount) / productState.PowerScale;
            backlog.Add(new Task(backlogPower));
            backlogMass += backlogPower;
        }
    }

    public Developer[] ActiveDevelopers { get => activeDevelopers; }
    public ProductState ProductState { get => productState; }
    public bool AllowAudioSpawn { get => allowAudioSpawn; set => allowAudioSpawn = value; }

    public float FoundBugs()
    {
        return foundBugMass * ProductState.PowerScale;
    }

    public float Backlog()
    {
        return backlogMass * ProductState.PowerScale;
    }

    public Developer FetchDeveloperFromPool()
    {
        Developer developer = new Developer();

        int developerRoleAsIndex = Developer.RandomDeveloperIndex(allowAudioSpawn);

        developer.Role = (Developer.RoleType)developerRoleAsIndex;

        developer.Traits = Developer.RandomTraits();

        // Apply current influencer boost
        developer.Power = Math.Clamp(Developer.RandomPower() * currentInfluencerBoost, 0.1f, 1f);

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

        developer.Dialogue = new List<DialogueManager.Dialogue>(roleDialogues[developerRoleAsIndex].dialoguesPerPowerLevel[powerLevelAsDialogueIndex].dialogue);

        for (int i = 0; i < developer.Dialogue.Count; ++i)
        {
            if (developer.Dialogue[i].text == "SUMMON")
            {
                if (developer.Traits.Count > 0)
                {
                    int traitAsIndex = (int)developer.Traits[UnityEngine.Random.Range(0, developer.Traits.Count)];
                    DialogueManager.Dialogue randomTraitDialogue = traitDialogues[traitAsIndex].dialogue[UnityEngine.Random.Range(0, traitDialogues[traitAsIndex].dialogue.Count)];
                    developer.Dialogue.Insert(0, randomTraitDialogue);
                    break;
                }
            }
        }

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
                if (developer.Role == Developer.RoleType.Influencer && clearHell != null)
                {
                    Debug.Log("Cleared hell");
                    clearHell();
                }
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
        currentInfluencerBoost = 1f;
        currentProducerBoost = 1f;

        // Check for producers first so that their effects get applied correctly for this tick
        foreach (Developer developer in activeDevelopers)
        {
            if (developer == null || !developer.IsAlive) continue;
            if (developer.Role.Equals(Developer.RoleType.Producer))
            {
                // Influencer effect is constant, so we don't add here or multiply by deltatime
                currentProducerBoost += developer.Power;
                developer.WorkDone = developer.Power * productState.PowerScale;
            }
        }

        // Then handle the rest of the developers
        foreach (Developer developer in activeDevelopers)
        {
            if (developer == null || !developer.IsAlive) continue;

            if (developer.Role.Equals(Developer.RoleType.Influencer))
            {
                // Influencer effect is constant, so we don't add here or multiply by deltatime
                developer.WorkDone = HandleDeveloperRoleEffects(developer.Role, developer.Power);
            }
            else
            {
                developer.WorkDone += HandleDeveloperRoleEffects(developer.Role, developer.Power * deltaTime);
            }

            HandleDeveloperTraitEffects(developer.Traits, developer.Power * deltaTime);

            developer.Durability -= deltaTime;
        }
    }

    // Handles actions taken by developer depending on role, producers are skipped. Returns work done.
    private float HandleDeveloperRoleEffects(Developer.RoleType role, float power)
    {
        const float nonQaDebugPenalty = 0.5f;
        float workDone = 0f;
        // Handle role-specific effect based on power
        switch (role)
        {
            case Developer.RoleType.Designer:
                {
                    // If no more backlog tasks are needed, designers work on debugging
                    if (Backlog() + productState.MechanicsFeature >= 1f)
                    {
                        workDone = DoQaWork(power * currentProducerBoost * nonQaDebugPenalty);
                    }
                    else
                    {
                        // Create feature for the backlog based on power
                        float powerToSpend = power * currentProducerBoost;
                        backlog.Add(new Task(powerToSpend));
                        backlogMass += powerToSpend;
                        workDone = powerToSpend * productState.PowerScale;
                    }
                    break;
                }
            case Developer.RoleType.Programmer:
                {

                    // N increments on bugs or tasks depending on developer power
                    float powerToSpend = power * currentProducerBoost;
                    while (powerToSpend > 0f)
                    {
                        // Only work on mechanics if it's not full
                        bool mechanicsWorkToDo = backlog.Count != 0 && productState.MechanicsFeature < 1f;
                        if (foundBugs.Count != 0 && mechanicsWorkToDo)
                        {
                            // 50/50 split between bug fixing and backlog progress if both are not empty
                            float powerToSpendOnMechanics = powerToSpend / 2f;
                            float powerToSpendOnBugfixes = powerToSpend / 2f;

                            float backlogProgress = ProgressTasks(ref backlog, ref powerToSpendOnMechanics);
                            backlogMass -= backlogProgress;
                            workDone += productState.AddMechanicsFeature(backlogProgress);

                            float foundBugProgress = ProgressTasks(ref foundBugs, ref powerToSpendOnBugfixes);
                            foundBugMass -= foundBugProgress;
                            workDone += productState.AddPolishFeature(foundBugProgress);

                            powerToSpend = powerToSpendOnMechanics + powerToSpendOnBugfixes;
                        }
                        else if (foundBugs.Count != 0)
                        {
                            float foundBugProgress = ProgressTasks(ref foundBugs, ref powerToSpend);
                            foundBugMass -= foundBugProgress;
                            workDone += productState.AddPolishFeature(foundBugProgress);
                        }
                        else if (mechanicsWorkToDo)
                        {
                            float backlogProgress = ProgressTasks(ref backlog, ref powerToSpend);
                            backlogMass -= backlogProgress;
                            workDone += productState.AddMechanicsFeature(backlogProgress);
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
                    workDone = DoQaWork(power * currentProducerBoost);
                    break;
                }
            case Developer.RoleType.Artist:
                {
                    // If no more backlog tasks are needed, artists work on debugging
                    if (productState.VisualsFeature >= 1f)
                    {
                        workDone = DoQaWork(power * currentProducerBoost * nonQaDebugPenalty);
                    }
                    else
                    {
                        workDone = productState.AddVisualsFeature(power * currentProducerBoost);
                    }
                    break;
                }
            case Developer.RoleType.Audio:
                {
                    // If no more backlog tasks are needed, artists work on debugging
                    if (productState.AudioFeature >= 1f)
                    {
                        workDone = DoQaWork(power * currentProducerBoost * nonQaDebugPenalty);
                    }
                    else
                    {
                        workDone = productState.AddAudioFeature(power * currentProducerBoost);
                    }
                    break;
                }
            case Developer.RoleType.Producer:
                {
                    // Nothing to do, producers handled separately
                    break;
                }
            case Developer.RoleType.Influencer:
                {
                    currentInfluencerBoost += power;
                    workDone = power * productState.PowerScale;
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
        const float bugPower = 0.5f;
        var bugCreatingRoles = Array.AsReadOnly(new Developer.RoleType[] { Developer.RoleType.Programmer, Developer.RoleType.Artist, Developer.RoleType.Audio });
        if (bugCreatingRoles.Contains(role) && UnityEngine.Random.Range(0f, 1f) < bugCreationChance)
        {
            float bugCost = bugPower * Time.deltaTime;
            hiddenBugs.Add(new Task(bugCost));
            hiddenBugMass += bugCost;
            productState.AddPolishFeature(-bugCost);
        }

        return workDone;
    }

    // Progress QA work with provided power. Returns power-scaled work done.
    private float DoQaWork(float power)
    {
        float foundBugPower = ProgressTasks(ref hiddenBugs, ref power);
        if (foundBugPower > 0f)
        {
            foundBugs.Add(new Task(foundBugPower));
            hiddenBugMass -= foundBugPower;
            foundBugMass += foundBugPower;
        }
        return foundBugPower * productState.PowerScale;
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