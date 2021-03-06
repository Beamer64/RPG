﻿using Engine;
using RPG;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using Console = Colorful.Console;

namespace RPG_Console
{
    public class ConsoleProgram
    {
        public const string PLAYER_DATA_FILE_NAME = "PlayerData.xml";

        private static Player _player;

        private static RPGForm rpgForm = new RPGForm();

        public static bool running = true;

        private static void Main(string[] args)
        {

            // Load the player
            LoadGameData();

            Console.WriteLine("Type 'Help' to see a list of commands");

            DisplayCurrentLocation();

            // Connect player events to functions that will display in the UI
            _player.PropertyChanged += Player_OnPropertyChanged;
            _player.OnMessage += Player_OnMessage;

            // Infinite loop, until the user types "exit"
            while (running == true)
            {
                SaveGameData();

                Console.Write(">");
                string userInput = Console.ReadLine();

                // If they typed a blank line, loop back and wait for input again
                if (string.IsNullOrWhiteSpace(userInput))
                {
                    continue;
                }

                // Convert to lower-case, to make comparisons easier
                string cleanedInput = userInput.ToLower();

                // Save the current game data, and break out of the "while(true)" loop
                if (cleanedInput == "exit")
                {
                    SaveGameData();
                    break;
                }

                // If the user typed something, try to determine what to do
                ParseInput(cleanedInput);
            }
        }

        private static void Player_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentLocation")
            {
                DisplayCurrentLocation();
            }
        }

        private static void Player_OnMessage(object sender, MessageEventArgs e)
        {
            Console.WriteLine(e.Message);

            if (e.AddExtraNewLine)
            {
                Console.WriteLine("");
            }
        }

