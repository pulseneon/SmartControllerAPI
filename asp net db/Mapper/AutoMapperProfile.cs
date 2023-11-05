using asp_net_db.Models;
using asp_net_db.Models.Dto;
using AutoMapper;

namespace asp_net_db.Mapper
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            //CreateMap<LessonDto, Lesson>();
            //CreateMap<CourseDto, Course>();
            //CreateMap<TrackerDto, Tracker>();
            //CreateMap<HomeworkDto, Homework>();
            //CreateMap<CourseDto, Course>();
            //CreateMap<ContentDto, Content>();
            CreateMap<ServerDto, Server>();
        }
    }
}
