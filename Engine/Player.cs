﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml;

namespace Engine
{
    public class Player : LivingCreature
    {
        private int _gold;
        private int _experiencePoints;

        private Location _currentLocation;
        private Armor _armor;

        public event EventHandler<MessageEventArgs> OnMessage;

        public Weapon CurrentWeapon { get; set; }
        private Monster CurrentMonster { get; set; }

        public BindingList<InventoryItem> Inventory { get; set; }
        public BindingList<PlayerQuest> Quests { get; set; }

        public int Gold
        {
            get { return _gold; }
            set
            {
                _gold = value;
                OnPropertyChanged("Gold");
            }
        }
        public int ExperiencePoints
        {
            get { return _experiencePoints; }
            private set
            {
                _experiencePoints = value;
                OnPropertyChanged("ExperiencePoints");
                OnPropertyChanged("Level");
            }
        }

        public int Level
        {
            get { return ((ExperiencePoints / 100) + 1); }
        }

        public Location CurrentLocation
        {
            get { return _currentLocation; }
            set
            {
                _currentLocation = value;
                OnPropertyChanged("CurrentLocation");
            }
        }

        public List<Weapon> Weapons
        {
            get { return Inventory.Where(x => x.Details is Weapon).Select(x => x.Details as Weapon).ToList(); }
        }

        public List<HealingPotion> Potions
        {
            get { return Inventory.Where(x => x.Details is HealingPotion).Select(x => x.Details as HealingPotion).ToList(); }
        }

        /////////////////Player Stats////////////////////////////
        //sets the stats parameters for the player
        private Player(int currentHitPoints, int maximumHitpoints, int gold, int experiencePoints)
            : base(currentHitPoints, maximumHitpoints)
        {
            Gold = gold;
            ExperiencePoints = experiencePoints;

            Inventory = new BindingList<InventoryItem>();
            Quests = new BindingList<PlayerQuest>();
        }

        //creates a new default player
        public static Player CreateDefaultPlayer()
        {
            Player player = new Player(10, 10, 20, 0);
            player.Inventory.Add(new InventoryItem(World.ItemByID(World.ITEM_ID_RUSTY_SWORD), 1));
            player.CurrentLocation = World.LocationByID(World.LOCATION_ID_HOME);

            return player;
        }

        private void AddExperiencePoints(int experiencePointsToAdd)
        {
            ExperiencePoints += experiencePointsToAdd;
            MaximumHitPoints = (Level * 10);
        }

        //creates player from previous saved data
        public static Player CreatePlayerFromXmlString(string xmlPlayerData)
        {
            try
            {
                XmlDocument playerData = new XmlDocument();

                playerData.LoadXml(xmlPlayerData);

                int currentHitPoints = Convert.ToInt32(playerData.SelectSingleNode("/Player/Stats/CurrentHitPoints").InnerText);
                int maximumHitPoints = Convert.ToInt32(playerData.SelectSingleNode("/Player/Stats/MaximumHitPoints").InnerText);
                int gold = Convert.ToInt32(playerData.SelectSingleNode("/Player/Stats/Gold").InnerText);
                int experiencePoints = Convert.ToInt32(playerData.SelectSingleNode("/Player/Stats/ExperiencePoints").InnerText);

                Player player = new Player(currentHitPoints, maximumHitPoints, gold, experiencePoints);

                int currentLocationID = Convert.ToInt32(playerData.SelectSingleNode("/Player/Stats/CurrentLocation").InnerText);
                player.CurrentLocation = World.LocationByID(currentLocationID);

                if (playerData.SelectSingleNode("/Player/Stats/CurrentWeapon") != null)
                {
                    int currentWeaponID = Convert.ToInt32(playerData.SelectSingleNode("/Player/Stats/CurrentWeapon").InnerText);
                    player.CurrentWeapon = (Weapon)World.ItemByID(currentWeaponID);
                }

                foreach (XmlNode node in playerData.SelectNodes("/Player/InventoryItems/InventoryItem"))
                {
                    int id = Convert.ToInt32(node.Attributes["ID"].Value);
                    int quantity = Convert.ToInt32(node.Attributes["Quantity"].Value);

                    for (int i = 0; i < quantity; i++)
                    {
                        player.AddItemToInventory(World.ItemByID(id));
                    }
                }

                foreach (XmlNode node in playerData.SelectNodes("/Player/PlayerQuests/PlayerQuest"))
                {
                    int id = Convert.ToInt32(node.Attributes["ID"].Value);
                    bool isCompleted = Convert.ToBoolean(node.Attributes["IsCompleted"].Value);

                    PlayerQuest playerQuest = new PlayerQuest(World.QuestByID(id));
                    playerQuest.IsCompleted = isCompleted;

                    player.Quests.Add(playerQuest);
                }

                return player;
            }
            catch
            {
                // If there was an error with the XML data, return a default player object
                return Player.CreateDefaultPlayer();
            }
        }

