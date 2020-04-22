using System;
using System.IO;
using System.Threading.Tasks;

namespace LoadDevelopmentUI.Helper
{
    public interface ISavePhoto
    {
        void SavePhoto(string fileName, string locaiton, Stream stream);
        string GetPhotoPath(string fileName, string locaiton);
        bool PhotoExists(string fileName, string location);
        void DeletePhoto(string v, string iMAGE_DIRECTORY);
        void CopyPhoto(string imageFileName, string iMAGE_DIRECTORY, string path);
    }
}
