using TheDepot.Models;

namespace TheDepot.Services
{
    public static class CodeValidationService
    {
        public static Constants.Roles GetRole(string code)
        {
            switch (code)
            {
                case "1":
                    return Constants.Roles.Visitor;
                case "2":
                    return Constants.Roles.Guide;
                case "3":
                    return Constants.Roles.DepartmentHead;
                default:
                    return Constants.Roles.None;
            }
        }
    }
}
