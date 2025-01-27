using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlexisRock : MonoBehaviour
{
    public ShootableObject TheRock;
    public CutsceneBoomBoom boom;
    [Range(0, 30)] public float ExplosionTime, ThrowBombs,TurnOffCutScene;
    private void Awake()
    {
        Invoke(nameof(BreakTheRock),ExplosionTime);
        Invoke(nameof(DoBoom), ThrowBombs);
        Invoke(nameof(endScene), TurnOffCutScene);
    }
    void BreakTheRock()
    {   
        TheRock.HandleObjectDeath(TheRock.transform);
    }
    void DoBoom()
    {
        boom.DoTheBoom();
    }
    void endScene()
    {
       gameObject.SetActive(false);
    }
}
