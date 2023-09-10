using Project_Management.Models.DatabaseModel;

namespace Project_Management.Models.CombinedModels
{
    public class UserProjectWithProject
    {
        public UserProject UserProject { get; set; }
        public Project Project { get; set; }
    }
}
