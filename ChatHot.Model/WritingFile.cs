using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ChatHot.Model
{
    public class WritingFile:IDisposable
    {
        /// <summary>
        /// 写入成功 
        /// </summary>
        public event Action<Object,String> FileSuccess;
        private FileStream file =null;
        public long WritePointer { get; private set; }
        public String FileName { get; private set; }
        /// <summary>
        /// 可以理解为第二个Tag
        /// </summary>
        public String Mark { get; set; }
        public Object Tag { get; set; }
        /// <summary>
        /// 如果这个类出错 多半是因为（硬盘）内存不够
        /// </summary>
        /// <param name="FileName"></param>
        public WritingFile(String FileName)
        {
            file = new FileStream(FileName, FileMode.OpenOrCreate);
            file.Position = 0;
        }
        /// <summary>
        /// 写入内容
        /// </summary>
        /// <param name="Content">内容</param>
        public void Writer(byte[] Content)
        {
            file.Write(Content, 0, Content.Length);
        }
        /// <summary>
        /// 结束写入，释放资源
        /// </summary>
        public void WriterEnd()
        {
            FileSuccess?.Invoke(this, FileName);
            Dispose();
        }
        public void Dispose()
        {
            file.Dispose();
        }
    }
}
