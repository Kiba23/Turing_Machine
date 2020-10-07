using System.Collections.Generic;
using System.Text;

namespace TuringMachine
{
    public class Tape
    {
        public List<char> tapeString = new List<char>();

        public void Input(string input)
        {
            tapeString.AddRange(input);
        }

        public void IncreaseForwardString()
        {
            tapeString.Add(' ');
        }

        public void IncreaseBackwardString()
        {
            tapeString.Insert(0, ' ');
        }

        public string PrintTapeString()
        {
            var sb = new StringBuilder();
            foreach (var it in tapeString)
            {
                sb.Append(it);
            }
            return sb.ToString();
        }
    }
}