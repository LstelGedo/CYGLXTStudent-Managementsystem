using System;
using System.Collections.Generic;

namespace CYGLXTStudent.Resources.PublicClass
{
    public class Node
    {
        public Node()
        {
            this.NodeId = Guid.NewGuid().ToString();
            this.IsDeleted = false;
            this.Nodes = new List<Node>();
        }
        /// <summary>
        /// 每个节点的首先会显示一个ICON
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// 节点ID
        /// </summary>
        public string NodeId { get; set; }

        /// <summary>
        ///  节点名称
        /// </summary>
        public string NodeName { get; set; }

        /// <summary>
        /// 节点携带的内容   
        /// </summary>
        public string NodeContent { get; set; }

        /// <summary>
        /// 被删除
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 节点类型
        /// </summary>
        public NodeType NodeType { get; set; }

        /// <summary>
        /// 子节点集合
        /// </summary>
        public List<Node> Nodes { get; set; }
    }
    public enum NodeType
    {
        RootNode,//根节点
        LeafNode,//叶子节点
        StructureNode//结构节点，仅起到组织配置文件结构的作用，不参与修改
    }
}