        private static void ParseInput(string input)
        {
            //easter egg
            if (input.Contains("weast"))
            {
                //colors the word "mayonaise" white and leaves the rest pink
                //thats totally all it does.
                string mayo = "Is {0} an instrument?";
                string[] Mayonnaise = new string[]
                {
                "mayonnaise",
                };

                Console.Clear();
                Console.WriteLineFormatted(mayo, Color.White, Color.PaleVioletRed, Mayonnaise);

                Console.WriteLine("───────────────▄████████▄────────", Color.PaleVioletRed);
                Console.WriteLine("──────────────██▒▒▒▒▒▒▒▒██───────", Color.PaleVioletRed);
                Console.WriteLine("─────────────██▒▒▒▒▒▒▒▒▒██───────", Color.PaleVioletRed);
                Console.WriteLine("────────────██▒▒▒▒▒▒▒▒▒▒██───────", Color.PaleVioletRed);
                Console.WriteLine("───────────██▒▒▒▒▒▒▒▒▒██▀────────", Color.PaleVioletRed);
                Console.WriteLine("──────────██▒▒▒▒▒▒▒▒▒▒██─────────", Color.PaleVioletRed);
                Console.WriteLine("─────────██▒▒▒▒▒▒▒▒▒▒▒██─────────", Color.PaleVioletRed);
                Console.WriteLine("────────██▒████▒████▒▒██─────────", Color.PaleVioletRed);
                Console.WriteLine("────────██▒▒▒▒▒▒▒▒▒▒▒▒██─────────", Color.PaleVioletRed);
                Console.WriteLine("────────██▒────▒▒────▒██─────────", Color.PaleVioletRed);
                Console.WriteLine("────────██▒██──▒▒██──▒██─────────", Color.PaleVioletRed);
                Console.WriteLine("────────██▒────▒▒────▒██─────────", Color.PaleVioletRed);
                Console.WriteLine("────────██▒▒▒▒▒▒▒▒▒▒▒▒██─────────", Color.PaleVioletRed);
                Console.WriteLine("───────██▒▒█▀▀▀▀▀▀▀█▒▒▒▒██───────", Color.PaleVioletRed);
                Console.WriteLine("─────██▒▒▒▒▒█▄▄▄▄▄█▒▒▒▒▒▒▒██─────", Color.PaleVioletRed);
                Console.WriteLine("───██▒▒██▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██▒▒██───", Color.PaleVioletRed);
                Console.WriteLine("─██▒▒▒▒██▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██▒▒▒▒██─", Color.PaleVioletRed);
                Console.WriteLine("█▒▒▒▒██▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██▒▒▒▒█", Color.PaleVioletRed);
                Console.WriteLine("█▒▒▒▒██▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██▒▒▒▒█", Color.PaleVioletRed);
                Console.WriteLine("█▒▒████▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒████▒▒█", Color.PaleVioletRed);
                Console.WriteLine("▀████▒▒▒▒▒▒▒▒▒▓▓▓▓▒▒▒▒▒▒▒▒▒▒████▀", Color.PaleVioletRed);
                Console.WriteLine("──█▌▌▌▌▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▌▌▌███──", Color.PaleGreen);
                Console.WriteLine("───█▌▌▌▌▌▌▌▌▌▌▌▌▌▌▌▌▌▌▌▌▌▌▌▌█────", Color.PaleGreen);
                Console.WriteLine("───█▌▌▌▌▌▌▌▌▌▌▌▌▌▌▌▌▌▌▌▌▌▌▌▌█────", Color.PaleGreen);
                Console.WriteLine("────▀█▌▌▌▌▌▌▌▌▌▌▌▌▌▌▌▌▌▌▌██▀─────", Color.PaleGreen);
                Console.WriteLine("─────█▌▌▌▌▌▌████████▌▌▌▌▌██──────", Color.PaleGreen);
                Console.WriteLine("──────██▒▒██────────██▒▒██───────", Color.PaleVioletRed);
                Console.WriteLine("──────▀████▀────────▀████▀───────", Color.PaleVioletRed);
                Console.ResetColor();
            }

            else if (input.Contains("help") || input == "?")
            {
                DisplayHelpText();
            }

            else if (input.Contains("gui"))
            {
                SaveGameData();
                running = false;
                
                rpgForm.Show();
            }

            else if (input == "stats")
            {
                DisplayPlayerStats();
            }

            else if (input.Contains("look"))
            {
                DisplayCurrentLocation();
            }

            else if (input.Contains("north"))
            {
                if (_player.CurrentLocation.LocationToNorth == null)
                {
                    Console.WriteLine("You cannot move North");
                    Console.WriteLine("");
                }
                else
                {
                    _player.MoveNorth();
                }
            }

            else if (input.Contains("east"))
            {
                if (_player.CurrentLocation.LocationToEast == null)
                {
                    Console.WriteLine("You cannot move East");
                    Console.WriteLine("");
                }
                else
                {
                    _player.MoveEast();
                }
            }

            else if (input.Contains("south"))
            {
                if (_player.CurrentLocation.LocationToSouth == null)
                {
                    Console.WriteLine("You cannot move South");
                    Console.WriteLine("");
                }
                else
                {
                    _player.MoveSouth();
                }
            }

            else if (input.Contains("west"))
            {
                if (_player.CurrentLocation.LocationToWest == null)
                {
                    Console.WriteLine("You cannot move West");
                    Console.WriteLine("");
                }
                else
                {
                    _player.MoveWest();
                }
            }

            else if (input.Contains("inventory"))
            {
                foreach (InventoryItem inventoryItem in _player.Inventory)
                {
                    Console.WriteLine("{0}: {1}", inventoryItem.Description, inventoryItem.Quantity);
                }
            }

            else if (input.Contains("quests"))
            {
                if (_player.Quests.Count == 0)
                {
                    Console.WriteLine("You do not have any quests");
                }
                else
                {
                    foreach (PlayerQuest playerQuest in _player.Quests)
                    {
                        Console.WriteLine("{0}: {1}", playerQuest.Name,
                            playerQuest.IsCompleted ? "Completed" : "Incomplete");
                    }
                }
            }

            else if (input.Contains("attack"))
            {
                AttackMonster();
            }

            else if (input.StartsWith("equip "))
            {
                EquipWeapon(input);
            }

            else if (input.StartsWith("drink "))
            {
                DrinkPotion(input);
            }

            else if (input.Contains("trade"))
            {
                ShowTradeInventories();
            }

            else if (input.StartsWith("buy "))
            {
                BuyItem(input);

                //brings trade info back up
                ShowTradeInventories();
            }

            else if (input.StartsWith("sell "))
            {
                SellItem(input);

                //brings trade info back up
                ShowTradeInventories();
            }

            else
            {
                Console.WriteLine("I do not understand");
                Console.WriteLine("Type 'Help' to see a list of available commands");
            }
        }

