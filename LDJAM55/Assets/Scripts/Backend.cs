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

    public void ProgressTick()
    {
        foreach (Developer developer in activeDevelopers)
        {
            if (developer.Durability < 1) continue;

            HandleDeveloperEffects(developer.Role, developer.Power);

            --developer.Durability;
        }
    }

    // Handles actions taken by developer depending on role
    private void HandleDeveloperEffects(Developer.RoleType role, int power)
    {
        switch (role)
        {
            case Developer.RoleType.Designer:
                // Create feature for the backlog based on power
                backlog.Add(new Task(power));
                break;
            case Developer.RoleType.Programmer:
                // N increments on bugs or tasks depending on developer power
                for (int step = 0; step < power; ++step)
                {
                    // Progress a bug if any exist, or a feature otherwise
                    if (foundBugs.Count != 0)
                    {
                        int completedTaskImpact = ProgressTask(ref foundBugs);
                        if (completedTaskImpact > 0)
                        {
                            // TODO: Increment polish because bug was fixed
                        }
                    }
                    else
                    {
                        int completedTaskImpact = ProgressTask(ref foundBugs);
                        if (completedTaskImpact > 0)
                        {
                            // TODO: Increment mechanics because feature was implemented
                        }
                    }
                }
                break;
            case Developer.RoleType.Artist:
                // TODO: Progress visuals
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
            case Developer.RoleType.Audio:
                // TODO: Progress audio
                break;
            case Developer.RoleType.Producer:
                // TODO: Boost other roles
                break;
            case Developer.RoleType.Influencer:
                // ??
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
            // Just using developer power to determine bug severity for now
            int bugCost = power;
            hiddenBugs.Add(new Task(bugCost));
            // TODO: Need to regress polish
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