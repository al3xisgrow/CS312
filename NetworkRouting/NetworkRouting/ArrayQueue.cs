using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkRouting
{
    class ArrayQueue : IQueue
    {
        int[] distance;
        int[] previous;
        int[] queue;
        int size = 0;
        int removed = 0;

        public void makeQueue(int[] _distance, int[] _previous, int[] _array, int numNodes)
        {
            distance = _distance;
            previous = _previous;
            queue = _array;
            size = numNodes;
        }

        public int deleteMin()
        {
            long minDistance = long.MaxValue;
            int minNode = -1;
            for (int i = 0; i < size; i++)
            {
                if (distance[i] < minDistance && queue[i] != -1)
                {
                    minDistance = distance[i];
                    minNode = i;
                }
            }
            if (minNode != -1)
            {
                queue[minNode] = -1;    // effectively removes it from the queue.
                removed++;
            }
            else
            {
                int tyler = -1;
            }
            return minNode;
        }

        public void decreaseKey(int node)
        {
            // do nothing.
        }

        public bool isEmpty()
        {
            return removed == size;
        }

    }
}
