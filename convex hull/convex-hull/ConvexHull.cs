using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace _1_convex_hull
{
    class ConvexHull
    {
        List<PointF> m_Points;
        PointF m_leftmost;
        PointF m_rightmost;
        PointF m_upperTangentPoint;
        PointF m_lowerTangentPoint;

        public ConvexHull() { }

        public ConvexHull(List<PointF> _list)
        {
            m_Points = _list;
            m_leftmost = findLeftMost();
            m_rightmost = findRightMost();
            if (_list.Count == 3)
            {
                MakeClockwise();
            }
        }

        private void MakeClockwise()
        {
            if (m_Points.Count == 2)
            {
                float slope = CalculateSlope(m_Points[0], m_Points[1]);
                if (slope > 0)
                {
                    PointF temp = m_Points[0];
                    //m_Points[0] = m_Points[1];
                    //m_Points[1] = temp;
                }
            }
            else
            {
                float slopeOne = CalculateSlope(m_Points[0], m_Points[1]);
                float slopeTwo = CalculateSlope(m_Points[0], m_Points[2]);
                if (slopeOne < slopeTwo)
                {
                    PointF temp = m_Points[1];
                    m_Points[1] = m_Points[2];
                    m_Points[2] = temp;
                }
            }
        }

        public List<PointF> Points { get => m_Points; set => m_Points = value; }
        public PointF Leftmost { get => m_leftmost; set => m_leftmost = value; }
        public PointF Rightmost { get => m_rightmost; set => m_rightmost = value; }
        public PointF UpperTangentPoint { get => m_upperTangentPoint; set => m_upperTangentPoint = value; }
        public PointF LowerTangentPoint { get => m_lowerTangentPoint; set => m_lowerTangentPoint = value; }

        private PointF findLeftMost()
        {
            PointF leftMost = m_Points[0];
            for (int i = 0; i < m_Points.Count; i++)
            {
                if (m_Points[i].X < leftMost.X)
                {
                    leftMost = m_Points[i];
                }
            }
            return leftMost;
        }

        private PointF findRightMost()
        {
            PointF rightMost = m_Points[m_Points.Count-1];
            for (int i = 0; i < m_Points.Count; i++)
            {
                if (m_Points[i].X > rightMost.X)
                {
                    rightMost = m_Points[i];
                }
            }
            return rightMost;
        }

        private float CalculateSlope(PointF leftPoint, PointF rightPoint)
        {
            return ((leftPoint.Y - rightPoint.Y) / (rightPoint.X - leftPoint.X));
        }
    }
}
