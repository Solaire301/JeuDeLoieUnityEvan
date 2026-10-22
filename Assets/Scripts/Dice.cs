using System;
using UnityEngine;
using UnityEngine.UI;

public class Dice : MonoBehaviour
{
    [SerializeField] private Button button;

    private bool subscribed;

    public event Action<int> Rolled;

    private void OnEnable()
    {
        if (button == null)
        {
            Debug.LogError("Dice : aucun Button assigné.", this);
            return;
        }

        if (subscribed)
        {
            return;
        }

        button.onClick.AddListener(OnButtonClicked);
        subscribed = true;
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    public void SetInteractable(bool interactable)
    {
        if (button == null)
        {
            Debug.LogError("Dice : impossible de changer l'état du bouton, aucun Button assigné.", this);
            return;
        }

        button.interactable = interactable;
    }

    private void Unsubscribe()
    {
        if (!subscribed)
        {
            return;
        }

        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClicked);
        }

        subscribed = false;
    }

    private void OnButtonClicked()
    {
        if (button == null)
        {
            Debug.LogError("Dice : clic ignoré, aucun Button assigné.", this);
            return;
        }

        int value = UnityEngine.Random.Range(1, 7);
        Rolled?.Invoke(value);
    }
}
