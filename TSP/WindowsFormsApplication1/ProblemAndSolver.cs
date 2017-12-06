using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using Priority_Queue;
using WindowsFormsApplication1;
using System.Windows.Forms;

namespace TSP
{

    class ProblemAndSolver
    {

        private class TSPSolution
        {
            /// <summary>
            /// we use the representation [cityB,cityA,cityC] 
            /// to mean that cityB is the first city in the solution, cityA is the second, cityC is the third 
            /// and the edge from cityC to cityB is the final edge in the path.  
            /// You are, of course, free to use a different representation if it would be more convenient or efficient 
            /// for your data structure(s) and search algorithm. 
            /// </summary>
            public ArrayList
                Route;

            /// <summary>
            /// constructor
            /// </summary>
            /// <param name="iroute">a (hopefully) valid tour</param>
            public TSPSolution(ArrayList iroute)
            {
                Route = new ArrayList(iroute);
            }

            /// <summary>
            /// Compute the cost of the current route.  
            /// Note: This does not check that the route is complete.
            /// It assumes that the route passes from the last city back to the first city. 
            /// </summary>
            /// <returns></returns>
            public double costOfRoute()
            {
                // go through each edge in the route and add up the cost. 
                int x;
                City here;
                double cost = 0D;

                for (x = 0; x < Route.Count - 1; x++)
                {
                    here = Route[x] as City;
                    cost += here.costToGetTo(Route[x + 1] as City);
                }

                // go from the last city to the first. 
                here = Route[Route.Count - 1] as City;
                cost += here.costToGetTo(Route[0] as City);
                return cost;
            }
        }

        #region Private members 

        /// <summary>
        /// Default number of cities (unused -- to set defaults, change the values in the GUI form)
        /// </summary>
        // (This is no longer used -- to set default values, edit the form directly.  Open Form1.cs,
        // click on the Problem Size text box, go to the Properties window (lower right corner), 
        // and change the "Text" value.)
        private const int DEFAULT_SIZE = 25;

        /// <summary>
        /// Default time limit (unused -- to set defaults, change the values in the GUI form)
        /// </summary>
        // (This is no longer used -- to set default values, edit the form directly.  Open Form1.cs,
        // click on the Time text box, go to the Properties window (lower right corner), 
        // and change the "Text" value.)
        private const int TIME_LIMIT = 60;        //in seconds

        private const int CITY_ICON_SIZE = 5;


        // For normal and hard modes:
        // hard mode only
        private const double FRACTION_OF_PATHS_TO_REMOVE = 0.20;

        /// <summary>
        /// the cities in the current problem.
        /// </summary>
        private City[] Cities;
        /// <summary>
        /// a route through the current problem, useful as a temporary variable. 
        /// </summary>
        private ArrayList Route;
        /// <summary>
        /// best solution so far. 
        /// </summary>
        private TSPSolution bssf; 

        /// <summary>
        /// how to color various things. 
        /// </summary>
        private Brush cityBrushStartStyle;
        private Brush cityBrushStyle;
        private Pen routePenStyle;


        /// <summary>
        /// keep track of the seed value so that the same sequence of problems can be 
        /// regenerated next time the generator is run. 
        /// </summary>
        private int _seed;
        /// <summary>
        /// number of cities to include in a problem. 
        /// </summary>
        private int _size;

        /// <summary>
        /// Difficulty level
        /// </summary>
        private HardMode.Modes _mode;

        /// <summary>
        /// random number generator. 
        /// </summary>
        private Random rnd;

        /// <summary>
        /// time limit in milliseconds for state space search
        /// can be used by any solver method to truncate the search and return the BSSF
        /// </summary>
        private int time_limit;
        #endregion

        #region Public members

        /// <summary>
        /// These three constants are used for convenience/clarity in populating and accessing the results array that is passed back to the calling Form
        /// </summary>
        public const int COST = 0;           
        public const int TIME = 1;
        public const int COUNT = 2;
        
        public int Size
        {
            get { return _size; }
        }

        public int Seed
        {
            get { return _seed; }
        }
        #endregion

        #region Constructors
        public ProblemAndSolver()
        {
            this._seed = 1; 
            rnd = new Random(1);
            this._size = DEFAULT_SIZE;
            this.time_limit = TIME_LIMIT * 1000;                  // TIME_LIMIT is in seconds, but timer wants it in milliseconds

            this.resetData();
        }

