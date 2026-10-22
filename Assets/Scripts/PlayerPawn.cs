using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPawn : MonoBehaviour
{
    [Min(0.01f)] [SerializeField] private float stepDuration = 0.35f;
    [Min(0f)] [SerializeField] private float pauseBetweenSteps = 0.05f;
    [SerializeField] private AnimationCurve easing = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private WaitForSeconds pauseWait;

    public int CurrentIndex { get; private set; } = -1;
    public bool LastWalkCompleted { get; private set; }

    private void Awake()
    {
        pauseWait = pauseBetweenSteps > 0f ? new WaitForSeconds(pauseBetweenSteps) : null;
    }

    public bool Initialize(Board board, int startIndex)
    {
        LastWalkCompleted = false;

        if (board == null)
        {
            Debug.LogError("PlayerPawn : Board manquant à l'initialisation.", this);
            return false;
        }

        if (!board.Validate())
        {
            return false;
        }

        if (!board.IsIndexValid(startIndex))
        {
            Debug.LogError($"PlayerPawn : index de départ invalide ({startIndex}).", this);
            return false;
        }

        Transform anchor = board.GetTile(startIndex).Anchor;
        if (anchor == null)
        {
            Debug.LogError($"PlayerPawn : ancre manquante pour l'index de départ {startIndex}.", this);
            return false;
        }

        transform.position = anchor.position;
        CurrentIndex = startIndex;
        return true;
    }

    public IEnumerator WalkTo(IReadOnlyList<int> path, Board board)
    {
        LastWalkCompleted = false;

        if (!CanWalk(path, board))
        {
            yield break;
        }

        for (int i = 0; i < path.Count; i++)
        {
            Transform anchor = board.GetTile(path[i]).Anchor;
            if (anchor == null)
            {
                Debug.LogError($"PlayerPawn : ancre manquante sur la case {path[i]} pendant le déplacement.", this);
                yield break;
            }

            yield return StartCoroutine(MoveTo(anchor.position));
            CurrentIndex = path[i];

            if (pauseWait != null && i < path.Count - 1)
            {
                yield return pauseWait;
            }
        }

        LastWalkCompleted = true;
    }

    private bool CanWalk(IReadOnlyList<int> path, Board board)
    {
        if (board == null)
        {
            Debug.LogError("PlayerPawn : Board manquant pour le déplacement.", this);
            return false;
        }

        if (path == null || path.Count == 0)
        {
            Debug.LogError("PlayerPawn : chemin vide, déplacement impossible.", this);
            return false;
        }

        if (!board.IsIndexValid(CurrentIndex))
        {
            Debug.LogError("PlayerPawn : pion non initialisé, déplacement impossible.", this);
            return false;
        }

        for (int i = 0; i < path.Count; i++)
        {
            if (!board.IsIndexValid(path[i]))
            {
                Debug.LogError($"PlayerPawn : index {path[i]} invalide dans le chemin.", this);
                return false;
            }

            if (board.GetTile(path[i]).Anchor == null)
            {
                Debug.LogError($"PlayerPawn : ancre manquante sur la case {path[i]}.", this);
                return false;
            }
        }

        return true;
    }

    private IEnumerator MoveTo(Vector3 target)
    {
        Vector3 start = transform.position;
        float duration = Mathf.Max(0.01f, stepDuration);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            transform.position = Vector3.Lerp(start, target, easing.Evaluate(t));
            yield return null;
        }

        transform.position = target;
    }
}
