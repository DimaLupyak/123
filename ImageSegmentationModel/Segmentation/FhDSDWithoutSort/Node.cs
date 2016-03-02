using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageSegmentationModel.Segmentation.FhDSDWithoutSort
{
    /// <summary>
    /// A UnionFindNode represents a set of nodes that it is a member of.
    /// 
    /// You can get the unique representative node of the set a given node is in by using the Find method.
    /// Two nodes are in the same set when their Find methods return the same representative.
    /// The IsUnionedWith method will check if two nodes' sets are the same (i.e. the nodes have the same representative).
    ///
    /// You can merge the sets two nodes are in by using the Union operation.
    /// There is no way to split sets after they have been merged.
    /// </summary>
    public class Node
    {
        private Node parent;
        private uint rank;
        public int Id { get; private set; }
        public int Size { get; private set; }
        public int SegmentId
        {
            get
            {
                return Find().Id;
            }
        }

        public int SegmentWeight { get; set; }
        /// <summary>
        /// Creates a new disjoint node, representative of a set containing only the new node.
        /// </summary>
        public Node(int id)
        {
            parent = this;
            Id = id;
            SegmentWeight = 0;
            Size = 1;
        }
        /// <summary>
        /// Returns the current representative of the set this node is in.
        /// Note that the representative is only accurate untl the next Union operation.
        /// </summary>
        public Node Find()
        {
            if (!ReferenceEquals(parent, this)) parent = parent.Find();
            return parent;
        }

        /// <summary>
        /// Determines whether or not this node and the other node are in the same set.
        /// </summary>
        public bool IsUnionedWith(Node other)
        {
            if (other == null) throw new ArgumentNullException("other");
            return ReferenceEquals(Find(), other.Find());
        }

        /// <summary>
        /// Merges the sets represented by this node and the other node into a single set.
        /// Returns whether or not the nodes were disjoint before the union operation (i.e. if the operation had an effect).
        /// </summary>
        /// <returns>True when the union had an effect, false when the nodes were already in the same set.</returns>
        public bool Union(Node other, int weight)
        {
            if (other == null) throw new ArgumentNullException("other");
            var root1 = this.Find();
            var root2 = other.Find();
            if (ReferenceEquals(root1, root2)) return false;

            if (root1.rank < root2.rank)
            {
                root1.parent = root2;
                root2.SegmentWeight += root1.SegmentWeight + weight;
                root2.Size += root1.Size;
            }
            else if (root1.rank > root2.rank)
            {
                root2.parent = root1;
                root1.SegmentWeight += root2.SegmentWeight + weight;
                root1.Size += root2.Size;
            }
            else
            {
                root2.parent = root1;
                root1.SegmentWeight += root2.SegmentWeight + weight;
                root1.Size += root2.Size;
                root1.rank++;
            }
            return true;
        }
    }
}
