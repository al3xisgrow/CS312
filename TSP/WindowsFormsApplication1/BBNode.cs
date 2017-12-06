using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSP;

namespace WindowsFormsApplication1
{
    class BBNode
    {
        public List<int> children{ get; set; }
        public int rowIndex { get; set; }
        public int[,] costMatrix { get; set; }
        public double localBound { get; set; }
        public ArrayList Route { get; set; }


        public BBNode(List<int> _list, int _row)
        {
            this.children = _list;
            this.rowIndex = _row;
            Route = new ArrayList();
        }

        public BBNode(List<int> _list, int[,] _matrix, int _row) : this(_list, _row)
        {
            this.costMatrix = _matrix;
        }

        public void reduceMatrix()
        {
            Console.WriteLine();
            for (int row = 0; row < costMatrix.GetLength(0); row++)
            {
                int rowMinVal = int.MaxValue;
                int rowMinIndex = -1;
                for (int col = 0; col < costMatrix.GetLength(1); col++)
                {
                    if (costMatrix[row, col] != double.PositiveInfinity && costMatrix[row, col] < rowMinVal)
                    {
                        rowMinVal = costMatrix[row, col];
                        rowMinIndex = col;
                    }
                }
                if (rowMinVal > 0 && rowMinVal < int.MaxValue)
                {
                    Console.WriteLine("RowMin Value: " + rowMinVal + " Found at index: " + row + "," + rowMinIndex);
                    for (int col = 0; col < costMatrix.GetLength(1); col++)
                    {
                        if(costMatrix[row, col] < int.MaxValue) costMatrix[row, col] -= rowMinVal;
                    }
                    Console.WriteLine("Adding " + rowMinVal + " to localBound of " + localBound);
                    localBound += rowMinVal;
                }

            }

            for (int col = 0; col < costMatrix.GetLength(1); col++)
            {
                int colMinVal = int.MaxValue;
                int colMinIndex = -1;
                for (int row = 0; row < costMatrix.GetLength(0); row++)
                {
                    if (costMatrix[row, col] != int.MaxValue && costMatrix[row,col] < colMinVal)
                    {
                        colMinVal = costMatrix[row, col];
                        colMinIndex = row;
                    }
                }
                if (colMinVal > 0 && colMinVal < int.MaxValue)
                {
                    Console.WriteLine("ColMin Value: " + colMinVal + " Found at index: " + col + "," + colMinIndex);
                    for (int row = 0; row < costMatrix.GetLength(0); row++)
                    {
                        if (costMatrix[row, col] < int.MaxValue)
                        {
                            costMatrix[row, col] -= colMinVal;
                        } 
                    }

                    Console.WriteLine("Adding " + colMinVal + " to localBound of " + localBound);
                    localBound += colMinVal;
                }
            }
        }
    }
}
