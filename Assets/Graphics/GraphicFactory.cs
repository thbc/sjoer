using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Positional;

namespace Assets.Graphics
{
    class GraphicFactory : HelperClasses.CSSingleton<GraphicFactory>
    {
        public Player aligner = null;

        AISHorizonShapeProvider aisHorizonShapeProvider;
        AISSkyShapeProvider aisSkyShapeProvider;

        NAVAIDHorizonShapeProvider navaidHorizonShapeProvider;

        AISHorizonFiller aisHorizonFiller;
        AISSkyFiller aisSkyFiller;

        NAVAIDHorizonFiller navaidHorizonFiller;
        Filler baseFiller;
        AISHorizonPositioner aisHorizonPositioner;
        AISSkyPositioner aisSkyPositioner;
        NAVAIDHorizonPositioner navaidHorizonPositioner;

        AISSkyPostProcessor aisSkyPostProcessor;
        AISHorizonPostProcessor aisHorizonPostProcessor;

        NAVAIDHorizonPostProcessor navaidHorizonPostProcessor;


        public GraphicFactory()
        {
            aisHorizonShapeProvider = new AISHorizonShapeProvider();

            //--new code:
            navaidHorizonShapeProvider = new NAVAIDHorizonShapeProvider();
            //--
            aisSkyShapeProvider = new AISSkyShapeProvider();
            aisHorizonFiller = new AISHorizonFiller();
            aisSkyFiller = new AISSkyFiller();

            navaidHorizonFiller = new NAVAIDHorizonFiller();

            baseFiller = new Filler();
            aisHorizonPositioner = new AISHorizonPositioner();
            aisSkyPositioner = new AISSkyPositioner();

            navaidHorizonPositioner = new NAVAIDHorizonPositioner();

            aisSkyPostProcessor = new AISSkyPostProcessor();
            aisHorizonPostProcessor = new AISHorizonPostProcessor();

            navaidHorizonPostProcessor = new NAVAIDHorizonPostProcessor();
        }

        public PostProcessor GetPostProcessor(DataType dataType, DisplayArea displayArea)
        {
            PostProcessor postProcessor;

            switch ((dataType, displayArea))
            {
                case (DataType.AIS, DisplayArea.HorizonPlane):
                    postProcessor = aisHorizonPostProcessor;
                    break;
                case (DataType.AIS, DisplayArea.SkyArea):
                    postProcessor = aisSkyPostProcessor;
                    break;

                case (DataType.NAVAID, DisplayArea.HorizonPlane):
                    postProcessor = navaidHorizonPostProcessor;
                    break;
                default:
                    throw new ArgumentException("No such data type", nameof(dataType));
            }

            postProcessor.SetAligner(aligner);
            return postProcessor;

        }

        public Filler GetFiller(DataType dataType, DisplayArea displayArea)
        {
            Filler filler;

            switch ((dataType, displayArea))
            {
                case (DataType.AIS, DisplayArea.HorizonPlane):
                    filler = aisHorizonFiller;
                    break;
                case (DataType.AIS, DisplayArea.SkyArea):
                    filler = aisSkyFiller;
                    break;
                    case (DataType.NAVAID, DisplayArea.HorizonPlane):
                    filler = navaidHorizonFiller;
                    break;
                default:
                    throw new ArgumentException("No such data type", nameof(dataType));
            }

            filler.SetAligner(aligner);
            return filler;
        }

        public Positioner getPositioner(DataType dataType, DisplayArea displayArea)
        {
            Positioner positioner;

            switch ((dataType, displayArea))
            {
                case (DataType.AIS, DisplayArea.HorizonPlane):
                    aisHorizonPositioner.SetAligner(aligner);
                    positioner = aisHorizonPositioner;
                    break;
                case (DataType.AIS, DisplayArea.SkyArea):
                    aisSkyPositioner.SetAligner(aligner);
                    positioner = aisSkyPositioner;
                    break;
                      case (DataType.NAVAID, DisplayArea.HorizonPlane):
                    navaidHorizonPositioner.SetAligner(aligner);
                    positioner = navaidHorizonPositioner;
                    break;
                default:
                    throw new ArgumentException("No such data source", nameof(dataType));
            }

            return positioner;
        }

        public ShapeProvider getShapeProvider(DataType dataType, DisplayArea displayArea)
        {
            ShapeProvider shape;

            switch ((dataType, displayArea))
            {
                case (DataType.AIS, DisplayArea.HorizonPlane):
                    shape = aisHorizonShapeProvider;
                    break;
                case (DataType.AIS, DisplayArea.SkyArea):
                    shape = aisSkyShapeProvider;
                    break;

                    case (DataType.NAVAID, DisplayArea.HorizonPlane):
                    shape = navaidHorizonShapeProvider;
                    break;
                default:
                    throw new ArgumentException("No such data type", nameof(dataType));
            }

            return shape;
        }
    }
}
