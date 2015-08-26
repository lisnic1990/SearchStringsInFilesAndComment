using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace rs_mob_updater
{

    public class ItemLine
    {
        public int direction_of_commenting;
        public String line;
    }

    public class ItemFromXML
    {
        public String path_to_dir;
        public String to_comment_prefix;
        public String to_comment_postfix;
        public String name_file;
        public List<ItemLine> itemLines = new List<ItemLine>();
    }

    class ReadXmlFile
    {
        static public List<ItemFromXML> ReadXML(String path)
        {
            if(path == null || path.Length == 0)
            {
                Program.MyWriteLine("ERRROR!!! " + "path == null || path.Length == 0");
                return null;
            }

            XmlTextReader reader = null;
            
            try 
	        {

                if (File.Exists(path))
                {
                    reader = new XmlTextReader(path);
                }
                else
                {
                    Program.MyWriteLine("ERRROR!!! " + "File not exist with path: " + path);
                }
		        
	        } catch (Exception) 
            {
                Program.MyWriteLine("ERRROR!!! " + " new XmlTextReader(path) ");
                return null;
            }

            if (reader == null)
            {
                Program.MyWriteLine("ERRROR!!! " + " reader = null ");
                return null;
            }

            List<ItemFromXML> listOfItemFromXML = new List<ItemFromXML>();
            ItemFromXML itemFromXML = new ItemFromXML();

            bool do_loop = true;
            while (do_loop)
            {

                try
                {
                    do_loop = reader.Read();
                }
                catch (Exception) 
                {
                    Program.MyWriteLine("ERRROR!!! " + "XML Invalid");
                    return null;
                }

                // Program.MyWriteLine("reader.NodeType: " + reader.NodeType + "|  reader.Name = " + reader.Name);

                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "item")
                    {
                        // Program.MyWriteLine("id: " + reader.GetAttribute("id"));
                        // Program.MyWriteLine("path_to_dir: " + reader.GetAttribute("path_to_dir"));
                        // Program.MyWriteLine("name_file: " + reader.GetAttribute("name_file"));
                        // Program.MyWriteLine("to_comment_prefix: " + reader.GetAttribute("to_comment_prefix"));
                        // Program.MyWriteLine("--------------------------------------");

                        itemFromXML = new ItemFromXML();
                        listOfItemFromXML.Add(itemFromXML);

                        itemFromXML.path_to_dir = reader.GetAttribute("path_to_dir");
                        itemFromXML.name_file = reader.GetAttribute("name_file");
                        itemFromXML.to_comment_prefix = reader.GetAttribute("to_comment_prefix");

                        itemFromXML.to_comment_postfix = reader.GetAttribute("to_comment_postfix");

                        if (itemFromXML.to_comment_postfix == null)
                        {
                            itemFromXML.to_comment_postfix = "";
                        }
                        
                    }

                    if (reader.Name == "sub")
                    {
                        // Program.MyWriteLine("direction_of_commenting: " + reader.GetAttribute("direction_of_commenting"));
                        // Program.MyWriteLine("line: " + reader.GetAttribute("line"));
                        // Program.MyWriteLine("--------------------------------------");

                        ItemLine itemLine = new ItemLine();
                        itemLine.direction_of_commenting = int.Parse(reader.GetAttribute("direction_of_commenting"));
                        itemLine.line = reader.GetAttribute("line");
                        itemFromXML.itemLines.Add(itemLine);
                    }
                }
            }

            reader.Close();

            Program.MyWriteLine("START READ XML FILE");
            Program.MyWriteLine("--------------------------------------");
            foreach (ItemFromXML item in listOfItemFromXML)
            {
                Program.MyWriteLine("name_file: " + item.name_file + " | Count Lines:" + item.itemLines.Count);
                foreach (ItemLine itemLine in item.itemLines)
                {
                    Program.MyWriteLine("   direction_of_commenting: " + itemLine.direction_of_commenting + " | line: " + itemLine.line);
                }
            }
            Program.MyWriteLine("--------------------------------------");
            Program.MyWriteLine("END READ XML FILE" + "\n");

            return listOfItemFromXML;
        }
    }


}
