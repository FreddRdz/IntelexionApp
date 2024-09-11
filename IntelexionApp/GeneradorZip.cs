using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelexionApp
{
    public class GeneradorZip
    {
        private Configuracion _config;
        private int _consecutivo;

        public GeneradorZip(Configuracion config)
        {
            _config = config;
            _consecutivo = 1;
        }

        public void GenerarArchivoZip()
        {
            string fechaActual = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
            string nombreCarpetaDia = DateTime.Now.ToString("dddd", new CultureInfo("es-ES"));

            string nombreZip = $"{fechaActual}.zip";

            string rutaZip = Path.Combine(_config.RutaSalidaZip, nombreZip); 

            using (FileStream zipFile = new(rutaZip, FileMode.Create))
            using (ZipArchive zip = new(zipFile, ZipArchiveMode.Create))
            {
                var carpetaDia = zip.CreateEntry($"{nombreCarpetaDia}/");

                var metaEntry = zip.CreateEntry($"{fechaActual}.meta");

                using (StreamWriter metaWriter = new(metaEntry.Open()))
                {
                    string[] imagenes = Directory.GetFiles(_config.RutaImagenes);
                    foreach (var imagen in imagenes)
                    {
                        string id = GenerarID();
                        DateTime fechaCreacion = File.GetCreationTime(imagen);

                        string rutaImagenZip = $"{nombreCarpetaDia}/{Path.GetFileName(imagen)}";

                        string lineaMeta = $"{id}{_config.Delimitador}{fechaCreacion:yyyy/MM/dd HH:mm:ss}{_config.Delimitador}{rutaImagenZip}";
                        metaWriter.WriteLine(lineaMeta);
                    }
                }

                string[] imagenesParaAgregar = Directory.GetFiles(_config.RutaImagenes);
                foreach (var imagen in imagenesParaAgregar)
                {
                    string rutaImagenZip = $"{nombreCarpetaDia}/{Path.GetFileName(imagen)}";
                    zip.CreateEntryFromFile(imagen, rutaImagenZip); 
                }
            }

            // Registrar en el log la creación del archivo zip
            LogArchivoGenerado(rutaZip);
        }

        private string GenerarID()
        {
            string diaJuliano = DateTime.Now.ToString("yy") + DateTime.Now.DayOfYear.ToString("000");
            string consecutivoStr = _consecutivo.ToString("D5");
            _consecutivo++;
            return diaJuliano + consecutivoStr;
        }

        private void LogArchivoGenerado(string rutaArchivo)
        {
            try
            {
                string rutaLog = _config.RutaLog;
                string directorioLog = Path.GetDirectoryName(rutaLog);

                if (!Directory.Exists(directorioLog))
                {
                    Directory.CreateDirectory(directorioLog);
                }

                if (!HasWritePermission(directorioLog))
                {
                    Console.WriteLine("No hay permisos para escribir en la ruta configurada. Escribiendo en Documentos.");
                    rutaLog = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "log.txt");
                }

                if (!File.Exists(rutaLog))
                {
                    using (StreamWriter sw = File.CreateText(rutaLog))
                    {
                        sw.WriteLine("Fecha y Hora de Creación | Nombre del Archivo | Ruta Completa del Archivo");
                    }
                }

                using StreamWriter logWriter = new(rutaLog, true);
                string fechaActual = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string nombreArchivo = Path.GetFileName(rutaArchivo);
                logWriter.WriteLine($"{fechaActual} | {nombreArchivo} | {rutaArchivo}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al escribir el log: {ex.Message}");
            }
        }

        private static bool HasWritePermission(string path)
        {
            try
            {
                string testFile = Path.Combine(path, Path.GetRandomFileName());
                using (FileStream fs = File.Create(testFile, 1, FileOptions.DeleteOnClose))
                { }
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
