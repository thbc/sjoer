using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Graphics
{
    public enum DataType
    {
        AIS/* , NAVAID */
    }

    public enum DisplayArea
    {
        HorizonPlane,
        SkyArea,
        HUD
    }

    public enum ExpandState
    {
        Collapsed,
        Hover,
        Target
    }
}
