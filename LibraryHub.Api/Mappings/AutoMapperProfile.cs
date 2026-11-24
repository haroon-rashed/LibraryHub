using AutoMapper;
using LibraryHub.Api.Entities;
using LibraryHub.Common.Dtos;

namespace LibraryHub.Api.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Book, BookDto>();
            CreateMap<SaveBookDto, Book>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.AvailableCopies, o => o.Ignore())
                .ForMember(d => d.Loans, o => o.Ignore());

            CreateMap<Member, MemberDto>();
            CreateMap<SaveMemberDto, Member>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.JoinedOn, o => o.Ignore())
                .ForMember(d => d.Loans, o => o.Ignore());
        }
    }
}