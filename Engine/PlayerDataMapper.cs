using System;
using System.Data;
using Npgsql;
using NpgsqlTypes;

namespace Engine
{
    public static class PlayerDataMapper
    {
        private static readonly string _connectionString = "Id=postgres;Password=password;Host=localhost;Port=5432;Database=RPG;";

        public static Player CreateFromDatabase()
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    Player player;

                    using (NpgsqlCommand savedGameCommand = connection.CreateCommand())
                    {
                        savedGameCommand.CommandType = CommandType.Text;
                        // reads the first rows in the SavedGame table.
                        savedGameCommand.CommandText = "SELECT TOP 1 * FROM SavedGame";

                        // Use when you expect the query to return a row, or rows
                        NpgsqlDataReader reader = savedGameCommand.ExecuteReader();

                        // Check if the query did not return a row/record of data
                        if (!reader.HasRows)
                        {
                            return null;
                        }

                        reader.Read();

                        // Get the column values for the row/record
                        int currentHitPoints = (int)reader["CurrentHitPoints"];
                        int maximumHitPoints = (int)reader["MaximumHitPoints"];
                        int gold = (int)reader["Gold"];
                        int experiencePoints = (int)reader["ExperiencePoints"];
                        int currentLocationID = (int)reader["CurrentLocationID"];

                        // Create the Player object, with the saved game values
                        player = Player.CreatePlayerFromDatabase(currentHitPoints, maximumHitPoints, gold,
                            experiencePoints, currentLocationID);
                    }

