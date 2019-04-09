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
        public int Level            { get { return ((ExperiencePoints / 100) + 1); } }

        public Location CurrentLocation { get; set; }

        public List<InventoryItem> Inventory { get; set; }
        public List<PlayerQuest> Quests      { get; set; }

        public Player(int currentHitPoints, int maximumHitpoints, int gold, int experiencePoints) 
            : base(currentHitPoints, maximumHitpoints)
        {
            Gold             = gold;
            ExperiencePoints = experiencePoints;

            Inventory = new List<InventoryItem>();
            Quests    = new List<PlayerQuest>();
        }

        //creates a new default player
        public static Player CreateDefaultPlayer()
        {
            Player player = new Player(10, 10, 20, 0);
            player.Inventory.Add(new InventoryItem(World.ItemByID(World.ITEM_ID_RUSTY_SWORD), 1));
            //player.CurrentLocation = World.LocationByID(World.LOCATION_ID_HOME);

            return player;
        }

        //checks for required items
        public bool HasRequiredItemToEnterThisLocation(Location location)
        {
            if(location.ItemRequiredToEnter == null)
            {
                //no item required to enter
                return true;
            }

            return Inventory.Exists(ii => ii.Details.ID == location.ItemRequiredToEnter.ID);
        }

        //checks if player has this quest
        public bool HasThisQuest(Quest quest)
        {
            return Quests.Exists(pq => pq.Details.ID == quest.ID);
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
            foreach (QuestCompletionItem qci in quest.QuestCompletionItems)
            {
                if (!Inventory.Exists(ii => ii.Details.ID == qci.Details.ID && ii.Quantity >= qci.Quantity))
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
            foreach (QuestCompletionItem qci in quest.QuestCompletionItems)
            {
                InventoryItem item = Inventory.SingleOrDefault(ii => ii.Details.ID == qci.Details.ID);

                if (item != null)
                {
                    // Subtract the quantity from the player's inventory that was needed to complete the quest
                    item.Quantity -= qci.Quantity;
                }
            }
        }

        //adds items to inventory
        public void AddItemToInventory(Item itemToAdd)
        {
            InventoryItem item = Inventory.SingleOrDefault(ii => ii.Details.ID == itemToAdd.ID);

            if (item == null)
            {
                // They didn't have the item, so add it to their inventory, with a quantity of 1
                Inventory.Add(new InventoryItem(itemToAdd, 1));
            }
            else
            {
                // They have the item in their inventory, so increase the quantity by one
                item.Quantity++;
            }
        }

        //marks finished quests as completed
        public void MarkQuestCompleted(Quest quest)
        {
            // Find the quest in the player's quest list
            PlayerQuest playerQuest = Quests.SingleOrDefault(pq => pq.Details.ID == quest.ID);

            if (playerQuest != null)
            {
                playerQuest.IsCompleted = true;
            }
        }
    }
}
