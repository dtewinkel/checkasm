using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amberfish.Canvas.Parsers
{
    public class DotParser
    {
        private readonly Dictionary<string, bool> uniqueNodes = new Dictionary<string, bool>();
        private readonly List<Tuple<string, string>> edges = new List<Tuple<string, string>>();

        public DotParser()
        {
        }

        public void Load(string dotGraph)
        {
            uniqueNodes.Clear();
            edges.Clear();

            var dotEdges = dotGraph.Split(';');
            foreach (var dotEdge in dotEdges)
            {
                if (!dotEdge.Contains("->"))
                    continue;

                var parts = dotEdge.Replace("->","?").Split('?');
                var nodeName = GetNodeName(parts[0]);
                if (!uniqueNodes.ContainsKey(nodeName))
                {
                    uniqueNodes.Add(nodeName, true);
                }
                var nodeName2 = GetNodeName(parts[1]);
                if (!uniqueNodes.ContainsKey(nodeName2))
                {
                    uniqueNodes.Add(nodeName2, true);
                }
                edges.Add(new Tuple<string, string>(nodeName, nodeName2));
            }
        }

        private string GetNodeName(string str)
        {
            return str.Trim().Trim('\"');
        }

        public List<string> GetNodes()
        {
            return uniqueNodes.Keys.ToList();
        }

        public List<Tuple<string,string>> GetEdges()
        {
            return edges;
        }


    }
}
