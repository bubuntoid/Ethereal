using System;
using System.Threading.Tasks;
using Ethereal.WebAPI.Contracts;
using Ethereal.WebAPI.Contracts.Infrastructure;
using Flurl;
using Flurl.Http;

namespace Ethereal.WebAPI.Client
{
    public class EtherealClient
    {
        private readonly IEtherealClientSettings settings;
        private readonly FlurlClient client;

        public EtherealClient(IEtherealClientSettings settings)
        {
            this.settings = settings;
            client = new FlurlClient(settings.Endpoint);
        }

        public Task<GuidResponseDto> InitializeAsync(InitializeJobRequestDto dto)
        {
            return client.Request()
                .AppendPathSegment("api/jobs/initialize")
                .PostJsonAsync(dto)
                .ReceiveJson<GuidResponseDto>();
        }

        public Task<ProcessingJobDto> GetAsync(Guid id)
        {
            return client.Request()
                .AppendPathSegment($"api/jobs/{id}")
                .GetJsonAsync<ProcessingJobDto>();
        }
    }
}