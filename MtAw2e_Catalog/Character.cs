// Character
// File defines the Character and Yantra classes for storing player character data. 

namespace MtAw2e_Catalog
{
    [Serializable]
    public struct Yantra
    {
        public string Description;
        public string DisplayName;
        public int DiceValue;
        public int ParadoxPenalty;
        public Yantra(string _desc, string _disp, int _value, int _penalty) 
        {
            DisplayName = _disp;
            Description = _desc; 
            DiceValue = _value;
            ParadoxPenalty = _penalty; // Must be zero or negative. Should be enforced via creation.
        }
    }
    [Serializable]
    public class Character
    {
        public string Name;
        public int Gnosis;
        public int Wisdom;
        public int DeathDots;
        public int FateDots;
        public int ForcesDots;
        public int LifeDots;
        public int MatterDots;
        public int MindDots;
        public int PrimeDots;
        public int SpaceDots;
        public int SpiritDots;
        public int TimeDots;
        public int AimedOffense;
        public List<Yantra> Yantras;
        public int[] RulingArcana;

        public Character(string _name, int _rule1, int _rule2, int _rule3)
        {
            Name = _name;
            Gnosis = DeathDots = FateDots = ForcesDots = LifeDots = MatterDots = MindDots =
                  PrimeDots = SpaceDots = SpiritDots = TimeDots = AimedOffense = Wisdom = 0;
            Yantras = new();
            RulingArcana = new int[3] { _rule1, _rule2, _rule3 };
        }
    }
}