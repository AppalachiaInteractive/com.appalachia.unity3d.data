using System.Collections.Generic;

namespace UltraLiteDB
{
    internal class IndexPage : BasePage
    {
        #region Constants and Static Readonly

        /// <summary>
        ///     If a Index Page has less that this free space, it's considered full page for new items.
        /// </summary>
        public const int INDEX_RESERVED_BYTES = 100;

        #endregion

        public IndexPage(uint pageID) : base(pageID)
        {
        }

        #region Fields and Autoproperties

        private Dictionary<ushort, IndexNode> _nodes = new Dictionary<ushort, IndexNode>();

        #endregion

        /// <summary>
        ///     Page type = Index
        /// </summary>
        public override PageType PageType => PageType.Index;

        public Dictionary<ushort, IndexNode> Nodes => _nodes;

        /// <summary>
        ///     Get node counter
        /// </summary>
        public int NodesCount => _nodes.Count;

        /// <summary>
        ///     Add new index node into this page. Update counters and free space
        /// </summary>
        public void AddNode(IndexNode node)
        {
            var index = _nodes.NextIndex();

            node.Position = new PageAddress(PageID, index);

            ItemCount++;
            FreeBytes -= node.Length;

            _nodes.Add(index, node);
        }

        /// <summary>
        ///     Delete node from this page and update counter and free space
        /// </summary>
        public void DeleteNode(IndexNode node)
        {
            ItemCount--;
            FreeBytes += node.Length;

            _nodes.Remove(node.Position.Index);
        }

        /// <summary>
        ///     Get an index node from this page
        /// </summary>
        public IndexNode GetNode(ushort index)
        {
            return _nodes[index];
        }

        #region Read/Write pages

        /// <inheritdoc />
        protected override void ReadContent(ByteReader reader)
        {
            _nodes = new Dictionary<ushort, IndexNode>(ItemCount);

            for (var i = 0; i < ItemCount; i++)
            {
                var index = reader.ReadUInt16();
                var levels = reader.ReadByte();

                var node = new IndexNode(levels);

                node.Page = this;
                node.Position = new PageAddress(PageID, index);
                node.Slot = reader.ReadByte();
                node.PrevNode = reader.ReadPageAddress();
                node.NextNode = reader.ReadPageAddress();
                node.KeyLength = reader.ReadUInt16();
                node.Key = reader.ReadBsonValue(node.KeyLength);
                node.DataBlock = reader.ReadPageAddress();

                for (var j = 0; j < node.Prev.Length; j++)
                {
                    node.Prev[j] = reader.ReadPageAddress();
                    node.Next[j] = reader.ReadPageAddress();
                }

                _nodes.Add(node.Position.Index, node);
            }
        }

        /// <inheritdoc />
        protected override void WriteContent(ByteWriter writer)
        {
            foreach (var node in _nodes.Values)
            {
                writer.Write(node.Position.Index);               // node Index on this page
                writer.Write((byte)node.Prev.Length);            // level length
                writer.Write(node.Slot);                         // index slot
                writer.Write(node.PrevNode);                     // prev node list
                writer.Write(node.NextNode);                     // next node list
                writer.Write(node.KeyLength);                    // valueLength
                writer.WriteBsonValue(node.Key, node.KeyLength); // value
                writer.Write(node.DataBlock);                    // data block reference

                for (var j = 0; j < node.Prev.Length; j++)
                {
                    writer.Write(node.Prev[j]);
                    writer.Write(node.Next[j]);
                }
            }
        }

        #endregion
    }
}
