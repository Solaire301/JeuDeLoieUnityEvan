using System;
using UnityEngine;

public enum TileType
{
    Normal,
    Advance,
    Retreat,
    SkipTurn
}

[Serializable]
public struct Tile
{
    [SerializeField] private Transform anchor;
    [SerializeField] private TileType type;
    [Min(0)] [SerializeField] private int effectSteps;
    [Min(1)] [SerializeField] private int turnsToSkip;

    public Transform Anchor => anchor;
    public TileType Type => type;
    public int EffectSteps => effectSteps;
    public int TurnsToSkip => turnsToSkip;
}
