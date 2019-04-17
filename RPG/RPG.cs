using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using Engine;

namespace RPG
{
    public partial class RPG : Form
    {
        private Player  _player;
        private Monster _currentMonster;

        //xml file the player data will save to
        private const string PLAYER_DATA_FILE_NAME = "PlayerData.xml";

        public RPG()
        {
            InitializeComponent();

            if (File.Exists(PLAYER_DATA_FILE_NAME))
            {
                _player = Player.CreatePlayerFromXmlString(File.ReadAllText(PLAYER_DATA_FILE_NAME));
            }
            else
            {
                _player = Player.CreateDefaultPlayer();
            }

            //updates UI labels to show current player stats
            lblHitPoints.DataBindings.Add("Text", _player, "CurrentHitPoints");
            lblGold.DataBindings.Add("Text", _player, "Gold");
            lblExperience.DataBindings.Add("Text", _player, "ExperiencePoints");
            lblLevel.DataBindings.Add("Text", _player, "Level");

            MoveTo(_player.CurrentLocation);
        }

        //on button click, moves the player:
        //North
        private void btnNorth_Click(object sender, EventArgs e)
        {
                MoveTo(_player.CurrentLocation.LocationToNorth);
            
        }

        //South
        private void btnSouth_Click(object sender, EventArgs e)
        {
                MoveTo(_player.CurrentLocation.LocationToSouth);
        }

