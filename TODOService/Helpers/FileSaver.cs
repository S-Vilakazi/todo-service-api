﻿namespace TODOService.Helpers
{
    public class FileSaver
    {
        IWebHostEnvironment _env;

        public FileSaver(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task FileSaveAsync(IFormFile file, string filePath)
        {
            // create file name 
            string filename = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            //create route 
            string route = Path.Combine(_env.WebRootPath, filePath);

            // check if directory exist if not create a directory
            if (!Directory.Exists(route))
            {
                Directory.CreateDirectory(route);
            }
            // create file route 
            string fileRoute = Path.Combine(route, filename);
            // using file stream copy of file 
            using(FileStream fs = File.Create(fileRoute))
            {
                await file.OpenReadStream().CopyToAsync(fs);
            }

        }

    }
}
