using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkRouting
{
    interface IQueue
    {
        void makeQueue(int[] distance, int[] prev, int[] array, int numNodes);

        int deleteMin();

        void decreaseKey(int node);

        bool isEmpty();
    }
}
