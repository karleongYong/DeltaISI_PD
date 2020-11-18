using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XyratexOSC.XMath
{
    /// <summary>
    /// A Matrix Class. Represents a rectangular array of doubles.
    /// </summary>
    public class Matrix
    {
        private double[,] _values;
        private int _rowCount = 3;
        private int _columnCount = 3;

        /// <summary>
        /// Gets or sets the <see cref="System.Double"/> with the specified row and column.
        /// </summary>
        /// <value>
        /// The <see cref="System.Double"/>.
        /// </value>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        public double this[int row, int column]
        {
            get { return _values[row, column]; }
            set { _values[row, column] = value; }
        }

        /// <summary>
        /// Gets the row count.
        /// </summary>
        /// <value>
        /// The row count.
        /// </value>
        public int RowCount
        {
            get { return _rowCount; }
        }

        /// <summary>
        /// Gets the column count.
        /// </summary>
        /// <value>
        /// The column count.
        /// </value>
        public int ColumnCount
        {
            get { return _columnCount; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix"/> class.
        /// </summary>
        public Matrix()
        {
            _values = new double[_rowCount, _columnCount];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix"/> class.
        /// </summary>
        /// <param name="rowCount">The row count.</param>
        /// <param name="columnCount">The column count.</param>
        public Matrix(int rowCount, int columnCount)
        {
            _rowCount = rowCount;
            _columnCount = columnCount;
            _values = new double[_rowCount, _columnCount];
        }

        /// <summary>
        /// Creates an identity matrix of the specified size [n,n].
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        public static Matrix Identity(int size)
        {
            Matrix resultMatrix = new Matrix(size, size);
            Parallel.For(0, size, (i) =>
            {
                for (int j = 0; j < size; j++)
                {
                    resultMatrix[i, j] = (i == j) ? 1.0 : 0.0;
                }
            }
            );
            return resultMatrix;
        }

        /// <summary>
        /// Clones this matrix instance.
        /// </summary>
        /// <returns></returns>
        public Matrix Clone()
        {
            Matrix resultMatrix = new Matrix(_rowCount, _columnCount);
            Parallel.For(0, _rowCount, (i) =>
            {
                for (int j = 0; j < _columnCount; j++)
                {
                    resultMatrix[i, j] = this[i, j];
                }
            }
            );
            return resultMatrix;
        }

        /// <summary>
        /// Transposes this matrix instance.
        /// </summary>
        /// <returns></returns>
        public Matrix Transpose()
        {
            Matrix resultMatrix = new Matrix(_columnCount, _rowCount);
            Parallel.For(0, _rowCount, (i) =>
            {
                for (int j = 0; j < _columnCount; j++)
                {
                    resultMatrix[j, i] = this[i, j];
                }
            }
            );
            return resultMatrix;
        }

        /// <summary>
        /// Gets the row values at the specified row index in array format.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <returns>The row value array.</returns>
        public double[] GetRow(int rowIndex)
        {
            double[] result = new double[_columnCount];

            for (int i = 0; i < _columnCount; i++)
                result[i] = _values[rowIndex, i];

            return result;
        }

        /// <summary>
        /// Gets the column values at the specified column index in array format.
        /// </summary>
        /// <param name="columnIndex">Index of the column.</param>
        /// <returns>
        /// The column value array.
        /// </returns>
        public double[] GetColumn(int columnIndex)
        {
            double[] result = new double[_rowCount];

            for (int i = 0; i < _rowCount; i++)
                result[i] = _values[i, columnIndex];

            return result;
        }

        #region Binary Math

        /// <summary>
        /// Adds the left matrix to the right matrix.
        /// </summary>
        /// <param name="leftMatrix">The left matrix.</param>
        /// <param name="rightMatrix">The right matrix.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">
        /// Matrices must have the same number of columns for addition.
        /// or
        /// Matrices must have the same number of rows for addition.
        /// </exception>
        public static Matrix Add(Matrix leftMatrix, Matrix rightMatrix)
        {
            if (leftMatrix.ColumnCount != rightMatrix.ColumnCount)
                throw new Exception("Matrices must have the same number of columns for addition.");

            if (leftMatrix.RowCount != rightMatrix.RowCount)
                throw new Exception("Matrices must have the same number of rows for addition.");

            Matrix resultMatrix = new Matrix(leftMatrix.RowCount, rightMatrix.ColumnCount);
            Parallel.For(0, leftMatrix.RowCount, (i) =>
            {
                for (int j = 0; j < leftMatrix.ColumnCount; j++)
                {
                    resultMatrix[i, j] = leftMatrix[i, j] + rightMatrix[i, j];
                }
            });

            return resultMatrix;
        }

        /// <summary>
        /// Adds the two matrices.
        /// </summary>
        /// <param name="leftMatrix">The left matrix.</param>
        /// <param name="rightMatrix">The right matrix.</param>
        /// <returns></returns>
        public static Matrix operator +(Matrix leftMatrix, Matrix rightMatrix)
        {
            return Matrix.Add(leftMatrix, rightMatrix);
        }

        /// <summary>
        /// Adds a scalar value to the specified matrix.
        /// </summary>
        /// <param name="leftMatrix">The left matrix.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns></returns>
        public static Matrix Add(Matrix leftMatrix, double scalar)
        {
            Matrix resultMatrix = new Matrix(leftMatrix.RowCount, leftMatrix.ColumnCount);
            Parallel.For(0, leftMatrix.RowCount, (i) =>
            {
                for (int j = 0; j < leftMatrix.ColumnCount; j++)
                {
                    resultMatrix[i, j] = leftMatrix[i, j] + scalar;
                }
            });

            return resultMatrix;
        }

        /// <summary>
        /// Adds a scalar value to the specified matrix.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="rightMatrix">The right matrix.</param>
        /// <returns></returns>
        public static Matrix Add(double scalar, Matrix rightMatrix)
        {
            return Matrix.Add(rightMatrix, scalar);
        }

        /// <summary>
        /// Adds a scalar to a matrix.
        /// </summary>
        /// <param name="leftMatrix">The left matrix.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns></returns>
        public static Matrix operator +(Matrix leftMatrix, double scalar)
        {
            return Matrix.Add(leftMatrix, scalar);
        }

        /// <summary>
        /// Adds a scalar to a matrix.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="rightMatrix">The right matrix.</param>
        /// <returns></returns>
        public static Matrix operator +(double scalar, Matrix rightMatrix)
        {
            return Matrix.Add(rightMatrix, scalar);
        }

        /// <summary>
        /// Subtracts the right matrix from the left matrix.
        /// </summary>
        /// <param name="leftMatrix">The left matrix.</param>
        /// <param name="rightMatrix">The right matrix.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">
        /// Matrices must have the same number of columns for subtraction.
        /// or
        /// Matrices must have the same number of rows for subtraction.
        /// </exception>
        public static Matrix Subtract(Matrix leftMatrix, Matrix rightMatrix)
        {
            if (leftMatrix.ColumnCount != rightMatrix.ColumnCount)
                throw new Exception("Matrices must have the same number of columns for subtraction.");

            if (leftMatrix.RowCount != rightMatrix.RowCount)
                throw new Exception("Matrices must have the same number of rows for subtraction.");

            Matrix resultMatrix = new Matrix(leftMatrix.RowCount, rightMatrix.ColumnCount);
            Parallel.For(0, leftMatrix.RowCount, (i) =>
            {
                for (int j = 0; j < leftMatrix.ColumnCount; j++)
                {
                    resultMatrix[i, j] = leftMatrix[i, j] - rightMatrix[i, j];
                }
            });

            return resultMatrix;
        }

        /// <summary>
        /// Subtracts the two matrices.
        /// </summary>
        /// <param name="leftMatrix">The left matrix.</param>
        /// <param name="rightMatrix">The right matrix.</param>
        /// <returns></returns>
        public static Matrix operator -(Matrix leftMatrix, Matrix rightMatrix)
        {
            return Matrix.Subtract(leftMatrix, rightMatrix);
        }

        /// <summary>
        /// Subtracts a scalar from the specified left matrix.
        /// </summary>
        /// <param name="leftMatrix">The left matrix.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns></returns>
        public static Matrix Subtract(Matrix leftMatrix, double scalar)
        {
            Matrix resultMatrix = new Matrix(leftMatrix.RowCount, leftMatrix.ColumnCount);
            Parallel.For(0, leftMatrix.RowCount, (i) =>
            {
                for (int j = 0; j < leftMatrix.ColumnCount; j++)
                {
                    resultMatrix[i, j] = leftMatrix[i, j] - scalar;
                }
            });

            return resultMatrix;
        }

        /// <summary>
        /// Subtracts the specified matrix from the specified scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="rightMatrix">The right matrix.</param>
        /// <returns></returns>
        public static Matrix Subtract(double scalar, Matrix rightMatrix)
        {
            Matrix resultMatrix = new Matrix(rightMatrix.RowCount, rightMatrix.ColumnCount);
            Parallel.For(0, rightMatrix.RowCount, (i) =>
            {
                for (int j = 0; j < rightMatrix.ColumnCount; j++)
                {
                    resultMatrix[i, j] = scalar - rightMatrix[i, j];
                }
            });

            return resultMatrix;
        }

        /// <summary>
        /// Subtracts a scalar from a matrix.
        /// </summary>
        /// <param name="leftMatrix">The left matrix.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns></returns>
        public static Matrix operator -(Matrix leftMatrix, double scalar)
        {
            return Matrix.Subtract(leftMatrix, scalar);
        }

        /// <summary>
        /// Subtracts a matrix from a scalar
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="rightMatrix">The right matrix.</param>
        /// <returns></returns>
        public static Matrix operator -(double scalar, Matrix rightMatrix)
        {
            return Matrix.Subtract(scalar, rightMatrix);
        }

        /// <summary>
        /// Multiplies the specified matrices. The column count of the left matrix must match the row count of the right matrix for multiplication.
        /// </summary>
        /// <param name="leftMatrix">The left matrix.</param>
        /// <param name="rightMatrix">The right matrix.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">The column count of the left matrix must match the row count of the right matrix for multiplication.</exception>
        public static Matrix Multiply(Matrix leftMatrix, Matrix rightMatrix)
        {
            if (leftMatrix.ColumnCount != rightMatrix.RowCount)
                throw new Exception("The column count of the left matrix must match the row count of the right matrix for multiplication.");

            Matrix resultMatrix = new Matrix(leftMatrix.RowCount, rightMatrix.ColumnCount);
            Parallel.For(0, resultMatrix.ColumnCount, (i) =>
            {
                for (int j = 0; j < leftMatrix.RowCount; j++)
                {
                    double value = 0.0;
                    for (int k = 0; k < rightMatrix.RowCount; k++)
                    {
                        value += leftMatrix[j, k] * rightMatrix[k, i];
                    }
                    resultMatrix[j, i] = value;
                }
            }
            );
            return resultMatrix;
        }

        /// <summary>
        /// Multiplies the specified matrices. The column count of the left matrix must match the row count of the right matrix for multiplication.
        /// </summary>
        /// <param name="leftMatrix">The left matrix.</param>
        /// <param name="rightMatrix">The right matrix.</param>
        /// <returns></returns>
        public static Matrix operator *(Matrix leftMatrix, Matrix rightMatrix)
        {
            return Matrix.Multiply(leftMatrix, rightMatrix);
        }

        /// <summary>
        /// Multiplies the matrix by a scalar value.
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="rightMatrix">The right matrix.</param>
        /// <returns></returns>
        public static Matrix Multiply(double left, Matrix rightMatrix)
        {
            Matrix resultMatrix = new Matrix(rightMatrix.RowCount, rightMatrix.ColumnCount);
            Parallel.For(0, resultMatrix.RowCount, (i) =>
            {
                for (int j = 0; j < rightMatrix.ColumnCount; j++)
                {
                    resultMatrix[i, j] = left * rightMatrix[i, j];
                }
            }
            );
            return resultMatrix;
        }

        /// <summary>
        /// Multiplies the matrix by a scalar value.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="rightMatrix">The right matrix.</param>
        /// <returns></returns>
        public static Matrix operator *(double left, Matrix rightMatrix)
        {
            return Matrix.Multiply(left, rightMatrix);
        }

        /// <summary>
        /// Multiplies the matrix by a scalar value.
        /// </summary>
        /// <param name="leftMatrix">The left matrix.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static Matrix Multiply(Matrix leftMatrix, double right)
        {
            Matrix resultMatrix = new Matrix(leftMatrix.RowCount, leftMatrix.ColumnCount);
            Parallel.For(0, leftMatrix.RowCount, (i) =>
            {
                for (int j = 0; j < leftMatrix.ColumnCount; j++)
                {
                    resultMatrix[i, j] = leftMatrix[i, j] * right;
                }
            }
            );
            return resultMatrix;
        }

        /// <summary>
        /// Multiplies the matrix by a scalar value.
        /// </summary>
        /// <param name="leftMatrix">The left matrix.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static Matrix operator *(Matrix leftMatrix, double right)
        {
            return Matrix.Multiply(leftMatrix, right);
        }

        /// <summary>
        /// Divides the specified left matrix by the scalar value.
        /// </summary>
        /// <param name="leftMatrix">The left matrix.</param>
        /// <param name="right">The right value.</param>
        /// <returns></returns>
        public static Matrix Divide(Matrix leftMatrix, double right)
        {
            Matrix resultMatrix = new Matrix(leftMatrix.RowCount, leftMatrix.ColumnCount);
            Parallel.For(0, leftMatrix.RowCount, (i) =>
            {
                for (int j = 0; j < leftMatrix.ColumnCount; j++)
                {
                    resultMatrix[i, j] = leftMatrix[i, j] / right;
                }
            }
            );
            return resultMatrix;
        }

        /// <summary>
        /// Divides the specified left matrix by the scalar value.
        /// </summary>
        /// <param name="leftMatrix">The left matrix.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static Matrix operator /(Matrix leftMatrix, double right)
        {
            return Matrix.Divide(leftMatrix, right);
        }

        /// <summary>
        /// Divides the two matrices.
        /// </summary>
        /// <param name="leftMatrix">The left matrix.</param>
        /// <param name="rightMatrix">The right matrix.</param>
        /// <returns></returns>
        public static Matrix Divide(Matrix leftMatrix, Matrix rightMatrix)
        {
            return Matrix.Multiply(leftMatrix, rightMatrix.Invert());
        }

        /// <summary>
        /// Divides the two matrices.
        /// </summary>
        /// <param name="leftMatrix">The left matrix.</param>
        /// <param name="rightMatrix">The right matrix.</param>
        /// <returns></returns>
        public static Matrix operator /(Matrix leftMatrix, Matrix rightMatrix)
        {
            return Matrix.Divide(leftMatrix, rightMatrix);
        }

        /// <summary>
        /// Returns the absolute value of the specified matrix. All values in the matrix become absolute value.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <returns></returns>
        public static Matrix Abs(Matrix matrix)
        {
            Matrix resultMatrix = new Matrix(matrix.RowCount, matrix.ColumnCount);

            Parallel.For(0, resultMatrix.ColumnCount, (i) =>
            {
                for (int j = 0; j < resultMatrix.RowCount; j++)
                {
                    resultMatrix[i, j] = Math.Abs(matrix[i, j]);
                }
            });

            return resultMatrix;
        }

        #endregion

        #region Casts

        /// <summary>
        /// Creates a matrix from the specified array.
        /// </summary>
        /// <param name="left">The left array.</param>
        /// <returns></returns>
        public static Matrix FromArray(double[] left)
        {
            int length = left.Length;
            Matrix resultMatrix = new Matrix(length, 1);
            for (int i = 0; i < length; i++)
            {
                resultMatrix[i, 0] = left[i];
            }
            return resultMatrix;
        }

        /// <summary>
        /// Creates a matrix from the specified array.
        /// </summary>
        /// <param name="left">The left array.</param>
        /// <returns></returns>
        public static implicit operator Matrix(double[] left)
        {
            return FromArray(left);
        }

        /// <summary>
        /// Casts a matrix to an array
        /// </summary>
        /// <param name="leftMatrix">The left matrix.</param>
        /// <returns></returns>
        public static double[] ToArray(Matrix leftMatrix)
        {
            double[] result = null;
            if (leftMatrix.ColumnCount > 1)
            {
                int numElements = leftMatrix.ColumnCount;
                result = new double[numElements];
                for (int i = 0; i < numElements; i++)
                {
                    result[i] = leftMatrix[0, i];
                }
            }
            else
            {
                int numElements = leftMatrix.RowCount;
                result = new double[numElements];
                for (int i = 0; i < numElements; i++)
                {
                    result[i] = leftMatrix[i, 0];
                }
            }
            return result;
        }

        /// <summary>
        /// Casts a matrix to an array
        /// </summary>
        /// <param name="leftMatrix">The left matrix.</param>
        /// <returns></returns>
        public static implicit operator double[](Matrix leftMatrix)
        {
            return ToArray(leftMatrix);
        }

        /// <summary>
        /// Creates a matrix from a 2D array
        /// </summary>
        /// <param name="left">The left.</param>
        /// <returns></returns>
        public static Matrix FromDoubleArray(double[,] left)
        {
            int length0 = left.GetLength(0);
            int length1 = left.GetLength(1);
            Matrix resultMatrix = new Matrix(length0, length1);
            for (int i = 0; i < length0; i++)
            {
                for (int j = 0; j < length1; j++)
                {
                    resultMatrix[i, j] = left[i, j];
                }
            }
            return resultMatrix;
        }

        /// <summary>
        /// Creates a matrix from a 2D array
        /// </summary>
        /// <param name="left">The left.</param>
        /// <returns></returns>
        public static implicit operator Matrix(double[,] left)
        {
            return FromDoubleArray(left);
        }

        /// <summary>
        /// Casts a matrix to a 2D array
        /// </summary>
        /// <param name="leftMatrix">The left matrix.</param>
        /// <returns></returns>
        public static double[,] ToDoubleArray(Matrix leftMatrix)
        {
            double[,] result = new double[leftMatrix.RowCount, leftMatrix.ColumnCount];
            for (int i = 0; i < leftMatrix.RowCount; i++)
            {
                for (int j = 0; j < leftMatrix.ColumnCount; j++)
                {
                    result[i, j] = leftMatrix[i, j];
                }
            }
            return result;
        }

        /// <summary>
        /// Casts a matrix to a 2D array
        /// </summary>
        /// <param name="leftMatrix">The left matrix.</param>
        /// <returns></returns>
        public static implicit operator double[,](Matrix leftMatrix)
        {
            return ToDoubleArray(leftMatrix);
        }

        #endregion

        /// <summary>
        /// Calculates the determinant of this matrix instance.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.Exception">Matrix must be square to find the determinant.</exception>
        public double Determinant()
        {
            if (RowCount != ColumnCount)
                throw new Exception("Matrix must be square to find the determinant.");

            if (RowCount == 1 && ColumnCount == 1)
                return this[0, 0];

            if (RowCount == 2 && ColumnCount == 2)
                return this[0,0] * this[1,1] - this[0,1] * this[1,0];

            double det = 0;
            int size = RowCount;

            // Make new arrays
            Matrix[] Matricies = new Matrix[size];
            for (int i = 0; i < size; i++)
                Matricies[i] = new Matrix(size - 1, size - 1);

            // insert values into the arrays
            for (int i = 0; i < size; i++)
            {
                for (int k = 1; k < size; k++)
                {
                    int temp = 0;
                    for (int w = 0; w < size - 1; w++)
                    {
                        if (temp == i)
                            temp++;
                        if (temp >= size)
                            temp = 0;

                        Matricies[i][k - 1, w] = this[k, temp];
                        temp++;
                    }
                }
            }

            // use the correct formula for adding/subtracting these values
            for (int i = 0; i < size; i++)
            {
                // 0, 2, etc are added
                if (i % 2 == 0)
                    det += this[0,i] * Matricies[i].Determinant();
                // 1, 3, etc are subtracted
                else
                    det -= this[0,i] * Matricies[i].Determinant();
            }

            return det;
        }

        /// <summary>
        /// Solves for the specified matrix.
        /// </summary>
        /// <param name="rightMatrix">The right matrix.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">
        /// The row count of the right matrix must be equal to the column count of this matrix to solve.
        /// or
        /// To solve for this matrix the row count must equal the column count.
        /// </exception>
        public Matrix SolveFor(Matrix rightMatrix)
        {

            if (rightMatrix.RowCount != _columnCount)
                throw new Exception("The row count of the right matrix must be equal to the column count of this matrix to solve.");

            if (_columnCount != _rowCount)
                throw new Exception("To solve for this matrix the row count must equal the column count.");

            Matrix resultMatrix = new Matrix(_columnCount, rightMatrix.ColumnCount);
            LUDecomposition resDecomp = LUDecompose();
            int[] nP = resDecomp.PivotArray;
            Matrix lMatrix = resDecomp.L;
            Matrix uMatrix = resDecomp.U;
            Parallel.For(0, rightMatrix.ColumnCount, k =>
            {
                //Solve for the corresponding d Matrix from Ld=Pb
                double sum = 0.0;
                Matrix dMatrix = new Matrix(_rowCount, 1);
                dMatrix[0, 0] = rightMatrix[nP[0], k] / lMatrix[0, 0];
                for (int i = 1; i < _rowCount; i++)
                {
                    sum = 0.0;
                    for (int j = 0; j < i; j++)
                    {
                        sum += lMatrix[i, j] * dMatrix[j, 0];
                    }
                    dMatrix[i, 0] = (rightMatrix[nP[i], k] - sum) / lMatrix[i, i];
                }
                //Solve for x using Ux = d
                resultMatrix[_rowCount - 1, k] = dMatrix[_rowCount - 1, 0];
                for (int i = _rowCount - 2; i >= 0; i--)
                {
                    sum = 0.0;
                    for (int j = i + 1; j < _rowCount; j++)
                    {
                        sum += uMatrix[i, j] * resultMatrix[j, k];
                    }
                    resultMatrix[i, k] = dMatrix[i, 0] - sum;
                }
            }
            );
            return resultMatrix;
        }

        /// <summary>
        /// Solves for the polynomial coefficients of x and y.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        public static double[] SolvePolyCoefs(double[] x, double[] y)
        {
            Matrix m = new Matrix(x.Length, x.Length);

            Parallel.For(0, x.Length, i =>
            {
                m[i, 0] = 1.0;
                for (int j = 1; j < x.Length; j++)
                {
                    m[i, j] = m[i, j - 1] * x[i];
                }
            }
            );

            return m.SolveFor(y);
        }

        /// <summary>
        /// Inverts this matrix instance.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.Exception">To invert this matrix, the row count and column count must be equal.</exception>
        public Matrix Invert()
        {
            if (_rowCount != _columnCount)
                throw new Exception("To invert this matrix, the row count and column count must be equal.");

            Matrix resultMatrix = SolveFor(Identity(_rowCount));
            Matrix matIdent = this * resultMatrix;

            return SolveFor(Identity(_rowCount));
        }

        #region Decompositions

        /// <summary>
        /// Calculates the QR Decomposition of this matrix.
        /// </summary>
        /// <returns></returns>
        public QRDecomposition QR()
        {
            return new QRDecomposition(this);
        }

        /// <summary>
        /// Calculates the LU Decomposition of this matrix.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.Exception">To decompose the matrix must have the same number of columns as rows.</exception>
        private LUDecomposition LUDecompose()
        {

            if (_columnCount != _rowCount)
                throw new Exception("To decompose the matrix must have the same number of columns as rows.");

            // Using Crout Decomp with P
            //
            // Ax = b //By definition of problem variables.
            //
            // LU = PA //By definition of L, U, and P.
            //
            // LUx = Pb //By substition for PA.
            //
            // Ux = d //By definition of d
            //
            // Ld = Pb //By subsitition for d.
            //
            //For 4x4 with P = I
            // [l11 0 0 0 ] [1 u12 u13 u14] [a11 a12 a13 a14]
            // [l21 l22 0 0 ] [0 1 u23 u24] = [a21 a22 a23 a24]
            // [l31 l32 l33 0 ] [0 0 1 u34] [a31 a32 a33 a34]
            // [l41 l42 l43 l44] [0 0 0 1 ] [a41 a42 a43 a44]
            LUDecomposition result = new LUDecomposition();
            try
            {
                int[] pivotArray = new int[_rowCount]; //Pivot matrix.
                Matrix uMatrix = new Matrix(_rowCount, _columnCount);
                Matrix lMatrix = new Matrix(_rowCount, _columnCount);
                Matrix workingUMatrix = Clone();
                Matrix workingLMatrix = new Matrix(_rowCount, _columnCount);
                Parallel.For(0, _rowCount, i =>
                {
                    pivotArray[i] = i;
                }
                );
                //Iterate down the number of rows in the U matrix.
                for (int i = 0; i < _rowCount; i++)
                {
                    //Do pivots first.
                    //I want to make the matrix diagonally dominate.
                    //Initialize the variables used to determine the pivot row.
                    double maxRowRatio = double.NegativeInfinity;
                    int maxRow = -1;
                    int maxPosition = -1;
                    //Check all of the rows below and including the current row
                    //to determine which row should be pivoted to the working row position.
                    //The pivot row will be set to the row with the maximum ratio
                    //of the absolute value of the first column element divided by the
                    //sum of the absolute values of the elements in that row.
                    Parallel.For(i, _rowCount, j =>
                    {
                        //Store the sum of the absolute values of the row elements in
                        //dRowSum. Clear it out now because I am checking a new row.
                        double rowSum = 0.0;
                        //Go across the columns, add the absolute values of the elements in
                        //that column to dRowSum.
                        for (int k = i; k < _columnCount; k++)
                        {
                            rowSum += System.Math.Abs(workingUMatrix[pivotArray[j], k]);
                        }
                        //Check to see if the absolute value of the ratio of the lead
                        //element over the sum of the absolute values of the elements is larger
                        //that the ratio for preceding rows. If it is, then the current row
                        //becomes the new pivot candidate.
                        if (rowSum == 0.0)
                        {
                            throw new SingularMatrixException();
                        }
                        double dCurrentRatio = System.Math.Abs(workingUMatrix[pivotArray[j], i]) / rowSum;
                        lock (this)
                        {
                            if (dCurrentRatio > maxRowRatio)
                            {
                                maxRowRatio = System.Math.Abs(workingUMatrix[pivotArray[j], i] / rowSum);
                                maxRow = pivotArray[j];
                                maxPosition = j;
                            }
                        }
                    }
                    );

                    //If the pivot candidate isn't the current row, update the
                    //pivot array to swap the current row with the pivot row.
                    if (maxRow != pivotArray[i])
                    {
                        int hold = pivotArray[i];
                        pivotArray[i] = maxRow;
                        pivotArray[maxPosition] = hold;
                    }
                    //Store the value of the left most element in the working U
                    //matrix in dRowFirstElementValue.
                    double rowFirstElementValue = workingUMatrix[pivotArray[i], i];
                    //Update the columns of the working row. j is the column index.
                    Parallel.For(0, _columnCount, j =>
                    {
                        if (j < i)
                        {
                            //If j<1, then the U matrix element value is 0.
                            workingUMatrix[pivotArray[i], j] = 0.0;
                        }
                        else if (j == i)
                        {
                            //If i == j, the L matrix value is the value of the
                            //element in the working U matrix.
                            workingLMatrix[pivotArray[i], j] = rowFirstElementValue;
                            //The value of the U matrix for i == j is 1
                            workingUMatrix[pivotArray[i], j] = 1.0;
                        }
                        else // j>i
                        {
                            //Divide each element in the current row of the U matrix by the
                            //value of the first element in the row
                            workingUMatrix[pivotArray[i], j] /= rowFirstElementValue;
                            //The element value of the L matrix for j>i is 0
                            workingLMatrix[pivotArray[i], j] = 0.0;
                        }
                    }
                    );
                    //For the working U matrix, subtract the ratioed active row from the rows below it.
                    //Update the columns of the rows below the working row. k is the row index.
                    for (int k = i + 1; k < _rowCount; k++)
                    {
                        //Store the value of the first element in the working row
                        //of the U matrix.
                        rowFirstElementValue = workingUMatrix[pivotArray[k], i];
                        //Go accross the columns of row k.
                        Parallel.For(0, _columnCount, j =>
                        {
                            if (j < i)
                            {
                                //If j<1, then the U matrix element value is 0.
                                workingUMatrix[pivotArray[k], j] = 0.0;
                            }
                            else if (j == i)
                            {
                                //If i == j, the L matrix value is the value of the
                                //element in the working U matrix.
                                workingLMatrix[pivotArray[k], j] = rowFirstElementValue;
                                //The element value of the L matrix for j>i is 0
                                workingUMatrix[pivotArray[k], j] = 0.0;
                            }
                            else //j>i
                            {
                                workingUMatrix[pivotArray[k], j] = workingUMatrix[pivotArray[k], j] - rowFirstElementValue * workingUMatrix[pivotArray[i], j];
                            }
                        }
                        );
                    }
                }
                Parallel.For(0, _rowCount, i =>
                {
                    for (int j = 0; j < _rowCount; j++)
                    {
                        uMatrix[i, j] = workingUMatrix[pivotArray[i], j];
                        lMatrix[i, j] = workingLMatrix[pivotArray[i], j];
                    }
                }
                );
                result.U = uMatrix;
                result.L = lMatrix;
                result.PivotArray = pivotArray;
            }
            catch (AggregateException ex2)
            {
                if (ex2.InnerExceptions.Count > 0)
                {
                    throw ex2.InnerExceptions[0];
                }
                else
                {
                    throw;
                }
            }
            return result;
        }

        /// <summary>
        /// Calculates the singular value decomposition of this matrix.
        /// </summary>
        /// <returns></returns>
		public SVDecomposition SingularValueDecomposition()
		{
			// Derived from LINPACK code.
            Matrix A = this.Clone();
			int m = A.RowCount;
			int n = A.ColumnCount;
			int nu = System.Math.Min(m, n);
			double[] s = new double[System.Math.Min(m + 1, n)];

            Matrix U = new Matrix(m, nu);
            Matrix V = new Matrix(n, n);

			double[] e = new double[n];
			double[] work = new double[m];
			bool wantu = true;
			bool wantv = true;
			
			// Reduce A to bidiagonal form, storing the diagonal elements
			// in s and the super-diagonal elements in e.
			
			int nct = System.Math.Min(m - 1, n);
			int nrt = System.Math.Max(0, System.Math.Min(n - 2, m));
			for (int k = 0; k < System.Math.Max(nct, nrt); k++)
			{
				if (k < nct)
				{
					// Compute the transformation for the k-th column and
					// place the k-th diagonal in s[k].
					// Compute 2-norm of k-th column without under/overflow.
					s[k] = 0;
					for (int i = k; i < m; i++)
						s[k] = MathExtensions.Hypotenuse(s[k], A[i,k]);

					if (s[k] != 0.0)
					{
						if (A[k,k] < 0.0)
							s[k] = -s[k];

						for (int i = k; i < m; i++)
							A[i,k] /= s[k];

						A[k,k] += 1.0;
					}

					s[k] = -s[k];
				}

				for (int j = k + 1; j < n; j++)
				{
					if ((k < nct) & (s[k] != 0.0))
					{
						
						// Apply the transformation.
						
						double t = 0;
						for (int i = k; i < m; i++)
							t += A[i,k] * A[i,j];

						t = -t / A[k,k];
						for (int i = k; i < m; i++)
							A[i,j] += t * A[i,k];
					}
					
					// Place the k-th row of A into e for the
					// subsequent calculation of the row transformation.
					
					e[j] = A[k,j];
				}

				if (wantu & (k < nct))
				{
					
					// Place the transformation in U for subsequent back
					// multiplication.
					
					for (int i = k; i < m; i++)
						U[i, k] = A[i,k];
				}
				if (k < nrt)
				{
					// Compute the k-th row transformation and place the
					// k-th super-diagonal in e[k].
					// Compute 2-norm without under/overflow.
					e[k] = 0;
					for (int i = k + 1; i < n; i++)
					{
						e[k] = MathExtensions.Hypotenuse(e[k], e[i]);
					}
					if (e[k] != 0.0)
					{
						if (e[k + 1] < 0.0)
						{
							e[k] = - e[k];
						}
						for (int i = k + 1; i < n; i++)
						{
							e[i] /= e[k];
						}
						e[k + 1] += 1.0;
					}
					e[k] = - e[k];
					if ((k + 1 < m) & (e[k] != 0.0))
					{
						
						// Apply the transformation.
						for (int i = k + 1; i < m; i++)
							work[i] = 0.0;

						for (int j = k + 1; j < n; j++)
							for (int i = k + 1; i < m; i++)
								work[i] += e[j] * A[i,j];

						for (int j = k + 1; j < n; j++)
						{
							double t = (- e[j]) / e[k + 1];
							for (int i = k + 1; i < m; i++)
								A[i,j] += t * work[i];
						}
					}
					if (wantv)
					{
						
						// Place the transformation in V for subsequent
						// back multiplication.
						
						for (int i = k + 1; i < n; i++)
							V[i,k] = e[i];
					}
				}
			}
			
			// Set up the final bidiagonal matrix or order p.
			
			int p = System.Math.Min(n, m + 1);
			if (nct < n)
				s[nct] = A[nct,nct];

			if (m < p)
				s[p - 1] = 0.0;

			if (nrt + 1 < p)
				e[nrt] = A[nrt, p - 1];

			e[p - 1] = 0.0;
			
			// If required, generate U.
			if (wantu)
			{
				for (int j = nct; j < nu; j++)
				{
					for (int i = 0; i < m; i++)
						U[i,j] = 0.0;

					U[j,j] = 1.0;
				}
				for (int k = nct - 1; k >= 0; k--)
				{
					if (s[k] != 0.0)
					{
						for (int j = k + 1; j < nu; j++)
						{
							double t = 0;
							for (int i = k; i < m; i++)
								t += U[i,k] * U[i,j];

							t = (- t) / U[k,k];

							for (int i = k; i < m; i++)
								U[i,j] += t * U[i,k];

						}
						for (int i = k; i < m; i++)
							U[i,k] = - U[i,k];

						U[k,k] = 1.0 + U[k,k];
						for (int i = 0; i < k - 1; i++)
							U[i,k] = 0.0;
					}
					else
					{
						for (int i = 0; i < m; i++)
							U[i,k] = 0.0;

						U[k,k] = 1.0;
					}
				}
			}
			
			// If required, generate V.
			
			if (wantv)
			{
				for (int k = n - 1; k >= 0; k--)
				{
					if ((k < nrt) & (e[k] != 0.0))
					{
						for (int j = k + 1; j < nu; j++)
						{
							double t = 0;
							for (int i = k + 1; i < n; i++)
								t += V[i,k] * V[i,j];

							t = (- t) / V[k + 1, k];

							for (int i = k + 1; i < n; i++)
								V[i,j] += t * V[i,k];
						}
					}

					for (int i = 0; i < n; i++)
						V[i,k] = 0.0;

					V[k,k] = 1.0;
				}
			}
			
			// Main iteration loop for the singular values.
			
			int pp = p - 1;
			int iter = 0;
			double eps = System.Math.Pow(2.0, - 52.0);
			while (p > 0)
			{
				int k, kase;
				
				// Here is where a test for too many iterations would go.
				
				// This section of the program inspects for
				// negligible elements in the s and e arrays.  On
				// completion the variables kase and k are set as follows.
				
				// kase = 1     if s(p) and e[k-1] are negligible and k<p
				// kase = 2     if s(k) is negligible and k<p
				// kase = 3     if e[k-1] is negligible, k<p, and
				//              s(k), ..., s(p) are not negligible (qr step).
				// kase = 4     if e(p-1) is negligible (convergence).
				
				for (k = p - 2; k >= - 1; k--)
				{
					if (k == - 1)
						break;

					if (System.Math.Abs(e[k]) <= eps * (System.Math.Abs(s[k]) + System.Math.Abs(s[k + 1])))
					{
						e[k] = 0.0;
						break;
					}
				}

				if (k == p - 2)
				{
					kase = 4;
				}
				else
				{
					int ks;
					for (ks = p - 1; ks >= k; ks--)
					{
						if (ks == k)
							break;

						double t = (ks != p?System.Math.Abs(e[ks]):0.0) + (ks != k + 1?System.Math.Abs(e[ks - 1]):0.0);
						if (System.Math.Abs(s[ks]) <= eps * t)
						{
							s[ks] = 0.0;
							break;
						}
					}
					if (ks == k)
					{
						kase = 3;
					}
					else if (ks == p - 1)
					{
						kase = 1;
					}
					else
					{
						kase = 2;
						k = ks;
					}
				}
				k++;
				
				// Perform the task indicated by kase.
				
				switch (kase)
				{
					// Deflate negligible s(p).
					case 1:  
					{
						double f = e[p - 2];
						e[p - 2] = 0.0;
						for (int j = p - 2; j >= k; j--)
						{
							double t = MathExtensions.Hypotenuse(s[j], f);
							double cs = s[j] / t;
							double sn = f / t;
							s[j] = t;
							if (j != k)
							{
								f = (- sn) * e[j - 1];
								e[j - 1] = cs * e[j - 1];
							}
							if (wantv)
							{
								for (int i = 0; i < n; i++)
								{
									t = cs * V[i,j] + sn * V[i, p - 1];
									V[i, p - 1] = (- sn) * V[i,j] + cs * V[i, p - 1];
									V[i,j] = t;
								}
							}
						}
					}
					break;
						
					// Split at negligible s(k).
					case 2:  
					{
						double f = e[k - 1];
						e[k - 1] = 0.0;
						for (int j = k; j < p; j++)
						{
							double t = MathExtensions.Hypotenuse(s[j], f);
							double cs = s[j] / t;
							double sn = f / t;
							s[j] = t;
							f = (- sn) * e[j];
							e[j] = cs * e[j];
							if (wantu)
							{
								for (int i = 0; i < m; i++)
								{
									t = cs * U[i,j] + sn * U[i, k - 1];
									U[i, k - 1] = (- sn) * U[i,j] + cs * U[i, k - 1];
									U[i,j] = t;
								}
							}
						}
					}
					break;
						
					// Perform one qr step.
					case 3:  
					{
						// Calculate the shift.
						
						double scale = System.Math.Max(System.Math.Max(System.Math.Max(System.Math.Max(System.Math.Abs(s[p - 1]), System.Math.Abs(s[p - 2])), System.Math.Abs(e[p - 2])), System.Math.Abs(s[k])), System.Math.Abs(e[k]));
						double sp = s[p - 1] / scale;
						double spm1 = s[p - 2] / scale;
						double epm1 = e[p - 2] / scale;
						double sk = s[k] / scale;
						double ek = e[k] / scale;
						double b = ((spm1 + sp) * (spm1 - sp) + epm1 * epm1) / 2.0;
						double c = (sp * epm1) * (sp * epm1);
						double shift = 0.0;
						if ((b != 0.0) | (c != 0.0))
						{
							shift = System.Math.Sqrt(b * b + c);
							if (b < 0.0)
							{
								shift = - shift;
							}
							shift = c / (b + shift);
						}
						double f = (sk + sp) * (sk - sp) + shift;
						double g = sk * ek;
						
						// Chase zeros.
						
						for (int j = k; j < p - 1; j++)
						{
							double t = MathExtensions.Hypotenuse(f, g);
							double cs = f / t;
							double sn = g / t;
							if (j != k)
							{
								e[j - 1] = t;
							}
							f = cs * s[j] + sn * e[j];
							e[j] = cs * e[j] - sn * s[j];
							g = sn * s[j + 1];
							s[j + 1] = cs * s[j + 1];
							if (wantv)
							{
								for (int i = 0; i < n; i++)
								{
									t = cs * V[i,j] + sn * V[i,j + 1];
									V[i,j + 1] = (- sn) * V[i,j] + cs * V[i,j + 1];
									V[i,j] = t;
								}
							}
							t = MathExtensions.Hypotenuse(f, g);
							cs = f / t;
							sn = g / t;
							s[j] = t;
							f = cs * e[j] + sn * s[j + 1];
							s[j + 1] = (- sn) * e[j] + cs * s[j + 1];
							g = sn * e[j + 1];
							e[j + 1] = cs * e[j + 1];
							if (wantu && (j < m - 1))
							{
								for (int i = 0; i < m; i++)
								{
									t = cs * U[i,j] + sn * U[i,j + 1];
									U[i,j + 1] = (- sn) * U[i,j] + cs * U[i,j + 1];
									U[i,j] = t;
								}
							}
						}
						e[p - 2] = f;
						iter = iter + 1;
					}
					break;
					
					// Convergence.
					case 4:  
					{
						// Make the singular values positive.
						
						if (s[k] <= 0.0)
						{
							s[k] = (s[k] < 0.0?- s[k]:0.0);
							if (wantv)
							{
								for (int i = 0; i <= pp; i++)
									V[i,k] = - V[i,k];
							}
						}
						
						// Order the singular values.
						
						while (k < pp)
						{
							if (s[k] >= s[k + 1])
							{
								break;
							}
							double t = s[k];
							s[k] = s[k + 1];
							s[k + 1] = t;
							if (wantv && (k < n - 1))
							{
                                for (int i = 0; i < n; i++)
                                {
                                    t = V[i, k + 1]; 
                                    V[i, k + 1] = V[i, k]; 
                                    V[i, k] = t;
                                }
							}
							if (wantu && (k < m - 1))
							{
                                for (int i = 0; i < m; i++)
                                {
                                    t = U[i,k + 1]; 
                                    U[i,k + 1] = U[i,k]; 
                                    U[i,k] = t;
                                }
							}
							k++;
						}
						iter = 0;
						p--;
					}
					break;
				}
			}

            Matrix S = new Matrix(n, n);
            for (int i = 0; i < n; i++)
                S[i,i] = s[i];

            Matrix sizedU = new Matrix(m, System.Math.Min(m + 1, n));
            for (int i = 0; i < sizedU.RowCount; i++)
                for (int j = 0; j < sizedU.ColumnCount; j++)
                    sizedU[i,j] = U[i,j];

            return new SVDecomposition(s, S, sizedU, V);
		}

        #endregion
    }
    
    #region Class : LUDecompositionResults

    /// <summary>
    /// Results for LU Decomposition
    /// </summary>
    public class LUDecomposition
    {
        private Matrix _lMatrix;
        private Matrix _uMatrix;
        private int[] _pivotArray;

        /// <summary>
        /// Initializes a new instance of the <see cref="LUDecomposition"/> class.
        /// </summary>
        public LUDecomposition()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LUDecomposition"/> class.
        /// </summary>
        /// <param name="matL">The mat L.</param>
        /// <param name="matU">The mat U.</param>
        /// <param name="nPivotArray">The n pivot array.</param>
        public LUDecomposition(Matrix matL, Matrix matU, int[] nPivotArray)
        {
            _lMatrix = matL;
            _uMatrix = matU;
            _pivotArray = nPivotArray;
        }

        /// <summary>
        /// Gets or sets the L Matrix.
        /// </summary>
        /// <value>
        /// The L Matrix.
        /// </value>
        public Matrix L
        {
            get { return _lMatrix; }
            set { _lMatrix = value; }
        }

        /// <summary>
        /// Gets or sets the U Matrix.
        /// </summary>
        /// <value>
        /// The U Matrix.
        /// </value>
        public Matrix U
        {
            get { return _uMatrix; }
            set { _uMatrix = value; }
        }

        /// <summary>
        /// Gets or sets the pivot array.
        /// </summary>
        /// <value>
        /// The pivot array.
        /// </value>
        public int[] PivotArray
        {
            get { return _pivotArray; }
            set { _pivotArray = value; }
        }

    }

    #endregion

    #region Class : SVDecompositionResults

    /// <summary>
    /// Results for Singular Value Decomposition.
    /// </summary>
    public class SVDecomposition
    {
        /// <summary>
        /// Gets or sets the singular values.
        /// </summary>
        /// <value>
        /// The singular values.
        /// </value>
        public double[] SingularValues
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the S Matrix.
        /// </summary>
        /// <value>
        /// The S Matrix.
        /// </value>
        public Matrix S
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the U Matrix.
        /// </summary>
        /// <value>
        /// The U Matrix.
        /// </value>
        public Matrix U
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the V Matrix.
        /// </summary>
        /// <value>
        /// The V Matrix.
        /// </value>
        public Matrix V
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SVDecomposition"/> class.
        /// </summary>
        /// <param name="singularValues">The singular values.</param>
        /// <param name="s">The s matrix.</param>
        /// <param name="u">The u matrix.</param>
        /// <param name="v">The v matrix.</param>
        public SVDecomposition(double[] singularValues, Matrix s, Matrix u, Matrix v)
        {
            SingularValues = singularValues;
            S = s;
            U = u;
            V = v;

        }
    }

    #endregion

    # region Class : QRDecomposition

    /// <summary>
    /// Represents a QR decomposition result.
    /// </summary>
    public class QRDecomposition
    {
        private int _rowCount;
        private int _colCount;
        private Matrix _qrMatrix;
        private Matrix _qMatrix;
        private Matrix _rMatrix;
        private Matrix _hMatrix;
        private double[] _rDiag;

        /// <summary>Is the matrix full rank?</summary>
        /// <returns>     true if R, and hence A, has full rank.
        /// </returns>
        virtual public bool FullRank
        {
            get
            {
                for (int j = 0; j < _colCount; j++)
                {
                    if (_rDiag[j] == 0)
                        return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Gets or sets the Q Matrix.
        /// </summary>
        /// <value>
        /// The Q Matrix.
        /// </value>
        public Matrix QR
        {
            get
            {
                return _qrMatrix;
            }
            set
            {
                _qrMatrix = value;
            }
        }

        /// <summary>
        /// Gets or sets the Q Matrix.
        /// </summary>
        /// <value>
        /// The Q Matrix.
        /// </value>
        public Matrix Q
        {
            get 
            { 
                return _qMatrix; 
            }
            set 
            { 
                _qMatrix = value; 
            }
        }

        /// <summary>
        /// Gets or sets the R Matrix.
        /// </summary>
        /// <value>
        /// The R Matrix.
        /// </value>
        public Matrix R
        {
            get 
            { 
                return _rMatrix; 
            }
            set 
            { 
                _rMatrix = value; 
            }
        }

        /// <summary>
        /// Gets or sets the H Matrix.
        /// </summary>
        /// <value>
        /// The H Matrix.
        /// </value>
        public Matrix H
        {
            get
            {
                return _hMatrix;
            }
            set
            {
                _hMatrix = value;
            }
        }

        /// <summary>
        /// Gets or sets the R diagonal.
        /// </summary>
        /// <value>
        /// The R diagonal.
        /// </value>
        public double[] RDiagonal
        {
            get
            {
                return _rDiag;
            }
            set
            {
                _rDiag = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QRDecomposition"/> class.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        public QRDecomposition(Matrix matrix)
        {
            QR = matrix.Clone();

            _rowCount = matrix.RowCount;
            _colCount = matrix.ColumnCount;
            _rDiag = new double[_colCount];

            // Calculate qr

            for (int k = 0; k < _colCount; k++)
            {
                // Compute 2-norm of k-th column
                double nrm = 0;
                for (int i = k; i < _rowCount; i++)
                {
                    nrm = MathExtensions.Hypotenuse(nrm, QR[i, k]);
                }

                if (nrm != 0.0)
                {
                    // Form k-th Householder vector.
                    if (QR[k, k] < 0)
                    {
                        nrm = -nrm;
                    }

                    for (int i = k; i < _rowCount; i++)
                    {
                        QR[i, k] /= nrm;
                    }

                    QR[k, k] += 1.0;

                    // Apply transformation to remaining columns.
                    for (int j = k + 1; j < _colCount; j++)
                    {
                        double s = 0.0;

                        for (int i = k; i < _rowCount; i++)
                            s += QR[i, k] * QR[i, j];

                        s = (-s) / QR[k, k];

                        for (int i = k; i < _rowCount; i++)
                            QR[i, j] += s * QR[i, k];
                    }
                }

                _rDiag[k] = -nrm;
            }

            // Generate Q matrix (orthogonal factor)
 
            Q = new Matrix(_rowCount, _colCount);

            for (int k = _colCount - 1; k >= 0; k--)
            {
                for (int i = 0; i < _rowCount; i++)
                    Q[i, k] = 0.0;

                Q[k, k] = 1.0;

                for (int j = k; j < _colCount; j++)
                {
                    if (QR[k, k] != 0)
                    {
                        double s = 0.0;

                        for (int i = k; i < _rowCount; i++)
                            s += QR[i, k] * Q[i, j];

                        s = (-s) / QR[k, k];

                        for (int i = k; i < _rowCount; i++)
                            Q[i, j] += s * QR[i, k];
                    }
                }
            }

            // Generate R matrix (upper triangle factor)

            R = new Matrix(_colCount, _colCount);

            for (int i = 0; i < _colCount; i++)
            {
                for (int j = 0; j < _colCount; j++)
                {
                    if (i < j)
                        R[i, j] = QR[i, j];
                    else if (i == j)
                        R[i, j] = _rDiag[i];
                    else
                        R[i, j] = 0.0;
                }
            }

            H = new Matrix(_rowCount, _colCount);

            for (int i = 0; i < _rowCount; i++)
            {
                for (int j = 0; j < _colCount; j++)
                {
                    if (i >= j)
                        H[i, j] = QR[i, j];
                    else
                        H[i, j] = 0.0;
                }
            }
        }

        /// <summary>
        /// Performs a QR decomposition.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Matrix row dimensions must agree for QR decomposition.</exception>
        /// <exception cref="System.SystemException">QR decomposition cannot be solved because matrix is rank deficient.</exception>
        public Matrix Solve(Matrix matrix)
        {
            if (matrix.RowCount != _rowCount)
                throw new System.ArgumentException("Matrix row dimensions must agree for QR decomposition.");

            if (!this.FullRank)
                throw new System.SystemException("QR decomposition cannot be solved because matrix is rank deficient.");

            // Copy right hand side
            int nx = matrix.ColumnCount;
            Matrix m = matrix.Clone();

            // Compute Y = transpose(Q)*B
            for (int k = 0; k < _colCount; k++)
            {
                for (int j = 0; j < nx; j++)
                {
                    double s = 0.0;
                    for (int i = k; i < _rowCount; i++)
                        s += QR[i, k] * m[i, j];

                    s = (-s) / QR[k, k];

                    for (int i = k; i < _rowCount; i++)
                        m[i, j] += s * QR[i, k];
                }
            }

            // Solve R*X = Y;
            for (int k = _colCount - 1; k >= 0; k--)
            {
                for (int j = 0; j < nx; j++)
                    m[k, j] /= _rDiag[k];

                for (int i = 0; i < k; i++)
                    for (int j = 0; j < nx; j++)
                        m[i, j] -= m[k, j] * QR[i, k];
            }

            return m;
        }
    }

    #endregion

    /// <summary>
    /// Exception for invalid operations on singular matrix.
    /// </summary>
    public class SingularMatrixException : ArithmeticException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SingularMatrixException"/> class.
        /// </summary>
        public SingularMatrixException()
            : base("Invalid operation on a singular matrix.")
        {
        }
    }
}