        //saves player data to xml file
        public string ToXmlString()
        {
            XmlDocument playerData = new XmlDocument();

            // Create the top-level XML node
            XmlNode player = playerData.CreateElement("Player");
            playerData.AppendChild(player);

            // Create the "Stats" child node to hold the other player statistics nodes
            XmlNode stats = playerData.CreateElement("Stats");
            player.AppendChild(stats);

            // Create the child nodes for the "Stats" node
            CreateNewChildXmlNode(playerData, stats, "CurrentHitPoints", CurrentHitPoints);
            CreateNewChildXmlNode(playerData, stats, "MaximumHitPoints", MaximumHitPoints);
            CreateNewChildXmlNode(playerData, stats, "Gold", Gold);
            CreateNewChildXmlNode(playerData, stats, "ExperiencePoints", ExperiencePoints);
            CreateNewChildXmlNode(playerData, stats, "CurrentLocation", CurrentLocation.ID);

            if (CurrentWeapon != null)
            {
                CreateNewChildXmlNode(playerData, stats, "CurrentWeapon", CurrentWeapon.ID);
            }

            // Create the "InventoryItems" child node to hold each InventoryItem node
            XmlNode inventoryItems = playerData.CreateElement("InventoryItems");
            player.AppendChild(inventoryItems);

            // Create an "InventoryItem" node for each item in the player's inventory
            foreach (InventoryItem item in Inventory)
            {
                XmlNode inventoryItem = playerData.CreateElement("InventoryItem");

                AddXmlAttributeToNode(playerData, inventoryItem, "ID", item.Details.ID);
                AddXmlAttributeToNode(playerData, inventoryItem, "Quantity", item.Quantity);

                inventoryItems.AppendChild(inventoryItem);
            }

            // Create the "PlayerQuests" child node to hold each PlayerQuest node
            XmlNode playerQuests = playerData.CreateElement("PlayerQuests");
            player.AppendChild(playerQuests);

            // Create a "PlayerQuest" node for each quest the player has acquired
            foreach (PlayerQuest quest in Quests)
            {
                XmlNode playerQuest = playerData.CreateElement("PlayerQuest");

                AddXmlAttributeToNode(playerData, playerQuest, "ID", quest.Details.ID);
                AddXmlAttributeToNode(playerData, playerQuest, "IsCompleted", quest.IsCompleted);

                playerQuests.AppendChild(playerQuest);
            }

            return playerData.InnerXml; // The XML document, as a string, so we can save the data to disk
        }
        private void CreateNewChildXmlNode(XmlDocument document, XmlNode parentNode, string elementName, object value)
        {
            XmlNode node = document.CreateElement(elementName);
            node.AppendChild(document.CreateTextNode(value.ToString()));
            parentNode.AppendChild(node);
        }
        private void AddXmlAttributeToNode(XmlDocument document, XmlNode node, string attributeName, object value)
        {
            XmlAttribute attribute = document.CreateAttribute(attributeName);
            attribute.Value = value.ToString();
            node.Attributes.Append(attribute);
        }

        /////////////////////////Handles player movement/////////////////////////////
        public void MoveTo(Location location)
        {
            //Does the location have any required items
            if (PlayerDoesNotHaveTheRequiredItemToEnter(location))
            {
                RaiseMessage("You must have a " + location.ItemRequiredToEnter.Name + " to enter this location.");
                RaiseMessage("");
                return;
            }

            // Update the player's current location
            CurrentLocation = location;

            //Heals the player upon new area entry
            CompletelyHeal();

            // Does the location have a quest?
            if (location.HasAQuest)
            {
                if (PlayerDoesNotHaveThisQuest(location.QuestAvailableHere))
                {
                    GiveQuestToPlayer(location.QuestAvailableHere);
                }
                else
                {
                    if (PlayerHasNotCompleted(location.QuestAvailableHere) &&
                      PlayerHasAllQuestCompletionItemsFor(location.QuestAvailableHere))
                    {
                        GivePlayerQuestRewards(location.QuestAvailableHere);
                    }
                }
            }
            SetTheCurrentMonsterForTheCurrentLocation(location);
        }

        //takes player home on death
        private void MoveHome()
        {
            MoveTo(World.LocationByID(World.LOCATION_ID_HOME));
        }

