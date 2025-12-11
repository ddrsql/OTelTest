using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using VoloAbp.OTel.Permissions;

namespace VoloAbp.OTel.Authors;

[Authorize(OTelPermissions.Authors.Default)]
public class AuthorAppService : OTelAppService, IAuthorAppService
{
    private readonly IAuthorRepository _authorRepository;
    private readonly AuthorManager _authorManager;

    public AuthorAppService(
        IAuthorRepository authorRepository,
        AuthorManager authorManager)
    {
        _authorRepository = authorRepository;
        _authorManager = authorManager;
    }

    public async Task<AuthorDto> GetAsync(Guid id)
    {
        var author = await _authorRepository.GetAsync(id);
        var enableState = author.GetProperty<bool>("EnableState");
        var scoring = author.GetProperty<decimal>("Scoring");
        var remark = author.GetProperty<string>("Remark");
        var remarkObject = author.GetProperty("Remark");
        return ObjectMapper.Map<Author, AuthorDto>(author);
    }

    public async Task<PagedResultDto<AuthorDto>> GetListAsync(GetAuthorListDto input)
    {
        if (input.Sorting.IsNullOrWhiteSpace())
        {
            input.Sorting = nameof(Author.Name);
        }

        var authors = await _authorRepository.GetListAsync(
            input.SkipCount,
            input.MaxResultCount,
            input.Sorting,
            input.Filter
        );

        var totalCount = input.Filter == null
            ? await _authorRepository.CountAsync()
            : await _authorRepository.CountAsync(
                author => author.Name.Contains(input.Filter));

        return new PagedResultDto<AuthorDto>(
            totalCount,
            ObjectMapper.Map<List<Author>, List<AuthorDto>>(authors)
        );
    }

    [Authorize(OTelPermissions.Authors.Create)]
    public async Task<AuthorDto> CreateAsync(CreateAuthorDto input)
    {
        var author = await _authorManager.CreateAsync(
            input.Name,
            input.BirthDate,
            input.ShortBio
        );
        // 使用 SetProperty 方法添加扩展属性
        author.SetEnableState(false);
        author.SetScoring(0.15);
        author.SetRemark("中文测试");

        await _authorRepository.InsertAsync(author);

        return ObjectMapper.Map<Author, AuthorDto>(author);
    }

    [Authorize(OTelPermissions.Authors.Edit)]
    public async Task UpdateAsync(Guid id, UpdateAuthorDto input)
    {
        var author = await _authorRepository.GetAsync(id);

        if (author.Name != input.Name)
        {
            await _authorManager.ChangeNameAsync(author, input.Name);
        }

        author.BirthDate = input.BirthDate;
        author.ShortBio = input.ShortBio;

        author.SetEnableState(true);
        author.SetScoring(author.GetScoring() + 0.01);
        author.SetRemark($"中文测试{DateTime.Now.ToString()}");

        await _authorRepository.UpdateAsync(author);
    }

    [Authorize(OTelPermissions.Authors.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _authorRepository.DeleteAsync(id);
    }

    public async Task TestLocalizer(int id)
    {
        throw new Exception(L["NotFound", id].ToString());
        //throw new UserFriendlyException(L["NotFound", id].ToString());
    }
}
