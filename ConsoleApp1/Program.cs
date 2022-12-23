using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ConsoleApp1
{
    public struct Cell
    {
        public bool isFixed;
        public int data;
    }

    public struct Block
    {
        public Cell[] cells;
        public Block(Cell[] cells)
        {
            this.cells = cells;
        }

        public void FillRandom(Random random)
        {
            // for each block, put the numbers 1-9 inside randomly
            // we start with a list of all 9 unused numbers
            List<int> unused = new List<int>(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });

            // remove the numbers already used by fixed cells
            for (int inside = 0; inside < 9; inside++)
            {
                if (cells[inside].isFixed)
                {
                    unused.Remove(cells[inside].data);
                }
            }

            // populate block array
            for (int inside = 0; inside < 9; inside++)
            {
                if (!cells[inside].isFixed)
                {
                    // put in a random unused number
                    int next = random.Next(unused.Count);
                    cells[inside].data = unused[next];
                    unused.RemoveAt(next);
                }
            }
        }

        /**
         *  Swaps the two cells at indices a and b
         */
        public void SwapCells(int a, int b) {
            Cell tmp = cells[a];
            cells[a] = cells[b];
            cells[b] = tmp;
        }

        /**
         *  Returns all pairs of non-fixed cells
         */
        public IEnumerable<(int, int)> SwappablePairs()
        {
            for (int a = 0; a < 9; a++)
            {
                if (cells[a].isFixed) continue;
                for (int b = a + 1; b < 9; b++)
                {
                    if (cells[b].isFixed) continue;
                    yield return (a, b);
                }
            }
        }

        /**
         *  Prints the values of all cells in this block. *** This ugly function is for testing only, remove before submitting. ***
         */
        public void printCells()
        {

            string row1 = $"{cells[0].data} {cells[1].data} {cells[2].data}";
            string row2 = $"{cells[3].data} {cells[4].data} {cells[5].data}";
            string row3 = $"{cells[6].data} {cells[7].data} {cells[8].data}";

            Console.WriteLine(row1);
            Console.WriteLine(row2);
            Console.WriteLine(row3);
            Console.WriteLine(Environment.NewLine);
        }

    }

    public struct Sudoku
    {
        public Block[] blocks;
        public Sudoku(string grid, Random random)
        {
            // create 2d cell array with x and y being columns and rows
            Cell[,] cells = new Cell[9, 9];
            string[] numbers = grid.Split();

            for (int i = 0; i < numbers.Length; i++)
            {
                // get the column and row from the number index
                int column = i % 9;
                int row = i / 9;

                // if the number is nonzero, its value is fixed and should never be swapped later on
                cells[column, row].isFixed = (numbers[i] != "0");
                cells[column, row].data = int.Parse(numbers[i]);
            }

            // put cells in blocks, where each block is a 3x3 region of the sudoku
            this.blocks = new Block[9];
            for (int i = 0; i < 9; i++)
            {
                this.blocks[i] = new Block(new Cell[9]);
            }

            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    if (cells[x, y].isFixed)
                    {
                        SetCell(x, y, cells[x, y]);
                    }
                }
            }

            // fill the rest of the blocks randomly
            for (int i = 0; i < 9; i++)
            {
                this.blocks[i].FillRandom(random);
            }
        }

        public void RandomWalk(Random random, int iterations = 1)
        {
            // Repeat the swap for the given number of iterations
            for (int i = iterations; i > 0; i--)
            {
                // Choose a random value between 1 and 9 which will be the index of the randomly selected block
                int iBlockChosen = random.Next(0, 9);

                // Get the block object
                Block block = this.blocks[iBlockChosen];

                // Create list which will contain the cells to shuffle
                List<int> cellsToShuffle = new List<int>();

                // Iterate over every cell in the block
                for (int j = 0; j < 9; j++)
                {
                    // Check if cell is fixed or not
                    if (!block.cells[j].isFixed)
                        // Cell is not fixed, add to the list
                        cellsToShuffle.Add(j);
                }

                // Get random integer between 1 and the number of elements to be shuffled
                int iRandom1 = random.Next(0, cellsToShuffle.Count);
                int iRandom2 = random.Next(0, cellsToShuffle.Count);

                while (iRandom2 == iRandom1)
                {
                    iRandom2 = random.Next(0, cellsToShuffle.Count);
                }

                // Swap
                block.SwapCells(cellsToShuffle[iRandom1], cellsToShuffle[iRandom2]);
            }
        }

        public bool HillClimb(Random random)
        {
            // 1. Kies willekeurig een van de 9 (3 × 3)-blokken
            int block = random.Next(9);

            // 2. Probeer alle mogelijke swaps - binnen het blok - van 2 niet-gefixeerde cijfers
            int bestA = 0;
            int bestB = 0;
            int startScore = Evaluate();
            int score = startScore;

            foreach ((int a, int b) in blocks[block].SwappablePairs()) {
                // bekijk successor
                blocks[block].SwapCells(a, b);
                int newScore = Evaluate();

                // ga terug naar huidige toestand
                blocks[block].SwapCells(a, b);

                if (newScore <= score) {
                    bestA = a;
                    bestB = b;
                    score = newScore;
                }
            }

            // 3. Kies hieruit de beste indien die een verbetering of gelijke score oplevert
            if (bestA == bestB) {
                // geen verbetering gevonden
                return false;
            } else {
                // kies successor
                blocks[block].SwapCells(bestA, bestB);
                return score < startScore;
            }
        }

        /**
        * Get the cell at a specified column and row
        */
        public Cell GetCell(int x, int y)
        {
            int block = (y / 3) * 3 + (x / 3);
            int inside = (y % 3) * 3 + (x % 3);
            return this.blocks[block].cells[inside];
        }

        /**
         * Modifies the cell at a specified column and row
         */
        public void SetCell(int x, int y, Cell cell)
        {
            int block = (y / 3) * 3 + (x / 3);
            int inside = (y % 3) * 3 + (x % 3);
            this.blocks[block].cells[inside] = cell;
        }

        /**
        * Pretty print the Sudoku to the console
        */
        public void Echo()
        {
            for (int i = 0; i < 81; i++)
            {
                if (i % 3 == 0) Console.Write(" ");
                if ((i % 9) == 0) Console.WriteLine();
                if ((i % (9 * 3)) == 0) Console.WriteLine();

                int x = i % 9;
                int y = i / 9;
                Console.Write(GetCell(x, y).data.ToString());
            }
            Console.WriteLine();
        }

        public int Evaluate()
        {
            int score = 0;
            for (int i = 0; i < 9; i++) //Checks score for horizontal rows.
            {
                bool[] missing = { true, true, true, true, true, true, true, true, true }; //Create array to keep track of missing numbers
                for (int j = 0; j < 9; j++)
                {
                    missing[GetCell(i, j).data - 1] = false; //Turn index of missing number to false
                }
                for (int k = 0; k < 9; k++)
                {
                    if (missing[k]) score++; //If there are missing numbers, add them to the score.
                }
            }

            for (int j = 0; j < 9; j++) //Checks score for vertical rows.
            {
                bool[] missing = { true, true, true, true, true, true, true, true, true };
                for (int i = 0; i < 9; i++)
                {
                    missing[GetCell(i, j).data - 1] = false;
                }
                for (int k = 0; k < 9; k++)
                {
                    if (missing[k]) score++;
                }
            }
            return score;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string grid1, grid2, grid3, grid4, grid5;

            grid1 = "0 0 3 0 2 0 6 0 0 9 0 0 3 0 5 0 0 1 0 0 1 8 0 6 4 0 0 0 0 8 1 0 2 9 0 0 7 0 0 0 0 0 0 0 8 0 0 6 7 0 8 2 0 0 0 0 2 6 0 9 5 0 0 8 0 0 2 0 3 0 0 9 0 0 5 0 1 0 3 0 0";
            grid2 = "2 0 0 0 8 0 3 0 0 0 6 0 0 7 0 0 8 4 0 3 0 5 0 0 2 0 9 0 0 0 1 0 5 4 0 8 0 0 0 0 0 0 0 0 0 4 0 2 7 0 6 0 0 0 3 0 1 0 0 7 0 4 0 7 2 0 0 4 0 0 6 0 0 0 4 0 1 0 0 0 3";
            grid3 = "0 0 0 0 0 0 9 0 7 0 0 0 4 2 0 1 8 0 0 0 0 7 0 5 0 2 6 1 0 0 9 0 4 0 0 0 0 5 0 0 0 0 0 4 0 0 0 0 5 0 7 0 0 9 9 2 0 1 0 8 0 0 0 0 3 4 0 5 9 0 0 0 5 0 7 0 0 0 0 0 0";
            grid4 = "0 3 0 0 5 0 0 4 0 0 0 8 0 1 0 5 0 0 4 6 0 0 0 0 0 1 2 0 7 0 5 0 2 0 8 0 0 0 0 6 0 3 0 0 0 0 4 0 1 0 9 0 3 0 2 5 0 0 0 0 0 9 8 0 0 1 0 2 0 6 0 0 0 8 0 0 6 0 0 2 0";
            grid5 = "0 2 0 8 1 0 7 4 0 7 0 0 0 0 3 1 0 0 0 9 0 0 0 2 8 0 5 0 0 9 0 4 0 0 8 7 4 0 0 2 0 8 0 0 3 1 6 0 0 3 0 2 0 0 3 0 2 7 0 0 0 6 0 0 0 5 6 0 0 0 0 8 0 7 6 0 5 1 0 9 0";
            
            string solved = "4 5 3 8 2 6 1 9 7 8 9 2 5 7 1 6 3 4 1 6 7 4 9 3 5 2 8 7 1 4 9 5 2 8 6 3 5 8 6 1 3 7 2 4 9 3 2 9 6 8 4 7 5 1 9 3 5 2 1 8 4 7 6 6 7 1 3 4 5 9 8 2 2 4 8 7 6 9 3 1 5";

            int tests = 1000;
            int iLimit = 20;
            Random random = new Random();
            string toSolve = grid5;

            for (int S = 15; S < 30; S++) {
                int[] results = new int[tests];

                int average = 0;
                var timer = new Stopwatch();

                timer.Start();
                for (int i = 0; i < tests; i++) {
                    Sudoku s1 = new Sudoku(toSolve, random);

                    while (s1.Evaluate() > 0)
                    {
                        int iCurrIteration = 0;

                        s1.RandomWalk(random, S);

                        while (iCurrIteration < iLimit) {
                            results[i]++;
                            if (!s1.HillClimb(random)) {
                                iCurrIteration += 1;
                            } else {
                                if (s1.Evaluate() == 0) break;
                                iCurrIteration = 0;
                            }
                        }
                    }

                    average += results[i];
                }
                timer.Stop();

                average /= tests;
                long variance = 0;
                foreach (int total in results) {
                    int deviation = average - total;
                    variance += deviation * deviation;
                }
                double v = Math.Sqrt((double) variance / (tests - 1));

                long time = timer.ElapsedMilliseconds;
                time /= tests;

                Console.Write("S = ");
                Console.WriteLine(S);
                Console.Write("Min #steps: ");
                Console.WriteLine(results.Min());
                Console.Write("Max #steps: ");
                Console.WriteLine(results.Max());
                Console.Write("Mean #steps: ");
                Console.WriteLine(average);
                Console.Write("Variance #steps: ");
                Console.WriteLine(v);
                Console.Write("Average time (ms): ");
                Console.WriteLine(time);
                Console.WriteLine();
            }
        }
    }
}

// S = 1,  grid4: 12188 steps, 649 ms
// S = 2,  grid4:  9579 steps, 526 ms
// S = 3,  grid4:  8518 steps, 458 ms
// S = 4,  grid4:  
// S = 6,  grid4:  9185 steps, 496 ms
// S = 10, grid4:  9876 steps, 543 ms
