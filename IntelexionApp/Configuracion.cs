using System.Xml.Linq;

namespace IntelexionApp
{
    public class Configuracion
    {
        public string RutaImagenes { get; set; }
        public string Delimitador { get; set; }
        public string RutaSalidaZip { get; set; }
        public string RutaLog { get; set; }

        public static Configuracion CargarConfiguracion(string ruta)
        {
            XElement config = XElement.Load(ruta);
            return new Configuracion
            {
                RutaImagenes = config.Element("rutaImagenes").Value,
                Delimitador = config.Element("delimitador").Value,
                RutaSalidaZip = config.Element("rutaSalidaZip").Value,
                RutaLog = config.Element("rutaLog").Value
            };
        }
    }
}
