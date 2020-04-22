using System;
using System.IO;
using System.Threading.Tasks;
using LoadDevelopmentUI.Helper;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

[assembly: Dependency(typeof(LoadDevelopmentUI.iOS.SavePhoto))]
namespace LoadDevelopmentUI.iOS
{
    public class SavePhoto : ISavePhoto
    {
        public void CopyPhoto(string fileName, string location, string fromPath)
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            documentsPath = Path.Combine(documentsPath, location);

            string toFilePath = Path.Combine(documentsPath, fileName);

            File.Copy(fromPath, toFilePath);

        }

        public void DeletePhoto(string fileName, string location)
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            documentsPath = Path.Combine(documentsPath, location);

            string filePath = Path.Combine(documentsPath, fileName);

            File.Delete(filePath);
        }

        public string GetPhotoPath(string fileName, string location)
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            documentsPath = Path.Combine(documentsPath, location);

            return Path.Combine(documentsPath, fileName);
        }

        public bool PhotoExists(string fileName, string location)
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            documentsPath = Path.Combine(documentsPath, location);

            string filePath = Path.Combine(documentsPath, fileName);

            return File.Exists(filePath);
        }

        void ISavePhoto.SavePhoto(string fileName, string location, Stream stream)
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            documentsPath = Path.Combine(documentsPath, location);
            if (!Directory.Exists(documentsPath))
			    Directory.CreateDirectory(documentsPath);

            string filePath = Path.Combine(documentsPath, fileName);

            byte[] bArray = new byte[stream.Length];
            using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                using (stream)
                {
                    stream.Read(bArray, 0, (int)stream.Length);
                }
                int length = bArray.Length;
                fs.Write(bArray, 0, length);
            }
        }
    }
}
