using NPCEnums;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Faction")]
public class FactionData : ScriptableObject
{
    public FactionType factionType;
    public string factionName;
    public Sprite factionIcon;

    [System.Serializable]
    public class FactionRelation
    {
        public FactionData otherFaction;
        [Range(-100, 100)] public int attitude;
    }

    public List<FactionRelation> relations;

    public int GetAttitudeTowards(FactionData other)
    {
        var rel = relations.Find(r => r.otherFaction == other);
        return rel != null ? rel.attitude : 0;
    }

    public bool IsHostileTowards(FactionData other) => GetAttitudeTowards(other) < 0;
    public void ModifyAttitudeTowards(FactionData other, int delta)
    {
        var rel = relations.Find(r => r.otherFaction == other);
        if (rel != null)
        {
            rel.attitude = Mathf.Clamp(rel.attitude + delta, -100, 100);
        }
        else
        {
            relations.Add(new FactionRelation
            {
                otherFaction = other,
                attitude = Mathf.Clamp(delta, -100, 100)
            });
        }
    }

}
