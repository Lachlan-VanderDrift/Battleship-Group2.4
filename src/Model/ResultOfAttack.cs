using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.src.Model
{
    public enum ResultOfAttack
    {
        /// <summary>
        /// The player hit something
        /// </summary>
        Hit,

        /// <summary>
        /// The player missed
        /// </summary>
        Miss,

        /// <summary>
        /// The player destroyed a ship
        /// </summary>
        Destroyed,

        /// <summary>
        /// That location was already shot.
        /// </summary>
        ShotAlready,

        /// <summary>
        /// The player killed all of the opponents ships
        /// </summary>
        GameOver
    }
}