        //moves the player north
        public void MoveNorth()
        {
            if (CurrentLocation.LocationToNorth != null)
            {
                MoveTo(CurrentLocation.LocationToNorth);
            }
        }

        //moves the player south
        public void MoveSouth()
        {
            if (CurrentLocation.LocationToSouth != null)
            {
                MoveTo(CurrentLocation.LocationToSouth);
            }
        }

        //moves the player east
        public void MoveEast()
        {
            if (CurrentLocation.LocationToEast != null)
            {
                MoveTo(CurrentLocation.LocationToEast);
            }
        }

        //moves the player west
        public void MoveWest()
        {
            if (CurrentLocation.LocationToWest != null)
            {
                MoveTo(CurrentLocation.LocationToWest);
            }
        }

        ///////////////////////////Handles Monsters/////////////////////////////
        private void LootTheCurrentMonster()
        {
            RaiseMessage("");
            RaiseMessage("You defeated the " + CurrentMonster.Name);
            RaiseMessage("You receive " + CurrentMonster.RewardExperiencePoints + " experience points");
            RaiseMessage("You receive " + CurrentMonster.RewardGold + " gold");

            AddExperiencePoints(CurrentMonster.RewardExperiencePoints);
            Gold += CurrentMonster.RewardGold;

            // Give monster's loot items to the player
            foreach (InventoryItem inventoryItem in CurrentMonster.LootItems)
            {
                AddItemToInventory(inventoryItem.Details);

                RaiseMessage(string.Format("You loot {0} {1}", inventoryItem.Quantity, inventoryItem.Description));
            }

            RaiseMessage("");
        }

        //adds monster to location
        private void SetTheCurrentMonsterForTheCurrentLocation(Location location)
        {
            // Populate the current monster with this location's monster (or null, if there is no monster here)
            CurrentMonster = location.NewInstanceOfMonsterLivingHere();

            if (CurrentMonster != null)
            {
                RaiseMessage("You see a " + CurrentMonster.Name);
            }
        }

        private void LetTheMonsterAttack()
        {
            int damageToPlayer = RandomNumberGenerator.NumberBetween(0, CurrentMonster.MaximumDamage) - RandomNumberGenerator.NumberBetween(0, _armor.MaximumDefense);

            RaiseMessage("The " + CurrentMonster.Name + " did " + damageToPlayer + " points of damage.");

            CurrentHitPoints -= damageToPlayer;

            if (IsDead)
            {
                RaiseMessage("The " + CurrentMonster.Name + " killed you.");

                MoveHome();
            }
        }

        ///////////////////////Handles Quests////////////////////////////
        private void GiveQuestToPlayer(Quest quest)
        {
            RaiseMessage("You receive the " + quest.Name + " quest.");
            RaiseMessage(quest.Description);
            RaiseMessage("To complete it, return with:");

            foreach (QuestCompletionItem qci in quest.QuestCompletionItems)
            {
                RaiseMessage(string.Format("{0} {1}", qci.Quantity,
                    qci.Quantity == 1 ? qci.Details.Name : qci.Details.NamePlural));
            }

            RaiseMessage("");

            Quests.Add(new PlayerQuest(quest));
        }

        //player does not have this quest
        private bool PlayerDoesNotHaveThisQuest(Quest quest)
        {
            return Quests.All(pq => pq.Details.ID != quest.ID);
        }

        //marks finished quests as completed
        private void MarkPlayerQuestCompleted(Quest quest)
        {
            PlayerQuest playerQuest = Quests.SingleOrDefault(pq => pq.Details.ID == quest.ID);

            if (playerQuest != null)
            {
                playerQuest.IsCompleted = true;
            }
        }

        private bool PlayerHasNotCompleted(Quest quest)
        {
            return Quests.Any(pq => pq.Details.ID == quest.ID && !pq.IsCompleted);
        }

        private void GivePlayerQuestRewards(Quest quest)
        {
            RaiseMessage("");
            RaiseMessage("You complete the '" + quest.Name + "' quest.");
            RaiseMessage("You receive: ");
            RaiseMessage(quest.RewardExperiencePoints + " experience points");
            RaiseMessage(quest.RewardGold + " gold");
            RaiseMessage(quest.RewardItem.Name, true);

            AddExperiencePoints(quest.RewardExperiencePoints);
            Gold += quest.RewardGold;

            RemoveQuestCompletionItems(quest);
            AddItemToInventory(quest.RewardItem);

            MarkPlayerQuestCompleted(quest);
        }

