
using Domain.Entities;
using Infrastructure.Database;
using Domain.RepositoriesContracts;
using Microsoft.EntityFrameworkCore;
using Common.DTO.Response;
using AutoMapper;

namespace Infrastructure.Repository
{
    public class TimetableRepository : BaseRepository<Timetable, int>, ITimetableRepository
    {
        public TimetableRepository(IMapper mapper, ISqlServerDbContext context) : base(mapper, context)
        {
        }
    }
}
