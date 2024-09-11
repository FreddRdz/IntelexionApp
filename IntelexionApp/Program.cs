using System.IO;
using System.Xml.Linq;

namespace IntelexionApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            string rutaBase = AppDomain.CurrentDomain.BaseDirectory;

            string rutaConfig = Path.Combine(rutaBase, "config.xml");

            Configuracion config = Configuracion.CargarConfiguracion(rutaConfig);

            GeneradorZip generador = new(config);
            generador.GenerarArchivoZip();

            Console.WriteLine("Archivo zip generado exitosamente.");
        }
    }
}