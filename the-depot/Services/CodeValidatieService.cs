namespace the_depot.Services
{
    public static class CodeValidatieService
    {
        public static Constants.Roles getRole(string code)
        {
            if (code == "1")
                return Constants.Roles.Bezoeker;
            else if (code == "2")
                return Constants.Roles.Gids;
            else if (code == "3")
                return Constants.Roles.Afdelingshoofd;

            return Constants.Roles.Bezoeker;
        }
    }
}
