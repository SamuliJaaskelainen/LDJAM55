using DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class Backend : MonoBehaviour
{
    public List<Developer> developerPool = new List<Developer>();

    // "Desks"
    Developer[] activeDevelopers = new Developer[4];

    List<Task> hiddenBugs = new();
    List<Task> foundBugs = new();
    List<Task> backlog = new();

    ProductState productState = new();

    // The last tick's boosts are moved here at the beginning of the next tick, these will then affect that tick
    // Producer boost currently affects the designer, programmer, artist and audio roles
    float currentProducerBoost = 0f;
    float currentInfluencerBoost = 0f;

    public Developer FetchDeveloperFromPool()
    {
        if (developerPool.Count == 0)
        {
            Debug.LogError("Empty developer pool!");
        }

        Developer selectedDeveloper = developerPool.ElementAt(UnityEngine.Random.Range(0, developerPool.Count));

        // Power is lower-bounded by current influencer boost level
        selectedDeveloper.Power = Math.Max(selectedDeveloper.Power, currentInfluencerBoost);

        return selectedDeveloper;
    }

    public void AddActiveDeveloper(Developer developer)
    {
        bool developerAdded = false;
        for (int i = 0; i < activeDevelopers.Length; ++i)
        {
            if (activeDevelopers[i] == null)
            {
                // TODO: Signal to UI to add developer graphics
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
        float deltaTime = Time.deltaTime;

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
                        // Progress a bug if any exist, or a feature otherwise
                        if (foundBugs.Count != 0)
                        {
                            productState.PolishFeature += ProgressTasks(ref foundBugs, ref powerToSpend);
                        }
                        else if (backlog.Count != 0)
                        {
                            productState.MechanicsFeature += ProgressTasks(ref backlog, ref powerToSpend);
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
                    productState.VisualsFeature += power + currentProducerBoost;
                    break;
                }
            case Developer.RoleType.Audio:
                {
                    productState.AudioFeature += power + currentProducerBoost;
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
        const float bugCreationChance = 0.5f;
        var bugCreatingRoles = Array.AsReadOnly(new Developer.RoleType[] { Developer.RoleType.Programmer, Developer.RoleType.Artist, Developer.RoleType.Audio });
        if (bugCreatingRoles.Contains(role) && UnityEngine.Random.Range(0f, 1f) < bugCreationChance)
        {
            // TODO: Just using developer power to determine bug severity for now. Not affected by producer boost.
            float bugCost = power;
            hiddenBugs.Add(new Task(bugCost));
            productState.PolishFeature -= bugCost;
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
                    productState.FunScore += power;
                    break;
                case Developer.Trait.Innovation:
                    productState.InnovationScore += power;
                    break;
                case Developer.Trait.Theme:
                    productState.ThemeScore += power;
                    break;
                case Developer.Trait.Graphics:
                    productState.GraphicsScore += power;
                    break;
                case Developer.Trait.Audio:
                    productState.AudioScore += power;
                    break;
                case Developer.Trait.Humor:
                    productState.HumorScore += power;
                    break;
                case Developer.Trait.Mood:
                    productState.MoodScore += power;
                    break;
                default:
                    Debug.LogError("Missing trait type!");
                    break;
            }
        }

    }
}