        public ProblemAndSolver(int seed)
        {
            this._seed = seed;
            rnd = new Random(seed);
            this._size = DEFAULT_SIZE;
            this.time_limit = TIME_LIMIT * 1000;                  // TIME_LIMIT is in seconds, but timer wants it in milliseconds

            this.resetData();
        }

        public ProblemAndSolver(int seed, int size)
        {
            this._seed = seed;
            this._size = size;
            rnd = new Random(seed);
            this.time_limit = TIME_LIMIT * 1000;                        // TIME_LIMIT is in seconds, but timer wants it in milliseconds

            this.resetData();
        }
        public ProblemAndSolver(int seed, int size, int time)
        {
            this._seed = seed;
            this._size = size;
            rnd = new Random(seed);
            this.time_limit = time*1000;                        // time is entered in the GUI in seconds, but timer wants it in milliseconds

            this.resetData();
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Reset the problem instance.
        /// </summary>
        private void resetData()
        {

            Cities = new City[_size];
            Route = new ArrayList(_size);
            bssf = null;

            if (_mode == HardMode.Modes.Easy)
            {
                for (int i = 0; i < _size; i++)
                    Cities[i] = new City(rnd.NextDouble(), rnd.NextDouble());
            }
            else // Medium and hard
            {
                for (int i = 0; i < _size; i++)
                    Cities[i] = new City(rnd.NextDouble(), rnd.NextDouble(), rnd.NextDouble() * City.MAX_ELEVATION);
            }

            HardMode mm = new HardMode(this._mode, this.rnd, Cities);
            if (_mode == HardMode.Modes.Hard)
            {
                int edgesToRemove = (int)(_size * FRACTION_OF_PATHS_TO_REMOVE);
                mm.removePaths(edgesToRemove);
            }
            City.setModeManager(mm);

            cityBrushStyle = new SolidBrush(Color.Black);
            cityBrushStartStyle = new SolidBrush(Color.Red);
            routePenStyle = new Pen(Color.Blue,1);
            routePenStyle.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// make a new problem with the given size.
        /// </summary>
        /// <param name="size">number of cities</param>
        public void GenerateProblem(int size, HardMode.Modes mode)
        {
            this._size = size;
            this._mode = mode;
            resetData();
        }

        /// <summary>
        /// make a new problem with the given size, now including timelimit paremeter that was added to form.
        /// </summary>
        /// <param name="size">number of cities</param>
        public void GenerateProblem(int size, HardMode.Modes mode, int timelimit)
        {
            this._size = size;
            this._mode = mode;
            this.time_limit = timelimit*1000;                                   //convert seconds to milliseconds
            resetData();
        }

        /// <summary>
        /// return a copy of the cities in this problem. 
        /// </summary>
        /// <returns>array of cities</returns>
        public City[] GetCities()
        {
            City[] retCities = new City[Cities.Length];
            Array.Copy(Cities, retCities, Cities.Length);
            return retCities;
        }

        /// <summary>
        /// draw the cities in the problem.  if the bssf member is defined, then
        /// draw that too. 
        /// </summary>
        /// <param name="g">where to draw the stuff</param>
        public void Draw(Graphics g)
        {
            float width  = g.VisibleClipBounds.Width-45F;
            float height = g.VisibleClipBounds.Height-45F;
            Font labelFont = new Font("Arial", 10);

            // Draw lines
            if (bssf != null)
            {
                // make a list of points. 
                Point[] ps = new Point[bssf.Route.Count];
                int index = 0;
                foreach (City c in bssf.Route)
                {
                    if (index < bssf.Route.Count -1)
                        g.DrawString(" " + index +"("+c.costToGetTo(bssf.Route[index+1]as City)+")", labelFont, cityBrushStartStyle, new PointF((float)c.X * width + 3F, (float)c.Y * height));
                    else 
                        g.DrawString(" " + index +"("+c.costToGetTo(bssf.Route[0]as City)+")", labelFont, cityBrushStartStyle, new PointF((float)c.X * width + 3F, (float)c.Y * height));
                    ps[index++] = new Point((int)(c.X * width) + CITY_ICON_SIZE / 2, (int)(c.Y * height) + CITY_ICON_SIZE / 2);
                }

                if (ps.Length > 0)
                {
                    g.DrawLines(routePenStyle, ps);
                    g.FillEllipse(cityBrushStartStyle, (float)Cities[0].X * width - 1, (float)Cities[0].Y * height - 1, CITY_ICON_SIZE + 2, CITY_ICON_SIZE + 2);
                }

                // draw the last line. 
                g.DrawLine(routePenStyle, ps[0], ps[ps.Length - 1]);
            }

            // Draw city dots
            foreach (City c in Cities)
            {
                g.FillEllipse(cityBrushStyle, (float)c.X * width, (float)c.Y * height, CITY_ICON_SIZE, CITY_ICON_SIZE);
            }

        }

        /// <summary>
        ///  return the cost of the best solution so far. 
        /// </summary>
        /// <returns></returns>
        public double costOfBssf ()
        {
            if (bssf != null)
                return (bssf.costOfRoute());
            else
                return -1D; 
        }

        /// <summary>
        /// This is the entry point for the default solver
        /// which just finds a valid random tour 
        /// </summary>
        /// <returns>results array for GUI that contains three ints: cost of solution, time spent to find solution, number of solutions found during search (not counting initial BSSF estimate)</returns>
        public string[] defaultSolveProblem()
        {
            int i, swap, temp, count=0;
            string[] results = new string[3];
            int[] perm = new int[Cities.Length];
            Route = new ArrayList();
            Random rnd = new Random();
            Stopwatch timer = new Stopwatch();

            timer.Start();

            do
            {
                for (i = 0; i < perm.Length; i++)                                 // create a random permutation template
                    perm[i] = i;
                for (i = 0; i < perm.Length; i++)
                {
                    swap = i;
                    while (swap == i)
                        swap = rnd.Next(0, Cities.Length);
                    temp = perm[i];
                    perm[i] = perm[swap];
                    perm[swap] = temp;
                }
                Route.Clear();
                for (i = 0; i < Cities.Length; i++)                            // Now build the route using the random permutation 
                {
                    Route.Add(Cities[perm[i]]);
                }
                bssf = new TSPSolution(Route);
                count++;
            } while (costOfBssf() == double.PositiveInfinity);                // until a valid route is found
            timer.Stop();

            results[COST] = costOfBssf().ToString();                          // load results array
            results[TIME] = timer.Elapsed.ToString();
            results[COUNT] = count.ToString();

            return results;
        }


        SimplePriorityQueue<BBNode> priorityQueue= new SimplePriorityQueue<BBNode>();

        /// <summary>
        /// performs a Branch and Bound search of the state space of partial tours
        /// stops when time limit expires and uses BSSF as solution
        /// </summary>
        /// <returns>results array for GUI that contains three ints: cost of solution, time spent to find solution, number of solutions found during search (not counting initial BSSF estimate)</returns>
        public string[] bBSolveProblem()
        {
            string[] results = new string[3];
            Stopwatch timer = new Stopwatch();
            int intermediateSolutions = 0;
            int numStatesCreated = 0;
            int maxInQueue = 0;
            int numStatesPruned = 0;
            defaultSolveProblem();

            timer.Start();

            // initialize the root node
            List<int> children = new List<int>();
            int[,] matrix = initializeCostMatrix(Cities, ref children, 0);
            //printMatrix(matrix, "Absolute Cost:");
            BBNode rootNode = new BBNode(children, matrix, 0);      // 0 is it's row index in the cost matrix
            rootNode.reduceMatrix();
            //printMatrix(rootNode.costMatrix, "Root Reduced Matrix:");
            //Console.WriteLine("\nRoot.LocalBound = " + rootNode.localBound);
            rootNode.Route.Add(Cities[0]);          // the route starts at itself

            // initialize the priority queue (add the root)
            priorityQueue.Enqueue(rootNode, 0);
            double costOfBSSF = bssf.costOfRoute();
            //Console.WriteLine("BSSF cost:" + costOfBSSF);

            bool timesUp = false;

            while (!timesUp && priorityQueue.Count > 0)
            {
                BBNode node = priorityQueue.Dequeue();
                if (node.localBound < costOfBSSF)
                {
                    foreach (int child in node.children)
                    {
                        // Check if time has expired
                        if (timer.Elapsed.TotalMilliseconds > time_limit)
                        {
                            timesUp = true;
                        }
                        // Find the cost to the child node from the current node
                        int costToNode = node.costMatrix[node.rowIndex, child];

                        if ((costToNode < int.MaxValue) && (costToNode + node.localBound) < costOfBSSF)      // we want to create a new state
                        {
                            List<int> childrenToPass = new List<int>(node.children);
                            childrenToPass.Remove(child);
                            int[,] childMatrix = infinityOutMatrix(node.costMatrix, node.rowIndex, child);
                            BBNode childNode = new BBNode(childrenToPass, childMatrix, child);              // Create a state
                            childNode.localBound = node.localBound + node.costMatrix[node.rowIndex, child];
                            childNode.reduceMatrix();
                            childNode.Route.AddRange(node.Route);
                            childNode.Route.Add(Cities[child]);
                            numStatesCreated++;

                            // if it's a leaf, update the BSSF
                            if (childNode.children.Count == 0)
                            {
                                if (childNode.localBound < costOfBSSF)
                                {
                                    bssf = new TSPSolution(childNode.Route);
                                    costOfBSSF = bssf.costOfRoute();
                                    intermediateSolutions++;
                                }
                            }
                            else    // otherwise, add it to the queue.
                            {
                                priorityQueue.Enqueue(childNode, (float)(childNode.localBound / (childNode.Route.Count * 1000)));
                                if (priorityQueue.Count > maxInQueue)
                                {
                                    maxInQueue = priorityQueue.Count;
                                }
                            }
                        }
                        else
                        {
                            numStatesPruned++;
                        }
                        // otherwise ignore it. Definitely not part of the solution.
                    }
                }
                else numStatesPruned++;
                
            }

            timer.Stop();
            results[COST] = bssf.costOfRoute().ToString();    // load results into array here, replacing these dummy values
            results[TIME] = timer.Elapsed.ToString();
            results[COUNT] = intermediateSolutions.ToString();

            // Used to fill out the table in the report
            String storedStates = "Max Stored States: " + maxInQueue + "\n";
            String statesCreated = "Total States Created: " + numStatesCreated + "\n";
            String statesPruned = "Total State Pruned: " + numStatesPruned + "\n";
            MessageBox.Show(storedStates + statesCreated + statesPruned);
            Console.WriteLine(storedStates);
            Console.WriteLine(statesCreated);
            Console.WriteLine(statesPruned);

            return results;
        }


        private void printRoute(BBNode node)
        {
            List<City> cities = new List<City>(Cities);
            Console.WriteLine("Route: ");
            foreach (Object obj in node.Route)
            {
                City city = (City)obj;
                Console.Write(cities.IndexOf(city) + "->");
            }
            Console.WriteLine(cities.IndexOf((City)node.Route[0]));
        }

        private void printMatrix(int[,] matrix, string message=null)
        {
            Console.WriteLine();
            if (message != null)
            {
                Console.WriteLine(message);
            }
            Console.Write("[");
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                Console.Write("[");
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    Console.Write(matrix[i, j]);
                    if (j < matrix.GetLength(1) - 1) Console.Write(",");
                }
                Console.Write("]\n");
            }
            Console.Write("]");
        }