        //checks player inventory for all required quest items
        private bool PlayerHasAllQuestCompletionItemsFor(Quest quest)
        {
            // See if the player has all the items needed to complete the quest here
            foreach (QuestCompletionItem qci in quest.QuestCompletionItems)
            {
                // Check each item in the player's inventory, to see if they have it, and enough of it
                if (!Inventory.Any(ii => ii.Details.ID == qci.Details.ID && ii.Quantity >= qci.Quantity))
                {
                    return false;
                }
            }

            // If we got here, then the player must have all the required items, and enough of them, to complete the quest.
            return true;
        }

        //removes required quest items in inventory
        public void RemoveQuestCompletionItems(Quest quest)
        {
            foreach (QuestCompletionItem qci in quest.QuestCompletionItems)
            {
                // Subtract the quantity from the player's inventory that was needed to complete the quest
                InventoryItem item = Inventory.SingleOrDefault(ii => ii.Details.ID == qci.Details.ID);

                if (item != null)
                {
                    RemoveItemFromInventory(item.Details, qci.Quantity);
                }
            }
        }

        ///////////////////////Handles Items///////////////////////////////
        //checks for required items
        public bool HasRequiredItemToEnterThisLocation(Location location)
        {
            if (location.DoesNotHaveAnItemRequiredToEnter)
            {
                //no item required to enter
                return true;
            }

            return Inventory.Any(ii => ii.Details.ID == location.ItemRequiredToEnter.ID);
        }

        //player doesnt have the required items to enter
        private bool PlayerDoesNotHaveTheRequiredItemToEnter(Location location)
        {
            return !HasRequiredItemToEnterThisLocation(location);
        }

        //adds items to inventory
        public void AddItemToInventory(Item itemToAdd, int quantity = 1)
        {
            InventoryItem existingItemInInventory = Inventory.SingleOrDefault(ii => ii.Details.ID == itemToAdd.ID);

            if (existingItemInInventory == null)
            {
                Inventory.Add(new InventoryItem(itemToAdd, quantity));
            }
            else
            {
                existingItemInInventory.Quantity += quantity;
            }

            RaiseInventoryChangedEvent(itemToAdd);
        }

        //removes items from inventory
        public void RemoveItemFromInventory(Item itemToRemove, int quantity = 1)
        {
            InventoryItem item = Inventory.SingleOrDefault(ii => ii.Details.ID == itemToRemove.ID && ii.Quantity >= quantity);

            if (item != null)
            {
                item.Quantity -= quantity;

                if (item.Quantity == 0)
                {
                    Inventory.Remove(item);
                }

                RaiseInventoryChangedEvent(itemToRemove);
            }
        }

        ////////////////////////handles weapons/////////////////////////
        public void UseWeapon(Weapon weapon)
        {
            // Determine the amount of damage to do to the monster
            int damage = RandomNumberGenerator.NumberBetween(weapon.MinimumDamage, weapon.MaximumDamage);

            if (damage == 0)
            {
                RaiseMessage("You missed the " + CurrentMonster.Name);
            }
            else
            {
                CurrentMonster.CurrentHitPoints -= damage;
                RaiseMessage("You hit the " + CurrentMonster.Name + " for " + damage + " points.");
            }

            // Check if the monster is dead
            if (CurrentMonster.IsDead)
            {
                LootTheCurrentMonster();

                // "Move" to the current location, to refresh the current monster
                MoveTo(CurrentLocation);
            }
            else
            {
                LetTheMonsterAttack();
            }
        }

        ////////////////////////handles potions//////////////////////////
        public void UsePotion(HealingPotion potion)
        {
            RaiseMessage("You drink a " + potion.Name);

            HealPlayer(potion.AmountToHeal);

            RemoveItemFromInventory(potion);

            // The player used their turn to drink the potion, so let the monster attack now
            LetTheMonsterAttack();
        }

        ///////////////////////handles effects//////////////////////////
        //fully heals the player
        private void CompletelyHeal()
        {
            CurrentHitPoints = MaximumHitPoints;
        }

        //partially heals the player
        private void HealPlayer(int hitPointsToHeal)
        {
            CurrentHitPoints = Math.Min(CurrentHitPoints + hitPointsToHeal, MaximumHitPoints);
        }

        ///////////////////////handles updates//////////////////////////////
        //tells UI about changes
        private void RaiseInventoryChangedEvent(Item item)
        {
            if (item is Weapon)
            {
                OnPropertyChanged("Weapons");
            }

            if (item is HealingPotion)
            {
                OnPropertyChanged("Potions");
            }
        }
        //makes messages happen
        private void RaiseMessage(string message, bool addExtraNewLine = false)
        {
            OnMessage?.Invoke(this, new MessageEventArgs(message, addExtraNewLine));
        }
    }
}