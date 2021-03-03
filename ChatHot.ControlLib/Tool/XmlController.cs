using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ChatHot.ControlLib.Tool
{
    public class XmlController : XmlDocument
    {
        /// <summary>
        /// 默认Xml文档中的 第一行
        /// </summary>
        public const string StanDardXmlNode = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
        /// <summary>
        /// 默认Xml文档中的 第二行
        /// </summary>
        public const string DefaultDataNode = "Body";
        public XmlNode Body { get => ChildNodes[1]; }
        public String XmlPath { get; private set; }
        public XmlController(String XmlPath, FileMode FileMode = FileMode.OpenOrCreate)
        {
            if (FileMode == FileMode.Open)
            {
            }
            else if (FileMode == FileMode.Create)
            {
                Create();
            }
            else if (FileMode == FileMode.OpenOrCreate)
            {
                if (!File.Exists(XmlPath))
                {
                    Create();
                }
            }
            else
            {
                throw new Exception("除Open与OpenOrCreate外没有其他的类型");
            }
            Open();
            void Create()
            {
                FileStream file = new FileStream(XmlPath, FileMode.Create);
                StreamWriter writer = new StreamWriter(file);
                writer.WriteLine(StanDardXmlNode);
                writer.WriteLine($"<{DefaultDataNode }></{DefaultDataNode}>");
                writer.Dispose();
                file.Dispose();
            }
            void Open()
            {
                Load(XmlPath);
            }
            this.XmlPath = XmlPath;
        }
        public XmlNode ModelToNode(Object XmlModel)
        {
            XmlNode node = CreateElement(XmlModel.GetType().Name);
            foreach (var Prop in XmlModel.GetType().GetProperties())
            {
                Object Value = Prop.GetValue(XmlModel);
                XmlNode row = CreateElement(Prop.Name);
                row.InnerText = (Value ?? "").ToString();
                node.AppendChild(row);
            }
            return node;
        }
        public T NodeToModel<T>(XmlNode Node)
        {
            T node = Activator.CreateInstance<T>();
            foreach (var Prop in typeof(T).GetProperties())
            {
                if (Node[Prop.Name] != null)
                {
                    var n = node.GetType().GetProperty(Prop.Name);
                    String PropertyType = n.PropertyType.Name;
                    if (n.PropertyType.Name == "Nullable`1")
                    {
                        PropertyType = n.PropertyType.GetGenericArguments().First().Name;
                    }
                    n.SetValue(node, Convert.ChangeType(Node[Prop.Name].InnerText, Enum.Parse<TypeCode>(PropertyType)));
                }
            }
            return node;
        }
        //获取所有Body ChildNodes[1] 中的节点
        public List<T> GetBodyElements<T>()
        {
            List<T> l = new List<T>();
            foreach (XmlNode ele in ChildNodes[1].ChildNodes)
            {
                l.Add(NodeToModel<T>(ele));
            }
            return l;
        }
        public void Save()
        {
            Save(XmlPath);
        }
        public void SelectALL(Action<XmlNode> CallBack)
        {
            foreach(XmlNode n in Body.ChildNodes)
            {
                CallBack?.Invoke(n);
            }
        }
    }
}
