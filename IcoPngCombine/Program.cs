using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.IconLib;
using System.Drawing.IconLib.Exceptions;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace IcoPngCombine
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: IcoPngCombine.exe [start_folder]");
                Console.WriteLine("Start folder should contain this directory/file structure:");
                Console.WriteLine("    size\\category\\icon_name.png");
                Console.WriteLine("Like you can see in this repo: https://github.com/KDE/oxygen-icons");
                Console.WriteLine("This tool creates a \"results\" folder in the start folder and puts all of the combined icons there");

                return;
            }

            string startFolder = args[0];
            if (!Directory.Exists(startFolder))
            {
                Console.Error.WriteLine($"Directory {startFolder} doesn't exist.");

                return;
            }

            string results = Path.Combine(startFolder, "results");
            Directory.CreateDirectory(results);

            string[] sizes = Directory.GetDirectories(startFolder);

            HashSet<string> createdIcons = new HashSet<string>();

            foreach (string scannedSize in sizes)
            {
                string[] categories = Directory.GetDirectories(scannedSize);
                foreach (string category in categories)
                {
                    string catName = Path.GetFileName(category);

                    string[] icons = Directory.GetFiles(category, "*.png");
                    foreach (string icon in icons)
                    {
                        string iconFileName = Path.GetFileName(icon);
                        string iconName = Path.GetFileNameWithoutExtension(icon);

                        string iconFullPath = Path.Combine(catName, iconFileName);
                        string iconId = Path.Combine(catName, iconName);

                        if (createdIcons.Contains(iconId))
                        {
                            continue;
                        }

                        createdIcons.Add(iconId);

                        MultiIcon img = new MultiIcon();
                        int index = -1, selectedIndex = -1, selectedIndexSize = 0;
                        List<string> szs = new List<string>();
                        foreach (string sz in sizes)
                        {
                            string fullPath = Path.Combine(sz, iconFullPath);
                            if (!File.Exists(fullPath))
                            {
                                continue;
                            }

                            string sizeName = Path.GetFileName(sz);
                            
                            SingleIcon ic = img.Add(sizeName);

                            try
                            {
                                Bitmap bmp = (Bitmap) Bitmap.FromFile(fullPath);

                                try
                                {
                                    ic.CreateFrom(bmp, IconOutputFormat.Vista);

                                    szs.Add(sizeName);
                                    
                                    index += 1;
                                    if (!Int32.TryParse(new String(sizeName.TakeWhile(c => c >= '0' && c <= '9').ToArray()), out int size))
                                    {
                                        size = 0;
                                    }

                                    if (selectedIndex < 0 || selectedIndexSize < size)
                                    {
                                        selectedIndex = index;
                                        selectedIndexSize = size;
                                    }
                                }
                                catch (ImageTooBigException)
                                {
                                    Console.WriteLine($"Image {sizeName}\\{iconId} has dimensions that are too big: w={bmp.Width}, h={bmp.Height}");
                                
                                    img.Remove(ic);
                                }
                            }
                            catch (InvalidPixelFormatException ex)
                            {
                                Console.WriteLine($"Ignoring size {sizeName} of {iconId} because {ex.Message}, required is {PixelFormat.Format32bppArgb}");
                                
                                img.Remove(ic);
                            }
                        }

                        if (selectedIndex < 0)
                        {
                            continue;
                        }

                        string dirName = Path.Combine(results, catName);
                        if (!Directory.Exists(dirName))
                        {
                            Directory.CreateDirectory(dirName);
                        }

                        img.SelectedIndex = selectedIndex;

                        Console.WriteLine($"Saving {iconId} with sizes [{String.Join(", ", szs)}]");

                        string icoName = Path.Combine(results, iconId) + ".ico";
                        img.Save(icoName, MultiIconFormat.ICO);
                    }
                }
            }

            Console.WriteLine($"Done. Created {createdIcons.Count} icons.");
        }
    }
}