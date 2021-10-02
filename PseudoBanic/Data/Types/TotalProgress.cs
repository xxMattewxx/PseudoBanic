using System.Linq;

namespace PseudoBanic.Data
{
    public class TotalProgress
    {
        public int ID;
        public string Name;
        public int TotalDone;
        public int TotalExisting;

        public bool IsValid()
        {
            if (Name == null || Name.Length < 5) return false;
            if (TotalDone <= 0 || TotalExisting <= 0) return false;

            return true;
        }

        /*public static TotalProgress GetByMetadataID(int metadataID)
        {
            using var dbContext = new TasksDbContext();
            var user = dbContext.Tasks.Where(task => task.MetadataID == metadataID).FirstOrDefault();

            return user;
        }*/
    }
}