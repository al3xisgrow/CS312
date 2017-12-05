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
            // If the sequences are longer than the desired alignment length, align only the desired amount.
            int rows = maxLengthVal < sequenceA.Sequence.Length + 1? maxLengthVal : sequenceA.Sequence.Length + 1;
            int cols = maxLengthVal < sequenceB.Sequence.Length + 1? maxLengthVal : sequenceB.Sequence.Length + 1;
            // Create the cost matrix and the matrix used to track the path.
            int[,] matrix = new int[rows,cols];
            int[,] prev = new int[rows,cols];
            initializeMatrices(matrix, prev, rows, cols);

            // If it's not banded, do the unrestriced algorithm. Otherwise do banded.
            if (!banded) unrestricted(matrix, prev, rows, cols, sequenceA, sequenceB);
            else bandedAlg(matrix, prev, rows, cols, sequenceA, sequenceB);

            // The score is stored in the last cell.
            score = matrix[rows - 1, cols - 1];
            // Find the alignment strings by using the path stored in prev
            findAlignments(alignment, prev, rows, cols, score, sequenceA.Sequence, sequenceB.Sequence);

            // If the strings are too long to display, just display 100 characters.
            if (alignment[0].Length > 100) alignment[0] = alignment[0].Substring(0, 100);
            if (alignment[1].Length > 100) alignment[1] = alignment[1].Substring(0, 100);

            result.Update(score,alignment[0],alignment[1]);                  // bundling your results into the right object type 
            return(result);
        }

        private void findAlignments(string[] alignment, int[,] prev, int rows, int cols, int score, string sequenceA, string sequenceB)
        {
            // There was no alignment made. Return.
            if (score == int.MaxValue)
            {
                alignment[0] = alignment[1] = "No Alignment Possible";
                return;
            }

            int row, col;
            row = rows - 1;     // Index of last row
            col = cols - 1;     // Index of last column
            // Continue building the strings until we reach the first cell 
            while (row > 0 && cols > 0)
            {
                // Get path value.
                switch (prev[row, col])
                {
                    case DIAG:
                        // Was either a match or mismatch
                        alignment[0] = alignment[0].Insert(0, sequenceA.Substring(row-1, 1));
                        alignment[1] = alignment[1].Insert(0, sequenceB.Substring(col-1, 1));
                        row--;
                        col--;
                        break;
                    case UP:
                        // indel from above
                        // keep left, insert - in top
                        alignment[0] = alignment[0].Insert(0, sequenceA.Substring(row-1, 1));
                        alignment[1] = alignment[1].Insert(0, "-");
                        row--;
                        break;
                    case LEFT:
                        // indel from the left
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
            // If the difference is greater than 3, they cannot be aligned.
            if (Math.Abs(rows - cols) > 3)
            {
                // Set the score cell to int.MaxValue to indicate that they weren't aligned.
                matrix[rows - 1, cols - 1] = int.MaxValue;
                return;
            }

            int i, j;
            i = j = 0;
            // Get the larger of the two out of the rows and columns. This is because we need to get to the bottom right.
            int maxD = rows > cols ? rows : cols;
            // This loop travels down the diagonal
            for (i = 1; i < maxD; i++)
            {
                // This loop uses an offset to compute the cell on the diagonal, 3 to the right, and 3 below.
                for (j = 0; j < 4; j++)
                {
                    // Compute cell to the right
                    if (i + j < cols && i < rows) matrix[i, i + j] = computeVal(matrix, prev, i, i + j, sequenceA, sequenceB);
                    // Compute cell below.
                    if (i + j < rows && i < cols) matrix[i + j, i] = computeVal(matrix, prev, i + j, i, sequenceA, sequenceB);
                }
            }
        }

        private int computeVal(int[,] matrix, int[,] prev, int row, int col, GeneSequence sequenceA, GeneSequence sequenceB)
        {
            // Get the two letters to compare.
            char letterA = sequenceA.Sequence[row - 1];
            char letterB = sequenceB.Sequence[col - 1];
            int diagVal = letterA == letterB ? -3 : 1;      // If they are the same, the diagonal score is -3, otherwise 1
            int indelVal = 5;

            // If the diagnoal score is the smallest of the three, store the DIAG value in the prev matrix and return the cost for the cell at matrix[row,col]
            if (matrix[row - 1, col - 1] + diagVal <= matrix[row - 1, col] + indelVal && matrix[row - 1, col - 1] + diagVal <= matrix[row, col - 1] + indelVal)
            {
                prev[row, col] = DIAG;
                return matrix[row - 1, col - 1] + diagVal;
            }
            // Else if the indel score from above is the smallest, store the UP value in the prev matrix and return the cost
            else if (matrix[row - 1, col] + indelVal < matrix[row - 1, col - 1] + diagVal && matrix[row - 1, col] + indelVal <= matrix[row, col - 1] + indelVal)
            {
                prev[row, col] = UP;
                return matrix[row - 1, col] + indelVal;
            }
            // Else the indel score from the left must be the smallest. Store it in prev and return the cost.
            else
            {
                prev[row, col] = LEFT;
                return matrix[row, col - 1] + indelVal;
            }
        }

        private void unrestricted(int[,] matrix, int[,] prev, int rows, int cols, GeneSequence sequenceA, GeneSequence sequenceB)
        {
            // For every cell from top left to bottom right, compute the value. Compute by row.
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
            // Initialize the first row and first column with indel values.
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

        // Helper function to pring matrices to the console. Helpful to debug the smaller problems before moving to bigger ones.
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
