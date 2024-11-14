using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//position,rotation,inventaire (List<Item> voir playerInventory)

//Milestone 3
//carnet de bord aucune idée
//hotbar de build
//main principale
//équipement sur le player

public class PlayerContainer : SaveContainer
{
    //List convertie des enfants du gameobject (container)
    [SerializeField] List<PlayerSaveable> players;
    [SerializeField] GameObject player;
    [SerializeField] SODatabase soDataBase;

    protected override void Convert()
    {
        players.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject go = transform.GetChild(i).gameObject;
            Player playerComponent = go.GetComponent<Player>();
            PlayerSaveable playerSaveable = new PlayerSaveable();

            playerSaveable.name = go.name;
            playerSaveable.inventory = new List<ItemSaveable>();
            playerSaveable.handInventory = new List<ItemSaveable>();
            for (int j = 0; j < playerComponent.GetInventory().Count; j++)
            {
                playerSaveable.inventory.Add(default(ItemSaveable));
                playerSaveable.inventory[j] = Serialize.ConvertSOItem(playerComponent.GetInventory()[j]);
            }
            int test = playerComponent.GetHandSlotsItems().Count;
            for (int j = 0; j < playerComponent.GetHandSlotsItems().Count; j++)
            {
                playerSaveable.handInventory.Add(default(ItemSaveable));
                playerSaveable.handInventory[j] = Serialize.ConvertSOItem(playerComponent.GetHandSlotsItems()[j]);
            }


            dictionaryPrefabs.TryGetValue(playerSaveable.name, out playerSaveable.key);
            playerSaveable.transform = Serialize.ConvertSerialized(go.transform);

            players.Add(playerSaveable);
        }

        SaveSystem.Instance.ClassContainer.playersList = players;
    }

    protected override void Load()
    {
        players = SaveSystem.Instance.ClassContainer.playersList;

        if (players.Count != 0)
        {
            Player playerComponent = player.GetComponent<Player>();

            player.name = players[0].name;
            List<Item> tempInventory = new List<Item>();
            for (int i = 0; i < playerComponent.GetInventory().Count; i++)
            {
                Item tempItem = new Item();

                if (players[0].inventory[i].id >= 0)
                {
                    tempItem.itemType = soDataBase.AllItems.Where(item => item.id == players[0].inventory[i].id).First(item => item);
                    tempItem.stacks = players[0].inventory[i].number;
                }
                else
                {
                    tempItem = null;
                }

                tempInventory.Add(tempItem);
            }
            playerComponent.SetInventory(tempInventory);

            List<Item> tempHandInventory = new List<Item>();
            for (int i = 0; i < playerComponent.GetHandSlotsItems().Count; i++)
            {
                Item tempItem = new Item();

                if (players[0].handInventory[i].id >= 0)
                {
                    tempItem.itemType = soDataBase.AllItems.Where(item => item.id == players[0].handInventory[i].id).First(item => item);
                    tempItem.stacks = players[0].handInventory[i].number;
                }
                else
                {
                    tempItem = null;
                }

                tempHandInventory.Add(tempItem);
            }
            playerComponent.SetHandSlotsItems(tempHandInventory);

            player.transform.position = Serialize.UnConvert(players[0].transform, player.transform).position;
            player.transform.rotation = Serialize.UnConvert(players[0].transform, player.transform).rotation;
        }
    }
}

[System.Serializable]
public class PlayerSaveable
{
    public int key;
    public string name;
    public List<ItemSaveable> inventory;
    public List<ItemSaveable> handInventory;

    public TransformSerialized transform;
}

[System.Serializable]
public class ItemSaveable
{
    public int id;
    public int number;
}