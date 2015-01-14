﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Configuration;


namespace ReportCompare
{
    class Program
    {
        [STAThread]
        static void Main()
        {

            Application.EnableVisualStyles();

            Properties.Settings.Default.DiffPdfFile = AppDomain.CurrentDomain.BaseDirectory + "apps\\diffpdf.exe";
            Properties.Settings.Default.ComparePdfFile = AppDomain.CurrentDomain.BaseDirectory + "apps\\comparepdf.exe";

            Application.Run(new MainWindow());
        }

        public static void start()
        {
            string[] sourceFolder = getFilesFromPath(Properties.Settings.Default.SourcePath);
            string[] targetFolder = getFilesFromPath(Properties.Settings.Default.TargetPath);
            compare(sourceFolder, targetFolder);
        }


        static void compare(string[] path1, string[] path2)
        {
            int count = 0;
            string comparePdfExec = Properties.Settings.Default.ComparePdfFile;
            string diffPdfExec = Properties.Settings.Default.DiffPdfFile;
            foreach (string source in path1)
            {
                foreach (string target in path2)
                {
                    if (Path.GetFileName(source) == Path.GetFileName(target))
                    {
                        count++;
                        int exitCode = runCmd(comparePdfExec, source, target);
                        log("Programm Exit Code: " + exitCode);
                        if (exitCode >= 10)
                        {
                            log(string.Format("Найдены различия в файле: {0}", source));
                            DialogResult result1 = MessageBox.Show("Найдены различия в файле:\n" + source + "\nСравнить их визуально?",
                             "Внимание",
                            MessageBoxButtons.YesNo);
                            if (result1 == DialogResult.Yes) 
                            {
                                runCmd(diffPdfExec, source, target); 
                            }
                            else if (result1 == DialogResult.No) { }
                        }
                    }
                }
            }
        }

        // Запускает исполняемый файл с двумя агрументам, ждет завершения, возвращает ExitCode
        static int runCmd(string programm, string arg1, string arg2)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = programm; // Путь к файлу
            startInfo.Arguments = arg1 + " " + arg2; //агрумент 
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
            return process.ExitCode;
        }

        //Возвращает массив файлов указанного расширения по указанному path
        static string[] getFilesFromPath(String path)
        {
            return Directory.GetFiles(@path, "*.pdf");
        }


        // Метод для выбора файла
        public static string selectFile()
        {
            OpenFileDialog choofdlog = new OpenFileDialog();
            choofdlog.Filter = "Executable|*.exe";
            choofdlog.FilterIndex = 1;
            choofdlog.Multiselect = false;
            string fileName = null;

            if (choofdlog.ShowDialog() == DialogResult.OK)
            {
                fileName = choofdlog.FileName;
                //string[] arrAllFiles = choofdlog.FileNames; //used when Multiselect = true           
            }
            return fileName;
        }

        //Метод для выбора пути
        public static string selectFolder()
        {
            string path = null;
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowDialog();
            if (fbd.SelectedPath != "") path = fbd.SelectedPath;
            return path;
        }

        //Вывод в консоль с временем
        public static void log(string s) 
        {
            string date = string.Format("{0:HH:mm:ss tt}", DateTime.Now);
            Console.WriteLine(date + s);
        }

    }
}
