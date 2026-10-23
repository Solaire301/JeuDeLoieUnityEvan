using UnityEngine;

/// <summary>
/// Conserve la case du joueur entre les changements de scène (level).
/// Les données vivent en mémoire tant que l'application tourne.
/// </summary>
public static class PlayerProgress
{
    private const int NoSavedIndex = -1;

    public static int SavedTileIndex { get; private set; } = NoSavedIndex;
    public static bool HasSavedTile => SavedTileIndex >= 0;

    // Réinitialise à chaque lancement du Play Mode (utile si "Domain Reload" est désactivé).
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetOnPlay()
    {
        SavedTileIndex = NoSavedIndex;
    }

    public static void Save(int tileIndex)
    {
        SavedTileIndex = tileIndex;
    }

    public static void Clear()
    {
        SavedTileIndex = NoSavedIndex;
    }
}
