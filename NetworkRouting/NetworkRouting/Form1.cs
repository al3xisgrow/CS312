﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NetworkRouting
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void clearAll()
        {
            startNodeIndex = -1;
            stopNodeIndex = -1;
            sourceNodeBox.Clear();
            sourceNodeBox.Refresh();
            targetNodeBox.Clear();
            targetNodeBox.Refresh();
            arrayTimeBox.Clear();
            arrayTimeBox.Refresh();
            heapTimeBox.Clear();
            heapTimeBox.Refresh();
            differenceBox.Clear();
            differenceBox.Refresh();
            pathCostBox.Clear();
            pathCostBox.Refresh();
            arrayCheckBox.Checked = false;
            arrayCheckBox.Refresh();
            return;
        }

        private void clearSome()
        {
            arrayTimeBox.Clear();
            arrayTimeBox.Refresh();
            heapTimeBox.Clear();
            heapTimeBox.Refresh();
            differenceBox.Clear();
            differenceBox.Refresh();
            pathCostBox.Clear();
            pathCostBox.Refresh();
            return;
        }

        private void generateButton_Click(object sender, EventArgs e)
        {
            int randomSeed = int.Parse(randomSeedBox.Text);
            int size = int.Parse(sizeBox.Text);

            Random rand = new Random(randomSeed);
            seedUsedLabel.Text = "Random Seed Used: " + randomSeed.ToString();

            clearAll();
            this.adjacencyList = generateAdjacencyList(size, rand);
            List<PointF> points = generatePoints(size, rand);
            resetImageToPoints(points);
            this.points = points;
        }

        // Generates the distance matrix.  Values of -1 indicate a missing edge.  Loopbacks are at a cost of 0.
        private const int MIN_WEIGHT = 1;
        private const int MAX_WEIGHT = 100;
        private const double PROBABILITY_OF_DELETION = 0.35;

        private const int NUMBER_OF_ADJACENT_POINTS = 3;

        private List<HashSet<int>> generateAdjacencyList(int size, Random rand)
        {
            List<HashSet<int>> adjacencyList = new List<HashSet<int>>();

            for (int i = 0; i < size; i++)
            {
                HashSet<int> adjacentPoints = new HashSet<int>();
                while (adjacentPoints.Count < 3)
                {
                    int point = rand.Next(size);
                    if (point != i) adjacentPoints.Add(point);
                }
                adjacencyList.Add(adjacentPoints);
            }

            return adjacencyList;
        }

        private List<PointF> generatePoints(int size, Random rand)
        {
            List<PointF> points = new List<PointF>();
            for (int i = 0; i < size; i++)
            {
                points.Add(new PointF((float) (rand.NextDouble() * pictureBox.Width), (float) (rand.NextDouble() * pictureBox.Height)));
            }
            return points;
        }

        private void resetImageToPoints(List<PointF> points)
        {
            pictureBox.Image = new Bitmap(pictureBox.Width, pictureBox.Height);
            Graphics graphics = Graphics.FromImage(pictureBox.Image);
            Pen pen;

            if (points.Count < 100)
                pen = new Pen(Color.Blue);
            else
                pen = new Pen(Color.LightBlue);
            foreach (PointF point in points)
            {
                graphics.DrawEllipse(pen, point.X, point.Y, 2, 2);
            }

            this.graphics = graphics;
            pictureBox.Invalidate();
        }

        // These variables are instantiated after the "Generate" button is clicked
        private List<PointF> points = new List<PointF>();
        private Graphics graphics;
        private List<HashSet<int>> adjacencyList;

        // Use this to generate paths (from start) to every node; then, just return the path of interest from start node to end node
        private void solveButton_Click(object sender, EventArgs e)
        {
            // This was the old entry point, but now it is just some form interface handling
            bool ready = true;

            if(startNodeIndex == -1)
            {
                sourceNodeBox.Focus();
                sourceNodeBox.BackColor = Color.Red;
                ready = false;
            }
            if(stopNodeIndex == -1)
            {
                if(!sourceNodeBox.Focused)
                    targetNodeBox.Focus();
                targetNodeBox.BackColor = Color.Red;
                ready = false;
            }
            if (points.Count > 0)
            {
                resetImageToPoints(points);
                paintStartStopPoints();
            }
            else
            {
                ready = false;
            }
            if(ready)
            {
                clearSome();
                solveButton_Clicked();  // Here is the new entry point
            }
        }

        private void solveButton_Clicked()
        {
            // *** Implement this method, use the variables "startNodeIndex" and "stopNodeIndex" as the indices for your start and stop points, respectively ***
            //ScrollMessageBox msg = new ScrollMessageBox();
            //String str = "Adjacency List:\n";
            //msg.appendText("Adjacency List:");
            //for (int i = 0; i < adjacencyList.Count; i++)
            //{
            //    String nodes = "";
            //    nodes += ("\tnode " + i + ":");
            //    List<int> list = adjacencyList[i].ToList();
            //    for (int j = 0; j < adjacencyList[i].Count; j++)
            //    {
            //        nodes += ("\t" + list[j]);
            //    }
            //    //str += "\n";
            //    msg.appendText(nodes);
            //}
            ////msg.setText(str);
            //msg.Show();

            // Initializes the HeapQueue arrays.
            g_distanceHQ = new int[points.Count];
            g_previousHQ = new int[points.Count];
            g_arrayHQ = new int[points.Count];

            // Stopwatch used to measure performance.
            Stopwatch sw = new Stopwatch();
            sw.Start();
            runHeapQueue();
            sw.Stop();
            double heapTime = sw.Elapsed.TotalMilliseconds;
            string elapsed = (heapTime/ 1000).ToString();
            heapTimeBox.Text = elapsed;
            
            // If the compare array box is checked, run the test on the ArrayQueue
            if (arrayCheckBox.Checked)
            {
                g_distanceAQ = new int[adjacencyList.Count];
                g_previousAQ = new int[adjacencyList.Count];
                g_arrayAQ = new int[adjacencyList.Count];

                sw.Reset();
                sw.Start();
                runArrayQueue();
                sw.Stop();
                double arrayTime = (sw.Elapsed.TotalMilliseconds);
                elapsed = (arrayTime / 1000).ToString();
                arrayTimeBox.Text = elapsed;

                double speedup = arrayTime / heapTime;
                differenceBox.Text = speedup.ToString();
            }

            display(arrayCheckBox.Checked);
        }

        // These arrays are used for the ArrayQueue. They are passed by reference in makequeue. 
        int[] g_distanceAQ;         // Holds the distance values for each node. The indexes represent the nodes.
        int[] g_previousAQ;         // Holds the previous node number for each node. 
                                    // if g_previousAQ[2] = 3, then there is an edge 3->2 that is currently the lowest cost edge to node 2
        int[] g_arrayAQ;            // Array structure that becomes the queue. Just given default values and then passed in.

        int[] g_distanceHQ;         // Same function, just for the heap
        int[] g_previousHQ;         // Same function, just for the heap
        int[] g_arrayHQ;            // Same function, just for the heap

        // Creates an ArrayQueue and calls Dykstra's with the ArrayQueue Arrays from above.
        private void runArrayQueue()
        {
            ArrayQueue queue= new ArrayQueue();
            Dykstra(queue, g_distanceAQ, g_previousAQ, g_arrayAQ);

            /*
            int currIndex = stopNodeIndex;
            string path = "Path: ";
            while (currIndex != startNodeIndex)
            {
                if (currIndex < 0)
                    break;
                path += (currIndex + "-");
                currIndex = g_previousAQ[currIndex];
                if (currIndex == startNodeIndex)
                {
                    path += startNodeIndex;
                    MessageBox.Show(path);
                }
            }
            */
        }

        // Creates an HeapQueue and calls Dykstra's with the ArrayQueue Arrays from above.
        private void runHeapQueue()
        {
            HeapQueue queue = new HeapQueue();
            Dykstra(queue, g_distanceHQ, g_previousHQ, g_arrayHQ);
            /* int currIndex = stopNodeIndex;
            string path = "Path: ";
            while (currIndex != startNodeIndex)
            {
                if (currIndex < 0)
                {
                    MessageBox.Show(path);
                    break;
                }
                path += (currIndex + "-");
                currIndex = g_previousHQ[currIndex];
                if (currIndex == startNodeIndex)
                {
                    path += startNodeIndex;
                    MessageBox.Show(path);
                }
            } */
        }

        /** Dykstra's Algorithm:
         * Dykstra(G,l,s)
            for all v in V
                dist(v) = inf
                prev(v) = null
            distance(s) = 0
            H.makeQueue(V)
            while H not empty
                u = H.deleteMin() //pops the smalles distance node
                for all (u,v) in Edges
                    if dist(v) > dist(u) + cost(u,v)
                        dist(v) = dist(u) + cost(u,v)
                        prev(v) = u
                        H.decreaseKey(e) // Tells the queue that somebody's priority has changed. Might need to bump it up the priority queue
         */
        private void Dykstra(IQueue queue, int[] distance, int[] previous, int[] array)
        {
            // Initialize the values in the arrays.
            for (int i = 0; i < adjacencyList.Count; i++)
            {
                distance[i] = int.MaxValue;
                previous[i] = -1;
                array[i] = 0;
            }
            // Set distance to start node to 0
            distance[startNodeIndex] = 0;
            // Call makeQueue. This has different run time depending on the type of IQueue
            queue.makeQueue(distance, previous, array, adjacencyList.Count);
            // Executes the loop until everything has been popped from the loop.
            while (!queue.isEmpty())
            {
                // Call to pop from front of priority queue. 
                int node = queue.deleteMin();
                // Error Checking. This should never evaluate to true.
                if (distance[node] == int.MaxValue) return;
                // For all the nodes that this node is directionally connected to, see if the distance can be shortened.
                for (int i = 0; i < adjacencyList[node].Count; i++)
                {
                    int neighbor = adjacencyList[node].ToList()[i];
                    int cost = (int) calculateCost(node, neighbor);
                    // If the current distance is greater than if it went through this node.
                    if (distance[neighbor] == int.MaxValue || distance[neighbor] > (distance[node] + cost))
                    {
                        distance[neighbor] = distance[node] + cost;
                        previous[neighbor] = node;
                        queue.decreaseKey(neighbor);
                    }
                }
            }

        }

        // This function prints the path onto the pictureBox. The boolean value determines which search's results are used to print. 
        // If true, it prints the results from the ArrayQueue. It's now irrelevant, as they both are completed. But was helpful in debugging.
        private void display(bool showArray)
        {
            int[] backtrace;
            int[] dist;
            if (showArray)
            {
                backtrace = g_previousAQ;
                dist = g_distanceAQ;
            }
            else
            {
                backtrace = g_previousHQ;
                dist = g_distanceHQ;
            }
            Graphics graphics = Graphics.FromImage(pictureBox.Image);
            Pen pen = new Pen(Color.Black);

            int currIndex = stopNodeIndex;
            // Start from the last node, and work backwards through the path using the previous array until I arrive at the start node.
            while (currIndex != startNodeIndex)
            {
                // This catches if there is no path from the target to the source. If backtrace[currIndex] == -1, we reached a root node that isn't the source node. So there's no path.
                if (backtrace[currIndex] == -1 && backtrace[currIndex] != stopNodeIndex)
                {
                    resetImageToPoints(points);
                    MessageBox.Show("There is no path from Source: " + startNodeIndex + " to Target: " + stopNodeIndex);
                    return;
                }
                graphics.DrawLine(pen, points[currIndex], points[backtrace[currIndex]]);
                drawDistance(ref graphics, currIndex, backtrace);
                this.graphics = graphics;
                pictureBox.Invalidate();
                currIndex = backtrace[currIndex];
            }
            // Update the total distance of the path.
            pathCostBox.Text = dist[stopNodeIndex].ToString();
        }

        // Draws the cost of each segment of the path next to the line.
        private void drawDistance(ref Graphics graphics, int currIndex, int[] backtrace)
        {
            graphics.DrawString(((int)calculateCost(currIndex, backtrace[currIndex])).ToString(), 
                                                    new Font("Arial", 11), new SolidBrush(Color.Black), 
                                                    midpoint(points[currIndex], points[backtrace[currIndex]]));
        }

        // Calculates the cost between two points by using the pythagorean theorem.
        private double calculateCost(int _node, int _neighbor)
        {
            PointF node = points[_node];
            PointF neighbor = points[_neighbor];
            float rise = node.Y - neighbor.Y;
            float run = node.X - neighbor.X;
            return (Math.Sqrt(rise * rise + run * run));
        }

        // Calculates the midpoint between two points. Used to draw the costs.
        private PointF midpoint(PointF p1, PointF p2)
        {
            return new PointF((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
        }

        private Boolean startStopToggle = true;
        private int startNodeIndex = -1;
        private int stopNodeIndex = -1;
        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (points.Count > 0)
            {
                Point mouseDownLocation = new Point(e.X, e.Y);
                int index = ClosestPoint(points, mouseDownLocation);
                if (startStopToggle)
                {
                    startNodeIndex = index;
                    sourceNodeBox.ResetBackColor();
                    sourceNodeBox.Text = "" + index;
                }
                else
                {
                    stopNodeIndex = index;
                    targetNodeBox.ResetBackColor();
                    targetNodeBox.Text = "" + index;
                }
                resetImageToPoints(points);
                paintStartStopPoints();
            }
        }

        private void sourceNodeBox_Changed(object sender, EventArgs e)
        {
            if (points.Count > 0)
            {
                try{ startNodeIndex = int.Parse(sourceNodeBox.Text); }
                catch { startNodeIndex = -1; }
                if (startNodeIndex < 0 | startNodeIndex > points.Count-1)
                    startNodeIndex = -1;
                if(startNodeIndex != -1)
                {
                    sourceNodeBox.ResetBackColor();
                    resetImageToPoints(points);
                    paintStartStopPoints();
                    startStopToggle = !startStopToggle;
                }
            }
        }

        private void targetNodeBox_Changed(object sender, EventArgs e)
        {
            if (points.Count > 0)
            {
                try { stopNodeIndex = int.Parse(targetNodeBox.Text); }
                catch { stopNodeIndex = -1; }
                if (stopNodeIndex < 0 | stopNodeIndex > points.Count-1)
                    stopNodeIndex = -1;
                if(stopNodeIndex != -1)
                {
                    targetNodeBox.ResetBackColor();
                    resetImageToPoints(points);
                    paintStartStopPoints();
                    startStopToggle = !startStopToggle;
                }
            }
        }
        
        private void paintStartStopPoints()
        {
            if (startNodeIndex > -1)
            {
                Graphics graphics = Graphics.FromImage(pictureBox.Image);
                graphics.DrawEllipse(new Pen(Color.Green, 6), points[startNodeIndex].X, points[startNodeIndex].Y, 1, 1);
                this.graphics = graphics;
                pictureBox.Invalidate();
            }

            if (stopNodeIndex > -1)
            {
                Graphics graphics = Graphics.FromImage(pictureBox.Image);
                graphics.DrawEllipse(new Pen(Color.Red, 2), points[stopNodeIndex].X - 3, points[stopNodeIndex].Y - 3, 8, 8);
                this.graphics = graphics;
                pictureBox.Invalidate();
            }
        }

        private int ClosestPoint(List<PointF> points, Point mouseDownLocation)
        {
            double minDist = double.MaxValue;
            int minIndex = 0;

            for (int i = 0; i < points.Count; i++)
            {
                double dist = Math.Sqrt(Math.Pow(points[i].X-mouseDownLocation.X,2) + Math.Pow(points[i].Y - mouseDownLocation.Y,2));
                if (dist < minDist)
                {
                    minIndex = i;
                    minDist = dist;
                }
            }

            return minIndex;
        }
    }
}
