using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.AttributeUsage(System.AttributeTargets.Field,
    AllowMultiple = true)]
public class SeperatorAttribute : PropertyAttribute
{
    public readonly float Height;
    public readonly float Spacing;

    public SeperatorAttribute(float height = 1, float spacing = 10)
    {
        Height = height;
        Spacing = spacing;
    }
}

public class ShopGUI : MonoBehaviour
{
    private int[] AmountOwned = new int[3];
    private int[] AmountNeeded = new int[3];
    private int[] IndexOfMats = new int[3];
    public bool[] ArmourOwned = new bool[4];
    private int WhichArmour=0,WhichWeapon=0;
    private bool UpgradeArmour = true;
    [Header("Player Inventory")]
    [SerializeField]
    private LootHolder playerInventory;
    
    //Array of the shops Scriptable Objects
    public ShopInventory[] shopInventory;

    public ShopInventory[] WeaponUpgrades;

    //array for UI panels to coincide with the inventory
    public GameObject[] shopPanelsGO;

    //TODO: store a shopPanel prefab Scriptable Object from the UI
    public ShopTemplate[] shopPanels;

    //array for UI panels to coincide with the inventory
    public Button[] purchaseButtons;
    public Button[] purchaseWeaponButtons;
    public Image[] purchaseUpgradeParents;
    public EquipmentManager EquipmentManager;
    public GameObject[] CostValues;
    public GameObject CraftButton;
    public GameObject PopupMessage;
    public GameObject WeaponPopupMessage;
    private void Start()
    {

       //for (int i = 0; i < shopInventory.Length; i++)
       //    shopPanelsGO[i].SetActive(true);
      //  LoadShop();
       // ValidatePurchasable();
    }

    public void TabUpgradeArmor()
    {
        UpgradeArmour = true;
    }
    public void TabUpgradeWeapon()
    {
        UpgradeArmour = false;
    }
    public void LoadShop(int index)
    {
        for (int i=0;i<3; i++)
        {
            AmountOwned[i] = playerInventory.GetLootFromInventory(3 * index + i + 1);
            CostValues[i].GetComponent<TextMeshProUGUI>().text = playerInventory.GetLootFromInventory(3 * index + i+1).ToString();
            CostValues[i].GetComponent<TextMeshProUGUI>().text += "/";
            CostValues[i].GetComponent<TextMeshProUGUI>().text += shopInventory[index].cost[3 * index + i].ToString();
            AmountNeeded[i] = shopInventory[index].cost[3 * index + i];
        }
        // This is for checking if the player has a certain armor
        if (ArmourOwned[index] == true)
        {
            for (int i = 0; i < 3; i++)
            {
                if (i % 2 == 0)
                    CostValues[i].GetComponent<TextMeshProUGUI>().text = "";
                else CostValues[i].GetComponent<TextMeshProUGUI>().text = "Crafted";
            }
            //Refrence to Clicked button's Background Image Child
            //Dim the background & Icon Image of the button
            purchaseButtons[index].transform.GetChild(0).GetComponent<Image>().color = Color.gray;
            purchaseButtons[index].transform.GetChild(1).GetComponent<Image>().color = Color.gray;
        }
        WhichArmour = index;
      // for(int i =0; i<shopInventory.Length; i++)
      // {
      //     shopPanels[i].titleTxt.text = shopInventory[i].title;
      //     shopPanels[i].descriptionTxt.text = shopInventory[i].description;
      //
      //     int j = 0;
      //     foreach (Loot loot in playerInventory.Inventory)
      //     {
      //         shopPanels[i].costTxt[j].text = shopInventory[i].cost[j].ToString();
      //     }
      //     
      // }
    }

