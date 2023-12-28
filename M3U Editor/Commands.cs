using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static M3U_Editor.Commands;

namespace M3U_Editor
{
    internal class Commands
    {
        String commandFile = AppDomain.CurrentDomain.BaseDirectory+"commands.txt";
        private HashSet<string> skipList = new HashSet<string>();
        private List<ReplaceCommand> replaceList = new List<ReplaceCommand>();
        string remoteUrl = "";
        string outPath = "";
        public struct ReplaceCommand
        {
            public Regex search;
            public string replace;
            public ReplaceCommand(string search, string replace)
            {
                this.search = new Regex(search,RegexOptions.ECMAScript);
                this.replace = replace;
            }
        }
        internal Commands() {
            GetCommands();
        }

        public void GetCommands()
        {
            if (File.Exists(commandFile))
            {
                foreach (String line in new StreamReader(commandFile).ReadToEnd().Split("\n").ToList())
                {
                    string[] arr = line.Replace("\r", "").Split("\t");
                    if (arr.Length > 0)
                    {
                        string command = arr[0];
                        if (command.Equals("SKIP"))
                        {
                            skipList.Add(arr[1]);
                        }
                        else if (command.Equals("REPLACE"))
                        {
                            replaceList.Add(new ReplaceCommand(arr[1], arr[2]));
                        }
                        else if (command.Equals("URL"))
                        {
                            remoteUrl = arr[1];
                        }
                        else if (command.Equals("OUT"))
                        {
                            outPath = arr[1];
                        }
                    }
                }
            }
        }
        public void WriteCommands()
        {
            using (StreamWriter writer = new StreamWriter(commandFile))
            {
                foreach (String line in skipList)
                {
                    writer.WriteLine("SKIP\t" + line);
                }
                foreach (ReplaceCommand rc in replaceList)
                {
                    writer.WriteLine("REPLACE\t" + rc.search.ToString() + "\t"+rc.replace);
                }
                writer.WriteLine("URL\t" + remoteUrl);
                writer.WriteLine("OUT\t" + outPath);
            }
        }
        public string GetRemoteUrl()
        {
            return remoteUrl;
        }
        public void SetRemoteUrl(string remoteUrl)
        {
            this.remoteUrl = remoteUrl;
        }
        public string GetOutPath()
        {
            return outPath;
        }
        public void SetOutPath(string outPath)
        {
            this.outPath = outPath;
        }
        public HashSet<string> GetSkipList()
        {
            return skipList;
        }
        public List<ReplaceCommand> GetReplaceList()
        {
            return replaceList;
        }
        public void SetSkipList(HashSet<string> s)
        {
            skipList = s;
        }
        public void SetReplaceList(List<ReplaceCommand> rc)
        {
            replaceList = rc;
        }
    }
}
