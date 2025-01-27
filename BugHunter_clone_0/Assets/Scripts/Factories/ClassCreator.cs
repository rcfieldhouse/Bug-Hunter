using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassCreator : MonoBehaviour
{
    [SerializeField] ClassType ClassSelection;
    public GameObject ClassIcons;
    public List<Mesh> CharacterModels;
    public List<Material> CharacterMaterials;
    // Start is called before the first frame update
    private void Awake()
    {
        
        //CreateAClass();
       Invoke(nameof(CreateAClass), 0.05f);
    }
    public void CreateAClass()
    {
        if (GameObject.Find("SceneLoadData") != null)
        {
            ClassSelection = GameObject.Find("SceneLoadData").GetComponent<SceneLoadData>().GetClass();
        }
        CreateClass(gameObject);
       GetComponent<GUIHolder>().PickupPrompt.SetActive(false);
    }
    // Update is called once per frame
    public void CreateClass(GameObject Player)
    {
        ClassInterface ClassCreator = ClassFactory.SpawnClass(ClassSelection);
        ClassCreator.CreateClass(Player,CharacterModels,CharacterMaterials);
    }
    public void SetClass(ClassType classType)
    {
        ClassSelection = classType;
    }
    public ClassType GetClass()
    {
        return ClassSelection;
    }
}
