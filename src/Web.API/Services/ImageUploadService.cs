using System.Security.AccessControl;
using System.Security.Principal;
using Web.Domain.Interfaces;

namespace Web.API.Services
{
    public class ImageUploadService : IImageUploadService
    {
        private readonly string _uploadsFolder;

        public ImageUploadService(IWebHostEnvironment webHostEnvironment)
        {
            _uploadsFolder = Path.Combine(webHostEnvironment.ContentRootPath, "wwwroot", "uploads", "produtos");

            try
            {
                if (!Directory.Exists(_uploadsFolder))
                {
                    // Criar a pasta
                    Directory.CreateDirectory(_uploadsFolder);

                    // Definir permissões para SmarterASP.NET
                    var directoryInfo = new DirectoryInfo(_uploadsFolder);
                    var security = directoryInfo.GetAccessControl();

                    // Adicionar permissão para IUSR
                    var iusrSid = new SecurityIdentifier(WellKnownSidType.InteractiveSid, null);
                    security.AddAccessRule(new FileSystemAccessRule(
                        iusrSid,
                        FileSystemRights.Modify | FileSystemRights.Read | FileSystemRights.Write,
                        InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                        PropagationFlags.None,
                        AccessControlType.Allow));

                    // Aplicar as permissões
                    directoryInfo.SetAccessControl(security);
                }
            }
            catch (Exception ex)
            {
                // Log do erro
                Console.WriteLine($"Erro ao configurar pasta de uploads: {ex.Message}");
            }
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Arquivo inválido");

            // Validar extensão
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };

            if (!allowedExtensions.Contains(extension))
                throw new ArgumentException("Formato de arquivo não permitido");

            // Criar nome único para o arquivo
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(_uploadsFolder, fileName);

            // Salvar arquivo
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Retornar caminho relativo
            return $"uploads/produtos/{fileName}";
        }

        public Task DeleteImageAsync(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return Task.CompletedTask;

            var fullPath = Path.Combine(_uploadsFolder, Path.GetFileName(imagePath));
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            return Task.CompletedTask;
        }
    }
}