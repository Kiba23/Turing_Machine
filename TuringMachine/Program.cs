using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TuringMachine
{
    class TuringMachine
    {
        static async Task Main(string[] args)
        {
            var program = new TuringMachine();

            await program.Menu();
            
            Console.ReadLine();
        }

        private Tape tape = new Tape();
        private State state = new State();
        private Head head = new Head();
        private TuringMachine obj;
        private List<TuringMachine> eachFileObj = new List<TuringMachine>();
        private Dictionary<(string initialState, char read), (char write, char move, string toState)> instructions 
            = new Dictionary<(string initialState, char read), (char write, char move, string toState)>();
        private string file;

        private void Run()
        {
            state.currentState = instructions.Keys.ElementAt(0).initialState; // start point

            while (!head.success)
            {
                Step();
            }
        }

        private async Task RunAsync()
        {
            state.currentState = instructions.Keys.ElementAt(0).initialState; // start point

            Task.Run(() =>
            {
                while (!head.success) { Step(); }
            });
        }

        private void Step()
        {
            // Main logic for overriding current character
            if (instructions.TryGetValue((state.currentState, tape.tapeString[head.currentPosition]), out var values))
            {
                tape.tapeString[head.currentPosition] = values.write;
                head.MoveNext(values.move);
                state.currentState = values.toState;
                state.steps++;
                head.IfSuccess(state.currentState);
            }
            else
            {
                // Logic for * operator
                if (instructions.TryGetValue((state.currentState, '*'), out var starValues))
                {
                    head.MoveNext(starValues.move);
                    state.currentState = starValues.toState;
                    state.steps++;
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

            // Displaying all main info - whole tape, current state position and steps
            Display();
        }

        private void CheckForIncreaseString()
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

        private async Task Menu()
        {
            Console.WriteLine("--- MENU --- \n\n");
            Console.Write("Enter the name of files: ");
            string line = Console.ReadLine();
            string[] files = line.Split(' ');
            Console.WriteLine("Write asynchronously? (Y/N)");

            // Creating object corresponding each file (amount of obj. = amoung of files)
            foreach (var file in files)
            {
                obj = new TuringMachine();
                obj.file = file;
                obj.ParseFile();
                eachFileObj.Add(obj);
            }

            // Async or Not? - from user
            if (char.TryParse(Console.ReadLine(), out char checkAsync))
            {
                switch (checkAsync)
                {
                    case 'Y':
                        foreach (var obj in eachFileObj)
                        {
                            await obj.RunAsync();
                        }
                        break;
                    case 'N':
                        foreach (var obj in eachFileObj)
                        {
                            obj.Run();
                        }
                        break;
                    default:
                        throw new Exception("Wrong letter");
                }
            }
            else
            {
                throw new InvalidCastException("Wrong input!");
            }
        }

        private void ParseFile()
        {
            FileInfo fi = new FileInfo(this.file);

            if (fi.Exists)
            {
                Regex parts = new Regex(@"\w . . . \w"); // Pattern

                using (StreamReader sr = fi.OpenText())
                {
                    tape.Input(sr.ReadLine()); // tapeString initializing
                    sr.ReadLine(); // Skiping one line
                    head.currentPosition = Int32.Parse(sr.ReadLine()) - 1; // head start point initializing

                    string s = "";
                    string[] parameters;
                    while ((s = sr.ReadLine()) != null)
                    {
                        Match match = parts.Match(s);
                        if (match.Success)
                        {
                            parameters = s.Split(' ');
                            this.instructions.Add((parameters[0], char.Parse(parameters[1])),  // Initializing instructions
                                (char.Parse(parameters[2]), char.Parse(parameters[3]), parameters[4]));
                        }
                    }
                }
            }
            else
            {
                throw new FileNotFoundException("File not found");
            }
        }

        private void Display()
        {
            Console.WriteLine();
            Console.WriteLine(tape.PrintTapeString());
            Console.WriteLine(head.OutputHeadPosition());
            Console.WriteLine("step: " + state.steps);
            Console.WriteLine();

            if (head.success) // Outputs only at the end
            {
                Console.WriteLine("\n--- SUCCESS ---\n");
            }
        }
    }
}