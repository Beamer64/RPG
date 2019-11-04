namespace Engine
{
    class Armor : Item
    {
        public int MaximumDefense { get; set; }

        public Armor(int id, string name, string namePlural, int maximumDefense, int price) : base(id, name, namePlural, price)
        {
            MaximumDefense = maximumDefense;
        }
    }
}