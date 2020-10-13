using System;

namespace TuringMachine
{
    public class Head
    {
        public bool success { get; set; }
        public int currentPosition { get; set; }

        public void MoveNext(char side)
        {
            if (side == 'R')
            {
                currentPosition++;
            }
            else if (side == '*')
            {
                // do nothing
            }
            else
            {
                currentPosition--;
            }
        }

        public void IfSuccess(string currentState)
        {
            if (currentState == "H")
            {
                success = true;
            }
        }

        public string OutputHeadPosition()
        {
            string strike = "";
            for (int i = 0; i < currentPosition; i++)
            {
                strike += " ";
            }
            strike += "^";

            return strike;
        }
    }
}
