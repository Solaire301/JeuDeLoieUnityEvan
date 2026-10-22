using System;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private List<Tile> tiles = new List<Tile>();

    public int TileCount => tiles == null ? 0 : tiles.Count;

    private void Awake()
    {
        Validate();
    }

    public bool Validate()
    {
        if (tiles == null || tiles.Count < 2)
        {
            Debug.LogError($"Board : au moins deux cases sont requises (actuellement {TileCount}).", this);
            return false;
        }

        bool valid = true;
        for (int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i].Anchor == null)
            {
                Debug.LogError($"Board : ancre manquante sur la case {i}.", this);
                valid = false;
            }
        }

        return valid;
    }

    public bool IsIndexValid(int index)
    {
        return index >= 0 && index < TileCount;
    }

    public Tile GetTile(int index)
    {
        if (!IsIndexValid(index))
        {
            Debug.LogError($"Board : index de case invalide ({index}, nombre de cases {TileCount}).", this);
            return default;
        }

        return tiles[index];
    }

    public int ResolveTarget(int currentIndex, int signedSteps)
    {
        if (!Validate())
        {
            return -1;
        }

        if (!IsIndexValid(currentIndex))
        {
            Debug.LogError($"Board : index de départ invalide ({currentIndex}).", this);
            return -1;
        }

        int last = tiles.Count - 1;
        int period = 2 * last;
        int position = currentIndex + signedSteps;
        int folded = ((position % period) + period) % period;
        return folded <= last ? folded : period - folded;
    }

    public IReadOnlyList<int> GetMovementPath(int currentIndex, int signedSteps)
    {
        if (!Validate())
        {
            return Array.Empty<int>();
        }

        if (!IsIndexValid(currentIndex))
        {
            Debug.LogError($"Board : index de départ invalide ({currentIndex}).", this);
            return Array.Empty<int>();
        }

        int stepCount = Mathf.Abs(signedSteps);
        if (stepCount == 0)
        {
            return Array.Empty<int>();
        }

        int last = tiles.Count - 1;
        int direction = signedSteps > 0 ? 1 : -1;
        int position = currentIndex;
        List<int> path = new List<int>(stepCount);

        for (int i = 0; i < stepCount; i++)
        {
            int next = position + direction;
            if (next > last || next < 0)
            {
                direction = -direction;
                next = position + direction;
            }

            path.Add(next);
            position = next;
        }

        return path;
    }
}
