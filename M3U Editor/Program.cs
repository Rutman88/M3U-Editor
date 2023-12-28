// See https://aka.ms/new-console-template for more information
using M3U_Editor;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using static M3U_Editor.Commands;
using Microsoft.Win32;
string AppName = "M3UEditor";
bool IsStartup()
{
    RegistryKey rk = Registry.CurrentUser.OpenSubKey
        ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

    if (rk == null || rk.GetValue(AppName, null) == null)
    {
        return false;
    }
    return true;
}
void SetStartup()
{
    RegistryKey rk = Registry.CurrentUser.OpenSubKey
        ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
    

    if (!IsStartup())
        rk.SetValue(AppName, "\""+Environment.GetCommandLineArgs()[0]+"\"" + " --filter");
    else
        rk.DeleteValue(AppName, false);
}

Regex rx = new Regex("""group-title="([^"]*)" """.Trim(), RegexOptions.Compiled | RegexOptions.IgnoreCase);
//Console.WriteLine(args.GetValue(0).ToString());
//new StreamReader("C:\\Users\\Cutch\\Downloads\\commands").ReadToEnd().Split("\n");

String localFile = AppDomain.CurrentDomain.BaseDirectory+"playlist.m3u";
Commands cmds = new Commands();
List<string> GetGroups()
{
    HashSet<string> groups = new HashSet<string>();
    using (StreamReader sw = new StreamReader(localFile))
    {
        String line;
        while ((line = sw.ReadLine()) != null)
        {
            if (line.StartsWith("#EXTINF"))
            {
                String groupTitle = rx.Match(line).Groups[1].Value;
                groups.Add(groupTitle);
            }
        }
    }
    List<string> groupList = groups.ToList();
    groupList.Sort();
    return groupList;
}
void ListGroups()
{
    List<string> groupList = GetGroups();
    foreach (string g in groupList)
    {
        string group = g;
        foreach (ReplaceCommand rc in cmds.GetReplaceList())
        {
            group = rc.search.Replace(group, rc.replace);
        }
        Console.WriteLine(group);
    }
}
void FilterGroups()
{
    List<string> groupList = GetGroups();
    Dictionary<string, bool> dt = new Dictionary<string, bool>();
    foreach (string group in groupList)
    {
        dt.Add(group, !cmds.GetSkipList().Contains(group));
    }
    dt = MenuSelect.MultipleChoice(dt);
    List<bool> values = dt.Values.ToList();
    List<string> keys = dt.Keys.ToList();

    HashSet<string> skipList = new HashSet<string>();
    for (int i = 0; i < values.Count; i++)
    {
        if (values[i] == false)
        {
            skipList.Add(keys[i]);
        }
    }
    cmds.SetSkipList(skipList);
    cmds.WriteCommands();
}
void ReplaceGroups()
{
    Console.WriteLine("");
    Console.Write("Search: ");
    string search = Console.ReadLine();
    Console.Write("Replace: ");
    string replace = Console.ReadLine();
    if (search != null)
    {
        cmds.GetReplaceList().Add(new ReplaceCommand(search, replace == null ? "" : replace));
        cmds.WriteCommands();
    }
}
void SetRemoteUrl()
{
    Console.WriteLine("");
    Console.Write("Url: ");
    string url = Console.ReadLine();
    cmds.SetRemoteUrl(url);
    DownloadPlaylist();
    cmds.WriteCommands();
}
void SetOutpath()
{
    Console.WriteLine("");
    Console.Write("Output Path: ");
    string url = Console.ReadLine();
    cmds.SetOutPath(url);
    cmds.WriteCommands();
}
void DownloadPlaylist()
{
    using (WebClient myWebClient = new WebClient())
    {
        myWebClient.DownloadFile(cmds.GetRemoteUrl(), localFile);
    }
}
void FilterPlaylist()
{
    long channelCount = 0;
    using (StreamWriter writer = new StreamWriter(cmds.GetOutPath()))
    {
        bool getNextExtInf = false;
        using (StreamReader sw = new StreamReader(localFile))
        {
            String line;
            while ((line = sw.ReadLine()) != null)
            {
                if (line.StartsWith("#EXTINF"))
                {
                    getNextExtInf = false;
                    String groupTitle = rx.Match(line).Groups[1].Value;
                    if (cmds.GetSkipList().Contains(groupTitle))
                    {
                        getNextExtInf = true;
                    }
                    if (!getNextExtInf)
                    {
                        channelCount++;
                        foreach (ReplaceCommand rc in cmds.GetReplaceList())
                        {
                            line = rc.search.Replace(line, rc.replace);
                        }
                        writer.WriteLine(line);
                    }
                }
                else
                {
                    if (!getNextExtInf)
                    {
                        writer.WriteLine(line);
                    }
                }
            }
        }
    }
    Console.WriteLine($"{channelCount} channels");
}
if (!File.Exists(localFile) || File.GetLastWriteTimeUtc(localFile) < DateTime.UtcNow.AddDays(-1))
{
    DownloadPlaylist();
}
if (!File.Exists(localFile))
{
    Console.WriteLine("Could not download the m3u file. Set it if you havenot already.");
}
if (args.Count() > 0 && args.GetValue(0).ToString().Equals("--filter"))
{
    FilterPlaylist();
    return;
}
char option='0';
do
{
    switch (option)
    {
        case '1':
            ListGroups();
            break;
        case '2':
            FilterGroups();
            break;
        case '3':
            ReplaceGroups();
            break;
        case '4':
            SetRemoteUrl();
            break;
        case '5':
            SetOutpath();
            break;
        case '6':
            SetStartup();
            break;
        case '9':
            FilterPlaylist();
            break;
    }


    Console.WriteLine("");
    Console.WriteLine("Options:");
    Console.WriteLine("1. List");
    Console.WriteLine("2. Enable/Disable Groups");
    Console.WriteLine("3. Add Group Replace Filter");
    Console.WriteLine("4. Set Remote URL");
    Console.WriteLine("5. Set Output path");
    Console.WriteLine("6. {0} to Startup", IsStartup() ? "Remove" : "Add");
    Console.WriteLine("9. Manually run Playlist filter");
    Console.WriteLine("q. Exit");
    Console.WriteLine("");
    Console.Write("Enter: ");

} while ((option = Console.ReadLine()[0]) != 'q');