    public void LoadWeapon(int index)
    {
        int num=9;
        if (EquipmentManager.transform.parent.GetComponentInChildren<WeaponSwap>().transform.GetChild(0).gameObject.activeInHierarchy == false)
        {
            EquipmentManager.transform.parent.GetComponentInChildren<WeaponSwap>().transform.GetChild(0).gameObject.SetActive(true);
            num = 0;
        }
        if (EquipmentManager.transform.parent.GetComponentInChildren<WeaponSwap>().transform.GetChild(1).gameObject.activeInHierarchy == false)
        {
            EquipmentManager.transform.parent.GetComponentInChildren<WeaponSwap>().transform.GetChild(1).gameObject.SetActive(true);
            num = 1;
        }

        WhichWeapon = index;
        int Value = -1;
        // Level holds what gun upgrade level that they are at
        int Level = GetWeapon().GetComponent<Gun>().Level;
        for (int i = 0; i < 3; i++)
        {
            
            for (int j = 0; j < WeaponUpgrades[3 * index+Level].cost.Length; j++)
            {
                if(j>Value && WeaponUpgrades[3 * index+Level].cost[j] > 0)
                {
                    //j will be the index that the current weapon upgrade requires mats from 
                    //TODO get icons in 
                    AmountOwned[i] = playerInventory.GetLootFromInventory(j + 1);
                    CostValues[i].GetComponent<TextMeshProUGUI>().text = playerInventory.GetLootFromInventory(j + 1).ToString();
                    CostValues[i].GetComponent<TextMeshProUGUI>().text += "/";
                    CostValues[i].GetComponent<TextMeshProUGUI>().text += WeaponUpgrades[3 * index + Level].cost[j];
                    AmountNeeded[i]= WeaponUpgrades[3 * index + Level].cost[j];
                  //  Debug.Log(WeaponUpgrades[3 * index + Level].cost[j]+" found at "+j);
                    IndexOfMats[i] = j;
                    Value = j;
                    break;
                }
          
            }  
                    //  CostValues[i].GetComponent<TextMeshProUGUI>().text += shopInventory[index].cost[3 * index + i].ToString();
        }
        // Level 1
        if (Level == 1)
        {
            //Highlight Next Button
            purchaseUpgradeParents[index].transform.GetChild(1).GetComponent<Image>().color = Color.green;
            purchaseUpgradeParents[index].transform.GetChild(1).GetComponent<Button>().onClick.Invoke();
            //Hide the Lock Symbol
            purchaseUpgradeParents[index].transform.GetChild(1).GetChild(1).GetComponent<Image>().enabled = false;

            // grey out previous buttons and make them uninteractible 
            purchaseUpgradeParents[index].transform.GetChild(0).GetComponent<Image>().color = Color.gray;
            purchaseUpgradeParents[index].transform.GetChild(0).GetComponent<Button>().enabled = false;
            CraftButton.SetActive(false);
            WeaponPopupMessage.SetActive(true);
        }
        // Level 2
        if (Level == 2)
        {
            //Highlight Next Button
            purchaseUpgradeParents[index].transform.GetChild(2).GetComponent<Button>().onClick.Invoke();
            purchaseUpgradeParents[index].transform.GetChild(2).GetComponent<Image>().color = Color.green;
            //Hide the Lock Symbol
            purchaseUpgradeParents[index].transform.GetChild(2).GetChild(1).GetComponent<Image>().enabled = false;

            // grey out previous buttons and make them uninteractible 
            purchaseUpgradeParents[index].transform.GetChild(0).GetComponent<Image>().color = Color.gray;
            purchaseUpgradeParents[index].transform.GetChild(1).GetComponent<Image>().color = Color.gray;
            purchaseUpgradeParents[index].transform.GetChild(1).GetComponent<Button>().enabled = false;
            CraftButton.SetActive(false);
            WeaponPopupMessage.SetActive(true);
        }
        // Level 3
        if (Level == 3)
         {
            for (int i = 0; i < 3; i++)
            {
                if (i % 2 == 0)
                    CostValues[i].GetComponent<TextMeshProUGUI>().text = "";
                else CostValues[i].GetComponent<TextMeshProUGUI>().text = "Max";
            }
            //Gray out the background and Icon of the button after all 3 levels have been crafted
            purchaseWeaponButtons[index].transform.GetChild(0).GetComponent<Image>().color = Color.gray;
            purchaseWeaponButtons[index].transform.GetChild(1).GetComponent<Image>().color = Color.gray;

            //Gray out all 3 weapon upgrade since all have been crafted
            purchaseUpgradeParents[index].transform.GetChild(0).GetComponent<Image>().color = Color.gray;
            purchaseUpgradeParents[index].transform.GetChild(1).GetComponent<Image>().color = Color.gray;
            purchaseUpgradeParents[index].transform.GetChild(2).GetComponent<Image>().color = Color.gray;
            //These both disable the crafting button
            //purchaseUpgradeParents[index].transform.GetChild(2).GetComponent<Button>().enabled = false;
            CraftButton.SetActive(false);
            WeaponPopupMessage.SetActive(true);
        }
        EquipmentManager.transform.parent.GetComponentInChildren<WeaponSwap>().transform.GetChild(num).gameObject.SetActive(false);

    }
   
