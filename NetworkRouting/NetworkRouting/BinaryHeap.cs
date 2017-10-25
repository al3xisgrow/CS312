using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkRouting
{
    class BinaryHeap : IQueue
    {
        int[] previous;
        int[] distance;
        int[] queue;
        int size = 0;
        int numRemoved = 0;

        // Runs in O(|V|) time. 
        public void makeQueue(int[] _distance, int[] _prev, int[] _array, int _numNodes)
        {
            distance = _distance;
            previous = _prev;
            queue = new int[_numNodes + 1];
            queue[0] = 0;
            for (int i = 0; i < _numNodes; i++)
            {
                insert(i);
            }
        }

        public void insert(int node)
        {
            bubbleup(node, queue[0]);
            queue[0]++;
        }

        public void decreaseKey(int node)
        {
            int index = findIndex(node);
            bubbleup(node, index);
        }

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

        public bool isEmpty()
        {
            return queue[0] == 0;
        }

        private void bubbleup(int node, int position)
        {
            int parent = position / 2;
            while (position != 1 && distance[queue[parent]] > distance[node])
            {
                queue[position] = queue[parent];
                position = parent;
                parent = position/ 2;
            }
            queue[position] = node;
        }   

        private void siftdown(int node, int position)
        {
            int child = minchild(position);
            while (child != 0 && distance[queue[child]] < distance[node])
            {
                queue[position] = queue[child];
                position = child;
                child = minchild(position);
            }
            queue[position] = node;
        }

        private int minchild(int pos)
        {
            if (2 * pos > queue[0])
                return 0;       // no children
            else if (2 * pos + 1 > queue[0])
                return 2 * pos;
            else
                return distance[queue[2 * pos]] > distance[queue[2 * pos + 1]] ? 2 * pos + 1 : 2 * pos;
        }

        private int findIndex(int node)
        {
            for (int i = 1; i <= queue[0]; i++)
            {
                if (queue[i] == node)
                    return i;
            }
            return -1;
        }
        
    }
}
