using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace rs_mob_updater
{
    class Program
    {
        static string path_to_target_file;
        static string to_comment_prefix;
        static string to_comment_postfix;

        static List<ItemFromXML> listOfItemFromXML;

        public const String default_path = "list_files.xml";

        public static void MyWriteLine(string p = "")
        {
            System.Diagnostics.Debug.WriteLine(p);
            Console.WriteLine(p);
        }

        // "..\list_files.xml"

        static void Main(string[] args)
        {
            // получаю путь к файлу xml  с настроиками
            String url_file_xml = null;

            if (args.Length == 0)
            {
                MyWriteLine("Argument: Path to file is not exist!");
                MyWriteLine("Trying open file with default path: " + default_path);

                url_file_xml = default_path;
            }
            else if (args.Length > 0)
                {
                    MyWriteLine("Argument: Path to file:" + args[0]);

                    // получаю путь к файлу xml  с настроиками
                    url_file_xml = args[0];
                }

            // читаю файл и получаю список настроек
            listOfItemFromXML = ReadXmlFile.ReadXML(url_file_xml);

            if (listOfItemFromXML == null || listOfItemFromXML.Count == 0)
            {
                MyWriteLine("Список настроек пуст, либо равен null");
                // Console.ReadKey();
                return;
            }

            //-------------------------------------------------------------------------------------------------------------

            start_parse_files();

            // Console.ReadKey();
        }

        private static void start_parse_files()
        {
            MyWriteLine("start_parse_files -------------- START");
            foreach (ItemFromXML item in listOfItemFromXML)
            {
                foreach (ItemLine itemLine in item.itemLines)
                {
                    do_changes_by_direction_of_commenting(item, itemLine);
                }
            }
            MyWriteLine("start_parse_files -------------- END");

        }

        private static void do_changes_by_direction_of_commenting(ItemFromXML item, ItemLine itemLine)
        {
            String path_to_dir = item.path_to_dir;

            if (path_to_dir.Length > 0 && path_to_dir.LastIndexOf("/") != (path_to_dir.Length - 1))
            {
                MyWriteLine("/ - in path_to_dir: NOT EXIST !" + "\n");
                item.path_to_dir = path_to_dir + "/";
            }

            path_to_target_file = item.path_to_dir + item.name_file;
            to_comment_prefix = item.to_comment_prefix;
            to_comment_postfix = item.to_comment_postfix;

            switch (itemLine.direction_of_commenting)
            {
                // Коментирует, если ещё не коментировано:
                case 1:
                    MyWriteLine("Case 1 " + "[ Commented if did not make comments ]" + "\n");

                    do_1_for_line(itemLine.line);

                    break;

                // Де-коментирует, если еще не де-коментировано:
                case 2:
                    MyWriteLine("Case 2 " + "[ De-commented, if not de-comment ]" + "\n");

                    do_2_for_line(itemLine.line);

                    break;

                default:
                    MyWriteLine("Default case");
                    MyWriteLine("Press Any Key to exit");
                    // Console.ReadKey();
                    break;
            }

        }

        // Коментирует, если ещё не коментировано:

        private static void do_1_for_line(string target_line)
        {
            string to_search = target_line;
            string to_replace = to_comment_prefix + to_search + to_comment_postfix;

            if (isNotExist(path_to_target_file, to_replace))
            {
                searchAndRelace(path_to_target_file, to_search, to_replace);
            } 
            else
            {
                MyWriteLine("  Path: " + path_to_target_file);
                MyWriteLine("  to_search: " + to_search);
                MyWriteLine("  to_replace: " + to_replace);
                MyWriteLine("  Not Changed!!!" + "\n");
            }
        }

        //-------------------------------------------------------------------------------------------------------------

        // Де-коментирует, если еще не де-коментировано:

        private static void do_2_for_line(string target_line)
        {
            string to_replace = target_line;
            string to_search = to_comment_prefix + to_replace + to_comment_postfix;

            if (isExist(path_to_target_file, to_search))
            {
                searchAndRelace(path_to_target_file, to_search, to_replace);
            }
            else
            {
                MyWriteLine("  Path: " + path_to_target_file);
                MyWriteLine("  to_search: " + to_search);
                MyWriteLine("  to_replace: " + to_replace);
                MyWriteLine("  Not Changed!!!" + "\n");
            }
        }


        //-------------------------------------------------------------------------------------------------------------
        private static bool isExist(string url_file, string to_search)
        {
            String file_string = "";

            try
            {
                if (File.Exists(url_file))
                {
                    file_string = File.ReadAllText(url_file);
                }
                else
                {
                    MyWriteLine("ERRROR!!! " + "File not exist with path: " + url_file);
                    // Console.ReadKey();
                }
            }
            catch (IOException e)
            {
                if (e.Source != null)
                    MyWriteLine("ReadAllText Error");
                // Console.ReadKey();
                throw;
            }

            bool result = file_string.Contains(to_search);
            return result;
        }

        private static bool isNotExist(string url_file, string to_search)
        {
            String file_string = "";

            try
            {
                if (File.Exists(url_file))
                {
                    file_string = File.ReadAllText(url_file);
                }
                else
                {
                    MyWriteLine("ERRROR!!! " + "File not exist with path: " + url_file);
                    // Console.ReadKey();
                }
            }
            catch (IOException e)
            {
                if (e.Source != null)
                    MyWriteLine("Method: isNotExist. " + "ReadAllText Error");
                // Console.ReadKey();
                throw;
            }


            bool result = file_string.Contains(to_search);
            return !result;
        }

        //-------------------------------------------------------------------------------------------------------------

        private static void searchAndRelace(string url_file, string to_search, string to_replace)
        {
            string file_string = File.ReadAllText(url_file);

            // Если есть вообще такая строка:
            bool result = file_string.Contains(to_search);
            // if (Regex.IsMatch(file_string, to_search))
            if (result)
            {

               // to_search = "TextField.registerBitmapFont" + "("+ "*" + ");";
               // string str_to_write = Regex.Replace(file_string, to_search, to_replace);

               string str_to_write = file_string.Replace(to_search, to_replace);

                File.WriteAllText(url_file, str_to_write);

                MyWriteLine("  Path: " + url_file + ":");
                MyWriteLine("  Found string: " + to_search);
                MyWriteLine("  Replaced with a string: " + to_replace);
                MyWriteLine();
            }
        }

        //-------------------------------------------------------------------------------------------------------------
    }
}




/*
// поиск и замена:
String file_string_replaced = Regex.Replace(file_string, to_search, to_replace);

if (file_string != file_string_replaced)
{
    File.WriteAllText(url_file, file_string);

    MyWriteLine("  Path: " + url_file + ":");
    MyWriteLine("  Found string: " + to_search);
    MyWriteLine("  Replaced with a string: " + to_replace);
    MyWriteLine();
}
else
{
    MyWriteLine("  Без изменений эта строка осталась: " + to_replace);
    MyWriteLine();
}
*/
