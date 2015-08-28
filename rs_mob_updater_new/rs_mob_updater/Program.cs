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
        enum DIRECTION_OF_COMMENTING
        {
            Comment = 1,
            DeComment,
            Replace
        }

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
                
                return;
            }

            //-------------------------------------------------------------------------------------------------------------

            start_parse_files();

            
        }

        private static void start_parse_files()
        {
            MyWriteLine("START_PARSE_FILES -------------- START" + "\n");
            foreach (ItemFromXML item in listOfItemFromXML)
            {
                foreach (ItemLine itemLine in item.itemLines)
                {
                    do_changes_by_direction_of_commenting(item, itemLine);
                }
            }
            MyWriteLine("START_PARSE_FILES -------------- END");

        }

        /// <summary>
        /// Изменяет файл согласно заданным параметрам
        /// </summary>
        /// <param name="item">Элемент item из xml файла настроек, содержащий параметры поиска</param>
        /// <param name="itemLine">Строка, которую необходимо найти</param>
        private static void do_changes_by_direction_of_commenting(ItemFromXML item, ItemLine itemLine)
        {
            if (item.path_to_dir == null)
            {
                Program.MyWriteLine("ERRROR!!! " + " item.path_to_dir = null " + "\n");
                return;
            }

            String path_to_dir = item.path_to_dir;
            if (path_to_dir.Length > 0 && path_to_dir.LastIndexOf("/") != (path_to_dir.Length - 1))
            {
                MyWriteLine("/ - in path_to_dir: NOT EXIST !" + "\n");
                item.path_to_dir = path_to_dir + "/";
            }

            path_to_target_file = item.path_to_dir + item.name_file;
            to_comment_prefix = item.to_comment_prefix;
            to_comment_postfix = item.to_comment_postfix;

            DIRECTION_OF_COMMENTING dir_of_com = (DIRECTION_OF_COMMENTING) itemLine.direction_of_commenting;

            switch (dir_of_com)
            {
                // Коментирует, если ещё не коментировано:
                case DIRECTION_OF_COMMENTING.Comment:
                    MyWriteLine("Case 1 " + "[ Commented if did not make comments ]" + "\n");

                    do_1_for_line(itemLine.line);

                    break;

                // Де-коментирует, если еще не де-коментировано:
                case DIRECTION_OF_COMMENTING.DeComment:
                    MyWriteLine("Case 2 " + "[ De-commented, if not de-comment ]" + "\n");

                    do_2_for_line(itemLine.line);

                    break;

                // Заменяет строку line на line2:
                case DIRECTION_OF_COMMENTING.Replace:
                    MyWriteLine("Case 3 " + "[ Replace line ]" + "\n");

                    do_3_for_line(itemLine.line, itemLine.line2);

                    break;

                default:
                    MyWriteLine("Default case. " + "itemLine.direction_of_commenting: " + itemLine.direction_of_commenting.ToString() + "\n");
                    break;
            }

        }

        //-------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Коментирует, если ещё не коментировано
        /// </summary>
        /// <param name="target_line"></param>
        private static void do_1_for_line(string target_line)
        {
            string to_search = target_line;
            string to_replace = to_comment_prefix + to_search + to_comment_postfix;

            // if (isNotExist(path_to_target_file, to_replace))
            if (isExist(path_to_target_file, to_replace) == 0)
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


        /// <summary>
        /// Де-коментирует, если еще не де-коментировано
        /// </summary>
        /// <param name="target_line"></param>
        private static void do_2_for_line(string target_line)
        {
            string to_replace = target_line;
            string to_search = to_comment_prefix + to_replace + to_comment_postfix;

            if (isExist(path_to_target_file, to_search) == 1)
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

        private static void do_3_for_line(string to_search, string to_replace)
        {
            if (isExist(path_to_target_file, to_search) == 1)
            {
                searchAndRelace(path_to_target_file, to_search, to_replace);
            }
            else
            {
                MyWriteLine("  Path: " + path_to_target_file);
                MyWriteLine("  to_search: " + to_search);
                MyWriteLine("  to_replace: " + to_replace);
                MyWriteLine("  Not Contains!!!" + "\n");
            }
        }

        //-------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Проверяет, если файл содержит строку
        /// </summary>
        /// <param name="url_file">путь к файлу</param>
        /// <param name="to_search">строка для поиска</param>
        /// <returns>Возвращает -1 - нет файла, 0 - не найдена строка, 1 - найдена строка</returns>
        private static int isExist(string url_file, string to_search)
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
                    return -1;
                }
            }
            catch (IOException e)
            {
                if (e.Source != null)
                    MyWriteLine("ReadAllText Error: " + e.Message);
                return -1;
            }

            int result = (file_string.Contains(to_search))?1:0;
            return result;
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Ищет строку и заменяет её
        /// </summary>
        /// <param name="url_file">путь к файлу</param>
        /// <param name="to_search">строка для поиска</param>
        /// <param name="to_replace">строка для замены</param>
        private static void searchAndRelace(string url_file, string to_search, string to_replace)
        {
            string file_string = File.ReadAllText(url_file);

            // Если есть вообще такая строка:
            bool result = file_string.Contains(to_search);

            if (result)
            {
               string str_to_write = file_string.Replace(to_search, to_replace);

                File.WriteAllText(url_file, str_to_write);

                MyWriteLine("  Path: " + url_file + ":");
                MyWriteLine("  Found string: " + to_search);
                MyWriteLine("  Replaced with a string: " + to_replace);
                MyWriteLine();
            }
            else
            {
                MyWriteLine("  Path: " + url_file + ":");
                MyWriteLine("  Found string: " + to_search);
                MyWriteLine("  Not Contains!!!" + "\n");
                MyWriteLine();
            }
        }

        //-------------------------------------------------------------------------------------------------------------
    }
}