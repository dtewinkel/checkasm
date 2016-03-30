using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace CheckAsm
{
    public class Search
    {
        List<string> phrases = new List<string>();

        public List<string> Phrases
        {
            get { return phrases; }
            set { phrases = value; }
        }

        /// <summary>
        /// Searches tree for a phrase. Starting from the rootNode
        /// </summary>
        /// <param name="phrase">string to search</param>
        /// <param name="rootNode">rootNode</param>
        public List<TreeNode> Find(string phrase, TreeNode rootNode)
        {
            if (!phrases.Contains(phrase))
            {
                phrases.Add(phrase);
            }
            if (phrases.Count > 20)
                phrases.RemoveAt(0);
            return find(phrase, rootNode);
        }

        /// <summary>
        /// Searches tree for a phrase. Starting from the rootNode
        /// </summary>
        /// <param name="phrase">string to search</param>
        /// <param name="rootNode">rootNode</param>
        private List<TreeNode> find(string phrase, TreeNode rootNode)
        {
            List<TreeNode> ret = new List<TreeNode>();
            if (rootNode != null)
            {
                if (rootNode.Text.IndexOf(phrase, StringComparison.InvariantCultureIgnoreCase) != -1)
                {
                    ret.Add(rootNode);
                }
                foreach (TreeNode node in rootNode.Nodes)
                {
                    ret.AddRange(Find(phrase, node));
                }
            }
            return ret;
        }

    }
}