        private int[,] infinityOutMatrix(int[,] parent, int row, int col)
        {
            int[,] matrix = new int[parent.GetLength(0), parent.GetLength(1)];
            for (int i = 0; i < parent.GetLength(0); i++)
            {
                for (int j = 0; j < parent.GetLength(1); j++)
                {
                    if (i == row || j == col || (j == row && i == col))
                    {
                        matrix[i, j] = int.MaxValue;
                    }
                    else
                    {
                        matrix[i, j] = parent[i, j];
                    }
                }
            }
            return matrix;
        }

        private int[,] initializeCostMatrix(City[] cities, ref List<int> children, int me)
        {
            int[,] matrix = new int[cities.Length, cities.Length];
            for (int i = 0; i < cities.Length; i++)
            {
                for (int j = 0; j < cities.Length; j++)
                {
                    if (i == j)
                    {
                        matrix[i, j] = int.MaxValue;
                    }
                    else
                    {
                        matrix[i, j] = (int)cities[i].costToGetTo(cities[j]);
                    }
                }
                if (i != me) children.Add(i);
            }
            return matrix;
        }


        /////////////////////////////////////////////////////////////////////////////////////////////
        // These additional solver methods will be implemented as part of the group project.
        ////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// finds the greedy tour starting from each city and keeps the best (valid) one
        /// </summary>
        /// <returns>results array for GUI that contains three ints: cost of solution, time spent to find solution, number of solutions found during search (not counting initial BSSF estimate)</returns>
        public string[] greedySolveProblem()
        {
            string[] results = new string[3];

            // TODO: Add your implementation for a greedy solver here.

            results[COST] = "not implemented";    // load results into array here, replacing these dummy values
            results[TIME] = "-1";
            results[COUNT] = "-1";

            return results;
        }

        public string[] fancySolveProblem()
        {
            string[] results = new string[3];

            // TODO: Add your implementation for your advanced solver here.

            results[COST] = "not implemented";    // load results into array here, replacing these dummy values
            results[TIME] = "-1";
            results[COUNT] = "-1";

            return results;
        }
        #endregion
    }

}
