using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XyratexOSC.XMath
{
    /// <summary>
    /// Polygon represented as a list of <see cref="IntPoint2D"/>. The use of 64bit integer coordinates improves the speed and accuracy of the polygon math (versus using floating point arithmetic).
    /// </summary>
    public class Polygon : List<IntPoint2D>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon"/> class.
        /// </summary>
        public Polygon()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon"/> class.
        /// </summary>
        /// <param name="capacity">The number of elements that the new list can initially store.</param>
        public Polygon(int capacity)
            : base (capacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon"/> class.
        /// </summary>
        /// <param name="points">The points.</param>
        public Polygon(IEnumerable<IntPoint2D> points)
            : base(points)
        {
        }

        /*
        public IntPoint2D GetCenter()
        {
            //Add the first point at the end of the array.
            List<IntPoint2D> points = new List<IntPoint2D>();
            
            points.AddRange(this);
            points.Add(new IntPoint2D(this[0]));

            //Find the centroid.
            double X = 0;
            double Y = 0;

            Int64 secondFactor;

            for (int i = 0; i < points.Count - 2; i++)
            {
                secondFactor = points[i].X * points[i + 1].Y - points[i + 1].X * points[i].Y;
                X += (points[i].X + points[i + 1].X) * secondFactor;
                Y += (points[i].Y + points[i + 1].Y) * secondFactor;
            }

            //Divide by 6 times the polygon's area.
            double area = PolygonMath.Area(this);
            X /= (6 * area);
            Y /= (6 * area);

            //If the values are negative, the polygon is oriented counterclockwise. Reverse the signs.
            if (X < 0)
            {
                X = -X;
                Y = -Y;
            }

            return new IntPoint2D((Int64)X, (Int64)Y);
        }
        */

        /// <summary>
        /// Determines whether the specified point is contained within this polygon.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>
        ///   <c>true</c> if the specified point is contained within this polygon; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsPoint(IntPoint2D point)
        {
            IntPoint2D p1, p2;

            bool inside = false;

            if (this.Count < 3)
                return inside;

            var oldPoint = new IntPoint2D(this[Count - 1].X, this[Count - 1].Y);
            
            for (int i = 0; i < this.Count; i++)
            {
                var newPoint = new IntPoint2D(this[i]);
                
                if (newPoint.X > oldPoint.X)
                {
                    p1 = oldPoint;
                    p2 = newPoint;
                }
                else
                {
                    p1 = newPoint;
                    p2 = oldPoint;
                }

                if ((newPoint.X < point.X) == (point.X <= oldPoint.X)
                    && (point.Y - (long)p1.Y) * (p2.X - p1.X)
                    < (p2.Y - (long)p1.Y) * (point.X - p1.X))
                {
                    inside = !inside;
                }

                oldPoint = newPoint;
            }

            return inside;
        }


        /// <summary>
        /// Gets the center of the polygon bounds.
        /// </summary>
        /// <returns></returns>
        public IntPoint2D GetCenter()
        {
            IntRegion2D bounds = GetBounds();

            return new IntPoint2D((bounds.Left + bounds.Right) / 2, (bounds.Top + bounds.Bottom) / 2);
        }

        /// <summary>
        /// Gets the polygon bounds.
        /// </summary>
        /// <returns></returns>
        public IntRegion2D GetBounds()
        {
            Int64 minX = Int64.MaxValue;
            Int64 minY = Int64.MaxValue;

            Int64 maxX = Int64.MinValue;
            Int64 maxY = Int64.MinValue;

            foreach (IntPoint2D point in this)
            {
                if (point.X < minX)
                    minX = point.X;
                if (point.X > maxX)
                    maxX = point.X;
                if (point.Y < minY)
                    minY = point.Y;
                if (point.Y > maxY)
                    maxY = point.Y;
            }

            return new IntRegion2D(minX, minY, maxX, maxY);
        }
    }

    /// <summary>
    /// A list of <see cref="Polygon"/> objects.
    /// </summary>
    public class Polygons : List<Polygon>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Polygons"/> class.
        /// </summary>
        public Polygons()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Polygons"/> class.
        /// </summary>
        /// <param name="capacity">The number of elements that the new list can initially store.</param>
        public Polygons(int capacity)
            : base (capacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Polygons"/> class.
        /// </summary>
        /// <param name="polygons">The polygons.</param>
        public Polygons(IEnumerable<Polygon> polygons)
            : base(polygons)
        {
        }
    }

    /// <summary>
    /// A polygon that contains holes. It is represented as an Outer polygon and a list of polygon holes.
    /// </summary>
    public class ExPolygon
    {
        /// <summary>
        /// Gets or sets the outer polygon.
        /// </summary>
        /// <value>
        /// The outer polygon.
        /// </value>
        public Polygon Outer
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the list of polygon holes.
        /// </summary>
        /// <value>
        /// The polygon holes.
        /// </value>
        public Polygons Holes
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExPolygon"/> class.
        /// </summary>
        public ExPolygon()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExPolygon"/> class.
        /// </summary>
        /// <param name="outer">The outer polygon.</param>
        /// <param name="holes">The list of polygon holes.</param>
        public ExPolygon(Polygon outer, Polygons holes)
        {
            Outer = outer;
            Holes = holes;
        }
    }

    /// <summary>
    /// A list of <see cref="ExPolygon"/> objects.
    /// </summary>
    public class ExPolygons : List<ExPolygon>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExPolygons"/> class.
        /// </summary>
        public ExPolygons()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExPolygons"/> class.
        /// </summary>
        /// <param name="capacity">The number of elements that the new list can initially store.</param>
        public ExPolygons(int capacity)
            : base(capacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExPolygons"/> class.
        /// </summary>
        /// <param name="polygons">The polygons (containing holes).</param>
        public ExPolygons(IEnumerable<ExPolygon> polygons)
            : base(polygons)
        {
        }
    }

    /// <summary>
    /// Polygon clipping type.
    /// </summary>
    public enum ClipType 
    {
        /// <summary>
        /// Intersection
        /// </summary>
        ctIntersection,
        /// <summary>
        /// Union
        /// </summary>
        ctUnion,
        /// <summary>
        /// Difference
        /// </summary>
        ctDifference,
        /// <summary>
        /// Xor
        /// </summary>
        ctXor 
    }

    /// <summary>
    /// Polygon type.
    /// </summary>
    public enum PolyType 
    {
        /// <summary>
        /// The polygon subject
        /// </summary>
        ptSubject,
        /// <summary>
        /// The polygon clip
        /// </summary>
        ptClip 
    };
    
    /// <summary>
    /// Polygon filling type rules.
    /// </summary>
    /// <remarks>
    /// By far the most widely used winding rules for polygon filling are
    /// EvenOdd and NonZero (GDI, GDI+, XLib, OpenGL, Cairo, AGG, Quartz, SVG, Gr32)
    /// Others rules include Positive, Negative and ABS_GTR_EQ_TWO (only in OpenGL)
    /// see http://glprogramming.com/red/chapter11.html
    /// </remarks>
    public enum PolyFillType 
    {
        /// <summary>
        /// Even-odd
        /// </summary>
        pftEvenOdd,
        /// <summary>
        /// Non-zero
        /// </summary>
        pftNonZero,
        /// <summary>
        /// Positive
        /// </summary>
        pftPositive,
        /// <summary>
        /// Negative
        /// </summary>
        pftNegative 
    };

    /// <summary>
    /// Polygon join type.
    /// </summary>
    public enum JoinType 
    {
        /// <summary>
        /// Square
        /// </summary>
        jtSquare,
        /// <summary>
        /// Round
        /// </summary>
        jtRound,
        /// <summary>
        /// Miter
        /// </summary>
        jtMiter 
    };

    [Flags]
    internal enum EdgeSide 
    { 
        esNeither = 0, 
        esLeft = 1, 
        esRight = 2, 
        esBoth = 3 
    };

    [Flags]
    internal enum Protects 
    { 
        ipNone = 0, 
        ipLeft = 1,
        ipRight = 2, 
        ipBoth = 3 
    };

    internal enum Direction
    { 
        dRightToLeft, 
        dLeftToRight 
    };

    internal class TEdge
    {
        public Int64 xbot;
        public Int64 ybot;
        public Int64 xcurr;
        public Int64 ycurr;
        public Int64 xtop;
        public Int64 ytop;
        public double dx;
        public Int64 tmpX;
        public PolyType polyType;
        public EdgeSide side;
        public int windDelta; //1 or -1 depending on winding direction
        public int windCnt;
        public int windCnt2; //winding count of the opposite polytype
        public int outIdx;
        public TEdge next;
        public TEdge prev;
        public TEdge nextInLML;
        public TEdge nextInAEL;
        public TEdge prevInAEL;
        public TEdge nextInSEL;
        public TEdge prevInSEL;
    };

    internal class IntersectNode
    {
        public TEdge edge1;
        public TEdge edge2;
        public IntPoint2D pt;
        public IntersectNode next;
    };

    internal class LocalMinima
    {
        public Int64 Y;
        public TEdge leftBound;
        public TEdge rightBound;
        public LocalMinima next;
    };

    internal class Scanbeam
    {
        public Int64 Y;
        public Scanbeam next;
    };

    internal class OutRec
    {
        public int idx;
        public bool isHole;
        public OutRec FirstLeft;
        public OutRec AppendLink;
        public OutPt pts;
        public OutPt bottomPt;
        public OutPt bottomFlag;
        public EdgeSide sides;
    };

    internal class OutPt
    {
        public int idx;
        public IntPoint2D pt;
        public OutPt next;
        public OutPt prev;
    };

    internal class JoinRec
    {
        public IntPoint2D pt1a;
        public IntPoint2D pt1b;
        public int poly1Idx;
        public IntPoint2D pt2a;
        public IntPoint2D pt2b;
        public int poly2Idx;
    };

    internal class HorzJoinRec
    {
        public TEdge edge;
        public int savedIdx;
    };


}
