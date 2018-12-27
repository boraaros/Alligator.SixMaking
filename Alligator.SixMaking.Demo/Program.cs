using Alligator.SixMaking.Logics;
using Alligator.SixMaking.Model;
using Alligator.Solver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Alligator.SixMaking.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Hello tic-tac-toe demo!");

            IRules<IPosition, Ply> rules = new Rules(PliesPool.Instance, new MoveRules(), Disk.Red);
            ISolverConfiguration solverConfiguration = new SolverConfiguration();

            SolverFactory<IPosition, Ply> solverFactory = new SolverFactory<IPosition, Ply>(rules, solverConfiguration, SolverLog);
            ISolver<Ply> solver = solverFactory.Create();

            IPosition position = new Position();
            IList<Ply> history = new List<Ply>();
            bool aiStep = true;

            while (rules.LegalMovesAt(position).Any())
            {
                PrintPosition(position);
                Ply next;
                Position copy = new Position(position.History);

                if (aiStep)
                {
                    while (true)
                    {
                        try
                        {
                            next = AiStep(history, solver);
                            copy.Take(next);
                            break;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                }
                else
                {
                    while (true)
                    {
                        try
                        {
                            next = HumanStep();
                            copy.Take(next);
                            break;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                }
                position.Take(next);
                history.Add(next);
                aiStep = !aiStep;
            }
            if (!rules.IsGoal(position))
            {
                Console.WriteLine("Game over, DRAW!");
            }
            else
            {
                Console.WriteLine(string.Format("Game over, {0} WON!", aiStep ? "human" : "ai"));
            }

            PrintPosition(position);

            Console.ReadKey();
        }

        private static Ply HumanStep()
        {
            Console.Write("Next step [from:to:count]: ");
            while (true)
            {
                try
                {
                    string[] msg = Console.ReadLine().Split(':');
                    int from = int.Parse(msg[0]);
                    int to = int.Parse(msg[1]);
                    int count = int.Parse(msg[2]);
                    if (from == -1)
                    {
                        return PliesPool.Instance.GetInsertPly(to);
                    }
                    else
                    {
                        return PliesPool.Instance.GetMovePly(from, to, count);
                    }         
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        private static Ply AiStep(IList<Ply> history, ISolver<Ply> solver)
        {
            var next = solver.OptimizeNextMove(history);

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("AI is thinking...");
            //Console.WriteLine(string.Format("Evaluation value: {0} ({1})", evaluationValue, ToString(evaluationValue)));
            Console.WriteLine(string.Format("Optimal next step: {0}", next));
            //Console.WriteLine(string.Format("Forecast: {0}", string.Join(" ## ", forecast)));
            Console.ForegroundColor = ConsoleColor.White;

            return next;
        }

        private static string ToString(int evaluationValue)
        {
            return evaluationValue.ToString();
        }

        private static void PrintPosition(IPosition position)
        {
            Console.WriteLine();

            for (int i = 0; i < Constants.BoardSize; i++)
            {
                for (int j = 0; j < Constants.BoardSize; j++)
                {
                    for (int k = 0; k < 10; k++)
                    {
                        switch (position.DiskAt(Constants.BoardSize * i + j, k))
                        {
                            case Disk.None:
                                Console.Write(string.Format(" {0}", "."));
                                break;
                            case Disk.Red:
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write(string.Format(" {0}", "|"));
                                break;
                            case Disk.Yellow:
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.Write(string.Format(" {0}", "|"));
                                break;
                            default:
                                throw new ArgumentOutOfRangeException($"Unknown disk type: {position.DiskAt(Constants.BoardSize * i + j, k)}");
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    Console.Write(" ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        private static void SolverLog(string message)
        {
            var prevColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(string.Format("[SolverLog] {0}", message));
            Console.ForegroundColor = prevColor;
        }
    }
}