using System;
using System.Linq.Expressions;

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

        /**
        * Returns a number if this cell has one possible state
        * Otherwise returns null
        */
        public int? GetValue() {
            int? value = null;
            for (int i = 0; i < 9; i++) {
                if (possibleValues[i]) {
                    if (value != null) {
                        return null;
                    } else {
                        value = i + 1;
                    }
                }
            }
            return value;
        }

        public bool contains(int v) {
            return possibleValues[v - 1];
        }

        public void reduce(int v) {
            possibleValues[v - 1] = false;
        }

        public void assign(int v)
        {
            for (int i = 0; i < 9; i++)
            {
                if (i != v) possibleValues[i - 1] = false;
            }
        }

    }

    public struct Sudoku
    {
        public Cell[,] cells;
        public Sudoku(string grid)
        {
            // create 2d cell array with x and y being columns and rows
            cells = new Cell[9, 9];
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

        }

        public static bool IsNeighbors(int x1, int y1, int x2, int y2) {
            if (x1 == x2 && y1 == y2) {
                // same cell
                return false;
            }
        
            if (x1 == x2 || y1 == y2) {
                // same column or row
                return true;
            }

            int bx1 = x1 / 3;
            int by1 = y1 / 3;
            int bx2 = x2 / 3;
            int by2 = y2 / 3;
            if (bx1 == bx2 && by1 == by2) {
                // same block
                return true;
            }

            // independent
            return false;
        }

        /**
        * Returns if (v1, v2) is a member of the constraints C((x1, y2), (x2, y2))
        */
        public bool SatisfiesConstraints(int x1, int y1, int v1, int x2, int y2, int v2) {
            // first check if the values are part of the cells' domains
            if (!cells[x1, y1].contains(v1)) return false;
            if (!cells[x2, y2].contains(v2)) return false;
        
            if (x1 == x2 && y1 == y2) {
                // C(i, i) constraint is being checked
                if (v1 != v2) return false; // a cell can not have two values at once

                // check our cell with each other fixed cell
                for (int x = 0; x < 9; x++) {
                    for (int y = 0; y < 9; y++) {
                        if (x == x1 && y == y2) continue;

                        int? value = cells[x, y].GetValue();
                        if (value == null) continue;

                        // the cell is fixed, so let's check if our cell can have value `v1` if the iterated cell has `value`
                        if (!SatisfiesConstraints(x1, y2, v1, x, y, value.Value)) {
                            return false;
                        }
                    }
                }

                // none of the fixed cells complained, so this cell having value `v1` is valid!
                return true;
            }
        
            if (!IsNeighbors(x1, y1, x2, y2)) {
                // the two cells are not in the same column, row or block
                // this means that they are independent and can have any value in relation to each other
                return true;
            }

            // the two cells are neighbors
            // this means they cannot have the same value
            return v1 != v2;
        }

        public void MakeNodeConsistent() {
            for (int x = 0; x < 9; x++) {
                for (int y = 0; y < 9; y++) {
                    for (int v = 1; v <= 9; v++) {
                        if (!cells[x, y].contains(v)) continue;

                        // if a domain value does not satisfy the constrains, remove it
                        if (!SatisfiesConstraints(x, y, v, x, y, v)) cells[x, y].reduce(v);
                    }
                }
            }
        }

        //Chronological Back Tracking
        public bool CBT(Sudoku sudoku, int x, int y) 
        {
            //Stop backtracking after the final cell
            if (x == 8 && y == 9) return true;

            //When reaching end of the column, go to next row. (out of bounds)
            if (y == 9)
            {
                x++;               //increase row
                y = 0;             //set column back to 0
            }

            //Go to the next cell if cell is already filled
            if (sudoku.cells[x, y].GetValue() != null) return CBT(sudoku, x, y + 1); 

            //Try filling in numbers in ascending order from 1 to 9.
            for (int i = 1; i <= 9; i++)
            {
                if (sudoku.cells[x, y].contains(i))
                {
                    //assign cell here...


                    if (CBT(sudoku, x, y + 1)) return true; 
                }
                //back track here...
            }
            return false;
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
                Console.Write(cells[x, y].ToString());
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

            Console.WriteLine("MakeNodeConsistent");
            s1.MakeNodeConsistent();
            s1.Echo();
        }
    }
}
