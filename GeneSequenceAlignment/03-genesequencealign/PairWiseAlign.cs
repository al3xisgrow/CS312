using System;
using System.Collections.Generic;
using System.Text;

namespace GeneticsLab
{
    class PairWiseAlign
    {
        int MaxCharactersToAlign;

        public PairWiseAlign()
        {
            // Default is to align only 5000 characters in each sequence.
            this.MaxCharactersToAlign = 5000;
        }

        public PairWiseAlign(int len)
        {
            // Alternatively, we can use an different length; typically used with the banded option checked.
            this.MaxCharactersToAlign = len;
        }

        const int UP = 1;
        const int DIAG = 2;
        const int LEFT = 3;
        /// <summary>
        /// this is the function you implement.
        /// </summary>
        /// <param name="sequenceA">the first sequence</param>
        /// <param name="sequenceB">the second sequence, may have length not equal to the length of the first seq.</param>
        /// <param name="banded">true if alignment should be band limited.</param>
        /// <returns>the alignment score and the alignment (in a Result object) for sequenceA and sequenceB.  The calling function places the result in the dispay appropriately.
        /// 
        public ResultTable.Result Align_And_Extract(GeneSequence sequenceA, GeneSequence sequenceB, bool banded)
        {
            ResultTable.Result result = new ResultTable.Result();
            int score;                                                       // place your computed alignment score here
            string[] alignment = new string[2];                              // place your two computed alignments here
            alignment[0] = alignment[1] = "";

            int maxLengthVal = banded ? 15001 : MaxCharactersToAlign;

            int rows = maxLengthVal < sequenceA.Sequence.Length + 1? maxLengthVal : sequenceA.Sequence.Length + 1;
            int cols = maxLengthVal < sequenceB.Sequence.Length + 1? maxLengthVal : sequenceB.Sequence.Length + 1;

            int[,] matrix = new int[rows,cols];
            int[,] prev = new int[rows,cols];
            initializeMatrices(matrix, prev, rows, cols);

            if (!banded) unrestricted(matrix, prev, rows, cols, sequenceA, sequenceB);
            else bandedAlg(matrix, prev, rows, cols, sequenceA, sequenceB);

            score = matrix[rows - 1, cols - 1];
            findAlignments(alignment, prev, rows, cols, score, sequenceA.Sequence, sequenceB.Sequence);

            if (alignment[0].Length > 100) alignment[0] = alignment[0].Substring(0, 100);
            if (alignment[1].Length > 100) alignment[1] = alignment[1].Substring(0, 100);

            result.Update(score,alignment[0],alignment[1]);                  // bundling your results into the right object type 
            return(result);
        }

        private void findAlignments(string[] alignment, int[,] prev, int rows, int cols, int score, string sequenceA, string sequenceB)
        {
            if (score == int.MaxValue)
            {
                alignment[0] = alignment[1] = "No Alignment Possible";
                return;
            }

            int row, col;
            row = rows - 1;
            col = cols - 1;
            while (row > 0 && cols > 0)
            {
                switch (prev[row, col])
                {
                    case DIAG:
                        alignment[0] = alignment[0].Insert(0, sequenceA.Substring(row-1, 1));
                        alignment[1] = alignment[1].Insert(0, sequenceB.Substring(col-1, 1));
                        row--;
                        col--;
                        break;
                    case UP:
                        // keep left, insert - in top
                        alignment[0] = alignment[0].Insert(0, sequenceA.Substring(row-1, 1));
                        alignment[1] = alignment[1].Insert(0, "-");
                        row--;
                        break;
                    case LEFT:
                        // keep top, insert - in left
                        alignment[0] = alignment[0].Insert(0, "-");
                        alignment[1] = alignment[1].Insert(0, sequenceB.Substring(col-1, 1));
                        col--;
                        break;
                }
            }
        }

        private void bandedAlg(int[,] matrix, int[,] prev, int rows, int cols, GeneSequence sequenceA, GeneSequence sequenceB)
        {
            if (Math.Abs(rows - cols) > 3)
            {
                matrix[rows - 1, cols - 1] = int.MaxValue;
                return;
            }

            int i, j;
            i = j = 0;
            try
            {
                int maxD = rows > cols ? rows : cols;
                for (i = 1; i < maxD; i++)
                {
                    for (j = 0; j < 4; j++)
                    {
                        if (i + j < cols && i < rows) matrix[i, i + j] = computeVal(matrix, prev, i, i + j, sequenceA, sequenceB);
                        if (i + j < rows && i < cols) matrix[i + j, i] = computeVal(matrix, prev, i + j, i, sequenceA, sequenceB);
                    }
                }
            }
            catch (IndexOutOfRangeException e)
            {
                Console.WriteLine("rows=" + rows + " cols=" + cols);
                Console.WriteLine("i=" + i + " j=" + j);
                
            }
            
        }

        private int computeVal(int[,] matrix, int[,] prev, int row, int col, GeneSequence sequenceA, GeneSequence sequenceB)
        {
            char letterA = sequenceA.Sequence[row - 1];
            char letterB = sequenceB.Sequence[col - 1];
            int diagVal = letterA == letterB ? -3 : 1;
            int indelVal = 5;

            if (matrix[row - 1, col - 1] + diagVal <= matrix[row - 1, col] + indelVal && matrix[row - 1, col - 1] + diagVal <= matrix[row, col - 1] + indelVal)
            {
                prev[row, col] = DIAG;
                return matrix[row - 1, col - 1] + diagVal;
            }
            else if (matrix[row - 1, col] + indelVal < matrix[row - 1, col - 1] + diagVal && matrix[row - 1, col] + indelVal <= matrix[row, col - 1] + indelVal)
            {
                prev[row, col] = UP;
                return matrix[row - 1, col] + indelVal;
            }
            else
            {
                prev[row, col] = LEFT;
                return matrix[row, col - 1] + indelVal;
            }
        }

        private void unrestricted(int[,] matrix, int[,] prev, int rows, int cols, GeneSequence sequenceA, GeneSequence sequenceB)
        {
            for (int i = 1; i < rows; i++)
            {
                for (int j = 1; j < cols; j++)
                {
                    matrix[i, j] = computeVal(matrix, prev, i, j, sequenceA, sequenceB);
                }
            }
        }

        private void initializeMatrices(int[,] matrix, int[,] prev, int rows, int cols)
        {
            int val = 0;
            for (int i = 0; i < cols; i++)
            {
                matrix[0, i] = val;
                prev[0, i] = LEFT;
                val += 5;
            }
            val = 5;
            for (int j = 1; j < rows; j++)
            {
                matrix[j, 0] = val;
                prev[j, 0] = UP;
                val += 5;
            }
        }

        private void printMatrices(int[,] matrix, int[,] prev, int rows, int cols)
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Console.Write(matrix[i, j] + " ");
                }
                Console.Write("\n");
            }
            Console.Write("\n");

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Console.Write(prev[i, j] + " ");
                }
                Console.Write("\n");
            }
        }
    }
}
