using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;

namespace Beat_Saber_Version_Handler
{
    public static class BeatSaberSwitcher
    {
        internal static string ExecutionPath = Environment.CurrentDirectory;
        internal static readonly string[] ExcludeDataDirs = new string[]
        {
            "CustomLevels",
            "CustomWIPLevels"
        };

        public static string GetCurrentBeatSaberVersion(string DirPath)
        {
            var path = Path.Combine(DirPath, "BeatSaberVersion.txt");
            return File.ReadAllText(path);
        }

        public static DirectoryInfo CreateBackupDir(string Version)
        {
            var path = Path.Combine(ExecutionPath, Version);
            return Directory.CreateDirectory(path);
        }

        public static void BackUpBeatSaber(string BeatSaberLoc, DirectoryInfo BackupDir)
        {
            var BeatSaberDir = new DirectoryInfo(BeatSaberLoc);

            var rootFiles = BeatSaberDir.GetFiles();
            foreach (var rootFile in rootFiles)
            {
                rootFile.MoveTo(Path.Combine(BackupDir.FullName, rootFile.Name));
            }

            var rootDirs = BeatSaberDir.GetDirectories();
            foreach (var rootDir in rootDirs)
            {
                if (rootDir.Name == "Beat Saber_Data")
                {
                    HandleBeatSaberData(rootDir, BackupDir);
                    continue;
                }

                rootDir.MoveTo(Path.Combine(BackupDir.FullName, rootDir.Name));
            }
        }

        private static void HandleBeatSaberData(DirectoryInfo BeatSaberDataDir, DirectoryInfo BackupDir)
        {
            var backupDataDir = BackupDir.CreateSubdirectory("Beat Saber_Data");

            var rootFiles = BeatSaberDataDir.GetFiles();
            var rootDirs = BeatSaberDataDir.GetDirectories().Where(dir => !ExcludeDataDirs.Contains(dir.Name));

            foreach (var rootFile in rootFiles)
            {
                rootFile.MoveTo(Path.Combine(backupDataDir.FullName, rootFile.Name));
            }

            foreach (var rootDir in rootDirs)
            {
                rootDir.MoveTo(Path.Combine(backupDataDir.FullName, rootDir.Name));
            }
        }

        public static void MoveBeatSaberFromBackup(string Version, string BeatSaberLoc)
        {
            var BeatSaberDir = new DirectoryInfo(BeatSaberLoc);
            var BackupDir = new DirectoryInfo(Path.Combine(ExecutionPath, Version));

            var rootFiles = BackupDir.GetFiles();
            var rootDirs = BackupDir.GetDirectories();

            foreach (var rootFile in rootFiles)
            {
                rootFile.MoveTo(Path.Combine(BeatSaberDir.FullName, rootFile.Name));
            }

            foreach (var rootDir in rootDirs)
            {
                if (rootDir.Name == "Beat Saber_Data")
                {
                    MoveBackupBeatSaberDataContentToBeatSaberData(rootDir, new DirectoryInfo(Path.Combine(BeatSaberLoc, "Beat Saber_Data")));
                    continue;
                }

                rootDir.MoveTo(Path.Combine(BeatSaberDir.FullName, rootDir.Name));
            }

            BackupDir.Delete(true);
        }

        public static void MoveBackupBeatSaberDataContentToBeatSaberData(DirectoryInfo BeatSaberBackupDataDir, DirectoryInfo BeatSaberDataDir)
        {
            var rootFiles = BeatSaberBackupDataDir.GetFiles();
            var rootDirs = BeatSaberBackupDataDir.GetDirectories();

            foreach (var rootFile in rootFiles)
            {
                rootFile.MoveTo(Path.Combine(BeatSaberDataDir.FullName, rootFile.Name));
            }

            foreach (var rootDir in rootDirs)
            {
                rootDir.MoveTo(Path.Combine(BeatSaberDataDir.FullName, rootDir.Name));
            }
        }
    }
}