        //East
        private void btnEast_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToEast);
        }

        //West
        private void btnWest_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToWest);
        }

        //Handles player movement
        public void MoveTo(Location newLocation)
        {
            //checks if player has required items to enter
            if (!_player.HasRequiredItemToEnterThisLocation(newLocation))
            {
                rtbMessages.Text += "You need a " + newLocation.ItemRequiredToEnter.Name + " to get up in this bitch." + Environment.NewLine;
                rtbMessages.Text += Environment.NewLine;
                ScrollToBottomMsg();
                return;
            }

            //update players location
            _player.CurrentLocation = newLocation;

            //this will either show or hide movement button based on possible directions
            btnNorth.Visible = (newLocation.LocationToNorth != null);
            btnSouth.Visible = (newLocation.LocationToSouth != null);
            btnEast.Visible  = (newLocation.LocationToEast  != null);
            btnWest.Visible  = (newLocation.LocationToWest  != null);

            //displays current locations name and description in UI
            rtbLocation.Text  = newLocation.Name + Environment.NewLine;
            rtbLocation.Text += Environment.NewLine;
            rtbLocation.Text += newLocation.Description + Environment.NewLine;
            

            //heals player when entering new location
            //might change later to make game more difficult
            _player.CurrentHitPoints = _player.MaximumHitPoints;


            //checks location for a quest.
            if (newLocation.QuestAvailableHere != null)
            {
                //checks if player has quest and if its done
                bool playerAlreadyHasQuest = _player.HasThisQuest(newLocation.QuestAvailableHere);
                bool playerAlreadyCompletedQuest = _player.CompletedThisQuest(newLocation.QuestAvailableHere);


                //do they have the quest?
                if (playerAlreadyHasQuest)
                {
                    //is the quest done?
                    if (!playerAlreadyCompletedQuest)
                    {
                        //but do they have all the items for the quest?
                        bool playerHasAllItemsToCompleteQuest = _player.HasAllQuestCompletionItems(newLocation.QuestAvailableHere);


                        //player has everything needed for the quest
                        if (playerHasAllItemsToCompleteQuest)
                        {
                            //show message
                            rtbMessages.Text += Environment.NewLine;
                            rtbMessages.Text += "You finished the '" + newLocation.QuestAvailableHere.Name + "' quest, proud of you." + Environment.NewLine;
                            ScrollToBottomMsg();

                            //removes quest item from inventory
                            _player.RemoveQuestCompletionItems(newLocation.QuestAvailableHere);

                            //gives rewards for quest 
                            rtbMessages.Text += "You receive: " + Environment.NewLine;
                            rtbMessages.Text += newLocation.QuestAvailableHere.RewardExperiencePoints.ToString() + " experience points" + Environment.NewLine;
                            rtbMessages.Text += newLocation.QuestAvailableHere.RewardGold.ToString() + " gold" + Environment.NewLine;
                            rtbMessages.Text += newLocation.QuestAvailableHere.RewardItem.Name + Environment.NewLine;
                            rtbMessages.Text += Environment.NewLine;
                            ScrollToBottomMsg();

                            _player.AddExperiencePoints(newLocation.QuestAvailableHere.RewardExperiencePoints);
                            _player.Gold += newLocation.QuestAvailableHere.RewardGold;

                            //adds reward to inventory
                            _player.AddItemToInventory(newLocation.QuestAvailableHere.RewardItem);


                            //finds quest in list
                            //mark quest done
                            _player.MarkQuestCompleted(newLocation.QuestAvailableHere);
                        }
                    }
                }
                else
                {
                    //this tells the player about the quest
                    rtbMessages.Text += "You recive the " + newLocation.QuestAvailableHere.Name + " quest." + Environment.NewLine;
                    rtbMessages.Text += newLocation.QuestAvailableHere.Description + Environment.NewLine;
                    rtbMessages.Text += "To complete it, return with:" + Environment.NewLine;
                    ScrollToBottomMsg();

                    foreach (QuestCompletionItem qci in newLocation.QuestAvailableHere.QuestCompletionItems)
                    {
                        if (qci.Quantity == 1)
                        {
                            rtbMessages.Text += qci.Quantity.ToString() + " " + qci.Details.Name + Environment.NewLine;
                            ScrollToBottomMsg();
                        }
                        else
                        {
                            rtbMessages.Text += qci.Quantity.ToString() + " " + qci.Details.NamePlural + Environment.NewLine;
                            ScrollToBottomMsg();
                        }
                    }

                    rtbMessages.Text += Environment.NewLine;
                    ScrollToBottomMsg();

                    //adds quest to quest list
                    _player.Quests.Add(new PlayerQuest(newLocation.QuestAvailableHere));
                }
            }

            //checks location for monsters
            if (newLocation.MonsterLivingHere != null)
            {
                rtbMessages.Text += "You see a " + newLocation.MonsterLivingHere.Name + Environment.NewLine;
                rtbMessages.Text += Environment.NewLine;
                ScrollToBottomMsg();

                //makes a monster from World.monster list
                Monster standardMonster = World.MonsterByID(newLocation.MonsterLivingHere.ID);

                _currentMonster = new Monster(standardMonster.ID, standardMonster.Name, standardMonster.MaximumDamage, standardMonster.CurrentHitPoints,
                    standardMonster.MaximumHitPoints, standardMonster.RewardExperiencePoints, standardMonster.RewardGold);

                foreach (LootItem lootItem in standardMonster.LootTable)
                {
                    _currentMonster.LootTable.Add(lootItem);
                }

                cboWeapons.Visible   = true;
                cboPotions.Visible   = true;
                btnUseWeapon.Visible = true;
                btnUsePotion.Visible = true;
                selectAction.Visible = true;
            }
            else
            {
                _currentMonster = null;

                cboWeapons.Visible   = false;
                cboPotions.Visible   = false;
                btnUseWeapon.Visible = false;
                btnUsePotion.Visible = false;
                selectAction.Visible = false;
            }

            //refresh inventory list
            UpdateInventoryListInUI();

            // Refresh player's quest list
            UpdateQuestListInUI();

            //refresh weapon box list dropdown thing
            UpdateWeaponListInUI();

            // Refresh player's potions combobox
            UpdatePotionListInUI();
        }

        //updates inventory list
        private void UpdateInventoryListInUI()
        {
            dgvInventory.RowHeadersVisible = false;

            dgvInventory.ColumnCount = 2;
            dgvInventory.Columns[0].Name = "Inventory";
            dgvInventory.Columns[0].Width = 197;
            dgvInventory.Columns[1].Name = "Quantity";

            dgvInventory.Rows.Clear();

            foreach (InventoryItem inventoryItem in _player.Inventory)
            {
                if (inventoryItem.Quantity > 0)
                {
                    dgvInventory.Rows.Add(new[] { inventoryItem.Details.Name, inventoryItem.Quantity.ToString() });
                }
            }
        }

        //updates quest list
        private void UpdateQuestListInUI()
        {
            dgvQuests.RowHeadersVisible = false;

            dgvQuests.ColumnCount = 2;
            dgvQuests.Columns[0].Name = "Quest";
            dgvQuests.Columns[0].Width = 197;
            dgvQuests.Columns[1].Name = "Complete?";

            dgvQuests.Rows.Clear();

            foreach (PlayerQuest playerQuest in _player.Quests)
            {
                dgvQuests.Rows.Add(new[] { playerQuest.Details.Name, playerQuest.IsCompleted.ToString()});
            }
        }

        //updates weapon list
        private void UpdateWeaponListInUI()
        {
            List<Weapon> weapons = new List<Weapon>();

            foreach (InventoryItem inventoryItem in _player.Inventory)
            {
                if (inventoryItem.Details is Weapon)
                {
                    if (inventoryItem.Quantity > 0)
                    {
                        weapons.Add((Weapon)inventoryItem.Details);
                    }
                }
            }

            if (weapons.Count == 0)
            {
                // The player doesn't have any weapons, so hide the weapon combobox and "Use" button
                cboWeapons.Visible = false;
                btnUseWeapon.Visible = false;
            }
            else
            {
                cboWeapons.SelectedIndexChanged -= cboWeapons_SelectedIndexChanged;
                cboWeapons.DataSource = weapons;
                cboWeapons.SelectedIndexChanged += cboWeapons_SelectedIndexChanged;
                cboWeapons.DisplayMember = "Name";
                cboWeapons.ValueMember = "ID";

                if (_player.CurrentWeapon != null)
                {
                    cboWeapons.SelectedItem = _player.CurrentWeapon;
                }
                else
                {
                    cboWeapons.SelectedIndex = 0;
                }
            }
        }

        //updates potion list
        private void UpdatePotionListInUI()
        {
            List<HealingPotion> healingPotions = new List<HealingPotion>();

            foreach (InventoryItem inventoryItem in _player.Inventory)
            {
                if (inventoryItem.Details is HealingPotion)
                {
                    if (inventoryItem.Quantity > 0)
                    {
                        healingPotions.Add((HealingPotion)inventoryItem.Details);
                    }
                }
            }

            if (healingPotions.Count == 0)
            {
                // The player doesn't have any potions, so hide the potion combobox and "Use" button
                cboPotions.Visible = false;
                btnUsePotion.Visible = false;
            }
            else
            {
                cboPotions.DataSource = healingPotions;
                cboPotions.DisplayMember = "Name";
                cboPotions.ValueMember = "ID";

                cboPotions.SelectedIndex = 0;
            }
        }

        private void btnUseWeapon_Click(object sender, EventArgs e)
        {
            // Get the currently selected weapon from the cboWeapons ComboBox
            Weapon currentWeapon = (Weapon)cboWeapons.SelectedItem;

            // Determine the amount of damage to do to the monster
            int damageToMonster = RandomNumberGenerator.NumberBetween(currentWeapon.MinimumDamage, currentWeapon.MaximumDamage);

            // Apply the damage to the monster's CurrentHitPoints
            _currentMonster.CurrentHitPoints -= damageToMonster;

            // Display message
            rtbMessages.Text += "You hit the " + _currentMonster.Name + " for " + damageToMonster.ToString() + " points." + Environment.NewLine;
            ScrollToBottomMsg();

            // Check if the monster is dead
            if (_currentMonster.CurrentHitPoints <= 0)
            {
                // Monster is dead
                rtbMessages.Text += Environment.NewLine;
                rtbMessages.Text += "You defeated the " + _currentMonster.Name + Environment.NewLine;
                ScrollToBottomMsg();

                // Give player experience points for killing the monster
                _player.AddExperiencePoints(_currentMonster.RewardExperiencePoints);
                rtbMessages.Text += "You receive " + _currentMonster.RewardExperiencePoints.ToString() + " experience points" + Environment.NewLine;
                ScrollToBottomMsg();

                // Give player gold for killing the monster 
                _player.Gold += _currentMonster.RewardGold;
                rtbMessages.Text += "You receive " + _currentMonster.RewardGold.ToString() + " gold" + Environment.NewLine;
                ScrollToBottomMsg();

                // Get random loot items from the monster
                List<InventoryItem> lootedItems = new List<InventoryItem>();

                // Add items to the lootedItems list, comparing a random number to the drop percentage
                foreach (LootItem lootItem in _currentMonster.LootTable)
                {
                    if (RandomNumberGenerator.NumberBetween(1, 100) <= lootItem.DropPercentage)
                    {
                        lootedItems.Add(new InventoryItem(lootItem.Details, 1));
                    }
                }

                // If no items were randomly selected, then add the default loot item(s).
                if (lootedItems.Count == 0)
                {
                    foreach (LootItem lootItem in _currentMonster.LootTable)
                    {
                        if (lootItem.IsDefaultItem)
                        {
                            lootedItems.Add(new InventoryItem(lootItem.Details, 1));
                        }
                    }
                }

                // Add the looted items to the player's inventory
                foreach (InventoryItem inventoryItem in lootedItems)
                {
                    _player.AddItemToInventory(inventoryItem.Details);

                    if (inventoryItem.Quantity == 1)
                    {
                        rtbMessages.Text += "You loot " + inventoryItem.Quantity.ToString() + " " + inventoryItem.Details.Name + Environment.NewLine;
                        ScrollToBottomMsg();
                    }
                    else
                    {
                        rtbMessages.Text += "You loot " + inventoryItem.Quantity.ToString() + " " + inventoryItem.Details.NamePlural + Environment.NewLine;
                        ScrollToBottomMsg();
                    }
                }

                // Refresh player information and inventory controls
                lblGold.Text = _player.Gold.ToString();
                lblExperience.Text = _player.ExperiencePoints.ToString();
                lblLevel.Text = _player.Level.ToString();

                UpdateInventoryListInUI();
                UpdateWeaponListInUI();
                UpdatePotionListInUI();

                // Add a blank line to the messages box, just for appearance.
                rtbMessages.Text += Environment.NewLine;
                ScrollToBottomMsg();

                // Move player to current location (to heal player and create a new monster to fight)
                MoveTo(_player.CurrentLocation);
            }
            else
            {
                // Monster is still alive

                // Determine the amount of damage the monster does to the player
                int damageToPlayer = RandomNumberGenerator.NumberBetween(0, _currentMonster.MaximumDamage);

                // Display message
                rtbMessages.Text += "The " + _currentMonster.Name + " did " + damageToPlayer.ToString() + " points of damage." + Environment.NewLine;
                ScrollToBottomMsg();

                // Subtract damage from player
                _player.CurrentHitPoints -= damageToPlayer;


                if (_player.CurrentHitPoints <= 0)
                {
                    // Display message
                    rtbMessages.Text += "The " + _currentMonster.Name + " killed you." + Environment.NewLine;
                    ScrollToBottomMsg();

                    // Move player to "Home"
                    MoveTo(World.LocationByID(World.LOCATION_ID_HOME));
                }
            }
        }

        private void btnUsePotion_Click(object sender, EventArgs e)
        {
            // Get the currently selected potion from the combobox
            HealingPotion potion = (HealingPotion)cboPotions.SelectedItem;

            // Add healing amount to the player's current hit points
            _player.CurrentHitPoints = (_player.CurrentHitPoints + potion.AmountToHeal);

            // CurrentHitPoints cannot exceed player's MaximumHitPoints
            if (_player.CurrentHitPoints > _player.MaximumHitPoints)
            {
                _player.CurrentHitPoints = _player.MaximumHitPoints;
            }

            // Remove the potion from the player's inventory
            foreach (InventoryItem ii in _player.Inventory)
            {
                if (ii.Details.ID == potion.ID)
                {
                    ii.Quantity--;
                    break;
                }
            }

            // Display message
            rtbMessages.Text += "You drink a " + potion.Name + ", restoring " + potion.AmountToHeal.ToString() + " HP" + Environment.NewLine;
            ScrollToBottomMsg();

            // Monster gets their turn to attack

            // Determine the amount of damage the monster does to the player
            int damageToPlayer = RandomNumberGenerator.NumberBetween(0, _currentMonster.MaximumDamage);

            // Display message
            rtbMessages.Text += "The " + _currentMonster.Name + " did " + damageToPlayer.ToString() + " points of damage." + Environment.NewLine;
            ScrollToBottomMsg();

            // Subtract damage from player
            _player.CurrentHitPoints -= damageToPlayer;

            if (_player.CurrentHitPoints <= 0)
            {
                // Display message
                rtbMessages.Text += "The " + _currentMonster.Name + " killed you." + Environment.NewLine;
                ScrollToBottomMsg();

                // Move player to "Home"
                MoveTo(World.LocationByID(World.LOCATION_ID_HOME));
            }

            // Refresh player data in UI
            UpdateInventoryListInUI();
            UpdatePotionListInUI();
        }

        private void ScrollToBottomMsg()
        {
            rtbMessages.SelectionStart = rtbMessages.Text.Length;
            rtbMessages.ScrollToCaret();
        }

        private void cboWeapons_SelectedIndexChanged(object sender, EventArgs e)
        {
            _player.CurrentWeapon = (Weapon)cboWeapons.SelectedItem;
        }

        private void RPG_FormClosing(object sender, FormClosingEventArgs e)
        {
            File.WriteAllText(PLAYER_DATA_FILE_NAME, _player.ToXmlString());
        }
    }
}
 