using System.Diagnostics;

namespace TuringMachine
{
    public class State
    {
        public string currentState { get; set; }

        // Additional variable:
        public int steps = 0;
    }
}
