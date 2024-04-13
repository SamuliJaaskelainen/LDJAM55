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
    int currentProducerBoost = 0;
    int currentInfluencerBoost = 0;

    public void ProgressTick()
    {
        // Reset influencer and producer boosts
        currentInfluencerBoost = 0;
        currentProducerBoost = 0;

        // Check for producers first so that their effects get applied correctly for this tick
        foreach (Developer developer in activeDevelopers)
        {
            if (developer.Durability < 1) continue;
            if (!developer.Role.Equals(Developer.RoleType.Producer)) continue;

            currentProducerBoost = Math.Max(currentProducerBoost, developer.Power);

            --developer.Durability;

        }

        // Then handle the rest of the developers
        foreach (Developer developer in activeDevelopers)
        {
            if (developer.Durability < 1) continue;
            if (developer.Role.Equals(Developer.RoleType.Producer)) continue;

            HandleDeveloperEffects(developer.Role, developer.Power);

            --developer.Durability;
        }
    }

    // Handles actions taken by developer depending on role, producers are skipped
    private void HandleDeveloperEffects(Developer.RoleType role, int power)
    {
        // Handle role-specific effect based on power
        switch (role)
        {
            case Developer.RoleType.Designer:
                // Create feature for the backlog based on power
                backlog.Add(new Task(power + currentProducerBoost));
                break;
            case Developer.RoleType.Programmer:
                // N increments on bugs or tasks depending on developer power
                for (int step = 0; step < power + currentProducerBoost; ++step)
                {
                    // Progress a bug if any exist, or a feature otherwise
                    if (foundBugs.Count != 0)
                    {
                        state.Polish += ProgressTask(ref foundBugs);
                    }
                    else
                    {
                        state.Mechanics += ProgressTask(ref backlog);
                    }
                }
                break;
            case Developer.RoleType.QA:
                // N increments on finding existing bugs depending on tester power
                for (int step = 0; step < power; ++step)
                {
                    int completedTaskImpact = ProgressTask(ref hiddenBugs);
                    if (completedTaskImpact > 0)
                    {
                        // TODO: What should the workload of found bugs be? Using the unfound bugs size as a placeholder for now
                        // If the hidden bug was "completed", ie. found, create a found bug based off it
                        foundBugs.Add(new Task(completedTaskImpact));
                    }
                }
                break;
            case Developer.RoleType.Artist:
                state.VisualsScore += power + currentProducerBoost;
                break;
            case Developer.RoleType.Audio:
                state.AudioScore += power + currentProducerBoost;
                break;
            case Developer.RoleType.Producer:
                // Nothing to do, producers handled separately
                return;
            case Developer.RoleType.Influencer:
                currentInfluencerBoost = Math.Max(currentInfluencerBoost, power);
                break;
            default:
                Debug.LogError("Missing role type!");
                break;
        }

        // Probability that a bug will be created if this role is applicable
        const float bugCreationChance = 0.5f;
        var bugCreatingRoles = Array.AsReadOnly(new Developer.RoleType[] { Developer.RoleType.Programmer, Developer.RoleType.Artist, Developer.RoleType.Audio });
        if (bugCreatingRoles.Contains(role) && UnityEngine.Random.Range(0f, 1f) < bugCreationChance)
        {
            // TODO: Just using developer power to determine bug severity for now. Not affected by producer boost.
            int bugCost = power;
            hiddenBugs.Add(new Task(bugCost));
            state.Polish -= bugCost;
        }
    }

    // Increment the first task of the provided queue, and return its impact if it becomes completed. 0 otherwise.
    private int ProgressTask(ref List<Task> taskList)
    {
        int completedImpact = 0;
        if (taskList.Count != 0)
        {
            ++taskList.First().Progress;
            if (taskList.First().IsCompleted)
            {
                // TODO: for now just using the tasks's size as the impact
                completedImpact = taskList.First().Size;
            }
        }
        return completedImpact;
    }
}