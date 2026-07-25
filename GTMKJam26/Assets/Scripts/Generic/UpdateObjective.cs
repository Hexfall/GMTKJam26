using TMPro;
using UnityEngine;

public class UpdateObjective : MonoBehaviour
{
    [SerializeField][TextArea(2,20)] private string objective;
    private ObjectiveTracker  objectiveText;

    private void Start()
    {
        objectiveText = GameObject.FindGameObjectWithTag("ObjectiveTracker").GetComponentInChildren<ObjectiveTracker>();
    }

    public void Trigger()
    {
        objectiveText.Text = objective;
    }
}
