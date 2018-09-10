using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.src.Model
{
    public class SeaGridAdapter : ISeaGrid
    {
        private SeaGrid _MyGrid;
        /// <summary>
        /// Create the SeaGridAdapter, with the grid, and it will allow it to be changed
        /// </summary>
        /// <param name="grid">the grid that needs to be adapted</param>
        public SeaGridAdapter(SeaGrid grid)
        {
            _MyGrid = grid;
            _MyGrid.Changed += new EventHandler(MyGrid_Changed);
        }

        /// <summary>
        /// MyGrid_Changed causes the grid to be redrawn by raising a changed event
        /// </summary>
        /// <param name="sender">the object that caused the change</param>
        /// <param name="e">what needs to be redrawn</param>
        private void MyGrid_Changed(object sender, EventArgs e)
        {
            if (Changed != null)
            {
                Changed(this, e);
            }
        }

        #region ISeaGrid Members

        /// <summary>
        /// Changes the discovery grid. Where there is a ship we will sea water
        /// </summary>
        /// <param name="x">tile x coordinate</param>
        /// <param name="y">tile y coordinate</param>
        /// <returns>a tile, either what it actually is, or if it was a ship then return a sea tile</returns>
        public TileView this[int x, int y]
        {
            get
            {
                TileView result = _MyGrid[x, y];

                if (result == TileView.Ship)
                {
                    return TileView.Sea;
                }
                else
                {
                    return result;
                }
            }   
        }

        /// <summary>
        /// Indicates that the grid has been changed
        /// </summary>
        public event EventHandler Changed;

        /// <summary>
        /// Get the width of a tile
        /// </summary>
        int ISeaGrid.Width
        {
            get { return _MyGrid.Width; }
        }

        /// <summary>
        /// Get the height of the tile
        /// </summary>
        int ISeaGrid.Height
        {
            get { return _MyGrid.Height; }
        }

        /// <summary>
        /// HitTile calls oppon _MyGrid to hit a tile at the row, col
        /// </summary>
        /// <param name="row">the row its hitting at</param>
        /// <param name="col">the column its hitting at</param>
        /// <returns>The result from hitting that tile</returns>
        public AttackResult HitTile(int row, int col)
        {
            return _MyGrid.HitTile(row, col);
        }
        #endregion
    }
}
