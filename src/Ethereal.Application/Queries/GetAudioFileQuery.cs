using System;
using System.Threading.Tasks;
using Ethereal.Domain;

namespace Ethereal.Application.Queries
{
    public class GetAudioFileQuery
    {
        private readonly EtherealDbContext dbContext;

        public GetAudioFileQuery(EtherealDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task ExecuteAsync(Guid jobId)
        {
            // todo
        }
    }
}