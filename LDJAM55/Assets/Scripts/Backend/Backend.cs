using DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class Backend : MonoBehaviour
{
    [SerializeField]
    List<Developer> developerPool;

    // "Desks"
    Developer[] activeDevelopers = new Developer[4];

    List<Task> hiddenBugs;
    List<Task> foundBugs;
    List<Task> backlog;

    ProductState state;

    // The last tick's boosts are moved here at the beginning of the next tick, these will then affect that tick
    // Producer boost currently affects the designer, programmer, artist and audio roles
    float currentProducerBoost = 0;
    float currentInfluencerBoost = 0;

    public void ProgressTick()
    {
        float deltaTime = Time.deltaTime;

        // Reset influencer and producer boosts
        currentInfluencerBoost = 0f;
        currentProducerBoost = 0f;

        // Check for producers first so that their effects get applied correctly for this tick
        foreach (Developer developer in activeDevelopers)
        {
            if (!developer.isAlive) continue;
            if (!developer.Role.Equals(Developer.RoleType.Producer)) continue;

            currentProducerBoost = Math.Max(currentProducerBoost, developer.Power * deltaTime);

            developer.Durability -= deltaTime;

        }

        // Then handle the rest of the developers
        foreach (Developer developer in activeDevelopers)
        {
            if (!developer.isAlive) continue;
            if (developer.Role.Equals(Developer.RoleType.Producer)) continue;

            HandleDeveloperEffects(developer.Role, developer.Power * deltaTime);

            developer.Durability -= deltaTime;
        }
    }

    // Handles actions taken by developer depending on role, producers are skipped
    private void HandleDeveloperEffects(Developer.RoleType role, float power)
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
                            state.Polish += ProgressTasks(ref foundBugs, ref powerToSpend);
                        }
                        else if (backlog.Count != 0)
                        {
                            state.Mechanics += ProgressTasks(ref backlog, ref powerToSpend);
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
                    state.VisualsScore += power + currentProducerBoost;
                    break;
                }
            case Developer.RoleType.Audio:
                {
                    state.AudioScore += power + currentProducerBoost;
                    break;
                }
            case Developer.RoleType.Producer:
                {
                    // Nothing to do, producers handled separately
                    return;
                }
            case Developer.RoleType.Influencer:
                {
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
            state.Polish -= bugCost;
        }
    }

    // Progress the provided queue until it is empty or powerToSpend is 0. Return the sum of impacts of completed tasks.
    private float ProgressTasks(ref List<Task> taskList, ref float powerToSpend)
    {
        float completedImpact = 0;
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
}