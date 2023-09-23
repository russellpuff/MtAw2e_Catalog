// Spell
// File defines the Spell and AddOn classes for storing data regarding a single spell. 

namespace MtAw2e_Catalog
{
    [Serializable]
    public struct AddOn
    {
        public string Requirement;
        public string Effect;
        public AddOn() { Requirement = Effect = ""; }
    }
    [Serializable]
    public class Spell
    {
        public string Name;
        public string Arcana;
        public string Description;
        public string Practice;
        public string PrimaryFactor;
        public string Withstand;
        public string Cost;
        public List<AddOn> AddOns;
        public string guid;

        public Spell()
        {
            Name = Arcana = Description = Practice = PrimaryFactor = Withstand = Cost = guid = "";
            AddOns = new();
        }
    }
}
