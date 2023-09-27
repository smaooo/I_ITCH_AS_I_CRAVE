using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class ChosenDependency
{
    public enum Dependency {Family, Friend, Partner, Home, Knife, Pet, TBD}
    public static Dependency chosenDependency = Dependency.Knife;
}
