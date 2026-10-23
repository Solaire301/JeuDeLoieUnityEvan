using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private const int StartIndex = 0;

    [SerializeField] private Board board;
    [SerializeField] private Dice dice;
    [SerializeField] private PlayerPawn pawn;
    [Min(0f)][SerializeField] private float skipTurnDelay = 1f;

    private readonly HashSet<int> resolvedEffectTiles = new HashSet<int>();
    private WaitForSeconds skipTurnWait;
    private bool dependenciesReady;
    private bool busy;
    private bool won;
    private bool lastMoveSucceeded;
    private int turnNumber;
    private int pendingSkips;

    public event Action<int> TurnStarted;
    public event Action<int> TurnSkipped;
    public event Action<int> PawnMoving; 
    public event Action PlayerWon;

    private void Awake()
    {
        skipTurnWait = new WaitForSeconds(skipTurnDelay);

        dependenciesReady = board != null && dice != null && pawn != null;
        if (!dependenciesReady)
        {
            Debug.LogError("GameManager : Board, Dice et PlayerPawn doivent être assignés.", this);
        }

        if (dice != null)
        {
            dice.Rolled += OnRolled;
        }
    }

    private void Start()
    {
        if (dice != null)
        {
            dice.SetInteractable(false);
        }

        if (!dependenciesReady)
        {
            return;
        }

        int startIndex = StartIndex;
        if (PlayerProgress.HasSavedTile)
        {
            int saved = PlayerProgress.SavedTileIndex;

            // On ignore la sauvegarde si elle n'existe pas sur ce plateau
            // ou si elle correspondrait directement à la case d'arrivée.
            if (saved < board.TileCount - 1)
            {
                startIndex = saved;
            }
            else
            {
                Debug.LogWarning($"GameManager : case sauvegardée {saved} inutilisable sur ce plateau, départ à la case {StartIndex}.", this);
            }
        }

        if (!pawn.Initialize(board, startIndex))
        {
            Debug.LogError("GameManager : initialisation du pion impossible, la partie ne démarre pas.", this);
            return;
        }

        StartNextTurn();
    }

    private void OnDestroy()
    {
        if (dice != null)
        {
            dice.Rolled -= OnRolled;
        }
    }

    private void OnRolled(int value)
    {
        if (!dependenciesReady || busy || won)
        {
            return;
        }

        busy = true;
        dice.SetInteractable(false);
        StartCoroutine(PlayTurn(value));
    }

    // mouvement de cases
    private IEnumerator MovePawn(int signedSteps)
    {
        lastMoveSucceeded = false;

        IReadOnlyList<int> path = board.GetMovementPath(pawn.CurrentIndex, signedSteps);
        if (path.Count == 0)
        {
            Debug.LogError($"GameManager : chemin de déplacement invalide ({signedSteps} pas depuis la case {pawn.CurrentIndex}).", this);
            yield break;
        }

        PawnMoving?.Invoke(signedSteps);

        yield return StartCoroutine(pawn.WalkTo(path, board));

        if (!pawn.LastWalkCompleted)
        {
            Debug.LogError($"GameManager : déplacement de {signedSteps} pas non terminé.", this);
            yield break;
        }

        lastMoveSucceeded = true;
    }

    private IEnumerator PlayTurn(int roll)
    {
        yield return StartCoroutine(MovePawn(roll));
        if (!lastMoveSucceeded)
        {
            yield break;
        }

        resolvedEffectTiles.Clear();

        while (true)
        {
            int index = pawn.CurrentIndex;
            Tile tile = board.GetTile(index);

            if (tile.Type == TileType.SkipTurn)
            {
                pendingSkips += Mathf.Max(1, tile.TurnsToSkip);
                break;
            }

            if (tile.Type != TileType.Advance && tile.Type != TileType.Retreat)
            {
                break;
            }

            if (!resolvedEffectTiles.Add(index))
            {
                Debug.LogError($"GameManager : configuration cyclique détectée sur la case {index}, résolution des effets interrompue.", this);
                break;
            }

            int steps = Mathf.Abs(tile.EffectSteps);
            if (steps == 0)
            {
                Debug.LogError($"GameManager : la case {index} est de type {tile.Type} avec effectSteps à 0, effet ignoré.", this);
                break;
            }

            int signedSteps = tile.Type == TileType.Advance ? steps : -steps;

            yield return StartCoroutine(MovePawn(signedSteps));
            if (!lastMoveSucceeded)
            {
                yield break;
            }
        }

        if (pawn.CurrentIndex == board.TileCount - 1)
        {
            PlayerProgress.Clear(); // niveau terminé : le prochain level repart de la case 0
            won = true;
            busy = false;
            PlayerWon?.Invoke();
            yield break;
        }

        PlayerProgress.Save(pawn.CurrentIndex);

        while (pendingSkips > 0)
        {
            turnNumber++;
            pendingSkips--;
            TurnSkipped?.Invoke(turnNumber);
            yield return skipTurnWait;
        }

        StartNextTurn();
        busy = false;
    }

    private void StartNextTurn()
    {
        turnNumber++;
        TurnStarted?.Invoke(turnNumber);
        dice.SetInteractable(true);
    }
}
