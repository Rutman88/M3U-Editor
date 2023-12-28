using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace M3U_Editor
{
    internal class MenuSelect
    {
        public static Dictionary<string, bool> MultipleChoice(Dictionary<string, bool> selection)
        {

            int currentSelection = 0;
            //TextWriter oldOut = Console.Out;
            //TextWriter bufOut = new StringWriter();

            ConsoleKey key;
            Console.CursorVisible = false;
            List<string> keys = selection.Keys.ToList();
            do
            {
                Console.Clear();
                //Console.SetOut(bufOut);
                int start = Math.Max(currentSelection - 5, 0);
                for (int i = start; i < Math.Min(start + 20, selection.Keys.Count); i++)
                {
                    //Console.SetCursorPosition(startX + (i % optionsPerLine) * spacingPerLine, startY + i / optionsPerLine);

                    
                    if (selection.Values.ToList()[i] == true)
                        Console.ForegroundColor = currentSelection == i ? ConsoleColor.Cyan : ConsoleColor.DarkCyan;
                    else
                        Console.ForegroundColor = currentSelection == i ? ConsoleColor.Yellow : ConsoleColor.DarkRed;
                    


                    Console.WriteLine(keys[i]);

                    Console.ResetColor();
                }

                //Console.SetOut(oldOut);
                //Console.WriteLine(bufOut);

                key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        {
                            if (currentSelection >= 0)
                                currentSelection --;
                            break;
                        }
                    case ConsoleKey.DownArrow:
                        {
                            if (currentSelection < selection.Keys.Count)
                                currentSelection ++;
                            break;
                        }
                    case ConsoleKey.Enter:
                        {
                            selection[keys[currentSelection]] = !selection[keys[currentSelection]];
                            break;
                        }
                }
            } while (key != ConsoleKey.Escape && key != ConsoleKey.Q);

            Console.CursorVisible = true;

            return selection;
        }
    }
}
