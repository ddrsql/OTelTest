using Volo.Abp;

namespace VoloAbp.OTel.Authors;

public class AuthorAlreadyExistsException : BusinessException
{
    public AuthorAlreadyExistsException(string name)
        : base(OTelDomainErrorCodes.AuthorAlreadyExists)
    {
        WithData("name", name);
    }
}
