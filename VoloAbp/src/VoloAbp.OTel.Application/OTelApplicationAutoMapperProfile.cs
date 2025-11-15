using AutoMapper;
using VoloAbp.OTel.Authors;
using VoloAbp.OTel.Books;

namespace VoloAbp.OTel;

public class OTelApplicationAutoMapperProfile : Profile
{
    public OTelApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        CreateMap<Book, BookDto>();
        CreateMap<CreateUpdateBookDto, Book>();
        CreateMap<Author, AuthorDto>();
        CreateMap<Author, AuthorLookupDto>();
    }
}
