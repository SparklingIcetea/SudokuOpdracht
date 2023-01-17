using System;

namespace ConsoleApp1
{
    public struct Cell
    {
        // an array of 9 booleans that specify if the number is still possible
        public bool[] possibleValues;

        public override string ToString() {
            int? value = null;
            for (int i = 0; i < 9; i++) {
                if (possibleValues[i]) {
                    if (value != null) {
                        return " ";
                    } else {
                        value = i + 1;
                    }
                }
            }
            if (value == null) {
                return "x";
            }
            return value.ToString();
        }
    }

    public struct Block
    {
        public Cell[] cells;
        public Block(Cell[] cells)
        {
            this.cells = cells;
        }
    }

    public struct Sudoku
    {
        public Block[] blocks;
        public Sudoku(string grid)
        {
            // create 2d cell array with x and y being columns and rows
            Cell[,] cells = new Cell[9, 9];
            string[] numbers = grid.Split();

            for (int i = 0; i < numbers.Length; i++)
            {
                // get the column and row from the number index
                int column = i % 9;
                int row = i / 9;

                cells[column, row].possibleValues = new bool[9];
                if (numbers[i] == "0") {
                    // if the number is zero, it could have any value
                    for (int j = 0; j < 9; j++) {
                        cells[column, row].possibleValues[j] = true;
                    }
                } else {
                    // its possible value is only one
                    cells[column, row].possibleValues[int.Parse(numbers[i]) - 1] = true;
                }
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
                    SetCell(x, y, cells[x, y]);
                }
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
                if (i > 0) {
                    if ((i % 9) == 0) Console.WriteLine();
                    else if (i % 3 == 0) Console.Write("|");
                    if ((i % (9 * 3)) == 0) Console.WriteLine("---|---|---");
                }

                int x = i % 9;
                int y = i / 9;
                Console.Write(GetCell(x, y).ToString());
            }
            Console.WriteLine();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // string grid1, grid2, grid3, grid4, grid5;
            // grid1 = "0 0 3 0 2 0 6 0 0 9 0 0 3 0 5 0 0 1 0 0 1 8 0 6 4 0 0 0 0 8 1 0 2 9 0 0 7 0 0 0 0 0 0 0 8 0 0 6 7 0 8 2 0 0 0 0 2 6 0 9 5 0 0 8 0 0 2 0 3 0 0 9 0 0 5 0 1 0 3 0 0";
            // grid2 = "2 0 0 0 8 0 3 0 0 0 6 0 0 7 0 0 8 4 0 3 0 5 0 0 2 0 9 0 0 0 1 0 5 4 0 8 0 0 0 0 0 0 0 0 0 4 0 2 7 0 6 0 0 0 3 0 1 0 0 7 0 4 0 7 2 0 0 4 0 0 6 0 0 0 4 0 1 0 0 0 3";
            // grid3 = "0 0 0 0 0 0 9 0 7 0 0 0 4 2 0 1 8 0 0 0 0 7 0 5 0 2 6 1 0 0 9 0 4 0 0 0 0 5 0 0 0 0 0 4 0 0 0 0 5 0 7 0 0 9 9 2 0 1 0 8 0 0 0 0 3 4 0 5 9 0 0 0 5 0 7 0 0 0 0 0 0";
            // grid4 = "0 3 0 0 5 0 0 4 0 0 0 8 0 1 0 5 0 0 4 6 0 0 0 0 0 1 2 0 7 0 5 0 2 0 8 0 0 0 0 6 0 3 0 0 0 0 4 0 1 0 9 0 3 0 2 5 0 0 0 0 0 9 8 0 0 1 0 2 0 6 0 0 0 8 0 0 6 0 0 2 0";
            // grid5 = "0 2 0 8 1 0 7 4 0 7 0 0 0 0 3 1 0 0 0 9 0 0 0 2 8 0 5 0 0 9 0 4 0 0 8 7 4 0 0 2 0 8 0 0 3 1 6 0 0 3 0 2 0 0 3 0 2 7 0 0 0 6 0 0 0 5 6 0 0 0 0 8 0 7 6 0 5 1 0 9 0";
            // string solved = "4 5 3 8 2 6 1 9 7 8 9 2 5 7 1 6 3 4 1 6 7 4 9 3 5 2 8 7 1 4 9 5 2 8 6 3 5 8 6 1 3 7 2 4 9 3 2 9 6 8 4 7 5 1 9 3 5 2 1 8 4 7 6 6 7 1 3 4 5 9 8 2 2 4 8 7 6 9 3 1 5";

            // Ask sudoku from user in the same format as the sudokus above
            Console.Write("give me a sudoku: ");
            string toSolve = Console.ReadLine();

            // Parse sudoku
            Sudoku s1 = new Sudoku(toSolve);
            s1.Echo();
        }
    }
}
