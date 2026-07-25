using UnityEngine;

public class SpawnHiddenObject : MonoBehaviour
{
    [SerializeField]  private GameObject gameObject;

    public void Trigger()
    {
        gameObject.SetActive(true);
        gameObject.transform.position = gameObject.transform.parent.position;
        gameObject.transform.parent = transform.parent;
    }
}
