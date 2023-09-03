using FileManager.Model;
using FileManager.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;

namespace FileManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly ILogger<FilesController> logger;
        private readonly IWebHostEnvironment _environment;
        private readonly IFileRepository fileRepository;
        private readonly string _filesDirectory;

        public FilesController(IWebHostEnvironment environment, ILogger<FilesController> logger, IFileRepository fileRepository)
        {
            this.logger = logger;
            this.fileRepository = fileRepository;

            _environment = environment;
            _filesDirectory = Path.Combine(_environment.ContentRootPath, "Files");

            if (!Directory.Exists(_filesDirectory))
            {
                Directory.CreateDirectory(_filesDirectory);
            }
        }

        [HttpPost("upload")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UploadFile([FromForm] IFormFile image)
        {
            if (image != null)
            {
                var UUID = Guid.NewGuid().ToString().Substring(0, 5);
                await fileRepository.CreateAsync(new FileInformation { FileName = image.FileName, UUID = UUID });
                var filePath = Path.Combine(_filesDirectory, image.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    image.CopyTo(stream);
                }
                return Ok(new APIResponse { StatusCode = HttpStatusCode.Created, IsSuccess = true, ErrorMessage = null, Data = UUID });
            }

            return BadRequest(new APIResponse { StatusCode = HttpStatusCode.BadRequest, IsSuccess = false, ErrorMessage = "Изображение пустое", Data = null });
        }

        [HttpGet]
        [ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetAllFiles()
        {
            try
            {
                IEnumerable<FileInformation> list = await fileRepository.GetAll();
                return new APIResponse { StatusCode = HttpStatusCode.OK, IsSuccess = true, ErrorMessage = null, Data = list };
            }
            catch (Exception ex)
            {
                return new APIResponse { StatusCode = HttpStatusCode.InternalServerError, IsSuccess = false, ErrorMessage = ex.Message, Data = null };
            }
        }
        [HttpGet("info/{UUID}")]
        [ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetFileName(string UUID)
        {
            try
            {
                FileInformation fileInfo = await fileRepository.GetAsync(x => x.UUID == UUID);

                if (fileInfo != null)
                {
                    return new APIResponse { StatusCode = HttpStatusCode.OK, IsSuccess = true, ErrorMessage = null, Data = fileInfo };
                }
                else
                {
                    return NotFound(new APIResponse { StatusCode = HttpStatusCode.NotFound, IsSuccess = false, ErrorMessage = "Файл не найден", Data = null });
                }
            }
            catch (Exception ex)
            {
                return new APIResponse { StatusCode = HttpStatusCode.InternalServerError, IsSuccess = false, ErrorMessage = ex.Message, Data = null };
            }
        }

        [HttpGet("display/{UUID}")]
        [ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DisplayImage(string UUID)
        {
            FileInformation file = await fileRepository.GetAsync(u => u.UUID == UUID);

            if (file != null && file.FileName != null)
            {
                var filePath = Path.Combine(_filesDirectory, file.FileName);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound(new APIResponse { StatusCode = HttpStatusCode.NotFound, IsSuccess = false, ErrorMessage = "Изображение не найдено", Data = null });
                }

                return File(System.IO.File.ReadAllBytes(filePath), "image/jpeg");
            }

            return NotFound(new APIResponse { StatusCode = HttpStatusCode.NotFound, IsSuccess = false, ErrorMessage = "Изображение не найдено", Data = null });
        }

        [HttpDelete("delete/{UUID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteImage(string UUID)
        {
            FileInformation file = await fileRepository.GetAsync(u => u.UUID == UUID);
            logger.LogWarning($"{file.FileName} {file.UUID}");
            if (file != null && file.FileName != null)
            {
                var filePath = Path.Combine(_filesDirectory, file.FileName);
                if (System.IO.File.Exists(filePath))
                {
                    await fileRepository.Delete(file);
                    System.IO.File.Delete(filePath);
                    return Ok(new APIResponse { StatusCode = HttpStatusCode.OK, IsSuccess = true, ErrorMessage = null, Data = null });
                }
                else
                {
                    return NotFound(new APIResponse { StatusCode = HttpStatusCode.NotFound, IsSuccess = false, ErrorMessage = "Изображение не найдено", Data = null });
                }
            }
            return NotFound(new APIResponse { StatusCode = HttpStatusCode.NotFound, IsSuccess = false, ErrorMessage = "Изображение не найдено", Data = null });
        }
    }
}
    

