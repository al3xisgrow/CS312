using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkRouting
{
    class HeapQueue : IQueue
    {
        int[] previous;
        int[] distance;
        int[] queue;
        int[] indexes;
        int size = 0;
        int numRemoved = 0;
        

    // Runs in O(|V|) time. 
    public void makeQueue(int[] _distance, int[] _prev, int[] _array, int _numNodes)
        {
            distance = _distance;
            previous = _prev;
            indexes = new int[_numNodes];
            queue = new int[_numNodes + 1];
            queue[0] = 0;
            for (int i = 0; i < _numNodes; i++)
            {
                insert(i);
            }
        }

        // Inserts a node at the end of the queue, bubbles it up to as high as it should go according to its distance.
        public void insert(int node)
        {
            bubbleup(node, queue[0]);
            queue[0]++;
        }

        // called when a node's distance has been changed. Checks if it should be moved in the queue.
        public void decreaseKey(int node)
        {
            int index = findIndex(node);
            bubbleup(node, index);
        }

        // Returns the root node, and then rearranges the queue by putting the last element in the first position, and then sifting it down.
        public int deleteMin()
        {
            if (queue[0] == 0)
            {
                return -1;
            }
            else
            {
                int node = queue[1];
                siftdown(queue[queue[0]], 1);
                queue[0]--;
                return node;
            }
        }

        // The size is stored at queue[0]. Allows it to be accessed quickly, and made the queue indexed by 1, which simplified the math.
        public bool isEmpty()
        {
            return queue[0] == 0;
        }

        // Moves a node up the tree until it has less cost than it's children, but more than it's parent.
        private void bubbleup(int node, int position)
        {
            int parent = position / 2;
            while (position != 1 && distance[queue[parent]] > distance[node])
            {
                queue[position] = queue[parent];
                indexes[queue[parent]] = position;
                position = parent;
                parent = position/ 2;
            }
            queue[position] = node;
            indexes[node] = position;
        }   

        // Sifts down the tree, looking for the appropriate place to insert the node.
        private void siftdown(int node, int position)
        {
            int child = minchild(position);
            while (child != 0 && distance[queue[child]] < distance[node])
            {
                queue[position] = queue[child];
                indexes[queue[child]] = position;
                position = child;
                child = minchild(position);
            }
            queue[position] = node;
            indexes[node] = position;
        }

        // Helper function. Finds the child with the smallest distance.
        private int minchild(int pos)
        {
            if (2 * pos > queue[0])
                return 0;       // no children
            else if (2 * pos + 1 > queue[0])
                return 2 * pos;
            else
                return distance[queue[2 * pos]] > distance[queue[2 * pos + 1]] ? 2 * pos + 1 : 2 * pos;
        }

        // Helper function to help find index of a node. Not the optimal solution. Runs in O(|V|). Called by decrease key. 
        private int findIndex(int node)
        {
            return indexes[node];
        }
        
    }
}
