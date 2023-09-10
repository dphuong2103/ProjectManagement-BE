using Project_Management.Models.DatabaseModel;

namespace Project_Management.Models.CombinedModels
{
    public class UserWithUserProjectDetails
    {
        public User User { get; set; }
        public List<UserProjectWithProject> UserProjects { get; set; }
    }
}
