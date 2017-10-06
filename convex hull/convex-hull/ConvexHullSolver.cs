using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Linq;
using _1_convex_hull;

namespace _2_convex_hull
{
    class ConvexHullSolver
    {
        System.Drawing.Graphics g;
        System.Windows.Forms.PictureBox pictureBoxView;

        public ConvexHullSolver(System.Drawing.Graphics g, System.Windows.Forms.PictureBox pictureBoxView)
        {
            this.g = g;
            this.pictureBoxView = pictureBoxView;
        }

        public void Refresh()
        {
            // Use this especially for debugging and whenever you want to see what you have drawn so far
            pictureBoxView.Refresh();
        }

        public void Pause(int milliseconds)
        {
            // Use this especially for debugging and to animate your algorithm slowly
            pictureBoxView.Refresh();
            System.Threading.Thread.Sleep(milliseconds);
        }

        public void Solve(List<System.Drawing.PointF> pointList)
        {
            List<System.Drawing.PointF> sortedPoints = pointList.OrderBy(o => o.X).ToList();
            ConvexHull hull = DivideAndConquer(sortedPoints);
            Draw(hull);            
        }

        private ConvexHull DivideAndConquer(List<System.Drawing.PointF> p_pointList)
        {
            if (p_pointList.Count == 2 || p_pointList.Count == 3)
            {
                return new ConvexHull(p_pointList);
            }
            else if (p_pointList.Count == 3)
            {
                MakeClockwise(p_pointList);
                return new ConvexHull(p_pointList);
            }
            else
            {
                int midpoint;
                ConvexHull leftHull;
                ConvexHull rightHull;

                if (p_pointList.Count % 2 == 1)
                {
                    midpoint = (p_pointList.Count / 2) + 1;
                    leftHull = DivideAndConquer(p_pointList.GetRange(0, midpoint));
                    rightHull = DivideAndConquer(p_pointList.GetRange(midpoint, midpoint-1));
                }
                else
                {
                    midpoint = p_pointList.Count / 2;
                    leftHull = DivideAndConquer(p_pointList.GetRange(0, midpoint));
                    rightHull = DivideAndConquer(p_pointList.GetRange(midpoint, midpoint));
                }

                return Merge(leftHull, rightHull);
            }
        }

        private ConvexHull Merge(ConvexHull p_left, ConvexHull p_right)
        {
            findUpperTangent(p_left, p_right);
            findLowerTangent(p_left, p_right);
            List<PointF> newHullPoints = new List<PointF>();

            int leftIndex = p_left.Points.IndexOf(p_left.UpperTangentPoint);
            int leftEndIndex = p_left.Points.IndexOf(p_left.LowerTangentPoint);
            do
            {
                newHullPoints.Add(p_left.Points[leftIndex]);
                if (leftIndex == 0)
                {

                }

            } while (leftIndex != leftEndIndex);

            int rightIndex = p_right.Points.IndexOf(p_right.UpperTangentPoint);
            int rightEndIndex = p_right.Points.IndexOf(p_right.LowerTangentPoint);
            do
            {
                newHullPoints.Add(p_right.Points[rightIndex]);
            } while (rightIndex != rightEndIndex);

            return new ConvexHull(newHullPoints);
        }

        private void findUpperTangent(ConvexHull p_left, ConvexHull p_right)
        {
            Boolean leftPointChange = false;
            Boolean rightPointChange = false;
            List<PointF> leftList = p_left.Points;
            List<PointF> rightList = p_right.Points;
            PointF leftPoint = p_left.Rightmost;
            PointF rightPoint = p_right.Leftmost;

            do
            {
                float slope = calculateSlope(leftPoint, rightPoint);
                float checkSlope = slope;
                for (int i = 0; i < leftList.Count; i++)
                {
                    float currSlope = calculateSlope(leftList[i], rightPoint);
                    if (currSlope < slope)
                    {
                        checkSlope = currSlope;
                        leftPoint = leftList[i];
                        leftPointChange = true;
                    }
                }

                for (int j = 0; j < rightList.Count; j++)
                {
                    float currSlope = calculateSlope(leftPoint, rightList[j]);
                    if (currSlope > slope)
                    {
                        checkSlope = currSlope;
                        rightPoint = rightList[j];
                        rightPointChange = true;
                    }
                }
                if (checkSlope == slope)
                {
                    leftPointChange = rightPointChange = false;
                }
            } while (leftPointChange || rightPointChange);

            p_left.UpperTangentPoint = leftPoint;
            p_right.UpperTangentPoint = rightPoint;
        }
        
        private void findLowerTangent(ConvexHull p_left, ConvexHull p_right)
        {
            Boolean leftPointChange = false;
            Boolean rightPointChange = false;
            List<PointF> leftList = p_left.Points;
            List<PointF> rightList = p_right.Points;
            PointF leftPoint = p_left.Rightmost;
            PointF rightPoint = p_right.Leftmost;

            do
            {
                float slope = calculateSlope(leftPoint, rightPoint);
                float checkSlope = slope;
                for (int i = 0; i < leftList.Count; i++)
                {
                    float currSlope = calculateSlope(leftList[i], rightPoint);
                    if (currSlope > slope)
                    {
                        checkSlope = currSlope;
                        leftPoint = leftList[i];
                        leftPointChange = true;
                    }
                }

                for (int j = 0; j < rightList.Count; j++)
                {
                    float currSlope = calculateSlope(leftPoint, rightList[j]);
                    if (currSlope < slope)
                    {
                        checkSlope = currSlope;
                        rightPoint = rightList[j];
                        rightPointChange = true;
                    }
                }

                if (checkSlope == slope)
                {
                    leftPointChange = rightPointChange = false;
                }
            } while (leftPointChange || rightPointChange);

            p_left.LowerTangentPoint = leftPoint;
            p_right.LowerTangentPoint = rightPoint;
            
        }

        private float calculateSlope(PointF leftPoint, PointF rightPoint)
        {
            return ((leftPoint.Y - rightPoint.Y)/(rightPoint.X - leftPoint.X));
        }

        private void MakeClockwise(List<PointF> p_list)
        {
            float slopeOne = calculateSlope(p_list[0], p_list[1]);
            float slopeTwo = calculateSlope(p_list[0], p_list[2]);
            if (slopeOne < slopeTwo)
            {
                PointF temp = p_list[1];
                p_list[1] = p_list[2];
                p_list[2] = temp;
            }
        }

        public void Draw(ConvexHull p_hull)
        {
            throw new NotImplementedException();
        }
    }
}
