using Assets.Positional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.DataManagement
{
    class ParameterExtractor
    {
        // Takes in a string (JSON?) and spits out DTO object
        public virtual string[] get()
        {
            return new string[0];
        }
    }

    class BarentswatchAISParameterExtractor : ParameterExtractor
    {
        private Player aligner;
        public BarentswatchAISParameterExtractor(Player aligner)
        {
            this.aligner = aligner;
        }

        public override string[] get()
        {
            Tuple<Vector2, Vector2> latLonArea = aligner.GetCurrentLatLonArea();
            return new string[]
                {
                    // latmin lonmin latmax lonmax
                    latLonArea.Item1.x.ToString(),
                    latLonArea.Item1.y.ToString(),
                    latLonArea.Item2.x.ToString(),
                    latLonArea.Item2.y.ToString()
                };
        }
    }
}
