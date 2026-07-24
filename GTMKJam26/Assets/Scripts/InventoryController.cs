using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InventoryController : MonoBehaviour
{
    public CollectionQuest initialQuest;
    private Dictionary<string, int>  goalItems = new Dictionary<string, int>();
    public UnityEvent onFinished;
    
    private static InventoryController _instance;
    public static InventoryController Instance;

 
 
    void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(gameObject);
        else
            _instance = this;
    }

    public void Start()
    {
        DontDestroyOnLoad(gameObject);
        
        if (initialQuest != null)
            SetQuest(initialQuest);
    }

    public void SetQuest(CollectionQuest quest)
    {
        goalItems = new Dictionary<string, int>();
        foreach (QuestElement q in quest.items)
            goalItems.Add(q.item.name, q.amount);
    }

    public void Collect(ItemScriptableObject item)
    {
        if (!goalItems.ContainsKey(item.name))
            return;
        
        goalItems[item.name]--;
        if (goalItems[item.name] == 0)
            goalItems.Remove(item.name);
            
        if (goalItems.Count == 0)
            onFinished.Invoke();
    }
}
