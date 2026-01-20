using System;

[Flags]
public enum JPCharacterFaction : uint
{
    Neutral = 0,
    Player = 1 << 0,
    Enemy = 1 << 1,
    Spectator = uint.MaxValue
}