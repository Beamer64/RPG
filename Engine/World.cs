﻿using System.Collections.Generic;
using System.Linq;

namespace Engine
{
    public static class World
    {
        private static Narrative narrative = new Narrative();

        public static readonly List<Item> _items = new List<Item>();
        public static readonly List<Monster> _monsters = new List<Monster>();
        public static readonly List<Quest> _quests = new List<Quest>();
        public static readonly List<Location> _locations = new List<Location>();

        public const int UNSELLABLE_ITEM_PRICE = -1;

        //ITEMS LIST
        public const int ITEM_ID_RAT_TAIL = 1;
        public const int ITEM_ID_PIECE_OF_FUR = 2;
        public const int ITEM_ID_SNAKE_FANG = 3;
        public const int ITEM_ID_SNAKESKIN = 4;
        public const int ITEM_ID_SPIDER_FANG = 5;
        public const int ITEM_ID_SPIDER_SILK = 6;
        public const int ITEM_ID_ADVENTURER_PASS = 7;

        //WEAPON LIST
        public const int ITEM_ID_RUSTY_SWORD = 8;
        public const int ITEM_ID_CLUB = 9;

        //ARMOR LIST
        public const int ITEM_ID_RUSTY_SHIELD = 10;
        public const int ITEM_ID_LEATHER_CHEST = 11;

        //POTION LIST
        public const int ITEM_ID_HEALING_POTION = 12;

        //MONSTER LIST
        public const int MONSTER_ID_RAT = 1;
        public const int MONSTER_ID_SNAKE = 2;
        public const int MONSTER_ID_GIANT_SPIDER = 3;

        //QUEST LIST
        public const int QUEST_ID_CLEAR_ALCHEMIST_GARDEN = 1;
        public const int QUEST_ID_CLEAR_FARMERS_FIELD = 2;

        //LOCATIONS LIST
        public const int LOCATION_ID_HOME = 1;
        public const int LOCATION_ID_TOWN_SQUARE = 2;
        public const int LOCATION_ID_GUARD_POST = 3;
        public const int LOCATION_ID_ALCHEMIST_HUT = 4;
        public const int LOCATION_ID_ALCHEMISTS_GARDEN = 5;
        public const int LOCATION_ID_FARMHOUSE = 6;
        public const int LOCATION_ID_FARM_FIELD = 7;
        public const int LOCATION_ID_BRIDGE = 8;
        public const int LOCATION_ID_SPIDER_FIELD = 9;

        //STRING SECION FOR LOCATION DESCRIPTION

        static World()
        {
            PopulateItems();
            PopulateMonsters();
            PopulateQuests();
            PopulateLocations();
        }

        private static void PopulateItems()
        {
            _items.Add(new Weapon(ITEM_ID_RUSTY_SWORD, "Rusty Sword", "Rusty Swords", 0, 5, 5));
            _items.Add(new Weapon(ITEM_ID_CLUB, "Club", "Clubs", 3, 10, 8));

            _items.Add(new Armor(ITEM_ID_RUSTY_SHIELD, "Shield", "Rusty Shield", 8, 5));
            _items.Add(new Armor(ITEM_ID_LEATHER_CHEST, "Leather Chest Piece", "Basic Leather Chest Piece", 4, 5));

            _items.Add(new Item(ITEM_ID_RAT_TAIL, "Rat tail", "Rat tails", 1));
            _items.Add(new Item(ITEM_ID_PIECE_OF_FUR, "Piece of fur", "Peices of fur", 1));
            _items.Add(new Item(ITEM_ID_SNAKE_FANG, "Snake fang", "Snake fangs", 1));
            _items.Add(new Item(ITEM_ID_SNAKESKIN, "Snakeskin", "Snakeskins", 2));
            _items.Add(new Item(ITEM_ID_SPIDER_FANG, "Spider fang", "Spider fangs", 1));
            _items.Add(new Item(ITEM_ID_SPIDER_SILK, "Spider silk", "Spider silks", 1));

            _items.Add(new Item(ITEM_ID_ADVENTURER_PASS, "Adventurer pass", "Adventurer passes", UNSELLABLE_ITEM_PRICE));

            _items.Add(new HealingPotion(ITEM_ID_HEALING_POTION, "Healing potion", "Healing potions", 5, 3));
        }

