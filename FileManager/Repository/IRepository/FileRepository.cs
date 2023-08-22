using FileManager.Data;
using FileManager.Model;

namespace FileManager.Repository.IRepository
{
    public class FileRepository : Repository<FileInformation>, IFileRepository
    {
        private readonly ApplicationDbContext db;
        public FileRepository(ApplicationDbContext db) : base(db)
        {
            this.db = db;
        }
    }
}