        private static void DisplayHelpText()
        {
            Console.WriteLine("");
            Console.WriteLine("Available commands");
            Console.WriteLine("====================================");
            Console.WriteLine("Stats - Display player information");
            Console.WriteLine("Look - Get the description of your location");
            Console.WriteLine("Inventory - Display your inventory");
            Console.WriteLine("Quests - Display your quests");
            Console.WriteLine("Attack - Fight the monster");
            Console.WriteLine("Equip <weapon name> - Set your current weapon");
            Console.WriteLine("Drink <potion name> - Drink a potion");
            Console.WriteLine("Trade - display your inventory and vendor's inventory");
            Console.WriteLine("Buy <item name> - Buy an item from a vendor");
            Console.WriteLine("Sell <item name> - Sell an item to a vendor");
            Console.WriteLine("North - Move North");
            Console.WriteLine("South - Move South");
            Console.WriteLine("East - Move East");
            Console.WriteLine("West - Move West");
            Console.WriteLine("Menu - Save and return to main menu");
            Console.WriteLine("Exit - Save the game and exit");
        }

        private static void DisplayPlayerStats()
        {
            Console.WriteLine("");
            Console.WriteLine("Current hit points: {0}", _player.CurrentHitPoints);
            Console.WriteLine("Maximum hit points: {0}", _player.MaximumHitPoints);
            Console.WriteLine("Experience Points: {0}", _player.ExperiencePoints);
            Console.WriteLine("Level: {0}", _player.Level);
            Console.WriteLine("Gold: {0}", _player.Gold);
        }

        //shows player and vendor inventories
        private static void ShowTradeInventories()
        {
            Console.WriteLine("");

            if (_player.CurrentLocation.VendorWorkingHere != null)
            {

                Console.WriteLine("PLAYER INVENTORY");
                Console.Write("================ ");
                Console.Write("Gold: ", Color.Gold);
                Console.WriteLine(_player.Gold, Color.Gold);

                if (_player.Inventory.Count(x => x.Price != World.UNSELLABLE_ITEM_PRICE) == 0)
                {
                    Console.WriteLine("You do not have any inventory items");
                }
                else
                {
                    foreach (
                        InventoryItem inventoryItem in _player.Inventory.Where(x => x.Price != World.UNSELLABLE_ITEM_PRICE))
                    {
                        Console.WriteLine("{1} : {0} : Price : {2}", inventoryItem.Quantity, inventoryItem.Description,
                            inventoryItem.Price);
                    }
                }

                Console.WriteLine("");
                Console.WriteLine("VENDOR INVENTORY");
                Console.WriteLine("================");

                if (_player.CurrentLocation.VendorWorkingHere.Inventory.Count == 0)
                {
                    Console.WriteLine("The vendor does not have any inventory items");
                }
                else
                {
                    foreach (InventoryItem inventoryItem in _player.CurrentLocation.VendorWorkingHere.Inventory)
                    {
                        Console.WriteLine("{1} : {0} : Price : {2}", inventoryItem.Quantity, inventoryItem.Description,
                            inventoryItem.Price);
                    }
                    Console.WriteLine("");
                }
            }
            else
            {
                Console.WriteLine("There is no vendor at this location");
                Console.WriteLine("");
            }
        }