        private static void PopulateMonsters()
        {
            Monster rat = new Monster(MONSTER_ID_RAT, "Rat", 5, 3, 10, 3, 3);
            rat.LootTable.Add(new LootItem(ItemByID(ITEM_ID_RAT_TAIL), 50, false));
            rat.LootTable.Add(new LootItem(ItemByID(ITEM_ID_PIECE_OF_FUR), 50, true));
            rat.LootTable.Add(new LootItem(ItemByID(ITEM_ID_HEALING_POTION), 20, false));

            Monster snake = new Monster(MONSTER_ID_SNAKE, "Snake", 5, 3, 10, 3, 3);
            snake.LootTable.Add(new LootItem(ItemByID(ITEM_ID_SNAKE_FANG), 50, false));
            snake.LootTable.Add(new LootItem(ItemByID(ITEM_ID_SNAKESKIN), 50, true));
            snake.LootTable.Add(new LootItem(ItemByID(ITEM_ID_HEALING_POTION), 20, false));

            Monster giantSpider = new Monster(MONSTER_ID_GIANT_SPIDER, "Giant spider", 20, 5, 40, 10, 10);
            giantSpider.LootTable.Add(new LootItem(ItemByID(ITEM_ID_SPIDER_FANG), 75, true));
            giantSpider.LootTable.Add(new LootItem(ItemByID(ITEM_ID_SPIDER_SILK), 25, false));
            giantSpider.LootTable.Add(new LootItem(ItemByID(ITEM_ID_HEALING_POTION), 20, false));

            _monsters.Add(rat);
            _monsters.Add(snake);
            _monsters.Add(giantSpider);
        }

        private static void PopulateQuests()
        {
            Quest clearAlchemistGarden =
                new Quest(QUEST_ID_CLEAR_ALCHEMIST_GARDEN,
                "Clear the alchemis's garden",
                "Kill rats in the alchemist's garden and bring back 3 rat tails. You will receive a healing potion and 10 gold pieces.", 20, 10);

            clearAlchemistGarden.QuestCompletionItems.Add(new QuestCompletionItem(ItemByID(ITEM_ID_RAT_TAIL), 3));

            clearAlchemistGarden.RewardItem = ItemByID(ITEM_ID_HEALING_POTION);

            Quest clearFarmersField =
                new Quest(QUEST_ID_CLEAR_FARMERS_FIELD,
                "Clear the farmer's field",
                "Kill snakes in the farmer's field and bring back 3 snake fangs. You will receive an adventurer's pass and 20 gold pieces.", 20, 20);

            clearFarmersField.QuestCompletionItems.Add(new QuestCompletionItem(ItemByID(ITEM_ID_SNAKE_FANG), 3));

            clearFarmersField.RewardItem = ItemByID(ITEM_ID_ADVENTURER_PASS);


            _quests.Add(clearAlchemistGarden);
            _quests.Add(clearFarmersField);
        }

