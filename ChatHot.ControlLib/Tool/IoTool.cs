using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ChatHot.ControlLib.Tool
{
    public class IoTool
    {
        public static byte[] GetFileByte(String Path)
        {
            FileStream file = File.Open(Path, FileMode.Open);
            byte[] bs = new byte[file.Length];
            file.Position = 0;
            file.Read(bs, 0, (int)file.Length);
            file.Dispose();
            return bs;
        }
        public static byte[] GetFileByte(String Path, int Offset, int Count)
        {
            FileStream file = File.Open(Path, FileMode.Open);
            byte[] bs = new byte[Count];
            file.Read(bs, Offset, Count);
            file.Dispose();
            return bs;
        }
        /// <summary>
        /// 取文件的字节块
        /// </summary>
        /// <param name="Path">文件地址</param>
        /// <param name="BlockSize">每个块的大小</param>
        /// <param name="Index">第几个块，从1开始</param>
        /// <returns>字节</returns>
        public static byte[] GetFileByteBlock(String Path, int BlockSize, int Index)
        {
            return GetFileByte(Path, (Index - 1) * BlockSize, BlockSize * Index);
        }
        /// <summary>
        /// 大文件分割为多个包
        /// </summary>
        /// <param name="Path">文件地址</param>
        /// <param name="BlockSize">每个块的大小</param>
        /// <param name="CallBack">参数[每次读取的Byte，文件名称，是否为最后一个包，第几个包]</param>
        public static void GetFileByteBlockALL(String Path, int BlockSize, Action<Byte[], String, bool, int> CallBack)
        {
            FileStream file = File.Open(Path, FileMode.Open);
            byte[] bs = new byte[BlockSize];
            int PacketCount = 0;
            file.Position = 0;
            while (true)
            {
                PacketCount += 1;
                //如果下一个指针位置也是满块的
                if ((file.Position + BlockSize) < file.Length)
                {
                    file.Read(bs, 0, BlockSize);//将字节读入
                    CallBack?.Invoke(bs, file.Name, false, PacketCount);
                }
                //如果 = 或者 > 的话那就证明这已经是最后一个字节块了
                else
                {
                    bs = new byte[file.Length - file.Position];
                    file.Read(bs, 0, bs.Length);
                    CallBack?.Invoke(bs, file.Name, true, PacketCount);
                    break;
                }
            }
            file.Dispose();
        }
        public static DirectoryInfo CreateDirectory(String path)
        {
            return Directory.CreateDirectory(path);
        }
        public static void CreateFile(String FileName, byte[] Content)
        {
            FileStream file = File.Create(FileName);
            file.Write(Content, 0, Content.Length);
            file.Dispose();
        }
        public static void WriterFile(String FileName,byte[] Data,int Offset)
        {
            FileStream file = File.Open(FileName, FileMode.OpenOrCreate);
            file.Write(Data,Offset,Data.Length);
            file.Dispose();
        }
        public static bool IsExists(String FilePath)
        {
            return File.Exists(FilePath);
        }
        public static void SaveFile(String FilePath, byte[] Source)
        {
            FileStream file = new FileStream(FilePath, FileMode.OpenOrCreate);
            file.Write(Source, 0, Source.Length);
            file.Dispose();
        }
        public static void SaveFile(String FilePath, String Source)
        {
            FileStream file = new FileStream(FilePath, FileMode.OpenOrCreate);
            StreamWriter writer = new StreamWriter(file);
            writer.WriteLine(Source);
            writer.Dispose();
            file.Dispose();
        }
        public static String[] GetFiles(String DirectoryPath)
        {
            return Directory.GetFiles(DirectoryPath);
        }
        public static String GetFileName(String file)
        {
            file = file.Replace('\\', '/');
            //FileInfo f = new FileInfo(file);
            //return f.Name.Substring(0,f.Name.LastIndexOf('.'));
            int codePoint = file.LastIndexOf('.');
            if (file.IndexOf('/') != -1)
            {
                return file.Substring(file.LastIndexOf('/')+1,codePoint- file.LastIndexOf('/')-1);
            }
            else
            {
                return file.Substring(0, codePoint);
            }
        }
        public static String GetFileType(String file)
        {
            int codePoint = file.LastIndexOf('.');
            return file.Substring(codePoint, file.Length - codePoint);
        }
    }
}
