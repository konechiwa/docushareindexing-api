using DocuShareIndexingAPI.Entities;

namespace DocuShareIndexingAPI.Interface
{
    public interface ITokenService
    {
        /**
        * @dev Returns the token string from work on.
        * @param user The user logged in. 
        */
        string createToken(User user);
    }
}