        private static void DisplayCurrentLocation()
        {
            Console.WriteLine("");
            Console.WriteLine("You are at: {0}", _player.CurrentLocation.Name);

            if (_player.CurrentLocation.Description != "")
            {
                Console.WriteLine(_player.CurrentLocation.Description);
            }
            Console.WriteLine("");

            if (_player.CurrentLocation.VendorWorkingHere != null)
            {
                Console.WriteLine("You see a vendor here: {0}", _player.CurrentLocation.VendorWorkingHere.Name);
                Console.WriteLine("");
            }
        }

        private static void BuyItem(string input)
        {
            Console.WriteLine("");

            if (_player.CurrentLocation.VendorWorkingHere != null)
            {
                string itemName = input.Substring(4).Trim();

                if (string.IsNullOrEmpty(itemName))
                {
                    Console.WriteLine("You must enter the name of the item to buy");
                    Console.WriteLine("");
                }
                else
                {
                    // Get the InventoryItem from the trader's inventory
                    InventoryItem itemToBuy =
                        _player.CurrentLocation.VendorWorkingHere.Inventory.SingleOrDefault(
                            x => x.Details.Name.ToLower() == itemName);

                    // Check if the vendor has the item
                    if (itemToBuy == null | itemToBuy.Quantity <= 0)
                    {
                        Console.WriteLine("The vendor does not have any {0}", itemName);
                        Console.WriteLine("");
                    }
                    else
                    {
                        // Check if the player has enough gold to buy the item
                        if (_player.Gold < itemToBuy.Price)
                        {
                            Console.Write("You do not have enough ");
                            Console.Write("gold", Color.Gold);
                            Console.Write(" to buy any {0}", itemToBuy.Description);
                            Console.WriteLine("");
                        }
                        else
                        {
                            Console.WriteLine("How many would you like to buy?");
                            int buyQuantity = Convert.ToInt32(Console.ReadLine());

                            int buyQuantityPrice = itemToBuy.Price * buyQuantity;

                            Console.WriteLine("");

                            if (buyQuantity > itemToBuy.Quantity)
                            {
                                Console.WriteLine("The vendor does not have that many to sell.");
                                Console.WriteLine("");
                            }
                            else
                            {
                                // Success! Buy the item
                                _player.AddItemToInventory(itemToBuy.Details, buyQuantity);
                                _player.Gold -= buyQuantityPrice;

                                foreach (InventoryItem inventoryItem in _player.CurrentLocation.VendorWorkingHere.Inventory)
                                {
                                    if (inventoryItem.Details == itemToBuy.Details)
                                    {
                                        inventoryItem.Quantity -= buyQuantity;

                                        if (inventoryItem.Quantity <= 0)
                                        {
                                            inventoryItem.Quantity = 0;
                                        }
                                    }
                                }

                                Console.Write("You bought {0} {1} for {2} ", buyQuantity, itemToBuy.Details.Name, buyQuantityPrice);
                                Console.Write("gold", Color.Gold);
                                Console.WriteLine("");
                            }
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("There is no vendor at this location");
                Console.WriteLine("");
            }
        }

        private static void SellItem(string input)
        {
            Console.WriteLine("");

            if (_player.CurrentLocation.VendorWorkingHere != null)
            {
                string itemName = input.Substring(5).Trim();

                if (string.IsNullOrEmpty(itemName))
                {
                    Console.WriteLine("You must enter the name of the item to sell");
                    Console.WriteLine("");
                }
                else
                {
                    // Get the InventoryItem from the player's inventory
                    InventoryItem itemToSell =
                        _player.Inventory.SingleOrDefault(x => x.Details.Name.ToLower() == itemName &&
                                                               x.Quantity > 0 &&
                                                               x.Price != World.UNSELLABLE_ITEM_PRICE);

                    // Check if the player has the item entered
                    if (itemToSell == null | itemToSell.Quantity <= 0)
                    {
                        Console.WriteLine("The player cannot sell any {0}", itemName);
                        Console.WriteLine("");
                    }
                    else
                    {
                        Console.WriteLine("How many would you like to sell?");
                        int sellQuantity = Convert.ToInt32(Console.ReadLine());

                        int sellQuantityPrice = itemToSell.Price * sellQuantity;
                        Console.WriteLine("");


                        // Sell the item
                        _player.RemoveItemFromInventory(itemToSell.Details, sellQuantity);
                        _player.Gold += sellQuantityPrice;

                        foreach (InventoryItem inventoryItem in _player.CurrentLocation.VendorWorkingHere.Inventory)
                        {
                            if (inventoryItem.Details == itemToSell.Details)
                            {
                                inventoryItem.Quantity += sellQuantity;

                                if (inventoryItem.Quantity <= 0)
                                {
                                    inventoryItem.Quantity = sellQuantity;
                                }
                            }
                            if (inventoryItem == null)
                            {
                                _player.CurrentLocation.VendorWorkingHere.Inventory.Add(itemToSell);
                            }
                        }

                        Console.Write("You receive {0} ", sellQuantityPrice);
                        Console.Write("gold", Color.Gold);
                        Console.Write(" for your {0} {1}", sellQuantity, itemToSell.Details.Name);
                        Console.WriteLine("");
                    }
                }
            }
            else
            {
                Console.WriteLine("There is no vendor at this location");
                Console.WriteLine("");
            }
        }

        private static void AttackMonster()
        {
            Console.WriteLine("");
            if (!_player.CurrentLocation.HasAMonster)
            {
                Console.WriteLine("There is nothing here to attack");
            }
            else
            {
                if (_player.CurrentWeapon == null)
                {
                    // Select the first weapon in the player's inventory 
                    // (or 'null', if they do not have any weapons)
                    _player.CurrentWeapon = _player.Weapons.FirstOrDefault();
                }

                if (_player.CurrentWeapon == null)
                {
                    Console.WriteLine("You do not have any weapons");
                }
                else
                {
                    _player.UseWeapon(_player.CurrentWeapon);
                }
            }
        }

        private static void EquipWeapon(string input)
        {
            Console.WriteLine("");
            string inputWeaponName = input.Substring(6).Trim();

            if (string.IsNullOrEmpty(inputWeaponName))
            {
                Console.WriteLine("You must enter the name of the weapon to equip");
            }
            else
            {
                Weapon weaponToEquip =
                    _player.Weapons.SingleOrDefault(
                        x => x.Name.ToLower() == inputWeaponName || x.NamePlural.ToLower() == inputWeaponName);

                if (weaponToEquip == null)
                {
                    Console.WriteLine("You do not have the weapon: {0}", inputWeaponName);
                }
                else
                {
                    _player.CurrentWeapon = weaponToEquip;

                    Console.WriteLine("You equip your {0}", _player.CurrentWeapon.Name);
                }
            }
        }

        private static void DrinkPotion(string input)
        {
            Console.WriteLine("");
            string inputPotionName = input.Substring(6).Trim();

            if (string.IsNullOrEmpty(inputPotionName))
            {
                Console.WriteLine("You must enter the name of the potion to drink");
            }
            else
            {
                HealingPotion potionToDrink =
                    _player.Potions.SingleOrDefault(
                        x => x.Name.ToLower() == inputPotionName || x.NamePlural.ToLower() == inputPotionName);

                if (potionToDrink == null)
                {
                    Console.WriteLine("You do not have the potion: {0}", inputPotionName);
                }
                else
                {
                    _player.UsePotion(potionToDrink);
                }
            }
        }

        private static void LoadGameData()
        {
            _player = PlayerDataMapper.CreateFromDatabase();

            if (_player == null)
            {
                if (File.Exists(PLAYER_DATA_FILE_NAME))
                {
                    _player = Player.CreatePlayerFromXmlString(File.ReadAllText(PLAYER_DATA_FILE_NAME));
                }
                else
                {
                    _player = Player.CreateDefaultPlayer();
                }
            }
        }

        private static void SaveGameData()
        {
            File.WriteAllText(PLAYER_DATA_FILE_NAME, _player.ToXmlString());
            PlayerDataMapper.SaveToDatabase(_player);
        }
        public string TextReturnPath()
        {
            string Textfolder = Environment.CurrentDirectory;
            return Textfolder;
        }
    }
}