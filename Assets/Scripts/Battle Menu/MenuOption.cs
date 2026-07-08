using UnityEngine;

public class MenuOption : MonoBehaviour
{
    public string Text;
    public System.Action OnSelected;

    public MenuOption(string text, System.Action onSelected)
    {
        Text = text;
        OnSelected = onSelected;
    }
}
