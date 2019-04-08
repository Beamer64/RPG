using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Player : LivingCreature
    {
        public int Gold             { get; set; }
        public int ExperiencePoints { get; set; }
        public int Level            { get; set; }

        public Location CurrentLocation { get; set; }

        public List<InventoryItem> Inventory { get; set; }
        public List<PlayerQuest> Quests      { get; set; }

        public Player(int currentHitPoints, int maximumHitpoints, int gold, int experiencePoints, int level) 
            : base(currentHitPoints, maximumHitpoints)
        {
            Gold             = gold;
            ExperiencePoints = experiencePoints;
            Level            = level;

            Inventory = new List<InventoryItem>();
            Quests    = new List<PlayerQuest>();
        }

        //checks for required items
        public bool HasRequiredItemToEnterThisLocation(Location location)
        {
            if(location.ItemRequiredToEnter == null)
            {
                //no item required to enter
                return true;
            }

            //checks for required item in inventory
            foreach(InventoryItem ii in Inventory)
            {
                if(ii.Details.ID == location.ItemRequiredToEnter.ID)
                {
                    //Has required item
                    return true;
                }
            }

            //doesnt have required item
            return false;
        }

        //checks if player has this quest
        public bool HasThisQuest(Quest quest)
        {
            foreach(PlayerQuest playerQuest in Quests)
            {
                if(playerQuest.Details.ID == quest.ID)
                {
                    //if player has quest then return true
                    return true;
                }
            }
            
            //if player doesn't have quest return false
            return false;
        }

        //checks if quest is completed
        public bool CompletedThisQuest(Quest quest)
        {
            foreach(PlayerQuest playerQuest in Quests)
            {
                if(playerQuest.Details.ID == quest.ID)
                {
                    //has completed quest
                    return playerQuest.IsCompleted;
                }
            }

            //has not completed quest
            return false;
        }

        //checks player inventory for all required quest items
        public bool HasAllQuestCompletionItems(Quest quest)
        {
            foreach(QuestCompletionItem qci in quest.QuestCompletionItems)
            {
                bool foundItemsInPlayerInventory = false;

                //checks for quest items and correct amounts
                foreach(InventoryItem ii in Inventory)
                {
                    if(ii.Details.ID == qci.Details.ID)
                    {
                        //player has the item
                        foundItemsInPlayerInventory = true;

                        //player doesn't have enough of the item
                        if(ii.Quantity < qci.Quantity)
                        {
                            return false;
                        }
                    }
                }

                //player has no quest items
                if (!foundItemsInPlayerInventory)
                {
                    return false;
                }
            }

            //at this point, player must have every item with correct quantities
            return true;
        }

        //removes required quest items in inventory
        public void RemoveQuestCompletionItems(Quest quest)
        {
            foreach(QuestCompletionItem qci in quest.QuestCompletionItems)
            {
                foreach(InventoryItem ii in Inventory)
                {
                    if(ii.Details.ID == qci.Details.ID)
                    {
                        //subtracts quantity from inventory
                        ii.Quantity -= qci.Quantity;
                        break;
                    }
                }
            }
        }

        //adds items to inventory
        public void AddItemToInventory(Item itemToAdd)
        {
            foreach(InventoryItem ii in Inventory)
            {
                if(ii.Details.ID == itemToAdd.ID)
                {
                    //if they have the item in inventory already, increase the amount by 1
                    ii.Quantity++;

                    return;
                }
            }

            //player didnt have item. add one of this item to inventory
            Inventory.Add(new InventoryItem(itemToAdd, 1));
        }

        //marks finished quests as completed
        public void MarkQuestCompleted(Quest quest)
        {
            //checks for quest in quest list
            foreach(PlayerQuest pq in Quests)
            {
                if(pq.Details.ID == quest.ID)
                {
                    //marks quest as completed
                    pq.IsCompleted = true;

                    return;
                }
            }
        }
        
    }
}
