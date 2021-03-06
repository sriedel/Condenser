﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Condenser
{
    public class FileCarver
    {
        string[] files;
        string[] startHexCodes = SetupStartHexCodes();
        public static string name = "file";
        public string path;
        
        public int index = 0;

        //Dictionary<string, string> endHexCodes = new Dictionary<string, string>();

        public FileCarver()
        {
            
        }

        public FileCarver(string[] _files, string _path)
        {
            files = _files;
            path = _path;
        }

        private static string[] SetupStartHexCodes()
        {
            //string array of length 12.
            //6 types
            string[] _startHexCodes = 
            {
                "474946383761",                  //GIF
                "474946383961",                  //GIF
                "ffd8ffe00010",                  //JPG
                "ffd8ffee00",                    //JPG
                "ffd8ffe1",                      //JPG
                "ffd8fffe",                      //JPG
                "464c5601",                      //FLV
                "89504e47",                      //PNG
                "424df8a9",                      //BMP
                "424d6225",                      //BMP
                "424d7603",                      //BMP
                "3c21444f43545950452068746d6c3e" //HTML
            };
            return _startHexCodes;
        }

        /*private Dictionary<string, string> SetupEndHexCodes()
        {
            Dictionary<string, string> _endHexCodes = new Dictionary<string, string>();

            _endHexCodes.Add("gif1", "003b");
            _endHexCodes.Add("gif2", "00003b");
            _endHexCodes.Add("jpg1", "ffd9");
            _endHexCodes.Add("jpg2", "ffd9");
            _endHexCodes.Add("", "");
            _endHexCodes.Add("", "");
            _endHexCodes.Add("", "");

            return _endHexCodes;
        }*/

        public void CarveManager()
        {
            LogWrite.WriteLine("File Carver: Started Carve Manager.");
            int total = files.Length;
            for (int i = 0; i < total; i++)
            {
                LogWrite.WriteLine("File Carver: Carving file " + files[i].ToString() + ". " + i + " of " + total + " files.");
                Carve(GetBytes(files[i]), i);                
            }
        }

        public byte[] GetBytes(string filepath)
        {
            byte[] data;

            if (!File.Exists(filepath))
            {
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + filepath);
            }

            

            try
            {
                data = File.ReadAllBytes(filepath);                
            }
            catch
            {
                throw new FileLoadException("Couldn't load: " + filepath);                
            }

            return data;
        }



        public void Carve(byte[] data, int findex)
        {     

            
            int offset = 0;
            byte[] file = new byte[data.Length + 1];

            // We want to convert the byte array to a hex-formatted string.
            
            StringBuilder sBuilder = new StringBuilder();
            
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            string datastring = Convert.ToString(sBuilder);
            //Now we scan the string for the hex code we want.

            for (int i = 0; i < startHexCodes.Length; i++)
            {
                Match match = Regex.Match(datastring, startHexCodes[i], RegexOptions.IgnoreCase & RegexOptions.Singleline);
                if (match.Success)
                {
                    LogWrite.WriteLine("File Carver: Found start hex pattern for file. Type: " + startHexCodes[i]);
                    offset = (match.Index / 2); //We need to divide it by two as hex is two chars and regex is checking them one at a time.
                                        
                    if (offset > 0)
                    {
                        int copysize = data.Length - offset;
                        Array.Copy(data, offset, file, 0, copysize); 
                    }
                    else { file = data; }
                    switch (i)
                    { 
                        case 0:
                            GIF(file, findex);
                            break;
                        case 1:
                            GIF(file, findex);
                            break;
                        case 2:
                            JPG(file, findex);
                            break;
                        case 3:
                            JPG(file, findex);
                            break;
                        case 4:
                            JPG(file, findex);
                            break;
                        case 5:
                            JPG(file, findex);
                            break;
                        case 6:
                            FLV(file, findex);
                            break;
                        case 7:
                            PNG(file, findex);
                            break;
                        case 8:
                            BMP(file, findex);
                            break;
                        case 9:
                            BMP(file, findex);
                            break;
                        case 10:
                            BMP(file, findex);
                            break;
                        case 11:
                            HTML(file, findex);
                            break;
                    }
                    break;
                }                
            }
        }




        public void JPG(byte[] file, int findex)
        {
            string fpath = Path.Combine(path, @"jpg\");
            string type = ".jpg";
            DirectoryInfo dir = new DirectoryInfo(fpath);
            if (!dir.Exists)
            {
                Directory.CreateDirectory(fpath);
            }
            if (File.Exists(fpath + name + findex + type))
            {
                File.Delete(fpath + name + findex + type);                
            }

            File.WriteAllBytes(fpath + name + findex + type, file);
            LogWrite.WriteLine("File Carver: Created JPG:\n" + fpath + name + findex + type + "\n\n");
        }

        public void GIF(byte[] file, int findex)
        {
            string fpath = Path.Combine(path, @"gif\");
            string type = ".gif";
            DirectoryInfo dir = new DirectoryInfo(fpath);
            if (!dir.Exists)
            {
                Directory.CreateDirectory(fpath);
            }
            if (File.Exists(fpath + name + findex + type))
            {
                File.Delete(fpath + name + findex + type);
            }

            File.WriteAllBytes(fpath + name + findex + type, file);
            LogWrite.WriteLine("File Carver: Created GIF:\n" + fpath + name + findex + type + "\n\n");
        }
        public void PNG(byte[] file, int findex)
        {
            string fpath = Path.Combine(path, @"png\");
            string type = ".png";
            DirectoryInfo dir = new DirectoryInfo(fpath);
            if (!dir.Exists)
            {
                Directory.CreateDirectory(fpath);
            }
            if (File.Exists(fpath + name + findex + type))
            {
                File.Delete(fpath + name + findex + type);
            }

            File.WriteAllBytes(fpath + name + findex + type, file);
            LogWrite.WriteLine("File Carver: Created PNG:\n" + fpath + name + findex + type + "\n\n");
        }
        public void FLV(byte[] file, int findex)
        {
            string fpath = Path.Combine(path, @"flv\");
            string type = ".flv";
            DirectoryInfo dir = new DirectoryInfo(fpath);
            if (!dir.Exists)
            {
                Directory.CreateDirectory(fpath);
            }
            if (File.Exists(fpath + name + findex + type))
            {
                File.Delete(fpath + name + findex + type);
            }

            File.WriteAllBytes(fpath + name + findex + type, file);
            LogWrite.WriteLine("File Carver: Created FLV:\n" + fpath + name + findex + type + "\n\n");
        }
        public void BMP(byte[] file, int findex)
        {
            string fpath = Path.Combine(path, @"bmp\");
            string type = ".bmp";
            DirectoryInfo dir = new DirectoryInfo(fpath);
            if (!dir.Exists)
            {
                Directory.CreateDirectory(fpath);
            }
            if (File.Exists(fpath + name + findex + type))
            {
                File.Delete(fpath + name + findex + type);
            }
            File.WriteAllBytes(fpath + name + findex + type, file);
            LogWrite.WriteLine("File Carver: Created BMP:\n" + fpath + name + findex + type + "\n\n");
        }

        public void HTML(byte[] file, int findex)
        {
            string fpath = Path.Combine(path, @"html\");
            string type = ".html";
            DirectoryInfo dir = new DirectoryInfo(fpath);
            if (!dir.Exists)
            {
                Directory.CreateDirectory(fpath);
            }
            if (File.Exists(fpath + name + findex + type))
            {
                File.Delete(fpath + name + findex + type);
            }
            File.WriteAllBytes(fpath + name + findex + type, file);
            LogWrite.WriteLine("File Carver: Created HTML:\n" + fpath + name + findex + type + "\n\n");
        }

    }
}
