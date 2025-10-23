using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Champversity.DataAccess
{
    public class FileStorage
    {
        private readonly string _uploadDirectory;
        private readonly string _templateDirectory;
     
        public FileStorage(string baseDirectory)
        {
         _uploadDirectory = Path.Combine(baseDirectory, "Uploads");
      _templateDirectory = Path.Combine(baseDirectory, "Templates");
            
   // Ensure directories exist
Directory.CreateDirectory(_uploadDirectory);
        Directory.CreateDirectory(_templateDirectory);
   }
        
        public async Task<string> SaveUploadedFileAsync(IFormFile file)
        {
     if (file == null || file.Length == 0)
          return null;

            // Create unique filename
        string fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
          string filePath = Path.Combine(_uploadDirectory, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
          {
          await file.CopyToAsync(stream);
            }

            return fileName;
        }
        
     public string GetTemplatePath(string templateName)
      {
          return Path.Combine(_templateDirectory, templateName);
        }
        
        public bool IsValidExcelFile(IFormFile file)
        {
   if (file == null || file.Length == 0)
       return false;
         
     var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
      return extension == ".xlsx" || extension == ".xls";
        }
    
        public string GetUniversityFilePath(string universityName, DateTime date)
        {
            string dirPath = Path.Combine(_uploadDirectory, "UniversityFiles");
        Directory.CreateDirectory(dirPath);
    
       return Path.Combine(dirPath, $"{universityName}_{date:yyyyMMdd}.txt");
        }
        
     public string GetXmlFilePath(string studentId, string universityName)
        {
            string dirPath = Path.Combine(_uploadDirectory, "XmlFiles");
      Directory.CreateDirectory(dirPath);
            
 return Path.Combine(dirPath, $"{studentId}_{universityName}.xml");
      }
    }
}