        private static void PopulateLocations()
        {
            //vendors
            Vendor bobTheRatCatcher = new Vendor("Bob the Rat-Catcher");
            bobTheRatCatcher.AddItemToInventory(ItemByID(ITEM_ID_PIECE_OF_FUR), 5);
            bobTheRatCatcher.AddItemToInventory(ItemByID(ITEM_ID_RAT_TAIL), 3);
            bobTheRatCatcher.AddItemToInventory(ItemByID(ITEM_ID_HEALING_POTION), 1);
            bobTheRatCatcher.AddItemToInventory(ItemByID(ITEM_ID_RUSTY_SHIELD), 1);
            bobTheRatCatcher.AddItemToInventory(ItemByID(ITEM_ID_LEATHER_CHEST), 1);

            //Creats each location
            Location home = new Location(LOCATION_ID_HOME, "Home:", narrative.HomeDetail);

            Location townSquare = new Location(LOCATION_ID_TOWN_SQUARE, "Town square:", narrative.TownSqrDetail);

            Location alchemistHut = new Location(LOCATION_ID_ALCHEMIST_HUT, "Alchemist's hut:", narrative.AlchemistHutDetail);

            Location alchemistsGarden = new Location(LOCATION_ID_ALCHEMISTS_GARDEN, "Alchemist's garden:", narrative.AlchemistGardenDetail);

            Location farmhouse = new Location(LOCATION_ID_FARMHOUSE, "Farmhouse:", narrative.FarmhouseDetail);

            Location farmersField = new Location(LOCATION_ID_FARM_FIELD, "Farmer's field:", narrative.FarmerFieldDetail);

            Location guardPost = new Location(LOCATION_ID_GUARD_POST, "Guard post:", narrative.GaurdPostDetail, ItemByID(ITEM_ID_ADVENTURER_PASS));

            Location bridge = new Location(LOCATION_ID_BRIDGE, "Bridge:", narrative.BridgeDetail);

            Location spiderField = new Location(LOCATION_ID_SPIDER_FIELD, "Forest:", narrative.ForrestDetail);


            //Adds Quests to loactaions
            alchemistHut.QuestAvailableHere = QuestByID(QUEST_ID_CLEAR_ALCHEMIST_GARDEN);
            farmhouse.QuestAvailableHere = QuestByID(QUEST_ID_CLEAR_FARMERS_FIELD);


            //Adds Monsters (NAME, % chance of appearing)
            alchemistsGarden.AddMonster(MONSTER_ID_RAT, 34);
            alchemistsGarden.AddMonster(MONSTER_ID_SNAKE, 33);
            alchemistsGarden.AddMonster(MONSTER_ID_GIANT_SPIDER, 33);

            farmersField.AddMonster(MONSTER_ID_SNAKE, 75);

            spiderField.AddMonster(MONSTER_ID_GIANT_SPIDER, 75);

            // Link the locations together
            home.LocationToNorth = townSquare;

            townSquare.LocationToNorth = alchemistHut;
            townSquare.LocationToSouth = home;
            townSquare.LocationToEast = guardPost;
            townSquare.LocationToWest = farmhouse;
            townSquare.VendorWorkingHere = bobTheRatCatcher; //vendor

            farmhouse.LocationToEast = townSquare;
            farmhouse.LocationToWest = farmersField;

            farmersField.LocationToEast = farmhouse;

            alchemistHut.LocationToSouth = townSquare;
            alchemistHut.LocationToNorth = alchemistsGarden;

            alchemistsGarden.LocationToSouth = alchemistHut;

            guardPost.LocationToEast = bridge;
            guardPost.LocationToWest = townSquare;

            bridge.LocationToWest = guardPost;
            bridge.LocationToEast = spiderField;
            bridge.VendorWorkingHere = bobTheRatCatcher; //vendor

            spiderField.LocationToWest = bridge;

            // Add the locations to the list
            _locations.Add(home);
            _locations.Add(townSquare);
            _locations.Add(guardPost);
            _locations.Add(alchemistHut);
            _locations.Add(alchemistsGarden);
            _locations.Add(farmhouse);
            _locations.Add(farmersField);
            _locations.Add(bridge);
            _locations.Add(spiderField);
        }

        //finds item based on ID
        public static Item ItemByID(int id)
        {
            return _items.SingleOrDefault(x => x.ID == id);
        }

        public static Monster MonsterByID(int id)
        {
            return _monsters.SingleOrDefault(x => x.ID == id);
        }

        public static Quest QuestByID(int id)
        {
            return _quests.SingleOrDefault(x => x.ID == id);
        }

        public static Location LocationByID(int id)
        {
            return _locations.SingleOrDefault(x => x.ID == id);
        }
    }
}