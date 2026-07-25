using TMPro;
using UnityEngine;

public class UpdateObjective : MonoBehaviour
{
    [SerializeField][TextArea(2,20)] private string objective;
    private TextMeshProUGUI  objectiveText;

    private void Start()
    {
        objectiveText = GameObject.FindGameObjectWithTag("ObjectiveTracker").GetComponentInChildren<TextMeshProUGUI>();
    }

    public void Trigger()
    {
        objectiveText.text = objective;
    }
}
