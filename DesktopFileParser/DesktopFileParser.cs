using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DesktopFileParser
{
    //Goals
    //Load a file
    //Return Values in Dictionaries inside Dictionaries
    //Enable Adding Values
    //Enable Removing Values
    //Writing new Desktop Files
    public class Parser
    {
        /// <summary>
        /// Loads a .desktop file and parses it, returning it as a set of Dictionaries.
        /// </summary>
        /// <param name="path">Path to the .desktop file on your disk</param>
        /// <returns></returns>
        public Dictionary<string, Dictionary<string, string>> LoadFile(string path)
        {
            Dictionary<string, Dictionary<string, string>> DesktopFile = new Dictionary<string, Dictionary<string, string>>();
            bool inside = false;
            string name = "";
            string comment = "";
            int commentNumber = 0;
            int emptyLineNumber = 0;
            foreach (var line in File.ReadLines(path))
            {
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    string Name = line.Replace("[", "");
                    Name = Name.Replace("]", "");
                    DesktopFile.Add(Name, new Dictionary<string, string>());
                    inside = true;
                    name = Name;
                }

                else if (line.StartsWith("#"))
                {
                    if (inside == true)
                    {
                        comment = line;
                        commentNumber += 1;
                        DesktopFile[name].Add("comment" + commentNumber, comment);
                    }
                }
                else if (string.IsNullOrWhiteSpace(line))
                {
                    //Empty Line
                    if (inside == true)
                    {
                        DesktopFile[name].Add("emptyline" + emptyLineNumber, "");
                    }
                }
                else
                {
                    if (inside == false)
                    {
                        //Uh. The desktop file is *technically* invalid. We're going to ignore this bullshit.
                        //                                 Thanks.
                        //  https://specifications.freedesktop.org/desktop-entry-spec/latest/ar01s03.html
                    }
                    else
                    {
                        string[] thing = line.Split("=");
                        if (!DesktopFile[name].ContainsKey(thing[0]))
                        {
                            DesktopFile[name].Add(thing[0], thing[1]);
                        }
                        else
                        {
                            //Ignoring duplicated entries. For example, Discord has two entries for `Path`.
                        }
                        
                    }
                }
            }
            return DesktopFile;
        }
        
        /// <summary>
        /// Takes your Desktop File layout and generates a standards compliant .desktop file.
        /// Parses any key that starts with "comment" as a comment;
        /// Parses any key that starts with "emptyline" as an empty line (carriage return).
        /// </summary>
        /// <param name="desktopFile">The dictionary layout from LoadFile</param>
        /// <returns></returns>
        public string SaveFile (Dictionary<string, Dictionary<string, string>> desktopFile)
        {
            string Final = "";
            string currentPart = "";
            foreach (var DesktopKey in desktopFile) //Each Top Level Key
            {
                Final += "[" + DesktopKey.Key + "]" + Environment.NewLine;
                foreach (var Dict in DesktopKey.Value)
                {
                    if (Dict.Key.StartsWith("comment"))
                    {
                        Final += "#" + Dict.Value + Environment.NewLine;
                    }
                    else if (Dict.Key.StartsWith("emptyline"))
                    {
                        Final += Environment.NewLine;
                    }
                    else
                    {
                        Final += Dict.Key + "=" + Dict.Value + Environment.NewLine;
                    }
                }
            }
            return Final;
        }
    }
}