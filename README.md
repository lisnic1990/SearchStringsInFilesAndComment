# SearchStringsInFilesAndComment
Search strings in files and comment lines. Search options are configured in a xml file.

Поиск строк в файлах и комментирование/разкомментирование этих строк. Для поиска на входе скармливаешь конфигурационный xml файл.

Релизная версия утилиты находится в папке: 
rs_mob_updater/bin/Release
файл:
rs_mob_updater_new.exe

Как использовать:	
Программа консольная и запускается с входным параметром (аргументом):
строка - абсолютный/ относительный путь к файлу к настройками для поиска (к примеру: "list_files.xml")

Как настроить конфигурационный xml файл:

Пример xml файла (list_files.xml):

<?xml version="1.0" encoding="utf-8" ?>
<items>
...
<item path_to_dir="../" name_file="target_file_for_example.txt" to_comment_prefix="&lt;!--" to_comment_postfix="--&gt;">
	<sub direction_of_commenting="2" line="System.Console.WriteLine(&quot;Hello World!&quot;);" />
</item>
...
<item path_to_dir="../" name_file="target_file_for_example.txt" to_comment_prefix="//">
	<sub direction_of_commenting="2" line="System.Console.ReadKey();" />
</item>
...
</items>

1. Поле <items>...</items> - может содержать произвольное число <item/>.
2. Атрибуты <item/> :

	*path_to_dir	-	Путь к папке где находится целевой файл, в котором будет производиться поиск. (в конце обязательно должен быть "/"). Путь может быть относительный (относительно файла "rs_mob_updater_new.exe") или абсолютный.
	*name_file	-	Имя файла.
	*to_comment_prefix	-	строка , которая будет записана перед искомой строка.
	to_comment_postfix	-	строка , которая будет записана после искомой строки (Опционально, может и отсутствовать этот атрибут).
	
3. Поле <item>...</item> - может содержать произвольное число <sub/>.
3.1 Атрибуты <sub/> :

	*direction_of_commenting	-	направление комментирования.
		1 - комментирует.
		2 - разкомментирует.
	*line	-	Искомая строка.
	
* - Обязательный параметр.

Если встречаются специальные символы для xml файла, их необходимо заменить.
Здесь описаны спец. символы:
https://support.microsoft.com/ru-ru/kb/308060

