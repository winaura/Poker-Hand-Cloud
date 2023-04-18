using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCounter
{
    private int level;
    private int experience;
    private int minDefaultLevelValue;
    private int maxDefaultLevelValue;
    private int minExperienceBorder;
    private int maxExperienceBorder;
    private float experienceMultiplier;

    public int Level => level;
    public int MinExperienceBorder => minExperienceBorder;
    public int MaxExperienceBorder => maxExperienceBorder;
    public int Experience
    {
        get => experience;
        set
        {
            experience = value;
            minExperienceBorder = minDefaultLevelValue;
            maxExperienceBorder = maxDefaultLevelValue;
            level = 1;
            CalculateLevel();
        }
    }

    public LevelCounter(int experience) : this(experience, 0, 100, 1.1f) { }

    public LevelCounter(int experience, int minDefaultLevelValue, int maxDefaultLevelValue, float experienceMultiplier)
    {
        this.minDefaultLevelValue = minDefaultLevelValue;
        this.maxDefaultLevelValue = maxDefaultLevelValue;
        this.experienceMultiplier = experienceMultiplier;
        Experience = experience;
    }

    public void AddExperience(int experienceAdd)
    {
        experience += experienceAdd;
        CalculateLevel();
    }

    private void CalculateLevel()
    {
        while (experience >= maxExperienceBorder)
        {
            level++;
            int temp = maxExperienceBorder;
            maxExperienceBorder += Mathf.RoundToInt((maxExperienceBorder - minExperienceBorder) * experienceMultiplier);
            minExperienceBorder = temp;
        }
    }
}