using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XyratexOSC.Factory
{
    public class RecipeEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the filepath for the recipe.
        /// </summary>
        /// <value>
        /// The recipe filepath.
        /// </value>
        public string Filepath
        {
            get;
            set;
        }

        public RecipeEventArgs(string filepath)
            : base()
        {
            Filepath = filepath;
        }
    }
}