                    // Read the rows/records from the Quest table, and add them to the player
                    using (NpgsqlCommand questCommand = connection.CreateCommand())
                    {
                        questCommand.CommandType = CommandType.Text;
                        questCommand.CommandText = "SELECT * FROM Quest";

                        NpgsqlDataReader reader = questCommand.ExecuteReader();

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int questID = (int)reader["QuestID"];
                                bool isCompleted = (bool)reader["IsCompleted"];

                                // Build the PlayerQuest item, for this row
                                PlayerQuest playerQuest = new PlayerQuest(World.QuestByID(questID))
                                {
                                    IsCompleted = isCompleted
                                };

                                player.Quests.Add(playerQuest);
                            }
                        }
                    }

                    using (NpgsqlCommand inventoryCommand = connection.CreateCommand())
                    {
                        inventoryCommand.CommandType = CommandType.Text;
                        inventoryCommand.CommandText = "SELECT * FROM Inventory";

                        NpgsqlDataReader reader = inventoryCommand.ExecuteReader();

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int inventoryItemID = (int)reader["InventoryItemID"];
                                int quantity = (int)reader["Quantity"];

                                // Add the item to the player's inventory
                                player.AddItemToInventory(World.ItemByID(inventoryItemID), quantity);
                            }
                        }
                    }

                    return player;
                }
            }
            catch (Exception ex)
            {
                //returns null player...hopefully
            }

            return null;
        }

        public static void SaveToDatabase(Player player)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand existingRowCountCommand = connection.CreateCommand())
                    {
                        existingRowCountCommand.CommandType = CommandType.Text;
                        existingRowCountCommand.CommandText = "SELECT count(*) FROM SavedGame";

                        int existingRowCount = (int)existingRowCountCommand.ExecuteScalar();

                        if (existingRowCount == 0)
                        {
                            using (NpgsqlCommand insertSavedGame = connection.CreateCommand())
                            {
                                insertSavedGame.CommandType = CommandType.Text;
                                insertSavedGame.CommandText =
                                    "INSERT INTO SavedGame " +
                                    "(CurrentHitPoints, MaximumHitPoints, Gold, ExperiencePoints, CurrentLocationID) " +
                                    "VALUES " +
                                    "(@CurrentHitPoints, @MaximumHitPoints, @Gold, @ExperiencePoints, @CurrentLocationID)";

                                // Pass the values from the player object, to the SQL query, using parameters
                                insertSavedGame.Parameters.Add("@CurrentHitPoints", NpgsqlDbType.Integer);
                                insertSavedGame.Parameters["@CurrentHitPoints"].Value = player.CurrentHitPoints;

                                insertSavedGame.Parameters.Add("@MaximumHitPoints", NpgsqlDbType.Integer);
                                insertSavedGame.Parameters["@MaximumHitPoints"].Value = player.MaximumHitPoints;

                                insertSavedGame.Parameters.Add("@Gold", NpgsqlDbType.Integer);
                                insertSavedGame.Parameters["@Gold"].Value = player.Gold;

                                insertSavedGame.Parameters.Add("@ExperiencePoints", NpgsqlDbType.Integer);
                                insertSavedGame.Parameters["@ExperiencePoints"].Value = player.ExperiencePoints;

                                insertSavedGame.Parameters.Add("@CurrentLocationID", NpgsqlDbType.Integer);
                                insertSavedGame.Parameters["@CurrentLocationID"].Value = player.CurrentLocation.ID;

                                //insert new values
                                insertSavedGame.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            // update command
                            using (NpgsqlCommand updateSavedGame = connection.CreateCommand())
                            {
                                updateSavedGame.CommandType = CommandType.Text;
                                updateSavedGame.CommandText =
                                    "UPDATE SavedGame " +
                                    "SET CurrentHitPoints = @CurrentHitPoints, " +
                                    "MaximumHitPoints = @MaximumHitPoints, " +
                                    "Gold = @Gold, " +
                                    "ExperiencePoints = @ExperiencePoints, " +
                                    "CurrentLocationID = @CurrentLocationID";

                                updateSavedGame.Parameters.Add("@CurrentHitPoints", NpgsqlDbType.Integer);
                                updateSavedGame.Parameters["@CurrentHitPoints"].Value = player.CurrentHitPoints;

                                updateSavedGame.Parameters.Add("@MaximumHitPoints", NpgsqlDbType.Integer);
                                updateSavedGame.Parameters["@MaximumHitPoints"].Value = player.MaximumHitPoints;

                                updateSavedGame.Parameters.Add("@Gold", NpgsqlDbType.Integer);
                                updateSavedGame.Parameters["@Gold"].Value = player.Gold;

                                updateSavedGame.Parameters.Add("@ExperiencePoints", NpgsqlDbType.Integer);
                                updateSavedGame.Parameters["@ExperiencePoints"].Value = player.ExperiencePoints;

                                updateSavedGame.Parameters.Add("@CurrentLocationID", NpgsqlDbType.Integer);
                                updateSavedGame.Parameters["@CurrentLocationID"].Value = player.CurrentLocation.ID;

                                //insert new values
                                updateSavedGame.ExecuteNonQuery();
                            }
                        }
                    }

                    // Delete existing Quest rows
                    using (NpgsqlCommand deleteQuestsCommand = connection.CreateCommand())
                    {
                        deleteQuestsCommand.CommandType = CommandType.Text;
                        deleteQuestsCommand.CommandText = "DELETE FROM Quest";

                        deleteQuestsCommand.ExecuteNonQuery();
                    }

                    // Insert new Quest rows, from the player object
                    foreach (PlayerQuest playerQuest in player.Quests)
                    {
                        using (NpgsqlCommand insertQuestCommand = connection.CreateCommand())
                        {
                            insertQuestCommand.CommandType = CommandType.Text;
                            insertQuestCommand.CommandText = "INSERT INTO Quest (QuestID, IsCompleted) VALUES (@QuestID, @IsCompleted)";

                            insertQuestCommand.Parameters.Add("@QuestID", NpgsqlDbType.Integer);
                            insertQuestCommand.Parameters["@QuestID"].Value = playerQuest.Details.ID;
                            insertQuestCommand.Parameters.Add("@IsCompleted", NpgsqlDbType.Boolean);
                            insertQuestCommand.Parameters["@IsCompleted"].Value = playerQuest.IsCompleted;

                            insertQuestCommand.ExecuteNonQuery();
                        }
                    }

                    // Delete existing Inventory rows
                    using (NpgsqlCommand deleteInventoryCommand = connection.CreateCommand())
                    {
                        deleteInventoryCommand.CommandType = CommandType.Text;
                        deleteInventoryCommand.CommandText = "DELETE FROM Inventory";

                        deleteInventoryCommand.ExecuteNonQuery();
                    }

                    // Insert new Inventory rows, from the player object
                    foreach (InventoryItem inventoryItem in player.Inventory)
                    {
                        using (NpgsqlCommand insertInventoryCommand = connection.CreateCommand())
                        {
                            insertInventoryCommand.CommandType = CommandType.Text;
                            insertInventoryCommand.CommandText = "INSERT INTO Inventory (InventoryItemID, Quantity) VALUES (@InventoryItemID, @Quantity)";

                            insertInventoryCommand.Parameters.Add("@InventoryItemID", NpgsqlDbType.Integer);
                            insertInventoryCommand.Parameters["@InventoryItemID"].Value = inventoryItem.Details.ID;
                            insertInventoryCommand.Parameters.Add("@Quantity", NpgsqlDbType.Integer);
                            insertInventoryCommand.Parameters["@Quantity"].Value = inventoryItem.Quantity;

                            insertInventoryCommand.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //error
            }
        }
    }
}