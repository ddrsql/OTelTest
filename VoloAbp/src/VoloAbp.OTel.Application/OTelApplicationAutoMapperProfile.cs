using AutoMapper;
using Volo.Abp.AutoMapper;
using VoloAbp.OTel.Authors;
using VoloAbp.OTel.Books;
using VoloAbp.OTel.TestSuites.Aggregates;
using VoloAbp.OTel.TestSuites.Datas;
using VoloAbp.OTel.TestSuites.Dtos;

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

        CreateMap<TestSuite, TestSuiteDto>()
                .ForMember(dest => dest.AverageExecutionTime,
                    opt => opt.MapFrom(src => src.AverageExecutionTime.HasValue
                        ? src.AverageExecutionTime.Value.ToString("g")
                        : null))
                .Ignore(x => x.TestCases);

        CreateMap<CreateUpdateTestSuiteDto, TestSuite>()
            .Ignore(x => x.Id)
            .Ignore(x => x.TestCases)
            .Ignore(x => x.Status)
            .Ignore(x => x.LastExecutionTime)
            .Ignore(x => x.AverageExecutionTime)
            .Ignore(x => x.TotalTestCases)
            .Ignore(x => x.PassedTestCases)
            .Ignore(x => x.FailedTestCases)
            .Ignore(x => x.SuccessRate);

        CreateMap<TestConfiguration, TestConfigurationDto>().ReverseMap();
        CreateMap<TestCase, TestCaseDto>()
            .ForMember(dest => dest.ExecutionDuration,
                opt => opt.MapFrom(src => src.ExecutionDuration.HasValue
                    ? src.ExecutionDuration.Value.ToString("g")
                    : null));

        CreateMap<TestPriority, TestPriorityDto>();
        CreateMap<TestSuiteStatistics, TestSuiteStatisticsDto>();
        CreateMap<TestSuiteReport, TestSuiteReportDto>()
            .ForMember(dest => dest.TotalExecutionTime,
                opt => opt.MapFrom(src => src.TotalExecutionTime.ToString("g")));

        CreateMap<TestCaseReport, TestCaseReportDto>()
            .ForMember(dest => dest.ExecutionDuration,
                opt => opt.MapFrom(src => src.ExecutionDuration.ToString("g")));
    }
}
