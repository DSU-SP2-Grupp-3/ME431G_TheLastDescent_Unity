using UnityEngine;

[System.Serializable]
public class CinematicAnimationInfo
{
    [SerializeField]
    public int ActorID;
    [SerializeField]
    public Animator animator;
    [SerializeField]
    public string startTrigger, endTrigger;
    [SerializeField]
    public bool hasEndAnimation;
}
