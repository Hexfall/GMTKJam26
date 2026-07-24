using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CollectionQuest", menuName = "Scriptable Objects/CollectionQuest")]
public class CollectionQuest : ScriptableObject
{
    public List<QuestElement> items;
}

[Serializable]
public struct QuestElement
{
    public ItemScriptableObject item;
    public int amount;
}
