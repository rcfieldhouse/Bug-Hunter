using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootSpawner : MonoBehaviour
{
    public static LootSpawner instance;
    public GameObject prefab;
    public GameObject CommonDrop,UncommonDrop,RareDrop;
    private GameObject Drop;
    public List<GameObject> Prefabs;
    public Transform Transform;
    private float num;
    public float spawnForce = 20;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
 

    public void SprayLoot(Transform transform)
    {
        int x = 1;
        num = Random.Range(0.0f, 100.0f);
        LFInterface loot= LootFactory.CreateLoot(LootType.Grenade); 

        if (num < 45.0f)
        {
            x = 0;
            loot = LootFactory.CreateLoot(LootType.Health);
        }
            
        else if (num >55.0f)
        {
            x = 2;
            loot = LootFactory.CreateLoot(LootType.Ammo);
        }

        Create(loot,transform,x);
    }
    void Create(LFInterface Loot, Transform transform,int num)
    {
        
        
        Drop=Instantiate(Prefabs[num], transform.position, Quaternion.identity);
       
        Loot.Create(Drop);
        Rigidbody rb = Drop.GetComponent<Rigidbody>();
        float x = Random.Range(-1.0f, 1.0f);
        float y = Random.Range(0.0f, 1.0f);
        float z = Random.Range(-1.0f, 1.0f);
        if (rb != null)
        {
            rb.transform.position += Vector3.up;       
            Vector3 force = (Vector3.Normalize(new Vector3(x,y,z))+Vector3.up) * spawnForce;
            rb.velocity = force;
        }
    }

    public GameObject GetDropType(int LootNum)
    {
        GameObject TempObj= CommonDrop;
        switch (LootNum)
        {
            case 1:
            case 4:
            case 7:
            case 10:
                TempObj = CommonDrop;
                break;
            case 2:
            case 5:
            case 8:
            case 11:
                TempObj = UncommonDrop;
                break;
            case 3:
            case 6:
            case 9:
            case 12:
                TempObj = RareDrop;
                break;
        }
        return TempObj;
    }
    public void DropMaterials(Transform transform, int LootType)
    {
        
        Drop = Instantiate(GetDropType(LootType), transform.position+Vector3.up, Quaternion.identity);
        MaterialPickup newMat = new MaterialPickup(LootType);


      
        Drop.AddComponent<MaterialPickup>();
        Drop.GetComponent<MaterialPickup>().SetType(LootType);
      
        Drop.AddComponent<SphereCollider>().radius *= 5;
        Drop.GetComponent<SphereCollider>().isTrigger = true;
        Drop.AddComponent<LootMagnetMaterials>();

        //for making it schmoov
        Rigidbody rb = transform.GetComponent<Rigidbody>();
        if (rb == null)
            return;

        Vector3 force = (rb.transform.position - this.transform.position).normalized * spawnForce;
        rb.AddForce(force);
    }
}
