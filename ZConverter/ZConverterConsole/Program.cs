using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace ZConverterConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string directory, directoryOut, fileNameOut, fullFileNameOut;
            bool moveFilesWithAlreadyFormat = false, whiteColorConvert = false, copyAllFiles = false;
            ImageFormat formatOut;
            FileInfo[] convertFiles;

            directory = Environment.ProcessPath;
            formatOut = ImageFormat.Png;
            string formatForConvert = ".jpg;.jpeg;.png";


            //Чтение параметров
            //  -in  - Указатель папки или файла для конвертации. В директории будет создана папка ConvertedIMG
            //  -cef - Копировать файлы с необходимым форматом в результатирующую папку
            //  -caf - Копировать все файлы попадающиеся на пути
            //  -w   - Белый цвет для Png - прозрачный
            //  -fin - Форматы для конвертирования (указывать в формате -fin .*;.*;)
            //  -fout - Результатирующий формат, указывается только один

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-in":
                        directory = args[++i];
                        break;
                    case "-cef":
                        moveFilesWithAlreadyFormat = true;
                        break;
                    case "-caf":
                        copyAllFiles = true;
                        break;
                    case "-w":
                        whiteColorConvert = true;
                        break;
                    case "-fin":
                        formatForConvert = args[++i];
                        break;
                    case "-fout":
                        switch (args[++i].ToLower())
                        {
                            case "png":
                            case ".png":
                                formatOut = ImageFormat.Png;
                                break;
                            case "jpg":
                            case ".jpg":
                            case "jpeg":
                            case ".jpeg":
                                formatOut = ImageFormat.Jpeg;
                                break;
                            default: formatOut = ImageFormat.Png; break;

                        }
                        break;
                    default:
                        break;
                }
            }


            Console.WriteLine($"Directory in: \"{directory}\"");
            Console.WriteLine($"Format for convert: {formatForConvert}");
            Console.WriteLine($"Format to convert: {formatOut}");


            //Подготовка списка изображений
            DirectoryInfo dirInfo = new DirectoryInfo(directory);
            convertFiles = dirInfo.GetFiles();

            directoryOut = Path.Combine(dirInfo.FullName, "convertIMG");
            Directory.CreateDirectory(directoryOut);



            //Конвертирование
            Console.WriteLine($"\nStart: {DateTime.Now.ToString()}");
            Bitmap bmp;

            for (int i = 0; i < convertFiles.Length; i++)
            {

                fullFileNameOut = Path.Combine(directoryOut, convertFiles[i].Name);
                if (!FileForConvert(convertFiles[i].Name, formatForConvert))
                {
                    if (copyAllFiles & !File.Exists(fullFileNameOut))
                    {
                        File.Copy(convertFiles[i].FullName, fullFileNameOut);
                        Console.WriteLine($"\tCopied {convertFiles[i].Name} to out folder");
                    }
                    continue;
                }
                else if (Path.GetExtension(convertFiles[i].Name).ToLower() == "." + formatOut.ToString().ToLower())
                {
                    if ((copyAllFiles | moveFilesWithAlreadyFormat) & !File.Exists(fullFileNameOut))
                    {
                        File.Copy(convertFiles[i].FullName, fullFileNameOut);
                        Console.WriteLine($"\tCopied {convertFiles[i].Name} to out folder");
                    }
                    continue;
                }

                fileNameOut = $"{Path.GetFileNameWithoutExtension(convertFiles[i].Name)}.{formatOut}";
                fullFileNameOut = Path.Combine(directoryOut, fileNameOut);
                bmp = new Bitmap(convertFiles[i].FullName);
                if (whiteColorConvert)
                {
                    for (int x = 0; x < bmp.Width; x++)
                    {
                        for (int y = 0; y < bmp.Height; y++)
                        {
                            Color c = bmp.GetPixel(x, y);

                            //Делает прозрачным пиксели с чётко белым цветом в png
                            if (c.R == 255 && c.G == 255 && c.B == 255)
                                bmp.SetPixel(x, y, Color.FromArgb(0));
                        }
                    }
                }

                bmp.Save(Path.Combine(directoryOut, fileNameOut), formatOut);
                Console.WriteLine($"\tConverted {convertFiles[i].Name} to PNG");
            }
            Console.WriteLine($"End: {DateTime.Now.ToString()}");
        }

        private static bool FileForConvert(string fileName, string filesFormat)
        {
            string fileException = Path.GetExtension(fileName).ToLower();
            return filesFormat.Contains(fileException);
        }
    }
}