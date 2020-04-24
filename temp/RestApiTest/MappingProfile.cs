using AutoMapper;
using RestApiTest.Core.Models;
using RestApiTest.DTO;
using RestApiTest.DTO.ResponseDTOs;

namespace RestApiTest
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<BlogPost, BlogPostGetResponse>().ReverseMap();
            //cfg.CreateMap<Comment, CommentDTO>().ForMember(destination => destination.AuthorId, opts => opts.MapFrom(source => source.Author.Id)).ReverseMap();
            CreateMap<Comment, CommentDTO>().ReverseMap();
            CreateMap<ForumUser, ForumUserDTO>().ReverseMap(); //[Note] - trzeba użyć w odwołaniach EntityFramework'a include, żeby określić, żeby referencje były zaciągane, lub skonfigurować eager loading - Jak zapewnić, by Automapper mapował typy zagnieżdżone? W BlogPostDTO i CommentDTO nie wyświetla nic dla ForumUser
            CreateMap<NewsMessage, NewsMessageDTO>().ReverseMap();
            CreateMap<Core.Models.Tag, TagDTO>().ReverseMap();
            CreateMap<Vote, VoteDTO>().ReverseMap();
        }
    }
}
