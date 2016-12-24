using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SevenBridgesKönigsberg
{
    class Program
    {
        static KoenensbergSolver _solver = new KoenensbergSolver();

        static void Main(string[] args)
        {
            var solutions = _solver.SolveBridges();
            
            if (solutions.Count > 0)
            {
                Console.WriteLine("The following solutions found:");
                foreach(var path in solutions)
                {
                    Console.WriteLine(path);
                }
            }
            else
            {
                Console.WriteLine("No solutions found");
            }
            
            Console.WriteLine("Press ENTER to exit.");
            Console.ReadLine();
        }
    }
    
    public class KoenensbergSolver
    {
        readonly char[] _vertices = { 'a', 'b', 'c', 'd' };
        readonly string[] _edges = { "e1", "e2", "e3", "e4", "e5", "e6", "e7" };

        readonly Dictionary<char, string[]> _incidentEdges = new Dictionary<char, string[]>();
        readonly List<string> solutions = new List<string>();

        public KoenensbergSolver()
        {
            // Setup edge incidents
            
            _incidentEdges['a'] = new[] { "e1", "e2", "e5"};
            _incidentEdges['b'] = new[] { "e1", "e2", "e3", "e4", "e6" };
            _incidentEdges['c'] = new[] { "e3", "e4", "e7" };
            _incidentEdges['d'] = new[] { "e5", "e6", "e7" };
        }

        public List<string> SolveBridges()
        {
            solutions.Clear();
            
            // Walk over each vertex as starting vertex
            foreach (char v0 in _vertices)
            {
                Console.WriteLine("Starting with vertex " + v0);

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
            if (initialVertex == currentVertex && markedEdges.Count == _edges.Length)
            {
                // Solution found
                StringBuilder vertexEdges = new StringBuilder();
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
                solutions.Add(vertexEdges.ToString());
                return true;
            }

            Console.Write(currentVertex + " ");

            var unmarkedIncidentEdges = _incidentEdges[currentVertex].Except(markedEdges);
            if (unmarkedIncidentEdges.Count() == 0)
            {
                Console.WriteLine();
                Console.WriteLine("Dead end: cannot exit vertex {0}.", currentVertex);
                return false;
            }

            foreach (string e in unmarkedIncidentEdges)
            {
                markedEdges.Add(e);

                Console.Write(e + " ");

                // Locate vertex wchich is not currentVertex and is incident to edge e 
                char incidentVertex = GetIncidentVertex(e, currentVertex);

                bool isSolutionFound = FindRemainingPath(initialVertex, incidentVertex, markedEdges);

                if (!isSolutionFound)
                {
                    // Dead end reached. Unmark edge and continue looking for alternatives
                    markedEdges.Remove(e);
                }
            }

            return solutions.Count > 0;
        }

        private char GetIncidentVertex(string edge, char excludeVertex)
        {
            return _incidentEdges.Single(k => k.Key != excludeVertex && k.Value.Contains(edge)).Key;
        }
    }    
}