    public void Buy()
    {
        for (int i = 0; i < 3; i++)
            if (AmountOwned[i] < AmountNeeded[i])
                return;   
        
        if (UpgradeArmour == true)
        {
            for (int i = 0; i < 3; i++)
                playerInventory.GetInventory(3 * WhichArmour + i + 1).DecrementLoot(shopInventory[WhichArmour].cost[3 * WhichArmour + i]);
            PurchaseArmour();
            LoadShop(WhichArmour);
        }
        else if (UpgradeArmour == false)
        {
            int num = 9;
            if (EquipmentManager.transform.parent.GetComponentInChildren<WeaponSwap>().transform.GetChild(0).gameObject.activeInHierarchy == false)
            {
                EquipmentManager.transform.parent.GetComponentInChildren<WeaponSwap>().transform.GetChild(0).gameObject.SetActive(true);
                num = 0;
            }
            if (EquipmentManager.transform.parent.GetComponentInChildren<WeaponSwap>().transform.GetChild(1).gameObject.activeInHierarchy == false)
            {
                EquipmentManager.transform.parent.GetComponentInChildren<WeaponSwap>().transform.GetChild(1).gameObject.SetActive(true);
                num = 1;
            }
            int Level = GetWeapon().GetComponent<Gun>().Level;

            for (int i = 0; i < 3; i++)
                playerInventory.GetInventory(IndexOfMats[i] + 1).DecrementLoot(WeaponUpgrades[WhichWeapon+Level].cost[IndexOfMats[i]]);

            EquipmentManager.transform.parent.GetComponentInChildren<WeaponSwap>().transform.GetChild(num).gameObject.SetActive(false);
            PurchaseWeaponUpgrade();
            LoadWeapon(WhichWeapon);
        }
    
    }
    public GameObject GetWeapon()
    {   
        switch (WhichWeapon)
        {
            case 0:
                return EquipmentManager.transform.parent.GetComponentInChildren<WeaponSwap>().transform.GetComponentInChildren<AutomaticRifle>().gameObject;
              
            case 1:
                return EquipmentManager.transform.parent.GetComponentInChildren<WeaponSwap>().transform.GetComponentInChildren<Shotgun>().gameObject;
             
            case 2:
                return EquipmentManager.transform.parent.GetComponentInChildren<WeaponSwap>().transform.GetComponentInChildren<Shotgun>().gameObject;
               
            case 3:
                return EquipmentManager.transform.parent.GetComponentInChildren<WeaponSwap>().transform.GetComponentInChildren<Revolver>().gameObject;
               
            case 4:
                return EquipmentManager.transform.parent.GetComponentInChildren<WeaponSwap>().transform.GetComponentInChildren<SniperRifle>().gameObject;
               
            case 5:
                return EquipmentManager.transform.parent.GetComponentInChildren<WeaponSwap>().transform.GetComponentInChildren<AutomaticRifle>().gameObject;
            
            default: return null;
        }
      
    }
    public void PurchaseWeaponUpgrade()
    {
        int num = 9;
        if (EquipmentManager.transform.parent.GetComponentInChildren<WeaponSwap>().transform.GetChild(0).gameObject.activeInHierarchy == false)
        {
            EquipmentManager.transform.parent.GetComponentInChildren<WeaponSwap>().transform.GetChild(0).gameObject.SetActive(true);
            num = 0;
        }
        if (EquipmentManager.transform.parent.GetComponentInChildren<WeaponSwap>().transform.GetChild(1).gameObject.activeInHierarchy == false)
        {
            EquipmentManager.transform.parent.GetComponentInChildren<WeaponSwap>().transform.GetChild(1).gameObject.SetActive(true);
            num = 1;
        }
        GetWeapon().GetComponent<Gun>().UpgradeWeapon();

        EquipmentManager.transform.parent.GetComponentInChildren<WeaponSwap>().transform.GetChild(num).gameObject.SetActive(false);
    }
    public void PurchaseArmour()
    {
        switch (WhichArmour)
        {
            case 0:
                ArmourOwned[0] = true;
                EquipmentManager.slime_armor.isEquippable=true;
                break;
            case 1:
                ArmourOwned[1] = true;
                EquipmentManager.TickArmor.isEquippable = true;
                break;
            case 2:
                ArmourOwned[2] = true;
                EquipmentManager.ZephyrArmor.isEquippable = true;
                break;
            case 3:
                ArmourOwned[3] = true;
                EquipmentManager.BomberArmor.isEquippable = true;
                break;
        }
        PopupMessage.SetActive(true);
    }
    public void ValidatePurchasable()
    {
        //for every shop item [i]...
        for (int i = 0; i <shopInventory.Length; i++)
        {
            bool buttonWillBeEnabled = false;

            //Check if player has enough of item [j] for shop item [i]
            for(int j =0; j<playerInventory.Inventory.Count; j++)
            {
                buttonWillBeEnabled = true;
                if (playerInventory.Inventory[j].Quantity < shopInventory[i].cost[j])
                {
                    Debug.LogError("Player has Insufficient Funds! " +
                        "Disabling Button at position: " + i + 
                        "Due to a lack of funds at position: " + j);
                    buttonWillBeEnabled = false;                    
                    break;
                }
                
            }
            if(buttonWillBeEnabled)
            purchaseButtons[i].interactable = true;
        }
    }

    //TODO: Link With Ryan's new Weapon Script
    public void DisplayItemStats()
    {

    }


    /// <summary>
    /// Set's all of the players resources to 99 for testing.
    /// </summary>
    public void Cheat()
    {
        foreach(Loot loot in playerInventory.Inventory)
        {
            loot.SetQuantity(99);
        }
        //LoadShop();
       // ValidatePurchasable();
    }


    //[Header("Items")]


}
