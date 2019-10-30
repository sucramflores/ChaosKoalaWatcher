using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;

namespace Watcher
{
    class Program
    {

        public static string path = @"C:\Users\Marcus\source\repos\PDQ\Watcher\teste_folder\";
        public static string fileType = @"*.txt";
        public static DateTime lastTimeChecked = DateTime.Now.Date;
        public static List<ObjectFile> currentRepo;
        public static DirectoryInfo d;
        public static FileInfo[] currentFiles; 

        
        static void Main(string[] args)
        {
            
            if (args.Length == 2)
            {
                if (args[0] != null && Directory.Exists(args[0]))
                {
                    path = args[0];
                }
                if (args[1] != null && args[1].Substring(0, 2) == "*.")
                {
                    fileType = args[1];
                }
            }


            currentRepo = new List<ObjectFile>();
            d = new DirectoryInfo(path);
            currentFiles = d.GetFiles(fileType);
            foreach (FileInfo arquivo in currentFiles)
            {
                //string[] contentTemp = readContentSize($"{arquivo.FullName}");
                currentRepo.Add(new ObjectFile
                {
                    NameFile = arquivo.Name,
                    NameFileFull = arquivo.FullName,
                    Created = arquivo.CreationTime,
                    LastModified = arquivo.LastWriteTime,
                    NumberOfLines = readContentSize(arquivo.FullName)
                });
            }
            Console.WriteLine($"Watching {fileType} on {path}...");
            Console.WriteLine(new String(':', Console.BufferWidth));
            lastTimeChecked = DateTime.Now;
            Timer time = new Timer(StartNewThread, null, 0, 10000);
            Console.ReadLine();


        }

        static void StartNewThread(object o)
        {
            Task.Factory.StartNew(() => RunWatcher());
        }

        static void RunWatcher()
        {
            FileInfo[] newFiles = d.GetFiles(fileType); 
            foreach(FileInfo x in currentFiles)
            {
                if (!newFiles.Any(e => e.FullName == x.FullName))
                {
                    Console.WriteLine($"{DateTime.Now} - '{x.Name}' was DELETED (or RENAMED)");
                }
            }
            

            var filesNew = newFiles.Where(e =>
                                !currentFiles.Any(s => s.FullName == e.FullName)
                            );
            foreach (var x in filesNew)
            {
                var contentTemp = readContentSize($"{x.FullName}");
                Console.WriteLine($"{DateTime.Now} - '{x.Name}' was CREATED (has {contentTemp} lines)");
            }

            var filesChanged = newFiles.Where(e => e.LastWriteTime > lastTimeChecked && e.CreationTime<e.LastWriteTime);
            foreach(var x in filesChanged)
            {
                var numberOfLinesOldFile = currentRepo.FirstOrDefault(e => e.NameFileFull == x.FullName).NumberOfLines;
                var numberOfLinesNewFile = readContentSize($"{x.FullName}");
                var predicate = " (The number of lines still the same)";
                if (numberOfLinesOldFile != numberOfLinesNewFile)
                {
                    predicate = $"{(numberOfLinesNewFile > numberOfLinesOldFile ? '+' : '-')}{Math.Abs(numberOfLinesNewFile - numberOfLinesOldFile)} lines";
                    Console.WriteLine($"{DateTime.Now} - '{x.Name}' has been changed ({predicate})");
                }
                else
                {
                    Console.WriteLine($"{DateTime.Now} - '{x.Name}' might have been changed {predicate}");
                }
            }



            currentRepo.Clear();
            foreach (FileInfo arquivo in newFiles)
            {
                //string[] contentTemp = readContentSize($"{arquivo.FullName}");
                currentRepo.Add(new ObjectFile
                {
                    NameFile = arquivo.Name,
                    NameFileFull = arquivo.FullName,
                    Created = arquivo.CreationTime,
                    LastModified = arquivo.LastWriteTime,
                    NumberOfLines = readContentSize(arquivo.FullName)
                });
            }
            currentFiles = newFiles;
            lastTimeChecked = DateTime.Now;
                                 
        }


       

        private static int readContentSize(string fullNam)
        {
            //var file = File.ReadAllLines(fullNam).Length;
            string data;
            FileStream fsSource = new FileStream(fullNam, FileMode.Open, FileAccess.Read);
            using (StreamReader sr = new StreamReader(fsSource))
            {
                data = sr.ReadToEnd();
            }

            return data.Length;

        }
    }
}
