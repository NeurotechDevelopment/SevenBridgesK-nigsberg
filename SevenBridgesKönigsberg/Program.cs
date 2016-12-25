﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SevenBridgesKönigsberg
{
    class Program
    {
        static EulerCycleBacktrackSolver _solver = new EulerCycleBacktrackSolver(CreateTwoTrianglesGraph());

        static void Main(string[] args)
        {
            var solutions = _solver.FindCycles();
            
            if (solutions.Count > 0)
            {
                ConsoleWriter.WriteLine("The following solutions found:");
                foreach(var path in solutions)
                {
                    Console.WriteLine(path);
                }
            }
            else
            {
                ConsoleWriter.WriteLine("No solutions found", ConsoleWriter.TextColor.Yellow);
            }
            
            Console.WriteLine("Press ENTER to exit.");
            Console.ReadLine();
        }

        static Dictionary<char, string[]> CreateSquareGraph()
        {
            Dictionary<char, string[]> incidentEdges = new Dictionary<char, string[]>();
            incidentEdges['a'] = new[] { "e1", "e4" };
            incidentEdges['b'] = new[] { "e1", "e2" };
            incidentEdges['c'] = new[] { "e2", "e3" };
            incidentEdges['d'] = new[] { "e3", "e4" };           

            return incidentEdges;
        }

        static Dictionary<char, string[]> CreateTwoTrianglesGraph()
        {
            Dictionary<char, string[]> incidentEdges = new Dictionary<char, string[]>();
            incidentEdges['a'] = new[] { "e1", "e3" };
            incidentEdges['b'] = new[] { "e3", "e2", "e4", "e6" };
            incidentEdges['c'] = new[] { "e5", "e6" };
            incidentEdges['d'] = new[] { "e4", "e5" };
            incidentEdges['e'] = new[] { "e1", "e2" };

            return incidentEdges;
        }

        static Dictionary<char, string[]> CreateKoenegsbergGraph()
        {
            Dictionary<char, string[]> incidentEdges = new Dictionary<char, string[]>();
            incidentEdges['a'] = new[] { "e1", "e2", "e5" };
            incidentEdges['b'] = new[] { "e1", "e2", "e3", "e4", "e6" };
            incidentEdges['c'] = new[] { "e3", "e4", "e7" };
            incidentEdges['d'] = new[] { "e5", "e6", "e7" };
            return incidentEdges;
        }
    }    
    
    /// <summary>
    /// Finds all Euler cycles on a given fully connected graph.
    /// </summary>
    public class EulerCycleBacktrackSolver
    {
        readonly char[] _vertices;
        readonly int _edgeCount;

        readonly Dictionary<char, string[]> _incidentEdges = new Dictionary<char, string[]>();
        readonly List<string> solutions = new List<string>();

        public EulerCycleBacktrackSolver(Dictionary<char, string[]> incidentEdges)
        {
            _vertices = incidentEdges.Keys.ToArray();
            _incidentEdges = incidentEdges;
            _edgeCount = incidentEdges.Values.SelectMany(edges => edges).Distinct().Count();           
        }

        public List<string> FindCycles()
        {
            solutions.Clear();
            
            // Walk over each vertex as starting vertex
            foreach (char v0 in _vertices)
            {
                ConsoleWriter.WriteLine("Starting with vertex " + v0);

                FindRemainingPath(v0, v0);
            }

            return solutions;
        }

        /// <summary>
        /// Recursively looks for path from currentVertex to initialVertex via edges that are not yet marked.
        /// Fills solutions field member.
        /// </summary>
        /// <returns>True if at least one solution was found.</returns>
        private bool FindRemainingPath(char initialVertex, char currentVertex, List<string> _markedEdges = null)
        {
            List<string> markedEdges = _markedEdges != null ? _markedEdges.ToList() : new List<string>();
            if (initialVertex == currentVertex && markedEdges.Count == _edgeCount)
            {
                // Solution found
                string solution = BuildSolutionString(initialVertex, markedEdges);
                solutions.Add(solution);
                Console.WriteLine();
                ConsoleWriter.WriteLine("Solution found:" + solution, ConsoleWriter.TextColor.Green);
                return true;
            }

            var unmarkedIncidentEdges = _incidentEdges[currentVertex].Except(markedEdges);
            if (unmarkedIncidentEdges.Count() == 0)
            {
                Console.WriteLine();
                ConsoleWriter.WriteLine(string.Format("Dead end: cannot exit vertex {0}.", currentVertex), ConsoleWriter.TextColor.Yellow);
                return false;
            }

            foreach (string e in unmarkedIncidentEdges)
            {
                if (markedEdges.Count == 0)
                {
                    Console.Write(currentVertex + " ");
                }
                markedEdges.Add(e);
               
                Console.Write(e + " ");

                // Locate vertex wchich is not currentVertex and is incident to edge e 
                char incidentVertex = GetIncidentVertex(e, currentVertex);

                bool isSolutionFound = FindRemainingPath(initialVertex, incidentVertex, markedEdges);

                if (!isSolutionFound)
                {
                    // Dead end reached. Unmark edge and continue looking for alternatives
                    Console.WriteLine();
                    ConsoleWriter.WriteLine("Backtracking...", ConsoleWriter.TextColor.Yellow);
                    Console.Write(BuildSolutionString(initialVertex, markedEdges));
                }
                markedEdges.Remove(e);
            }

            return solutions.Count > 0;
        }

        private string BuildSolutionString(char initialVertex, List<string> markedEdges)
        {
            StringBuilder vertexEdges = new StringBuilder();
            char currentVertex = initialVertex;
            vertexEdges.Append(initialVertex);
            vertexEdges.Append(' ');
            
            foreach (var e in markedEdges)
            {
                vertexEdges.Append(e);
                vertexEdges.Append(' ');
                currentVertex = GetIncidentVertex(e, currentVertex);
                vertexEdges.Append(currentVertex.ToString());
                vertexEdges.Append(' ');
            }
            return vertexEdges.ToString();
        }

        private char GetIncidentVertex(string edge, char excludeVertex)
        {
            return _incidentEdges.Single(k => k.Key != excludeVertex && k.Value.Contains(edge)).Key;
        }
    }
    
    public static class ConsoleWriter
    {
        public enum TextColor { Default, Yellow, Green };
        static readonly ConsoleColor _defaultColor = Console.ForegroundColor;

        public static void WriteLine(string message, TextColor textColor = TextColor.Default)
        {
            SetConsoleColor(textColor);
            Console.WriteLine(message);
            RestoreConsoleColor();
        }

        private static void SetConsoleColor(TextColor textColor)
        {
            Console.ForegroundColor = GetColor(textColor);
        }

        private static void RestoreConsoleColor()
        {
            Console.ForegroundColor = _defaultColor;
        }

        private static ConsoleColor GetColor(TextColor textColor)
        {
            switch(textColor)
            {
                case TextColor.Default:return _defaultColor;
                case TextColor.Green:return ConsoleColor.Green;
                case TextColor.Yellow:return ConsoleColor.Yellow;
                default:throw new NotImplementedException(string.Format("Color '{0}' is not supported.", textColor));
            }
        }
    }
}
