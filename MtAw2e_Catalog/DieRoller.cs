// File provides definitions for the DieRoller class. 
// Utilizes the Storyteller System's d10 style. 

namespace CofDDice
{
    public class DieRoller
    {
        public DieRoller(int _diceNum, bool _isRote, int _rollAgain)
        {
            dice = _diceNum;
            rote = _isRote;
            rollAgain = (Again)_rollAgain;
            chance = (dice <= 0);
            if (chance) { dice = 1; }
        }
        private enum Again
        {
            TenAgain = 10,
            NineAgain = 9,
            EightAgain = 8
        }
        private readonly int dice = 0;
        private readonly bool rote;
        private readonly bool chance;
        private readonly Again rollAgain = 0;
        public int successes = 0;

        public string Roll()
        {
            var random = new Random();
            List<int> rolls = new();
            List<int> arolls = new();
            string m = chance ? "Chance die" : "Dice";
            m+= " rolled: [";

            for (int i = 1; i <= dice; i++) { rolls.Add(random.Next(1, 11)); }
            
            // If a chance die rolls a 1, it does not get rerolled.
            // Chance dice do not benefit from the rollAgain property.
            if (!chance || (chance && (rolls[0] != 1)))
            {
                foreach (int r in rolls)
                {
                    if (((r >= (int)rollAgain) && !chance) || (rote && (r < 8))) { arolls.Add(random.Next(1, 11)); }
                }
                rolls.AddRange(arolls); // Can't edit rolls in a foreach.
            }
            
            foreach (int r in rolls)
            {
                m += r.ToString() + ", ";
                if (((r >= 8) && !chance) || (chance && (r == 10))) { ++successes; }
            }

            m = m[..^2]; // Substring, remove last comma and space.
            m += "]\nSuccesses: " + successes.ToString();

            return m;
        }
    }
}