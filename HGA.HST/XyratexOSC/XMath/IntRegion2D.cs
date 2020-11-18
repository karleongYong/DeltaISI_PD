using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XyratexOSC.XMath
{
    /// <summary>
    /// 2D Rectanglular bounds in 64-bit integer coordinates
    /// </summary>
    public class IntRegion2D
    {
        /// <summary>
        /// Gets or sets the left position of this region.
        /// </summary>
        /// <value>
        /// The left position.
        /// </value>
        public Int64 Left
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the top position of this region.
        /// </summary>
        /// <value>
        /// The top position.
        /// </value>
        public Int64 Top
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the right position of this region.
        /// </summary>
        /// <value>
        /// The right position.
        /// </value>
        public Int64 Right
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the bottom position of this region.
        /// </summary>
        /// <value>
        /// The bottom position.
        /// </value>
        public Int64 Bottom
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntRegion2D"/> class.
        /// </summary>
        public IntRegion2D()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntRegion2D"/> class.
        /// </summary>
        /// <param name="left">The left position.</param>
        /// <param name="top">The top position.</param>
        /// <param name="right">The right position.</param>
        /// <param name="bottom">The bottom position.</param>
        public IntRegion2D(Int64 left, Int64 top, Int64 right, Int64 bottom)
        {
            this.Left = left; 
            this.Top = top;
            this.Right = right; 
            this.Bottom = bottom;
        }
    }
}
