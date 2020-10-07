using System;
using System.Collections.Generic;
using System.Linq;

namespace TuringMachine
{
    class TuringMachine
    {
        static void Main(string[] args)
        {
            var first = new TuringMachine();

            first.Instructions(
            ("0", ' ', ' ', 'R', "1"),
            ("0", '*', '*', 'R', "0"),
            ("1", ' ', ' ', 'L', "2"),
            ("1", '*', '*', 'R', "1"),
            ("2", '0', ' ', 'L', "3x"),
            ("2", '1', ' ', 'L', "3y"),
            ("2", ' ', ' ', 'L', "7"),
            ("3x", ' ', ' ', 'L', "4x"),
            ("3x", '*', '*', 'L', "3x"),
            ("3y", ' ', ' ', 'L', "4y"),
            ("3y", '*', '*', 'L', "3y"),
            ("4x", '0', 'x', 'R', "0"),
            ("4x", '1', 'y', 'R', "0"),
            ("4x", ' ', 'x', 'R', "0"),
            ("4x", '*', '*', 'L', "4x"),
            ("4y", '0', '1', '*', "5"),
            ("4y", '1', '0', 'L', "4y"),
            ("4y", ' ', '1', '*', "5"),
            ("4y", '*', '*', 'L', "4y"),
            ("5", 'x', 'x', 'L', "6"),
            ("5", 'y', 'y', 'L', "6"),
            ("5", ' ', ' ', 'L', "6"),
            ("5", '*', '*', 'R', "5"),
            ("6", '0', 'x', 'R', "0"),
            ("6", '1', 'y', 'R', "0"),
            ("7", 'x', '0', 'L', "7"),
            ("7", 'y', '1', 'L', "7"),
            ("7", ' ', ' ', 'R', "halt"),
            ("7", '*', '*', 'L', "7")
                )
                .Input("110110 101011");

            first.Run();

            Console.WriteLine("\n--- SUCCESS ---\n");
            Console.ReadLine();
        }

        private Tape tape = new Tape();
        private State state = new State();
        private Head head = new Head();
        private Dictionary<(string initialState, char read), (char write, char move, string toState)> instructions;
        private int steps = 0;

        public TuringMachine Instructions(
            params (string initialState, char read, char write, char move, string toState)[] instructions)
        {
            this.instructions = instructions.ToDictionary(i => (i.initialState, i.read),
                                                                i => (i.write, i.move, i.toState));
            return this;
        }

        public TuringMachine Input(string input)
        {
            tape.Input(input);
            return this;
        }

        public void Run()
        {
            head.currentPosition = 0;
            state.currentState = instructions.Keys.ElementAt(0).initialState;

            while (!head.success)
            {
                Step();
            }
        }

        public void Step()
        {
            // Main logic for overriding current character
            if (instructions.TryGetValue((state.currentState, tape.tapeString[head.currentPosition]), out var values))
            {
                tape.tapeString[head.currentPosition] = values.write;
                head.MoveNext(values.move);
                state.currentState = values.toState;
                steps++;
                head.IfSuccess(state.currentState);
            }
            else
            {
                // Logic for * operator
                if (instructions.TryGetValue((state.currentState, '*'), out var starValues))
                {
                    head.MoveNext(starValues.move);
                    state.currentState = starValues.toState;
                    steps++;
                    head.IfSuccess(state.currentState);
                }
                // If nothing is matched Exception would be thrown
                else
                {
                    throw new Exception("Haven't Find The Right Keys");
                }   
            }

            // Increasing string if the it's out of range
            CheckForIncreaseString();

            Display();
        }

        public void CheckForIncreaseString()
        {
            if (tape.tapeString.Count <= head.currentPosition)
            {
                tape.IncreaseForwardString();
            }
            else if (head.currentPosition < 0)
            {
                tape.IncreaseBackwardString();
                head.currentPosition = 0;
            }
        }

        public void Display()
        {
            Console.WriteLine(tape.PrintTapeString());
            Console.WriteLine(head.OutputHeadPosition());
            Console.WriteLine("step: " + steps);
            Console.WriteLine();
        }
    }
}