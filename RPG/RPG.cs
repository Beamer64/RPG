using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Engine;

namespace RPG
{
    public partial class RPG : Form
    {
        private Player  _player;
        private Monster _currentMonster;

        public RPG()
        {
            InitializeComponent();

            //start player at home and give that bitch a pig poker
            _player = new Player(10, 10, 20, 0, 1);

            MoveTo(World.LocationByID(World.LOCATION_ID_HOME));
            _player.Inventory.Add(new InventoryItem(World.ItemByID(World.ITEM_ID_RUSTY_SWORD), 1));


            //updates UI labels to show current player stats
            lblHitPoints.Text  = _player.CurrentHitPoints.ToString();
            lblGold.Text       = _player.Gold.ToString();
            lblExperience.Text = _player.ExperiencePoints.ToString();
            lblLevel.Text      = _player.Level.ToString();
        }

        //you guessed it, on button click, moves the player:
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

        private void MoveTo(Location newLocation)
        {
            //checks if location needs a required item to enter
            if(newLocation.ItemRequiredToEnter != null)
            {
                //checks if player has required item needed
                bool playerHasRequiredItem = false;

                foreach(InventoryItem ii in _player.Inventory)
                {
                    if(ii.Details.ID == newLocation.ItemRequiredToEnter.ID)
                    {
                        //player does have required item
                        playerHasRequiredItem = true;
                        break; //exits foreach loop
                    }
                }

                //checks if player doesn't have required item needed
                if (!playerHasRequiredItem)
                {
                    //tells player they need the item to enter
                    rtbMessages.Text += "You need a " + newLocation.ItemRequiredToEnter.Name + " to get up in this bitch." + Environment.NewLine;
                    return;
                }
            }

            //update players location
            _player.CurrentLocation = newLocation;

            //this will either show or hide movement button based on possible directions
            btnNorth.Visible = (newLocation.LocationToNorth != null);
            btnSouth.Visible = (newLocation.LocationToSouth != null);
            btnEast.Visible = (newLocation.LocationToEast != null);
            btnWest.Visible = (newLocation.LocationToWest != null);

            //displays current locations name and description in UI
            rtbLocation.Text = newLocation.Name + Environment.NewLine;
            rtbLocation.Text = newLocation.Description + Environment.NewLine;

            //heals player when entering new location
            //might change later to make game more difficult
            _player.CurrentHitPoints = _player.MaximumHitPoints;

            //updates hit points in UI
            lblHitPoints.Text = _player.CurrentHitPoints.ToString();

            //checks location for a quest.
            if (newLocation.QuestAvailableHere != null)
            {
                //checks if player has quest and if its done
                bool playerAlreadyHasQuest = false;
                bool playerAlreadyCompletedQuest = false;

                foreach (PlayerQuest playerQuest in _player.Quests)
                {
                    if (playerQuest.Details.ID == newLocation.QuestAvailableHere.ID)
                    {
                        playerAlreadyHasQuest = true;

                        if (playerQuest.IsCompleted)
                        {
                            playerAlreadyCompletedQuest = true;
                        }
                    }
                }

                //do they have the quest?
                if (playerAlreadyHasQuest)
                {
                    //is the quest done?
                    if (!playerAlreadyCompletedQuest)
                    {
                        //but do they have all the items for the quest?
                        bool playerHasAllItemsToCompleteQuest = true;

                        foreach(QuestCompletionItem qci in newLocation.QuestAvailableHere.QuestCompletionItems)
                        {
                            bool foundItemInPlayerInventory = false;

                            //Checks inventory or correct item and quantity
                            foreach(InventoryItem ii in _player.Inventory)
                            {
                                //player has item
                                if(ii.Details.ID == qci.Details.ID)
                                {
                                    foundItemInPlayerInventory = true;

                                    if(ii.Quantity < qci.Quantity)
                                    {
                                        //player doesnt have correct quantity
                                        playerHasAllItemsToCompleteQuest = false;
                                        break; //stops checking
                                    }

                                    break; //item found
                                }
                            }

                            //didnt find it, stop looking for the others
                            if (!foundItemInPlayerInventory)
                            {
                                playerHasAllItemsToCompleteQuest = false;
                                break; //stop looking
                            }
                        }

                        //player has everything needed for the quest
                        if (playerHasAllItemsToCompleteQuest)
                        {
                            //show message
                            rtbMessages.Text += Environment.NewLine;
                            rtbMessages.Text += "You finished the '" + newLocation.QuestAvailableHere.Name + "' quest, proud of you." + Environment.NewLine;

                            //removes quest item from inventory
                            foreach(QuestCompletionItem qci in newLocation.QuestAvailableHere.QuestCompletionItems)
                            {
                                foreach(InventoryItem ii in _player.Inventory)
                                {
                                    if(ii.Details.ID == qci.Details.ID)
                                    {
                                        //only takes away amount needed
                                        ii.Quantity -= qci.Quantity;
                                        break;
                                    }
                                }
                            }

                            //gives rewards for quest 
                            rtbMessages.Text += "You receive: " + Environment.NewLine;
                            rtbMessages.Text += newLocation.QuestAvailableHere.RewardExperiencePoints.ToString() + " experience points" + Environment.NewLine;
                            rtbMessages.Text += newLocation.QuestAvailableHere.RewardGold.ToString() + " gold" + Environment.NewLine;
                            rtbMessages.Text += newLocation.QuestAvailableHere.RewardItem.Name + Environment.NewLine;
                            rtbMessages.Text += Environment.NewLine;

                            _player.ExperiencePoints += newLocation.QuestAvailableHere.RewardExperiencePoints;
                            _player.Gold += newLocation.QuestAvailableHere.RewardGold;

                            //adds reward to inventory
                            bool addedItemToPlayerInventory = false;

                            foreach(InventoryItem ii in _player.Inventory)
                            {
                                if(ii.Details.ID == newLocation.QuestAvailableHere.RewardItem.ID)
                                {
                                    //already got it so just add to it
                                    ii.Quantity++;

                                    addedItemToPlayerInventory = true;

                                    break;
                                }
                            }

                            //didnt have item so adding 1
                            if (addedItemToPlayerInventory)
                            {
                                _player.Inventory.Add(new InventoryItem(newLocation.QuestAvailableHere.RewardItem, 1));
                            }

                            //mark quest done
                            //finds quest in list
                            foreach(PlayerQuest pq in _player.Quests)
                            {
                                if(pq.Details.ID == newLocation.QuestAvailableHere.ID)
                                {
                                    //Mark as done
                                    pq.IsCompleted = true;

                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    //this tells the player about the quest
                    rtbMessages.Text += "You recive the " + newLocation.QuestAvailableHere.Name + " quest." + Environment.NewLine;
                    rtbMessages.Text += newLocation.QuestAvailableHere.Description + Environment.NewLine;
                    rtbMessages.Text += "To complete it, return with:" + Environment.NewLine;

                    foreach(QuestCompletionItem qci in newLocation.QuestAvailableHere.QuestCompletionItems)
                    {
                        if(qci.Quantity == 1)
                        {
                            rtbMessages.Text += qci.Quantity.ToString() + " " + qci.Details.Name + Environment.NewLine;
                        }
                        else
                        {
                            rtbMessages.Text += qci.Quantity.ToString() + " " + qci.Details.NamePlural + Environment.NewLine;
                        }
                    }

                    rtbMessages.Text += Environment.NewLine;

                    //adds quest to quest list
                    _player.Quests.Add(new PlayerQuest(newLocation.QuestAvailableHere));
                }
            }

            //checks location for monsters
            if(newLocation.MonsterLivingHere != null)
            {
                rtbMessages.Text += "You see a " + newLocation.MonsterLivingHere.Name + Environment.NewLine;

                //makes a monster from World.monster list
                Monster standardMonster = World.MonsterByID(newLocation.MonsterLivingHere.ID);

                _currentMonster = new Monster(standardMonster.ID, standardMonster.Name, standardMonster.MaximumDamage, standardMonster.CurrentHitPoints,
                    standardMonster.MaximumHitPoints, standardMonster.RewardExperiencePoints, standardMonster.RewardGold);

                foreach(LootItem lootItem in standardMonster.LootTable)
                {
                    _currentMonster.LootTable.Add(lootItem);
                }

                cboWeapons.Visible   = true;
                cboPotions.Visible   = true;
                btnUseWeapon.Visible = true;
                btnUsePotion.Visible = true;
            }
            else
            {
                _currentMonster = null;

                cboWeapons.Visible   = false;
                cboPotions.Visible   = false;
                btnUseWeapon.Visible = false;
                btnUsePotion.Visible = false;
            }

            //refresh inventory list
            dgvInventory.RowHeadersVisible = false;

            dgvInventory.ColumnCount = 2;
            dgvInventory.Columns[0].Name = "Name";
            dgvInventory.Columns[0].Width = 197;
            dgvInventory.Columns[1].Name = "Quantity";

            dgvInventory.Rows.Clear();

            foreach(InventoryItem inventoryItem in _player.Inventory)
            {
                if(inventoryItem.Quantity > 0)
                {
                    dgvInventory.Rows.Add(new[] { inventoryItem.Details.Name, inventoryItem.Quantity.ToString() });
                }
            }

            // Refresh player's quest list
            dgvQuests.RowHeadersVisible = false;

            dgvQuests.ColumnCount = 2;
            dgvQuests.Columns[0].Name = "Name";
            dgvQuests.Columns[0].Width = 197;
            dgvQuests.Columns[1].Name = "Completed?";

            dgvQuests.Rows.Clear();

            foreach (PlayerQuest playerQuest in _player.Quests)
            {
                dgvQuests.Rows.Add(new[] { playerQuest.Details.Name, playerQuest.IsCompleted.ToString() });
            }

            //refresh weapon box list dropdown thing
            List<Weapon> weapons = new List<Weapon>();

            foreach(InventoryItem inventoryItem in _player.Inventory)
            {
                if(inventoryItem.Details is Weapon)
                {
                    if(inventoryItem.Quantity > 0)
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
                cboWeapons.DataSource    = weapons;
                cboWeapons.DisplayMember = "Name";
                cboWeapons.ValueMember   = "ID";

                cboWeapons.SelectedIndex = 0;
            }

            // Refresh player's potions combobox
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

        }

        private void btnUsePotion_Click(object sender, EventArgs e)
        {

        }
    }
}
 